// Dylan Taylor
// PhysiCell Scene Explorer

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Displacer : MonoBehaviour, Modifier
{
	public Vector3 offset;
	
	public void init(){}
	public void modify(GameObject g){
		g.GetComponent<CellData>().moveDisplayPos(offset);
	}
	
	/*SERIALIZATION STRUCTURE
	Displacer 3
	[x-coordinate] [y-coordinate] [z-coordinate]
	[x-scale] [y-scale] [z-scale]
	[offset-x] [offset-y] [offset-y]
	*/
	
	// return a string in the serialization structure
	public string writeToString(){
		string serialization = "";
		serialization += "Displacer 3\n";
		serialization += transform.position.x + " " + transform.position.y + " " + transform.position.z + "\n";
		serialization += transform.localScale.x + " " + transform.localScale.y + " " + transform.localScale.z + "\n";
		serialization += offset.x + " " + offset.y + " " + offset.z;
		
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
			
			// construct the offset vector from the third line
			string[] offsets = lines[2].Split(' ');
			offset = new Vector3(Single.Parse(offsets[0]), Single.Parse(offsets[1]), Single.Parse(offsets[2]));
		}
		catch(IndexOutOfRangeException ex){
			Debug.Log(ex);
		}
	}
}
