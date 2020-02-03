// Dylan Taylor
// PhysiCell Scene Explorer

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Modifier
{
	void init();
    void modify(GameObject g);
	string writeToString();
	void loadFromString(string serialization);
}
