using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterAI : MonoBehaviour {

	public Transform debugWaypoint;
	public int preferredClass = 0;
	public float pointTolerance;
	public float engageRange;
	public float idealRange;
	public float desiredSpeed;
	public BaseShip target;
	public Vector3[] waypoints = new Vector3[0];

	private float strafeTimer;
	private bool attackBack = false;
	private Vector3 tempWaypoint;
	private Vector3 targetLeadPos;
	private Rigidbody rb;
	private BaseShip bs;

	// Use this for initialization
	void Start () {
		bs = GetComponent<BaseShip> ();
		rb = GetComponent<Rigidbody> ();

		// For testing
		if (debugWaypoint != null) {
			waypoints = new Vector3[1];
			waypoints [0] = debugWaypoint.position;
		}
	}

	// Update is called once per frame
	void Update () {
		if (target == null) {
			GetNearestTarget (bs.isPlayerSide);
		} else {
			float targetDistance = (target.transform.position - transform.position).magnitude;
			if (targetDistance > engageRange) {
				target = null;
			}
		}
	}

	void FixedUpdate() {
		if (waypoints.Length > 0 && (!attackBack || target == null)) {
			if ((waypoints [0] - transform.position).magnitude <= pointTolerance) {
				RemoveWaypoint ();
			} else {
				if (!Pathfinding (waypoints[0])) {
					LookTowardsTarget (waypoints [0]);
					bs.FlightAssist((waypoints[0] - transform.position).normalized * desiredSpeed);
				}
			}
		} else if (target != null) {
			LookTowardsTarget (target.transform.position);
			if (!Pathfinding (target.transform.position)) {
				Attack ();
			}
		} else {
			bs.FlightAssist (Vector3.zero);
		}
	}

	/* AI Methods here */

	// Check if there is an obstacle in the way, if yes, then return true
	bool Pathfinding(Vector3 point) {
		//Vector3 pointDir = (point - transform.position).normalized;
		float scale = 3f;
		float checkDistance = rb.velocity.magnitude;
		checkDistance = Mathf.Clamp (checkDistance, 50f, Mathf.Infinity);
		bool hasToMove = false;
		RaycastHit hit;
		int layermask = 1 << 8;
		layermask = ~layermask;
		if (Physics.SphereCast (transform.position, bs.shipSize * scale, transform.forward, out hit, checkDistance, layermask)) {
			hasToMove = true;
			Vector3 avoidDirection = chooseAvoidDir ((hit.point - hit.collider.gameObject.transform.position)).normalized;
			bs.FlightAssist (avoidDirection * desiredSpeed);
		} else if (Physics.Raycast (transform.position, transform.forward, out hit, checkDistance, layermask)) {
			hasToMove = true;
			Vector3 avoidDirection = chooseAvoidDir ((hit.point - hit.collider.gameObject.transform.position)).normalized;
			bs.FlightAssist (avoidDirection * desiredSpeed);
		} else if (Physics.SphereCast (transform.position, bs.shipSize * scale, rb.velocity.normalized, out hit, checkDistance, layermask)) {
			hasToMove = true;
			rb.AddForce (-rb.velocity.normalized * bs.sideAccel);
		}

		return hasToMove;
	}

	Vector3 chooseAvoidDir(Vector3 avoidDir) {
		Vector3 finalDir = transform.InverseTransformVector (avoidDir);
		if (Mathf.Abs (finalDir.x) >= Mathf.Abs (finalDir.y)) {
			if (finalDir.x >= 0) {
				finalDir = transform.right;
			} else {
				finalDir = -transform.right;
			}
		} else {
			if (finalDir.y >= 0) {
				finalDir = transform.up;
			} else {
				finalDir = -transform.up;
			}
		}
		return finalDir;
	}

	// Slash attack
	void Attack() {
		// Do strafing stuff
		Vector3 relVel = (target.transform.position - transform.position).normalized;
		Vector3 dodgeDir = Vector3.zero;
		relVel = transform.InverseTransformVector (relVel);
		if (relVel.x >= 0) {
			dodgeDir += transform.right;
		} else {
			dodgeDir += -transform.right;
		}

		if (relVel.y >= 0) {
			dodgeDir += transform.up;
		} else {
			dodgeDir += -transform.up;
		}

		Vector3 targetDistance = target.transform.position - transform.position;
		if (targetDistance.magnitude >= idealRange) {
			bs.FlightAssist ((targetDistance.normalized + dodgeDir).normalized * (target.GetComponent<Rigidbody> ().velocity.magnitude + desiredSpeed));
		} else {
			bs.FlightAssist (dodgeDir * desiredSpeed);
		}
	}

	/* Helper Methods here */

	// Get the nearest target
	void GetNearestTarget(bool isOnPlayerSide) {
		float tempDistance = 0;
		float distance = 0;
		bool isFirst = true;
		BaseShip nearestTarget = null;
		BaseShip[] targets = FindObjectsOfType<BaseShip>();
		for (int i = 0; i < targets.Length; i++) {
			if (targets [i] != null) {
				if ((targets[i].isPlayerSide && !isOnPlayerSide) || (!targets[i].isPlayerSide && isOnPlayerSide)) {
					if (isFirst) {
						distance = (targets [i].transform.position - transform.position).magnitude;
						isFirst = false;
					}

					tempDistance = (targets [i].transform.position - transform.position).magnitude;
					if (tempDistance <= distance && tempDistance <= engageRange && targets[i].shipClass == preferredClass) {
						distance = tempDistance;
						nearestTarget = targets [i];
					}
				}
			}
		}
		if (targets.Length > 0 && nearestTarget != null) {
			target = nearestTarget;
		}
	}

	// Look towards the target
	void LookTowardsTarget(Vector3 target) {
		Vector3 leadDir = target - transform.position;
		Vector3 newDir = Vector3.RotateTowards(transform.forward, leadDir, bs.rotateSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 0.0F);
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

	//first-order intercept using absolute target position
	public static Vector3 FirstOrderIntercept
	(
		Vector3 shooterPosition,
		Vector3 shooterVelocity,
		float shotSpeed,
		Vector3 targetPosition,
		Vector3 targetVelocity
	)  {
		Vector3 targetRelativePosition = targetPosition - shooterPosition;
		Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
		float t = FirstOrderInterceptTime
			(
				shotSpeed,
				targetRelativePosition,
				targetRelativeVelocity
			);
		return targetPosition + t*(targetRelativeVelocity);
	}

	//first-order intercept using relative target position
	public static float FirstOrderInterceptTime
	(
		float shotSpeed,
		Vector3 targetRelativePosition,
		Vector3 targetRelativeVelocity
	) {
		float velocitySquared = targetRelativeVelocity.sqrMagnitude;
		if(velocitySquared < 0.001f)
			return 0f;

		float a = velocitySquared - shotSpeed*shotSpeed;

		//handle similar velocities
		if (Mathf.Abs(a) < 0.001f)
		{
			float t = -targetRelativePosition.sqrMagnitude/
				(
					2f*Vector3.Dot
					(
						targetRelativeVelocity,
						targetRelativePosition
					)
				);
			return Mathf.Max(t, 0f); //don't shoot back in time
		}

		float b = 2f*Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
		float c = targetRelativePosition.sqrMagnitude;
		float determinant = b*b - 4f*a*c;

		if (determinant > 0f) { //determinant > 0; two intercept paths (most common)
			float	t1 = (-b + Mathf.Sqrt(determinant))/(2f*a),
			t2 = (-b - Mathf.Sqrt(determinant))/(2f*a);
			if (t1 > 0f) {
				if (t2 > 0f)
					return Mathf.Min(t1, t2); //both are positive
				else
					return t1; //only t1 is positive
			} else
				return Mathf.Max(t2, 0f); //don't shoot back in time
		} else if (determinant < 0f) //determinant < 0; no intercept path
			return 0f;
		else //determinant = 0; one intercept path, pretty much never happens
			return Mathf.Max(-b/(2f*a), 0f); //don't shoot back in time
	}
}
