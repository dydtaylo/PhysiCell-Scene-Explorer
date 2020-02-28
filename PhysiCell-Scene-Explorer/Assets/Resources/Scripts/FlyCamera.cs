// Dylan Taylor
// PhysiCell Scene Explorer

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyCamera : MonoBehaviour {
 
    /*
    Writen by Windexglow 11-13-10.  Use it, edit it, steal it I don't care.  
    Converted to C# 27-02-13 - no credit wanted.
    Simple flycam I made, since I couldn't find any others made public.  
    Made simple to use (drag and drop, done) for regular keyboard layout  
    wasd : basic movement
    shift : Makes camera accelerate
    space : Moves camera on X and Z axis only.  So camera doesn't gain any height*/
     
    float mainSpeed = 100.0f; //regular speed
    float shiftAdd = 250.0f; //multiplied by how long shift is held.  Basically running
    float maxShift = 1000.0f; //Maximum speed when holdin gshift
    float camSens = 0.25f; //How sensitive it with mouse
    private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
    private Vector3 arrowOffset = new Vector3(0, 0, 0);
	private float totalRun= 1.0f;
	
	public GameObject controller;
	public GameObject canvas;
	Text[] text;
	bool inputOn;
	bool mouseMovementEnabled = true;
    float zoomIncrement = 1;
	
	// Start is called before the first frame update
	void Start(){
		// enable input
		inputOn = true;
		
		// assemble text object array
		text = new Text[canvas.transform.childCount];
		int i = 0;
		foreach(Transform child in canvas.transform){
			text[i++] = child.GetComponent<Text>();
		}
		
		Cursor.visible = true;
		
		// tell the controller to load initial cells
		controller.GetComponent<Controller>().loadCells();
	}
	
	// Update is called once per frame
    void Update () {
		if(inputOn){
			Cursor.visible = false;
			if(mouseMovementEnabled){
				lastMouse = Input.mousePosition - lastMouse ;
				lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0 );
				lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x + arrowOffset.x , transform.eulerAngles.y + lastMouse.y + arrowOffset.y, 0);
			}
			else{
				lastMouse = lastMouse = new Vector3(transform.eulerAngles.x + arrowOffset.x , transform.eulerAngles.y + arrowOffset.y, 0);
			}
			transform.eulerAngles = lastMouse;
			lastMouse =  Input.mousePosition;
		}
        //Mouse camera angle done.  
       
        //Keyboard commands
        // float f = 0.0f;
        Vector3 p = GetBaseInput();
        if (Input.GetKey (KeyCode.LeftShift)){
            totalRun += Time.deltaTime;
            p  = p * totalRun * shiftAdd;
            p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
            p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
            p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
        }
        else{
            totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
            p = p * mainSpeed;
        }
       
       p = p * Time.deltaTime;
       Vector3 newPosition = transform.position;
       transform.Translate(p);
       
	   // update the text display
	   text[0].text = "x-pos: " + newPosition.x;
	   text[1].text = "y-pos: " + newPosition.y;
	   text[2].text = "z-pos: " + newPosition.z;
    }
     
    private Vector3 GetBaseInput() { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
		if(inputOn){
			// movement controls
			if (Input.GetKey (KeyCode.W)){
				p_Velocity += new Vector3(0, 0 , 1);
			}
			if (Input.GetKey (KeyCode.S)){
				p_Velocity += new Vector3(0, 0, -1);
			}
			if (Input.GetKey (KeyCode.A)){
				p_Velocity += new Vector3(-1, 0, 0);
			}
			if (Input.GetKey (KeyCode.D)){
				p_Velocity += new Vector3(1, 0, 0);
			}
			
			// parts I have added //
			
			// camera directional controls
			arrowOffset = new Vector3(0, 0, 0);
			if(Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.Z)){
				arrowOffset.x -= 5;
			}
			if(Input.GetKey(KeyCode.LeftArrow)){
				arrowOffset.y -= 5;
			}
			if(Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.Z)){
				arrowOffset.x += 5;
			}
			if(Input.GetKey(KeyCode.RightArrow)){
				arrowOffset.y += 5;
			}
			
			// zoom controls
			var scrollAmt = Input.GetAxis("Mouse ScrollWheel");
			if (scrollAmt > 0 || (Input.GetKey(KeyCode.Z) && Input.GetKey(KeyCode.UpArrow))){
				 GetComponent<Camera>().fieldOfView -= zoomIncrement;
			}
			else if (scrollAmt < 0 || (Input.GetKey(KeyCode.Z) && Input.GetKey(KeyCode.DownArrow))){
				 GetComponent<Camera>().fieldOfView += zoomIncrement;
			}
			
			// space to ascend
			if(Input.GetKey(KeyCode.Space)){
				p_Velocity += new Vector3(0, 1, 0);
			}
			// left control to descend
			if(Input.GetKey(KeyCode.LeftControl)){
				p_Velocity += new Vector3(0, -1, 0);
			}
			// h to toggle HUD
			if(Input.GetKey(KeyCode.H)){
				if(canvas.activeSelf){
					canvas.SetActive(false);
				}
				else{
					canvas.SetActive(true);
				}
			}	
			// k to take a screen shot
			if(Input.GetKey(KeyCode.K)){
				Cursor.visible = true;
				controller.GetComponent<Controller>().saveScreeshot();
			}
			
			
			// p to toggle mouseMovementEnabled
			if(Input.GetKey(KeyCode.P)){
				mouseMovementEnabled = !mouseMovementEnabled;
			}
			
			
			// m to toggle modification zone visibility
			if(Input.GetKey(KeyCode.M)){
				controller.GetComponent<Controller>().toggleModificationZoneVisibility();
			}
			
			// r to switch cell data files
			if(Input.GetKey(KeyCode.R)){
				Cursor.visible = true;
				controller.GetComponent<Controller>().loadCells();
				// reposition the camera to the origin after loading new cells
				transform.position = controller.GetComponent<Controller>().OriginObj.transform.position;
			}
			// use t to switch modification zone files
			if(Input.GetKey(KeyCode.T)){
				Cursor.visible = true;
				controller.GetComponent<Controller>().loadModificationZones();
			}
			// use g to add modification zones
			if(Input.GetKey(KeyCode.G)){
				Cursor.visible = true;
				controller.GetComponent<Controller>().addModificationZones();
			}
			// use b to save the current modification zones to a file
			if(Input.GetKey(KeyCode.B)){
				Cursor.visible = true;
				controller.GetComponent<Controller>().saveModificationZones();
			}
			
			// use y to reset the cells
			if(Input.GetKey(KeyCode.Y)){
				controller.GetComponent<Controller>().resetCells();
			}
			// use u to reapply the modification zones
			if(Input.GetKey(KeyCode.U)){
				controller.GetComponent<Controller>().applyModificationZones();
			}
			
			// use Esc to close the application
			if(Input.GetKey(KeyCode.Escape)){
				Application.Quit();
			}
		}
		
        return p_Velocity;
    }
	
	public void enableInput(){
		inputOn = true;
	}
	public void disableInput(){
		inputOn = false;
	}
}