// Dylan Taylor
// PhysiCell Scene Explorer

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Stratifier : MonoBehaviour, Modifier
{
	public Vector3 referencePoint;
	public Vector3 direction;
	public int numSubsections;
	public int spacing;
	
	Vector3[] subsections;
	
	public void init(){
		if(referencePoint == null){
			referencePoint = Vector3.Scale(transform.position, direction);
		}
		
		subsections = new Vector3[numSubsections];
		float widthOfSection = vectorSum(Vector3.Scale(transform.localScale, direction))/numSubsections;
		for(int i = 0; i < numSubsections; i++){
			subsections[i] = referencePoint + direction * widthOfSection * i;
		}
		
		
	}
	
    public void modify(GameObject g){

		int subsectionOfCell = 0;
		Vector3 relativePos;
		Vector3 cellPos = Vector3.Scale(g.GetComponent<CellData>().getRealPosition(), direction);
		float x, y, z;
		do{
			relativePos = subsections[subsectionOfCell];
			x = relativePos.x;
			y = relativePos.y;
			z = relativePos.z;
			subsectionOfCell++;
		}
		while(cellPos.x + cellPos.y + cellPos.z > x + y + z && subsectionOfCell < numSubsections);
		// while(cellPos.x >= x && cellPos.y >= y && cellPos.z >= z && subsectionOfCell < numSubsections);
		subsectionOfCell--;
		
		g.GetComponent<CellData>().moveDisplayPos(direction * spacing * subsectionOfCell);
	}
	
	/*SERIALIZATION STRUCTURE
	Stratifier 5
	[x-coordinate] [y-coordinate] [z-coordinate]
	[x-scale] [y-scale] [z-scale]
	[reference-x] [reference-y] [reference-z]
	[direction-x] [direction-y] [direction-z]
	[numSubsections] [spacing]
	*/
	
	// return a string in the serialization structure
	public string writeToString(){
		string serialization = "";
		serialization += "Stratifier 5\n";
		serialization += transform.position.x + " " + transform.position.y + " " + transform.position.z + "\n";
		serialization += transform.localScale.x + " " + transform.localScale.y + " " + transform.localScale.z + "\n";
		serialization += referencePoint.x + " " + referencePoint.y + " " + referencePoint.z + "\n";
		serialization += direction.x + " " + direction.y + " " + direction.z + "\n";
		serialization += numSubsections + " " + spacing;
		
		return serialization;
	}
	
	// parse a string in the serialization structure and set appropriate variables
	public void loadFromString(string serialization){
		try{
			Debug.Log(serialization);
			// divide the serialization by lines
			string[] lines = serialization.Split('\n');
			
			// construct the position vector from the first line
			string[] pos = lines[0].Split(' ');
			transform.position = new Vector3(Single.Parse(pos[0]), Single.Parse(pos[1]), Single.Parse(pos[2]));
			
			// construct the scale vector from the second line
			string[] scale = lines[1].Split(' ');
			transform.localScale = new Vector3(Single.Parse(scale[0]), Single.Parse(scale[1]), Single.Parse(scale[2]));
			
			// construct the refrence point from the third line
			string[] refPoint = lines[2].Split(' ');
			referencePoint = new Vector3(Single.Parse(refPoint[0]), Single.Parse(refPoint[1]), Single.Parse(refPoint[2]));
			
			// construct the direction vector from the fourth line
			string[] dir = lines[3].Split(' ');
			direction = new Vector3(Single.Parse(dir[0]), Single.Parse(dir[1]), Single.Parse(dir[2]));
			
			// parse numSubsections and spacing from the fifth line
			string[] data = lines[4].Split(' ');
			numSubsections = Int32.Parse(data[0]);
			spacing = Int32.Parse(data[1]);
		}
		catch(IndexOutOfRangeException ex){
			Debug.Log(ex);
		}
	}
	
	float vectorSum(Vector3 v){
		return v.x + v.y + v.z;
	}
}
