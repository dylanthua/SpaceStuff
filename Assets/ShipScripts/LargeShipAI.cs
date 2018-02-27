using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeShipAI : MonoBehaviour {

	public bool isSelected;
	public float pointTolerance;
	public bool broadsides;
	public int behavior;
	public float optimalRange;
	public BaseShip target;
	public Vector3[] waypoints = new Vector3[0];

	private BaseShip bs;

	// Use this for initialization
	void Start () {
		bs = GetComponent<BaseShip> ();
	}

	// Update is called once per frame
	void Update () {
		if (bs.isPlayerSide) {
			// Draw lines between waypoints
		}
	}

	void FixedUpdate() {

		// Check if we have arrived at a waypoint
		if (waypoints.Length > 0) {
			float distance = (transform.position - waypoints [0]).magnitude;
			if (distance <= pointTolerance) {
				RemoveWaypoint ();
			}
		}

		// Movement Handler

	}

	/* Helper Methods here */


	// Get the nearest target
	void GetNearestTarget(bool isOnPlayerSide) {
		float tempDistance = 0;
		float distance = 0;
		bool isFirst = true;
		LargeShipAI nearestTarget = null;
		LargeShipAI[] targets = FindObjectsOfType<LargeShipAI>();
		for (int i = 0; i < targets.Length; i++) {
			if (targets [i] != null) {
				BaseShip targetBS = targets [i].GetComponent<BaseShip> ();
				if ((targetBS.isPlayerSide && !isOnPlayerSide) || (!targetBS.isPlayerSide && isOnPlayerSide)) {
					if (isFirst) {
						distance = (targets [i].transform.position - transform.position).magnitude;
						isFirst = false;
					}

					tempDistance = (targets [i].transform.position - transform.position).magnitude;
					if (tempDistance <= distance) {
						distance = tempDistance;
						nearestTarget = targets [i];
					}
				}
			}
		}
		if (targets.Length > 0 && nearestTarget != null) {
			target = nearestTarget.GetComponent<BaseShip>();
		}
	}

	// Look towards the target
	void LookTowardsTarget(Vector3 target) {
		Vector3 leadDir = target - transform.position;
		Vector3 newDir = Vector3.RotateTowards (transform.forward, leadDir, bs.rotateSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 0.0F);
		Vector3 newUp = Vector3.Cross (newDir, transform.right);
		transform.rotation = Quaternion.LookRotation(newDir, newUp);
	}

	public void AddWaypoint(Vector3 newWaypoint) {
		Vector3[] newArray = new Vector3[waypoints.Length + 1];
		for (int i = 0; i < newArray.Length; i++) {
			if (i >= newArray.Length - 1) {
				newArray [i] = newWaypoint;
			} else {
				newArray [i] = waypoints [i];
			}
		}
		waypoints = newArray;
	}

	public void InsertWaypoint(Vector3 newWaypoint) {
		Vector3[] newArray = new Vector3[waypoints.Length + 1];
		for (int i = 0; i < newArray.Length; i++) {
			if (i == 0) {
				newArray [i] = newWaypoint;
			} else {
				newArray [i] = waypoints [i-1];
			}
		}
		waypoints = newArray;
	}

	public void RemoveWaypoint () {
		if (waypoints.Length > 0) {
			Vector3[] newArray = new Vector3[waypoints.Length - 1];
			for (int i = 1; i < waypoints.Length; i++) {
				newArray [i - 1] = waypoints [i];
			}
			waypoints = newArray;
		}
	}

	public void ClearWaypoints() {
		for (int i = 0; i < waypoints.Length; i++) {
			RemoveWaypoint ();
		}
	}
		
}
