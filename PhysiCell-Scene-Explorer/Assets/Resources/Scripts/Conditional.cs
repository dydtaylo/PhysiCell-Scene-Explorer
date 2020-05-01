using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Conditional : MonoBehaviour, Modifier
{
	public string expression;
	public GameObject modifier;
	ParseTree root;
	
	public void init(){
		// clean up expression and generate parse tree
		expression = expression.Replace("(", " ( ");
		expression = expression.Replace(")", " ) ");
		expression = expression.Replace("&", " & ");
		expression = expression.Replace("|", " | ");
		TreeScanner scan = new TreeScanner(expression);
		ParseTree currentNode = new ParseTree("");
		while(scan.hasNext()){
			string nextExpr = scan.nextExpression();
			// Debug.Log("|" + nextExpr + "|");
			
			if(nextExpr.Length > 0 && !nextExpr.Equals("(") && !nextExpr.Equals(")") && !nextExpr.Equals("&") && !nextExpr.Equals("|")){
				
				/*
				if(currentNode == null){
					currentNode = new ParseTree(nextExpr);
					break;
				}
				else{
				*/
					currentNode.setExpr(nextExpr);
					if(currentNode.getParent() != null){
						currentNode = currentNode.getParent();
					}
					else{
						break;
					}
				/*	
				}
				*/
			}
			else{
				if(nextExpr.Equals("(")){
					currentNode.setLeft(new ParseTree(""));
					currentNode = currentNode.getLeft();
				}
				else if(nextExpr.Equals("&") || nextExpr.Equals("|")){
					currentNode.setExpr(nextExpr);
					currentNode.setRight(new ParseTree(""));
					currentNode = currentNode.getRight();
				}
				else if(nextExpr.Equals(")")){
					if(currentNode.getParent() != null){
						currentNode = currentNode.getParent();
					}
					else{
						break;
					}
				}
			}
		}
		
		root = currentNode;
		Debug.Log(root.ToString());
		
		/*
		root = new Leaf(expression);
		
		// or altenatively just a hardcoded rule
		root = new InnerNode("&");
		root.setLeft(new Leaf("cycle_model > 1"));
		root.setRight(new Leaf("oncoprotein > 0"));
		*/
		
		// initialize modifier
		modifier.GetComponent<Modifier>().init();
	}
	
    public void modify(GameObject g){
		if(root == null || root.evaluate(g.GetComponent<CellData>())){
			modifier.GetComponent<Modifier>().modify(g);
		}
	}	
	
	/*SERIALIZATION STRUCTURE
	Conitional [3 + number of lines in [serialization of modifier]]
	[x-coordinate] [y-coordinate] [z-coordinate]
	[x-scale] [y-scale] [z-scale]
	[expression]
	[serialization of modifier]
	*/
	
	// return a string in the serialization structure
	public string writeToString(){
		string serialization = "";
		string serializationOfModifier = modifier.GetComponent<Modifier>().writeToString();
		serialization += "Conditional " + (3 + serializationOfModifier.Split('\n').Length) + "\n";
		serialization += transform.position.x + " " + transform.position.y + " " + transform.position.z + "\n";
		serialization += transform.localScale.x + " " + transform.localScale.y + " " + transform.localScale.z + "\n";
		serialization += expression + "\n";
		serialization += serializationOfModifier;
		
		return serialization;
	}
	
	// parse a string in the serialization structure and set appropriate variables
	public void loadFromString(string serialization){
		try{
			// divide the serialization by lines
			string[] lines = serialization.Split('\n');
			
			// construct the position vector from the first line
			string[] pos = lines[0].Split(' ');
			transform.position = new Vector3(Single.Parse(pos[0]), Single.Parse(pos[1]), Single.Parse(pos[2]));
			
			// construct the scale vector from the second line
			string[] scale = lines[1].Split(' ');
			transform.localScale = new Vector3(Single.Parse(scale[0]), Single.Parse(scale[1]), Single.Parse(scale[2]));
			
			// assign the expression
			expression = lines[2];
			
			// construct the modifier
			GameObject modParser = GameObject.Find("ModParser");
			string modifierSerialization = "";
			for(int i = 3; i < lines.Length; i++){
				modifierSerialization += lines[i] + "\n";
			}
			modifier = modParser.GetComponent<ModParser>().loadModZone(modifierSerialization);
			Renderer renderer = modifier.GetComponent<MeshRenderer>();
			renderer.enabled = false;
		}
		catch(IndexOutOfRangeException ex){
			Debug.Log(ex);
		}
	}
}
