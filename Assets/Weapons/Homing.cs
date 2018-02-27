using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homing : MonoBehaviour {

	public GameObject explode;
	public float speed;
	public float rotateSpeed;
	public BaseShip target;
	public float lifetime;

	Vector3 targetLeadPos;
	private float lifeTimer;
	private Rigidbody rb;
	private Rigidbody targetRb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}

	void Update() {
		lifeTimer += Time.deltaTime;
		if (lifeTimer >= lifetime) {
			Destroy (gameObject);
		}
	}

	void FixedUpdate() {
		if (target != null) {
			targetRb = target.GetComponent<Rigidbody> ();
			targetLeadPos = FirstOrderIntercept
			(
					transform.position,
					rb.velocity,
					speed,
					target.transform.position,
					targetRb.velocity
			);
			LookTowardsTarget (targetLeadPos);
		}
		rb.velocity = transform.forward * speed;
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

	// Helper methods below

	// Look towards the target
	void LookTowardsTarget(Vector3 target) {
		Vector3 leadDir = target - transform.position;
		Vector3 newDir = Vector3.RotateTowards(transform.forward, leadDir, rotateSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 0.0F);
		Vector3 newUp = Vector3.Cross (newDir, transform.right);
		transform.rotation = Quaternion.LookRotation(newDir, newUp);
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
