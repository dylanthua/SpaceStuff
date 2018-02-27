using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour {

	public float damage;

	private BaseShip whoFiredMe;

	// Use this for initialization
	void Start () {
		RaycastHit hit;
		if (Physics.Raycast (transform.position, -transform.forward, out hit)) {
			if (hit.collider.gameObject.GetComponent<BaseShip> () != null) {
				whoFiredMe = hit.collider.gameObject.GetComponent<BaseShip> ();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter (Collision col) 
	{
		BaseShip bs = col.gameObject.GetComponent<BaseShip> ();
		Destroyable ds = col.gameObject.GetComponent<Destroyable> ();
		if (bs == whoFiredMe) {
			return;
		}
		if (bs != null) {
			if (col.collider.CompareTag ("Turret")) {
				TurretAI ta = col.collider.GetComponent<TurretAI> ();
				if (ta != null) {
					ta.Damage (damage, whoFiredMe);
				}
			} else if (col.collider.CompareTag("Weak")) {
				bs.Damage (damage * 2, whoFiredMe);
			} else {
				bs.Damage (damage, whoFiredMe);
			}
		} else if (ds != null) {
			ds.Damage (damage);
		}
	}
}
