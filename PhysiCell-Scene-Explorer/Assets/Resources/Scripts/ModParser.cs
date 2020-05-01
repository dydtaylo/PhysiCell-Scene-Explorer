// Dylan Taylor
// PhysiCell Scene Explorer

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ModParser: MonoBehaviour
{
	GameObject[] modificationZones;
	GameObject controller;
	public void Start(){
		controller = GameObject.Find("Controller");
	}
	
	// parse the mod file at the specific file path
	public void parseMod(string filepath){
		try{
			var sr = new StreamReader(filepath);
			string line;
			
			// first, read the number of modification zones and allocate that much space
			line = sr.ReadLine();
			if(line != null){
				modificationZones = new GameObject[Int32.Parse(line)];
			}
			
			// foreach modification zone, read in the corresponding lines and invoke the corresponding Modifier.loadFromFile(string)
			string metadata;
			string serialization = "";
			int numActiveModZones = 0;
			GameObject currentModZone;
			
			for(int i = 0; i < modificationZones.Length; i++){
				// read and interpret the metadata of the next mod zone
				serialization = "";
				metadata = "";
				metadata += sr.ReadLine();
				serialization += metadata;
				serialization += "\n";
				
				string[] pieces = metadata.Split(' ');
				string type = pieces[0];
				int linesToRead = Int32.Parse(pieces[1]);
				/*
				bool isReferenceModZone = (type[0] == '#');
				if(isReferenceModZone){
					type = type.Substring(1);
				}
				*/

				// read in the actual data for the mod zone
				for(int j = 0; j < linesToRead; j++){
					serialization += sr.ReadLine();
					if((j + 1) != linesToRead){
						serialization += "\n";
					}
				}
				
				currentModZone = loadModZone(serialization);

				modificationZones[numActiveModZones] = currentModZone;
				numActiveModZones++;
				
				/*
				// // place the mod zone modification zone array or do nothing (for now)
				// currentModZone = Resources.Load<GameObject>("Modification Zones/" + type);
				if(isReferenceModZone){
					// don't do anything (for now)
				}
				else{
					// put the zone in the modification zones array
					currentModZone = Instantiate(currentModZone);
					modificationZones[numActiveModZones] = currentModZone;
					numActiveModZones++;
				}
			
				// // assemble the mod zone
				// currentModZone.GetComponent<Modifier>().loadFromString(serialization);
				*/
			}
		}
		catch(FileNotFoundException fe){
			Debug.Log(fe);
		}
		
		// remove all of the null entries in modification zones array
		int numNonNull = 0;
		foreach(GameObject modZone in modificationZones){
			if(modZone != null){
				numNonNull++;
			}
		}
		
		GameObject[] nonNullZones = new GameObject[numNonNull];
		int nonNullIndex = 0;
		foreach(GameObject modZone in modificationZones){
			if(modZone != null){
				nonNullZones[nonNullIndex++] = modZone;
			}
		}
		
		modificationZones = nonNullZones;

		/*
		GameObject modZone = Instantiate(Resources.Load<GameObject>("Modification Zones/Colorer"));
		modZone.transform.position = new Vector3(485, -341, -359);
		modZone.transform.localScale = new Vector3(1000, 1000, 1000);
		modZone.GetComponent<Colorer>().recolor = Color.red;

		modificationZones = new GameObject[1]{modZone};
		*/
	}
	
	public GameObject loadModZone(string serialization){
		Debug.Log("in modParser\n" + serialization);
		string[] lines = serialization.Split('\n');
		string type = lines[0].Split(' ')[0];
		serialization = "";
		for(int i = 1; i < lines.Length; i++){
			serialization += lines[i];
			if(i != lines.Length - 1){
				serialization += "\n";
			}
		}
				
		// load the proper modification zone
		GameObject modZone = Resources.Load<GameObject>("Modification Zones/" + type);
		Debug.Log("|" + type + "|" + modZone);
		modZone = Instantiate(modZone);
		// assemble the mod zone
		modZone.GetComponent<Modifier>().loadFromString(serialization);
		
		// turn off the mod zone's visibility
		controller.GetComponent<Controller>().disableModificationZoneVisibility();
		
		return modZone;
	}
	
	public void writeMod(string filepath, GameObject[] modZones){
		// creates, opens, and closes file
		using(StreamWriter file = new StreamWriter(filepath)){
			// print the metadata
			file.WriteLine(modZones.Length);
			
			// print each modification zone's serialization
			foreach(GameObject modZone in modZones){
				file.WriteLine(modZone.GetComponent<Modifier>().writeToString());
			}
			file.Close();
		}
	}
	
	public GameObject[] getModificationZones(){
		return modificationZones;
	}
}
