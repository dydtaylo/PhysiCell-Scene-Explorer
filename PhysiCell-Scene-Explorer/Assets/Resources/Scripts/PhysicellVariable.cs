// Dylan Taylor
// PhysiCell Scene Explorer

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicellVariable
{
	public bool isVector;
	bool usesX;
    bool usesY;
	bool usesZ;
	Vector3 data;
	
	/* This class serves as a sort of wrapper for a Physicell data field
	   In Physicell, a data field can be a float vector or just a float
	   Whether or not a PhysicellVariable is a vector or not depends on the isVector bool
	   If a particular PhysicellVariable is not a vector, simply refer to data.x as its only value
	   otherwise, use all of data
	*/
	
	// initially, we have no data and make the assumption that this PhysicellVariable is not a vector
	public PhysicellVariable(){
		isVector = false;
		usesX = false;
		usesY = false;
		usesZ = false;
	}
	
	public PhysicellVariable(float x){
		data.x = x;
		isVector = false;
		usesX = true;
		usesY = false;
		usesZ = false;
	}
	
	public PhysicellVariable(Vector3 data){
		this.data = data;
		isVector = true;
		usesX = true;
		usesY = true;
		usesZ = true;
	}
	
	// fill in the appropriate x, y, or z component of data vector 
	// and set isVector to true if this particular PhysicellVariable is being used as a vector
	public void addData(float datum){
		if(!usesX){
			data.x = datum;
			usesX = true;
		}
		else if(!usesY){
			data.y = datum;
			usesY = true;
			isVector = true;
		}
		else if(!usesZ){
			data.z = datum;
			usesZ = true;
			isVector = true;
		}
	}
	
	// return data vector
	public Vector3 getData(){
		return data;
	}
	
	
	public override string ToString(){
		return "(" + data.x + ", " + data.y + ", " + data.z + ")";
	}
}
