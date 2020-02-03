using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class XmlParser
{
	string directory;
    string physicellCellsPath;
	string cellsPath;
	string[] variableNames;
	
	
	public void parseXML(string filepath){
		string variableNamesString = "";
		findDirectory(filepath);
		try{
			var sr = new StreamReader(filepath);
			string line;
			
			while((line = sr.ReadLine()) != null){
				
				// pick out the variable name on the line
				if(line.Contains("label ")){
					int start = line.IndexOf(">") + 1;
					int stop = line.IndexOf("</");
					int sizeStart = line.IndexOf("size=\"") + 6;
					int sizeStop = line.IndexOf("\">");
					int size = int.Parse(line.Substring(sizeStart, sizeStop - sizeStart));
					for(int i = 0; i < size; i++){
						string newVariableName = line.Substring(start, stop - start) + "!";
						newVariableName = newVariableName.Replace(' ', '_');
						variableNamesString += newVariableName;
					}
				}
				
				// pick out the file name in the line
				if(line.Contains("filename") && line.Contains("cells")){
					int start = line.IndexOf(">") + 1;
					int stop = line.IndexOf("</");
					if(line.Contains("physicell")){
						physicellCellsPath = directory + line.Substring(start, stop - start);
					}
					else{
						cellsPath = directory + line.Substring(start, stop - start);
					}

				}
			}
			variableNames = variableNamesString.Substring(0, variableNamesString.Length - 1).Split('!');
			sr.Close();
		}
		catch(FileNotFoundException fe){
			Debug.Log("Error: could not find file " + filepath  + "\n\n" + fe);
		}
	}
	
	public void findDirectory(string filepath){
		int index = filepath.Length - 1;
		char currentChar = ' ';
		
		while(currentChar != '\\' && currentChar != '/' && index >= 0){
			currentChar = filepath[index--];
		}
		index += 2;
		
		directory = filepath.Substring(0, index);
	}
	
	public string getPhysicellCellsPath(){
		return physicellCellsPath;
	}
	public string getCellsPath(){
		return cellsPath;
	}
	public string[] getVariableNames(){
		return variableNames;
	}
}
