using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour {

	public float heatCost;
	public float primaryCost;
	public float primaryCool;
	public float primarySpeed;
	public float primaryRange;
	public float secondaryCool;
	public float secondarySpeed;
	public float secondaryRange;

	public Transform[] weps1 = new Transform[1];
	public Transform[] weps2 = new Transform[1];
	public GameObject primaryObj;
	public GameObject secondaryObj;
	public float maxBatt;
	public BaseShip target;

	protected float batt;
	protected float primaryTimer;
	protected float secondaryTimer;
	protected bool canFireP = true;
	protected bool canFireS = true;
	protected GameObject launch1;
	protected GameObject launch2;

	// Use this for initialization
	void Start () {
		batt = maxBatt;
		primaryTimer = primaryCool;
		secondaryTimer = secondaryCool;
		AdditionalStart ();
	}
	
	// Update is called once per frame
	void Update () {
		
		primaryTimer += Time.deltaTime;
		primaryTimer = Mathf.Clamp (primaryTimer, 0, primaryCool);

		secondaryTimer += Time.deltaTime;
		secondaryTimer = Mathf.Clamp (secondaryTimer, 0, secondaryCool);


		if (primaryTimer >= primaryCool && batt >= primaryCost) {
			canFireP = true;
		} else {
			canFireP = false;
		}

		if (secondaryTimer >= secondaryCool && batt >= 1f) {
			canFireS = true;
		} else {
			canFireS = false;
		}

		AdditionalUpdate ();

	}

	public void setRotations(Transform dir) {
		for (int i = 0; i < weps1.Length; i++) {
			if (weps1 [i] != null) {
				weps1 [i].transform.localRotation = dir.transform.localRotation;
			}
		}
	}

	public virtual void Fire1() {
	}

	public virtual void Fire2() {
	}

	public virtual void Release1() {
	}

	public virtual void Release2() {
	}

	public virtual void Ability() {
	}

	public virtual void AdditionalUpdate() {
	}

	public virtual void AdditionalStart() {
	}

	public virtual float ReturnIsRdy() {
		return primaryTimer / primaryCool;
	}

	public virtual float ReturnBatt() {
		return batt;
	}

}
