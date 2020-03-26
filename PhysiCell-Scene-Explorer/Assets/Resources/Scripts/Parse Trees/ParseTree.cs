using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions; 
using UnityEngine;

public class ParseTree
{
	string expr;
	protected ParseTree left, right, parent;
	
	public ParseTree(string expr){
		this.expr = expr;
	}
	
	public void setExpr(string expr){
		this.expr = expr;
	}
	
	public bool evaluate(CellData cell){
		if(expr.Equals("&") && left != null && right != null){
			return left.evaluate(cell) && right.evaluate(cell);
		}
		else if(expr.Equals("|") && left != null && right != null){
			return left.evaluate(cell) || right.evaluate(cell);
		}
		else if(left != null && right == null){
			return left.evaluate(cell);
		}
		else if(left == null && right != null){
			return right.evaluate(cell);
		}
		else{
			// in this case, both children are null, so we must be a leaf
			return evaluateAsLeaf(cell);
		}
	}
	
	public bool evaluateAsLeaf(CellData cell){
		Scanner scan = new Scanner(expr);
		string first = scan.next();
		string op = scan.next();
		string second = scan.rest();
		
		if(first.Equals("") || op.Equals("") || second.Equals("")){
			return false;
		}
		
		PhysicellVariable pv1 = getVal(cell, first);
		PhysicellVariable pv2 = getVal(cell, second);
		Vector3 v1 = pv1.getData();
		Vector3 v2 = pv2.getData();


		if(op.Equals("<")){
			return lessThan(pv1, pv2);
		}
		else if(op.Equals("<=")){
			return lessThanEqualTo(pv1, pv2);
		}
		else if(op.Equals(">")){
			return greaterThan(pv1, pv2);
		}
		else if(op.Equals(">=")){
			return greaterThanEqualTo(pv1, pv2);
		}
		else if(op.Equals("=")){
			return equalTo(pv1, pv2);
		}
		else if(op.Equals("!=")){
			return notEqualTo(pv1, pv2);
		}
		else{
			return false;
		}
	}
	
	public bool lessThan(PhysicellVariable pv1, PhysicellVariable pv2){
		Vector3 v1 = pv1.getData();
		Vector3 v2 = pv2.getData();
		if(v1.x >= v2.x){
			return false;
		}
		if((pv1.isVector && pv2.isVector) && v1.y >= v2.y){
			return false;
		}
		if((pv1.isVector && pv2.isVector) && v1.z >= v2.z){
			return false;
		}
		return true;
	}
	public bool lessThanEqualTo(PhysicellVariable pv1, PhysicellVariable pv2){
		Vector3 v1 = pv1.getData();
		Vector3 v2 = pv2.getData();
		if(v1.x > v2.x){
			return false;
		}
		if((pv1.isVector && pv2.isVector) && v1.y > v2.y){
			return false;
		}
		if((pv1.isVector && pv2.isVector) && v1.z > v2.z){
			return false;
		}
		return true;
	}
	public bool greaterThan(PhysicellVariable pv1, PhysicellVariable pv2){
		Vector3 v1 = pv1.getData();
		Vector3 v2 = pv2.getData();
		if(v1.x <= v2.x){
			return false;
		}
		if((pv1.isVector && pv2.isVector) && v1.y <= v2.y){
			return false;
		}
		if((pv1.isVector && pv2.isVector) && v1.z <= v2.z){
			return false;
		}
		return true;
	}
	public bool greaterThanEqualTo(PhysicellVariable pv1, PhysicellVariable pv2){
		Vector3 v1 = pv1.getData();
		Vector3 v2 = pv2.getData();
		if(v1.x < v2.x){
			return false;
		}
		if((pv1.isVector && pv2.isVector) && v1.y < v2.y){
			return false;
		}
		if((pv1.isVector && pv2.isVector) && v1.z < v2.z){
			return false;
		}
		return true;
	}
	public bool equalTo(PhysicellVariable pv1, PhysicellVariable pv2){
		Vector3 v1 = pv1.getData();
		Vector3 v2 = pv2.getData();
		if(Math.Abs(v1.x - v2.x) > 0.00001){
			return false;
		}
		if((pv1.isVector && pv2.isVector) && Math.Abs(v1.y - v2.y) > 0.00001){
			return false;
		}
		if((pv1.isVector && pv2.isVector) && Math.Abs(v1.z - v2.z) > 0.00001){
			return false;
		}
		return true;
	}
	public bool notEqualTo(PhysicellVariable pv1, PhysicellVariable pv2){
		Vector3 v1 = pv1.getData();
		Vector3 v2 = pv2.getData();
		if(Math.Abs(v1.x - v2.x) > 0.00001){
			return true;
		}
		if((pv1.isVector && pv2.isVector) && Math.Abs(v1.y - v2.y) > 0.00001){
			return true;
		}
		if((pv1.isVector && pv2.isVector) && Math.Abs(v1.z - v2.z) > 0.00001){
			return true;
		}
		return false;
	}
	
	// not used as of now
	public float difference(Vector3 v1, Vector3 v2){
		float sum = 0;
		sum += v1.x - v2.x;
		sum += v1.y - v2.y;
		sum += v1.z - v2.z;
		return sum;
	}

	public PhysicellVariable getVal(CellData cell, string str){
		// vector case
		if(str.Contains("[")){
			str = str.Replace("[", "").Replace("]", "");
			str = str.Replace(",", " ");
			Regex re = new Regex("[ ]{2,}"); 
			str = re.Replace(str, " ");
			string[] pieces = str.Split(' ');
			/*
			Debug.Log(str);
			Debug.Log(pieces.Length);
			string s = "";
			for(int i = 0; i < pieces.Length; i++){
				s += pieces[i] + " ";
			}
			Debug.Log("pieces: " + s);
			*/
			Vector3 vec = new Vector3(float.Parse(pieces[1]), float.Parse(pieces[2]), float.Parse(pieces[3]));
			// Debug.Log(vec);
			return new PhysicellVariable(vec);
		}
		// scalar case
		else if(isNumber(str.Replace(" ", ""))){
			return new PhysicellVariable(float.Parse(str.Replace(" ", "")));
		}
		// variable case
		else{
			if(cell.hasVariable(str.Replace(" ", ""))){
				return cell.getVariable(str.Replace(" ", ""));
			}
			else{
				return new PhysicellVariable();
			}
		}
	}
	
	public bool isNumber(string str){
		for(int i = 0; i < str.Length; i++){
			if(!char.IsNumber(str[i]) && str[i] != '.'){
				return false;
			}
		}
		return true;
	}
	
	public ParseTree getParent(){
		return this.parent;
	}
	public ParseTree getLeft(){
		return this.left;
	}
	public ParseTree getRight(){
		return this.right;
	}
	
	public void setLeft(ParseTree node){
		this.left = node;
		this.left.setParent(this);
	}
	public void setRight(ParseTree node){
		this.right = node;
		this.right.setParent(this);
	}
	public void setParent(ParseTree p){
		this.parent = p;
	}
	
	public override string ToString(){
		string str = "";
		if(this.left != null){
			str += this.left.ToString();
		}
		str += this.expr;
		if(this.right != null){
			str += this.right.ToString();
		}
		
		return "(" + str + ")";
	}
}
