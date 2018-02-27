using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public GameObject explode;
	public float speed;
	public float lifetime;

	private Rigidbody rb;
	private float lifeTimer;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		rb.AddForce (speed * transform.forward, ForceMode.VelocityChange);
	}
	
	// Update is called once per frame
	void Update () {
		lifeTimer += Time.deltaTime;
		if (lifeTimer >= lifetime) {
			Destroy (gameObject);
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		Explode ();
	}

	void Explode() {
		if (explode != null) {
			Instantiate (explode, transform.position, transform.rotation);
		}
		Destroy (gameObject);
	}
}
