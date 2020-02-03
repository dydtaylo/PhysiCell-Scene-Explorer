// Dylan Taylor
// PhysiCell Scene Explorer

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Colorer : MonoBehaviour, Modifier
{
	public Color recolor;
	
	public void init(){}
    public void modify(GameObject g){
		g.GetComponent<Renderer>().material.SetColor("_Color", recolor);
    }
	
	/*SERIALIZATION STRUCTURE
	Colorer 3
	[x-coordinate] [y-coordinate] [z-coordinate]
	[x-scale] [y-scale] [z-scale]
	[R] [G] [B]
	*/
	
	// return a string in the serialization structure
	public string writeToString(){
		string serialization = "";
		serialization += "Colorer 3\n";
		serialization += transform.position.x + " " + transform.position.y + " " + transform.position.z + "\n";
		serialization += transform.localScale.x + " " + transform.localScale.y + " " + transform.localScale.z + "\n";
		serialization += recolor.r + " " + recolor.g + " " + recolor.b;
		
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
			
			// construct the color from the third line
			string[] rgb = lines[2].Split(' ');
			recolor = new Color(Single.Parse(rgb[0]), Single.Parse(rgb[1]), Single.Parse(rgb[2]));
		}
		catch(IndexOutOfRangeException ex){
			Debug.Log(ex);
		}
	}
}
