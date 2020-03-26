using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerNode /*: ParseTree */ {
	/*
    string connective;
	
	public InnerNode(string connective){
		this.connective = connective;
	}
	
	public override void setExpr(string expr){
		this.expr = expr;
	}
	
	public override bool evaluate(CellData cell){
		if(connective.Equals("&") && left != null && right != null){
			return left.evaluate(cell) && right.evaluate(cell);
		}
		else if(connective.Equals("|") && left != null && right != null){
			return left.evaluate(cell) || right.evaluate(cell);
		}
		else if(left != null && right == null){
			return left.evaluate(cell);
		}
		else if(left == null && right != null){
			return right.evaluate(cell);
		}
		else{
			return false;
		}
	}
	
	public override string ToString(){
		return left.ToString() + " " + connective + " " + right.ToString();
	}
	*/
}
