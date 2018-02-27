using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWepController : MonoBehaviour {

	public float detectionRange = 1000f;
	public BaseShip target;
	public Transform dir;

	private Rigidbody rb;
	private Vector3 targetLeadPos;
	private TurretAI[] turrAI;
	private Vector3 crosshairPos;
	private int targetIndex = 0;

	// Use this for initialization
	void Start () {
		turrAI = GetComponentsInChildren<TurretAI> ();
		rb = GetComponent<Rigidbody> ();
	}

	void OnGUI() {

	}
	
	// Update is called once per frame
	void Update () {

		if (target != null) {
			Rigidbody targetRb = target.gameObject.GetComponent<Rigidbody> ();
			if (turrAI.Length > 0) {
				targetLeadPos = FirstOrderIntercept
					(
					transform.position,
					rb.velocity,
					turrAI [0].wep.primarySpeed,
					target.transform.position,
					targetRb.velocity
				);
			} else {
				targetLeadPos = Vector3.zero;
			}

			float targetDistance = (target.transform.position - transform.position).magnitude;
			if (targetDistance > detectionRange) {
				target = null;
			}

		}

		for (int i = 0; i < turrAI.Length; i++) {
			turrAI [i].wep.target = target;
			if (Input.GetButton ("Fire1") && turrAI[i].turretGroup == 1) {
				turrAI[i].Fire ();
			} else if (Input.GetButton ("Fire2") && turrAI[i].turretGroup == 2) {
				turrAI[i].Fire ();
			}
		}


		// Target nearest enemy
		if (Input.GetButtonDown ("TargetNearest")) {
			float tempDistance = 0;
			float distance = 0;
			bool isFirst = true;
			BaseShip nearestTarget = null;
			BaseShip[] targets = FindObjectsOfType<BaseShip>();
			for (int i = 0; i < targets.Length; i++) {
				if (targets [i] != null && !targets [i].isPlayerSide) {
					if (isFirst) {
						distance = (targets [i].transform.position - transform.position).magnitude;
						isFirst = false;
					}

					tempDistance = (targets [i].transform.position - transform.position).magnitude;
					if (tempDistance <= distance && tempDistance <= detectionRange) {
						distance = tempDistance;
						nearestTarget = targets [i];
						targetIndex = i;
					}
				}
			}
			if (targets.Length > 0 && nearestTarget != null) {
				target = nearestTarget;
			}
		}

		// Cycle through targets
		if (Input.GetButtonDown ("TargetCycle")) {
			BaseShip[] targets = FindObjectsOfType<BaseShip> ();
			bool areThereTargets = false;

			for (int i = 0; i < targets.Length; i++) {
				float distance = (targets [i].transform.position - transform.position).magnitude;
				if (targets [i] != null && !targets [i].isPlayerSide && distance <= detectionRange) {
					areThereTargets = true;
				}
			}
				
			if (targets.Length > 0 && areThereTargets) {
				targetIndex++;
				if (targetIndex >= targets.Length) {
					targetIndex = 0;
				}

				float distance = (targets [targetIndex].transform.position - transform.position).magnitude;
				while (targets [targetIndex] == null || targets[targetIndex].isPlayerSide || distance > detectionRange) {
					targetIndex++;
					if (targetIndex >= targets.Length) {
						targetIndex = 0;
					}
					distance = (targets [targetIndex].transform.position - transform.position).magnitude;
				}
				target = targets [targetIndex];
			}
		}

		// Target nearest to center
		if (Input.GetButtonDown ("TargetCursor")) {
		}
			
	}

	public TurretAI[] GetTurrets() {
		return turrAI;
	}

	public Vector3 GetTargetLead() {
		return targetLeadPos;
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
