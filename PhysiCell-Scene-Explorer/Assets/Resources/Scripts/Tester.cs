using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		string str = "I prefer java  but c# is an       ok language too ";
		Scanner scan = new Scanner(str);
		
		while(scan.hasNext()){
			Debug.Log(scan.next());
		}
    }
}
