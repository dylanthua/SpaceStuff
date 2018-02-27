using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour {

	public float physicalSize;
	public float scale = 1f;
	public GameObject goodContact;
	public GameObject badContact;
	public GameObject projectile;

	private BaseShip[] ships = new BaseShip[0];
	private GameObject[] contacts = new GameObject[0];
	private float detectionRange;

	// Use this for initialization
	void Start () {
		detectionRange = GetComponentInParent<PlayerWepController> ().detectionRange;
	}
	
	// Update is called once per frame
	void Update () {
		if (Camera.main != null) {
			for (int i = 0; i < contacts.Length; i++) {
				if (contacts [i] != null) {
					Destroy (contacts [i]);
				}
			}

			ships = FindObjectsOfType<BaseShip> ();
			contacts = new GameObject[ships.Length];
			for (int i = 0; i < ships.Length; i++) {
				if ((ships [i].transform.position - transform.position).magnitude > detectionRange * scale) {
					contacts [i] = null;
				} else if (ships [i].gameObject.tag.Equals ("Player")) {
					contacts [i] = null;
				} else {
					Vector3 contactDir = ships [i].transform.position - transform.position;
					float contactMag = contactDir.magnitude;
					contactDir.Normalize ();
					contactMag *= physicalSize / (detectionRange * scale);
					contactDir *= contactMag;
					Vector3 finalPos = transform.position + contactDir;

					if (ships [i].isPlayerSide) {
						contacts [i] = Instantiate (goodContact, finalPos, Camera.main.transform.rotation, transform);
					} else {
						contacts [i] = Instantiate (badContact, finalPos, Camera.main.transform.rotation, transform);
					}
				}
			}
		}
	}

}
