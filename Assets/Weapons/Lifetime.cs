using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour {

	public ParticleSystem ps;
	public float lifetime;
	private float timer = 0;

	// Use this for initialization
	void Start () {
		if (ps != null) {
			ps.Play ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer >= lifetime) {
			Destroy (gameObject);
		}
	}
}
