using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contact : MonoBehaviour {

	public int type;

	// Use this for initialization
	void Start () {

		// 0 is bad, 1 is good, other is projectile
		LineRenderer lr = GetComponent<LineRenderer>();
		Radar rd = GetComponentInParent<Radar> ();
		lr.SetPosition (0, transform.position);
		Vector3 dir = rd.transform.position - transform.position;
		dir = rd.transform.InverseTransformVector (dir);
		dir = new Vector3 (0, dir.y, 0);
		dir = rd.transform.TransformVector (dir);
		lr.SetPosition (1, transform.position + dir);

		if (type == 0) {
			lr.startColor = Color.red;
			lr.endColor = Color.red;
		} else if (type == 1) {
			lr.startColor = Color.blue;
			lr.endColor = Color.blue;
		} else {
			lr.startColor = Color.yellow;
			lr.endColor = Color.yellow;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
