using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner
{
	protected string str;
	protected int head;
	
	public Scanner(string str){
		this.str = str;
		this.head = 0;
	}
	
	public string next(){
		string nextStr = "";
		
		// clear all white space
		while(head < str.Length && char.IsWhiteSpace(str[head])){
			head++;
		}
		
		// read until we hit another white space
		while(head < str.Length && !char.IsWhiteSpace(str[head])){
			nextStr += str[head++];
		}
		
		return nextStr;
	}
	
	public string rest(){
		string rStr = str.Substring(head);
		head = str.Length;
		return rStr;
	}
	
	public bool hasNext(){
		return head < str.Length;
	}
}
