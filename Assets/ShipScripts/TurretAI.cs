using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAI : MonoBehaviour {

	public string turretName;

	public float maxHp;

	public int turretGroup = 1;
	public bool playerControl = false;
	public int preferredClass;
	public bool isPlayerSide;
	public int type;
	public float rotationLimit;
	public LargeShipAI parent;
	public Rigidbody rb;
	public float rotateSpeed = 180f;

	public bool isDisabled = false;
	private float hp;
	private FighterAI fighterParent;
	private Vector3 targetLeadPos;
	private Rigidbody targetRb;
	private PlayerControls pc;

	public Weapons wep;

	// Use this for initialization
	void Start () {
		hp = maxHp;
		wep = GetComponent<Weapons> ();
		parent = GetComponentInParent<LargeShipAI> ();
		fighterParent = GetComponentInParent<FighterAI> ();
		rb = GetComponentInParent<Rigidbody> ();
		isPlayerSide = GetComponentInParent<BaseShip> ().isPlayerSide;
		pc = GetComponentInParent<PlayerControls> ();
	}
	
	// Update is called once per frame
	void Update () {
		// Type 0 seeks out nearest target, any other is forward facing weapon
		if (type == 0) {
			GetNearestTarget (isPlayerSide, preferredClass);
		} else if (parent != null) {
			wep.target = parent.target;
		} else if (fighterParent != null) {
			wep.target = fighterParent.target;
		}
	}

	public void Damage(float damage, BaseShip who) {
		BaseShip bs = GetComponentInParent<BaseShip> ();
		if (bs.GetShields() <= 0) {
			hp -= damage;
			bs.Damage (0, who);
			if (hp <= 0) {
				isDisabled = true;
			}
		} else {
			bs.Damage (damage, who);
		}

		hp = Mathf.Clamp (hp, 0, maxHp);
	}

	void FixedUpdate() {
		if (wep.target != null && !isDisabled && (pc == null || !pc.isManControl())) {
			targetRb = wep.target.GetComponent<Rigidbody> ();
			targetLeadPos = FirstOrderIntercept
				(
				transform.position,
				rb.velocity,
				wep.primarySpeed,
				wep.target.gameObject.transform.position,
				targetRb.velocity
			);
			LookTowardsTarget (targetLeadPos);
			if ((transform.position - targetLeadPos).magnitude < wep.primaryRange) {
				if (Vector3.Angle (transform.forward, (targetLeadPos - transform.position)) <= 0.5f) {
					if (!playerControl) {
						wep.Fire1 ();
					}
				}
			}
		} else if (playerControl && !isDisabled) {
			PlayerWepController pwc = GetComponentInParent<PlayerWepController> ();
			LookTowardsTarget (pwc.dir.position + pwc.dir.forward * 1000f);
		}
	}

	/* Helper Methods here */

	public void Fire() {
		wep.Fire1 ();
	}

	void ClampRotation(float amount) {
		float newX = ClampAngle (transform.localEulerAngles.x, amount);
		float newY = ClampAngle (transform.localEulerAngles.y, amount);
		float newZ = ClampAngle (transform.localEulerAngles.z, amount);
		transform.localEulerAngles = new Vector3 (newX, newY, newZ);
	}

	float ClampAngle(float angle, float clampAmount) {

		// Check if angle is in valid range
		if (angle >= clampAmount && angle <= 360 - clampAmount) {
			if (360 - angle >= 180) {
				return clampAmount;
			} else {
				return 360 - clampAmount;
			}
		}
		return angle;
	}

	// Get the nearest target
	void GetNearestTarget(bool isOnPlayerSide, int desiredClass) {
		float tempDistance = 0;
		float distance = 0;
		bool isFirst = true;

		BaseShip nearestTarget = null;
		BaseShip[] targets = FindObjectsOfType<BaseShip>();
		for (int i = 0; i < targets.Length; i++) {
			if (targets [i] != null) {
				if ((targets [i].isPlayerSide && !isOnPlayerSide) || (!targets [i].isPlayerSide && isOnPlayerSide)) {
					if (isFirst) {
						distance = (targets [i].transform.position - transform.position).magnitude;
						isFirst = false;
					}

					tempDistance = (targets [i].transform.position - transform.position).magnitude;
					if (tempDistance <= distance && targets[i].shipClass == desiredClass) {
						distance = tempDistance;
						nearestTarget = targets [i];
					}
				}
			}
		}
		if (targets.Length > 0 && nearestTarget != null) {
			wep.target = nearestTarget;
		}
	}

	// Look towards the target
	void LookTowardsTarget(Vector3 target) {
		Vector3 leadDir = target - transform.position;
		Vector3 newDir = Vector3.RotateTowards (transform.forward, leadDir, rotateSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 0.0F);
		transform.rotation = Quaternion.LookRotation(newDir, transform.up);
		ClampRotation (rotationLimit);
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
