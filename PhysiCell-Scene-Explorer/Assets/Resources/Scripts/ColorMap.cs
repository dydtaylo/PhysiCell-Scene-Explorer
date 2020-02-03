using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ColorMap : MonoBehaviour, Modifier
{
	public int[] cellTypes;
	public Color[] typeColor;
	Dictionary<int, Color> colorMap;
	
    public void init(){
		colorMap = new Dictionary<int, Color>();
		
		for(int i = 0; i < cellTypes.Length; i++){
			colorMap.Add(cellTypes[i], typeColor[i]);
		}
	}
	
	public void modify(GameObject g){
		int cellType = g.GetComponent<CellData>().getCellType();
		g.GetComponent<Renderer>().material.SetColor("_Color", colorMap[cellType]);
	}
	
	/*SERIALIZATION STRUCTURE
	ColorMap [2 + number of rules]
	[x-coordinate] [y-coordinate] [z-coordinate]
	[x-scale] [y-scale] [z-scale]
	[type_1] [color_1]
	[type_2] [color_2]
	...
	[type_(number of rules)] [color_(number of rules)]
	*/

	// return a string in the serialization structure
	public string writeToString(){
		string serialization = "";
		serialization += "ColorMap " + (2 + cellTypes.Length) + "\n";
		serialization += transform.position.x + " " + transform.position.y + " " + transform.position.z + "\n";
		serialization += transform.localScale.x + " " + transform.localScale.y + " " + transform.localScale.z + "\n";
		
		for(int i = 0; i < cellTypes.Length; i++){
			serialization += cellTypes[i] + " " + rgbString(typeColor[i]);
			if((i + 1) != cellTypes.Length){
				serialization += "\n";
			}
		}
		
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
			
			// construct the cellTypes and typeColor arrays
			cellTypes = new int[lines.Length - 2];
			typeColor = new Color[lines.Length - 2];
			
			// for each rule, set its components in cellTypes and typeColor
			string[] ruleLine;
			for(int i = 2; i < lines.Length; i++){
				ruleLine = lines[i].Split(' ');
				cellTypes[i - 2] = Int32.Parse(ruleLine[0]);
				typeColor[i - 2] = new Color(Single.Parse(ruleLine[1]), Single.Parse(ruleLine[2]), Single.Parse(ruleLine[3]));
			}
		}
		catch(IndexOutOfRangeException ex){
			Debug.Log(ex);
		}
		
	}
	
	// return the formatted string of the given color
	public string rgbString(Color c){
		return c.r + " " + c.g + " " + c.b;
	}
}
