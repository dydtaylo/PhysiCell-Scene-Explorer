using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScanner : Scanner
{
	public TreeScanner(string str) : base(str){}
	
	public string nextExpression(){
		string nextExpr = "";
		
		// clear all white space
		while(head < str.Length && char.IsWhiteSpace(str[head])){
			head++;
		}
		
		// read until we hit another white space
		while(head < str.Length && !isSpecialCharacter(str[head])){
			nextExpr += str[head++];
		}
		
		// in the case that the head is at a special char, it should be our next expression
		if(head < str.Length && nextExpr.Length == 0){
			nextExpr += str[head++];
		}
		
		return nextExpr;
	}
	
	public bool isSpecialCharacter(char c){
		return c == ')' || c == '(' || c == '&' || c == '|';
	}
}
