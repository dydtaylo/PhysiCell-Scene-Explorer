using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Leaf /*: ParseTree */ {
	/*
	string expr;
	string first, op, second;
	
	public Leaf(string expr){
		this.expr = expr;
		construct();
	}
	
	public override void setExpr(string expr){
		this.expr = expr;
		construct();
	}
	
	public void construct(){
		Scanner scan = new Scanner(expr);
		first = scan.next();
		op = scan.next();
		second = scan.next();
	}
	
	public override bool evaluate(CellData cell){
		if(first.Equals("") || op.Equals("") || second.Equals("")){
			return false;
		}
		
		PhysicellVariable pv1 = getVal(cell, first);
		PhysicellVariable pv2 = getVal(cell, second);
		Vector3 v1 = pv1.getData();
		Vector3 v2 = pv2.getData();
		float diff = difference(pv1.getData(), pv2.getData());

		if(op.Equals("<")){
			return diff < 0;
		}
		else if(op.Equals("<=")){
			return diff <= 0;
		}
		else if(op.Equals(">")){
			return diff > 0;
		}
		else if(op.Equals(">=")){
			return diff >= 0;
		}
		else if(op.Equals("=")){
			return Math.Abs(diff) < .00001;
		}
		else if(op.Equals("!=")){
			return Math.Abs(diff) > .00001;
		}
		else{
			return false;
		}
	}
	
	public float difference(Vector3 v1, Vector3 v2){
		float sum = 0;
		sum += v1.x - v2.x;
		sum += v1.y - v2.y;
		sum += v1.z - v2.z;
		return sum;
	}
	
	public PhysicellVariable getVal(CellData cell, string str){
		if(isNumber(str)){
			return new PhysicellVariable(float.Parse(str));
		}
		else{
			if(cell.hasVariable(str)){
				return cell.getVariable(str);
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
	
	public override string ToString(){
		return " (" + first + " " + op + " " + second + ") ";
	}
	*/
}
