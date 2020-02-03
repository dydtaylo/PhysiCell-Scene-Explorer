using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor;
using SimpleFileBrowser;

public class Controller : MonoBehaviour
{
	public GameObject mainCamera;
	public GameObject OriginObj;
	public GameObject cellPrefab;
	public GameObject modParser;
	public GameObject[] modificationZones;
	
	GameObject[] cells;
	string[] variableNames;

	int modificationZonesNeedUpdate;
	
    // Start is called before the first frame update
    void Start(){}
	
	// Update is called once per frame
    void Update(){
		// if applyModificationZones() is waiting to be called, decrement the flag until it is zero
		// the "reason" putting a delay on calling the function is explained in a comment at the end of loadModificationZones()
		if(modificationZonesNeedUpdate > 0){
			modificationZonesNeedUpdate--;
			if(modificationZonesNeedUpdate == 0){
				applyModificationZones();
			}
		}
	}
	
	IEnumerator ShowLoadDialogCoroutine(string cont)
	{
		// disable input while user selects a file
		mainCamera.GetComponent<FlyCamera>().disableInput();
		
		// Show a load file dialog and wait for a response from user
		yield return FileBrowser.WaitForLoadDialog( false, null, "Load File", "Load" );
		
		// once a file is selected
		if( FileBrowser.Success )
		{
			// turn input back on
			mainCamera.GetComponent<FlyCamera>().enableInput();
			
			// continue to the specified function
			if(cont.Equals("cells")){
				loadCellsFromFile(FileBrowser.Result);
			}
			else if(cont.Equals("save modification zones")){
				loadModificationZonesFromFile(FileBrowser.Result);
			}
			else if(cont.Equals("add modification zones")){
				addModificationZonesFromFile(FileBrowser.Result);
			}
		}
		
		// if the user closes the dialog without selecting an option
		if(!FileBrowser.IsOpen){
			// turn input back on
			mainCamera.GetComponent<FlyCamera>().enableInput();
		}
	}
	
	IEnumerator ShowSaveDialogCoroutine(string cont){
		// disable input while user selects a file
		mainCamera.GetComponent<FlyCamera>().disableInput();
		
		// show a save file dialog and wait for a user response
		yield return FileBrowser.WaitForSaveDialog(false, null, "Save File", "Save");
		
		// once a file is selected
		if(FileBrowser.Success){
			// turn input back on
			mainCamera.GetComponent<FlyCamera>().enableInput();
			
			// continue to the specified function
			if(cont.Equals("modification zones")){
				saveModificationZonesToFile(FileBrowser.Result);
			}
			else if(cont.Equals("screenshot")){
				saveScreenshotToFile(FileBrowser.Result);
			}
		}
		
		// if the user closes the dialog without selecting an option
		if(!FileBrowser.IsOpen){
			// turn input back on
			mainCamera.GetComponent<FlyCamera>().enableInput();
		}
	}
	
	// invokes the coroutine that yields until the user selects a file
	public void loadCells(){
		StartCoroutine( ShowLoadDialogCoroutine("cells") );
	}
	
	// loads cells from the given file
	public void loadCellsFromFile(string path){
		// clear all of the previous cells so the new ones can be loaded
		if(cells != null){
			foreach(GameObject cell in cells){
				Destroy(cell);
			}
		}
		
		var xmlParser = new XmlParser();
		var matParser = new MatParser();
		
		try{
			// string path = EditorUtility.OpenFilePanel("Select XML or MAT File", "", "");
			string physicellCellsPath;
			
			
			if(!path.Contains(".xml") && !path.Contains(".XML")){
				if(path.Contains(".mat") || path.Contains(".MAT")){
					physicellCellsPath = path;
					// just used the default variables
					variableNames = new string[32]{"ID", "position", "position", "position", "total_volume", 
											   "cell_type", "cycle_model", "current_phase", "elapsed_time_in_phase", 
											   "nuclear_volume", "cytopasmic_volume", "fluid_fraction", "calcified_fraction",
											   "orientation", "orientation", "orientation", "polarity", "migration_speed",
											   "motility_vector", "motility_vector", "motility_vector", "migration_bias",
											   "motility_bias_direction", "motility_bias_direction", "motility_bias_direction",
											   "persistence_time", "motility_reserved", "oncoprotein", "elastic_coefficient",
											   "kill_rate", "attachment_lifetime", "attachment_rate"};
				}
				else{
					throw new System.Exception("wrong file type, wanted XML or MAT");
				}
			}
			else{
				xmlParser.parseXML(path);
				// use the MAT file referenced in the XML file
				physicellCellsPath = xmlParser.getPhysicellCellsPath();
				// use the variables specified by the XML file
				variableNames = xmlParser.getVariableNames();
			}
			
			// parser.GetComponent<MatParser>().parseMat("Assets/Parsing Test Data/output00003696_cells_physicell.mat");
			
			// invoke the parse function in our MatParser
			matParser.parseMat(physicellCellsPath);
			// once the pare function has been invoked, we can access that matData
			float[,] data = matParser.getData();
			
			cells = new GameObject[greatestID(data) + 1];
			Vector3 origin = OriginObj.transform.position;
			
			// initialize all of the modification zones
			foreach(GameObject modZone in modificationZones){
				modZone.GetComponent<Modifier>().init();
			}
			
			float[] cellDataCol;
			int numCells = data.GetLength(1);
			for(int i = 0; i < numCells; i++){
				// get the current col of data
				cellDataCol = getDataColumn(data, i);
				// initialize each cell and place it in the scene and in cell array
				try{
					Vector3 pos = new Vector3(cellDataCol[1] + origin.x, cellDataCol[2] + origin.y, cellDataCol[3] + origin.z);
					// occasionally, cells are given an infinite for one of their coordinates, so we just ignore those cases
					if(pos.x > 100000 || pos.y > 100000 || pos.z > 100000){
						continue;
					}
					
					// create the cell
					GameObject g = Instantiate(cellPrefab, pos, Quaternion.identity);
					float radius = (float) (Math.Pow(cellDataCol[4]/((4f/3f) * Math.PI), 1f/3f));
					g.transform.localScale = new Vector3(radius, radius, radius);
					if(g == null){
						Debug.Log("ERROR at cell id: " + cellDataCol[0]);
					}
					
					// place the cell in our cells array
					cells[(int) cellDataCol[0]] = g;
					
					// give the cell a random color (just for fun)
					// g.GetComponent<Renderer>().material.SetColor("_Color", new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)));
					
					// place the cell in the correct position in the scene and give it the appropriate variable names
					g.GetComponent<CellData>().init(pos, variableNames);
					
					// instantiate CellData variable dictionary
					int variableIndex = 0;
					foreach(string variableName in variableNames){
						g.GetComponent<CellData>().addDataPoint(variableName, cellDataCol[variableIndex]);
						variableIndex++;
					}
					
					
					// apply all modification zones
					foreach(GameObject modZone in modificationZones){
						// if the cell is inside of modZone, apply modZone's modify function to it
						if(modZone.GetComponent<Collider>().bounds.Contains(g.GetComponent<CellData>().getRealPosition())){
							modZone.GetComponent<Modifier>().modify(g);
						}
					}
				}
				catch(Exception e){
					Debug.Log("Exception " + e +  " at cell id: " + cellDataCol[0]);
				}
			}
		}
		catch(Exception fe){
			Debug.Log(fe);
		}
	}
	
	public void loadModificationZones(){
		StartCoroutine(ShowLoadDialogCoroutine("save modification zones"));
	}
	public void loadModificationZonesFromFile(string path){
		try{
			// string path = EditorUtility.OpenFilePanel("Select Modification Zone Data File", "", "");
			modParser.GetComponent<ModParser>().parseMod(path);
			
			// remove the old modification zones
			foreach(GameObject modZone in modificationZones){
				Destroy(modZone);
			}
			
			// get the new modification zones
			modificationZones = modParser.GetComponent<ModParser>().getModificationZones();
		}
		catch(FileNotFoundException fe){
			Debug.Log(fe);
		}
		
		// update the scene with the new modification zones
		disableModificationZoneVisibility();
		// why not just call applyModificationZones() ? 
		// well, for some reason unity needs a couple frames to make the new modification zones work
		// idk why
		modificationZonesNeedUpdate = 2;
	}
	
	public void addModificationZones(){
		StartCoroutine(ShowLoadDialogCoroutine("add modification zones"));
	}
	public void addModificationZonesFromFile(string path){
		try{
			// string path = EditorUtility.OpenFilePanel("Select Modification Zone Data File", "", "");
			modParser.GetComponent<ModParser>().parseMod(path);
			
						
			// add the old and new modification zones
			GameObject[] newModZones = modParser.GetComponent<ModParser>().getModificationZones();
			GameObject[] oldModZones = modificationZones;
			modificationZones = new GameObject[newModZones.Length + oldModZones.Length];
			
			// add the old ones
			for(int i = 0; i < oldModZones.Length; i++){
				modificationZones[i] = oldModZones[i];
			}
			// add the new ones
			for(int i = 0; i < newModZones.Length; i++){
				modificationZones[oldModZones.Length + i] = newModZones[i];
			}
		}
		catch(FileNotFoundException fe){
			Debug.Log(fe);
		}
		
		// update the scene with the new modification zones
		disableModificationZoneVisibility();
		// why not just call applyModificationZones() ? 
		// well, for some reason unity needs a couple frames to make the new modification zones work
		// idk why
		modificationZonesNeedUpdate = 2;
	}
	
	public void saveModificationZones(){
		StartCoroutine(ShowSaveDialogCoroutine("modification zones"));
	}
	public void saveModificationZonesToFile(string path){
		try{
			// string path = EditorUtility.SaveFilePanel("Save Modification Zones File", "", "modZones", "txt");
			modParser.GetComponent<ModParser>().writeMod(path, modificationZones);
		}
		catch(Exception fe){
			Debug.Log(fe);
		}
	}
	
	public void saveScreeshot(){
		StartCoroutine(ShowSaveDialogCoroutine("screenshot"));
	}
	public void saveScreenshotToFile(string path){
		ScreenCapture.CaptureScreenshot(path + ".png", 2);
	}
	
	// reset all cells to starting positions and color and enabled all of their renderers
	// note: this might need to be updated to account for the various other transformations that could be done by a modification zone (e.g. scale, rotation, etc.)
	public void resetCells(){
		foreach(GameObject g in cells){
			if(g != null){
				g.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
				g.transform.position = g.GetComponent<CellData>().getRealPosition();
				g.GetComponent<Renderer>().enabled = true;
			}
		}
	}
	
	// for each cell, check if it is in any modification zone and, if it is, apply that modification zone's modify function to the cell
	public void applyModificationZones(){
		resetCells();
		foreach(GameObject modZone in modificationZones){
			modZone.GetComponent<Modifier>().init();
		}
		
		foreach(GameObject g in cells){
			if(g != null){
				// apply all modification zones
				foreach(GameObject modZone in modificationZones){
					// if the cell is inside of modZone, apply modZone's modify function to it
					if(modZone.GetComponent<Collider>().bounds.Contains(g.GetComponent<CellData>().getRealPosition())){
						modZone.GetComponent<Modifier>().modify(g);
					}
				}
			}
		}
	}
	
	// toggle each modification zone's mesh renderer
	public void toggleModificationZoneVisibility(){
		foreach(GameObject modZone in modificationZones){
			Renderer renderer = modZone.GetComponent<MeshRenderer>();
			renderer.enabled = !renderer.enabled;
		}
	}
	
	// disable each modification zone's mesh renderer
	public void disableModificationZoneVisibility(){
		foreach(GameObject modZone in modificationZones){
			Renderer renderer = modZone.GetComponent<MeshRenderer>();
			renderer.enabled = false;
		}
	}
	
	// absolute value
	float abs(float num){
		if(num > 0){
			return num;
		}
		
		return num * -1;
	}
	
	// get the greatest ID among all cells, used to determine the necesssary size of cells array
	int greatestID(float[,] data){
		float greatest = Single.MinValue;
		for(int i = 0; i < data.GetLength(1); i++){
			if(data[0, i] > greatest){
				greatest = data[0, i];
			}
		}
		
		return (int) greatest;
	}
	
	// get a columt of data from our parse mat file data
	// a column represents a cell's data
	float[] getDataColumn(float[,] data, int col){
		int colLength = data.GetLength(0);
		float[] dataColumn = new float[colLength];
		for(int i = 0; i < colLength; i++){
			dataColumn[i] = data[i, col];
		}
		return dataColumn;
	}
	
	
	// convert a string of comma separated floats into an array of floats
	// not used as of implementation of MatParser.cs
	float[] getFloatArr(string source, string delim){
		string[] sourceTokens = source.Split(delim.ToCharArray());
		float[] nums = new float[sourceTokens.Length];
		for(int i = 0; i < sourceTokens.Length; i++){
			try{
				nums[i] = float.Parse(sourceTokens[i]);
			}
			catch(Exception e){
				Debug.Log(e);
			}
		}
		return nums;
	}
	// convert a string of comma separated ints into an array of ints
	// not used as of implementation of MatParser.cs
	int[] getIntArr(string source, string delim){
		string[] sourceTokens = source.Split(delim.ToCharArray());
		int[] nums = new int[sourceTokens.Length];
		for(int i = 0; i < sourceTokens.Length; i++){
			try{
				nums[i] = int.Parse(sourceTokens[i]);
			}
			catch(Exception e){
				Debug.Log(e);
			}
		}
		return nums;
	}
}
