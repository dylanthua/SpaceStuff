using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour {

	public float hp;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Damage(float damage) {
		hp -= damage;
		if (hp <= 0) {
			Destroy (gameObject);
		}
	}
}
