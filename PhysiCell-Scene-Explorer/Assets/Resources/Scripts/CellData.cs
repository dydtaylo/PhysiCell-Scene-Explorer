// Dylan Taylor
// PhysiCell Scene Explorer

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellData : MonoBehaviour
{
	private Vector3 pos;
	private int cellType;
	private Dictionary<string, PhysicellVariable> variables;
	
	/* Cell data is primarily kept in the variables dictionary
	   Which variables a cell has depends on the XML file used to load cells
	   All other fields are simply used for convenience
	*/
	
	// this is invoked when the object is instantiated
	public void init(Vector3 pos, string[] variableNames){
		this.pos = pos;
		variables = new Dictionary<string, PhysicellVariable>();
		foreach(string variableName in variableNames){
			if(!variables.ContainsKey(variableName)){
				variables.Add(variableName, new PhysicellVariable());
			}
		}
	}
	
	// add a new variable and its corresponding data
	public void addDataPoint(string variableName, float dataPoint){
		variables[variableName].addData(dataPoint);
	}
	
	// returns the value associated the variable name
	public PhysicellVariable getVariable(string variableName){
		return variables[variableName];
	}
	
	public string printKeys(){
		string keys = "";
		foreach(string key in variables.Keys){
			keys += key + ", ";
		}
		return keys;
	}
	
	
	public Vector3 getRealPosition(){
		return this.pos;
	}
	public Vector3 getDisplayedPosition(){
		return transform.position;
	}
	
	public void setPos(Vector3 pos){
		this.pos = pos;
	}
	public void movePos(Vector3 shiftPos){
		this.pos += shiftPos;
	}
	
	public void setDisplayedPos(Vector3 dispPos){
		transform.position = dispPos;
	}
	public void moveDisplayPos(Vector3 shiftDispPos){
		transform.position += shiftDispPos;
	}
	
	public void updateDisplayedPositionWithTruePosition(){
		transform.position = this.pos;
	}
	public void updateDisplayedPosition(Vector3 dispPos){
		transform.position = dispPos;
	}

	public int getCellType(){
		return (int) (variables["cell_type"].getData().x);
	}
	
	// implement later
	// public bool isAlive();
	
	
	
	
	public override string ToString(){
		return "This cell is at position " + this.pos;
	}
}
