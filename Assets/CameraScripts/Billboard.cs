using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Camera.main != null) {
			//transform.forward = Camera.main.transform.forward;
			transform.rotation = Camera.main.transform.rotation;
		}
	}
}
