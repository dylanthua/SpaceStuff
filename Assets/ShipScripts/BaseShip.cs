using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseShip : MonoBehaviour {

	public int scoreAmnt;
	public string shipName;

	public float shipSize = 5f;

	public GameObject explosion;

	public int shipClass;
	public bool isPlayerSide;

	public float dodgeStrength;
	public float dodgeCooldown;
	public float dodgeHeat;

	public float engineHeatOutput;

	public float accel;
	public float sideAccel;
	public float reverseAccel;

	public float rotateSpeed;
	public float rollSpeed;

	public float rotateAccel;
	public float rollAccel;

	public float maxHp;

	public float maxShields;
	public float shieldRegenRate;
	public float shieldRegenTime;
	public float shieldHeatOutput;

	public float maxHeat;
	public float heatDisperse;

	protected float engineThrottle = 1f;
	protected float shieldLevel = 1f;

	protected float heat;

	protected BaseShip attackedBy;

	protected float hp;
	protected float shields;

	protected float shieldTimer;

	protected Rigidbody rb;

	protected float dodgeTimer;
	protected bool canDodge = true;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		hp = maxHp;
		shields = maxShields;
		dodgeTimer = dodgeCooldown;
	}

	void Update() {

		heat += shieldHeatOutput * Time.deltaTime * shieldLevel; 

		if (shieldTimer >= shieldRegenTime + ((1-shieldLevel) * shieldRegenTime)) {
			shields += shieldRegenRate * Time.deltaTime * shieldLevel;
			shields = Mathf.Clamp (shields, 0, maxShields);
		} else {
			shieldTimer += Time.deltaTime;
		}

		if (dodgeTimer >= dodgeCooldown) {
			canDodge = true;
		} else {
			dodgeTimer += Time.deltaTime;
			canDodge = false;
		}

		heat -= heatDisperse * Time.deltaTime;
		heat = Mathf.Clamp (heat, 0, maxHeat);
		if (heat >= maxHeat) {
			Death ();
		}
	}

	void FixedUpdate () {
	}

	public void Damage(float amount, BaseShip who) {
		attackedBy = who;
		shieldTimer = 0;
		if (shields > 0) {
			shields -= amount;
			shields = Mathf.Clamp (shields, 0, maxShields);
		} else {
			hp -= amount;
			if (hp <= 0) {
				Death ();
			}
		}
	}

	void Death() {
		if (explosion != null) {
			Instantiate (explosion, transform.position, transform.rotation);
		}
		if (FindObjectOfType<SurvivalController> () != null) {
			if (!isPlayerSide) {
				FindObjectOfType<SurvivalController> ().IncreaseScore (scoreAmnt);
			}
		}
		Destroy (gameObject);
	}

	public void FlightAssist(Vector3 desiredVelocity) {
		Vector3 dv = transform.InverseTransformVector (desiredVelocity);
		Vector3 vel = transform.InverseTransformVector (rb.velocity);

		float forceF = (dv.z - vel.z) * rb.mass;
		float forceH = (dv.x - vel.x) * rb.mass;
		float forceV = (dv.y - vel.y) * rb.mass;

		forceF = Mathf.Clamp (forceF, -reverseAccel*Time.fixedDeltaTime*engineThrottle, accel*Time.fixedDeltaTime*engineThrottle);
		forceH = Mathf.Clamp (forceH, -sideAccel*Time.fixedDeltaTime*engineThrottle, sideAccel*Time.fixedDeltaTime*engineThrottle);
		forceV = Mathf.Clamp (forceV, -sideAccel*Time.fixedDeltaTime*engineThrottle, sideAccel*Time.fixedDeltaTime*engineThrottle);

		rb.AddForce (forceF * transform.forward, ForceMode.Impulse);
		rb.AddForce (forceH * transform.right, ForceMode.Impulse);
		rb.AddForce (forceV * transform.up, ForceMode.Impulse);

		heat += (Mathf.Abs(forceF) / (accel*Time.fixedDeltaTime)) * engineHeatOutput * Time.fixedDeltaTime;
		heat += (Mathf.Abs(forceH) / (accel*Time.fixedDeltaTime)) * engineHeatOutput * Time.fixedDeltaTime;
		heat += (Mathf.Abs(forceV) / (accel*Time.fixedDeltaTime)) * engineHeatOutput * Time.fixedDeltaTime;
	}

	public void TorqueAssist (Vector3 desiredRSpeed) {
		Vector3 rs = transform.InverseTransformVector (desiredRSpeed);
		Vector3 rbAVel = transform.InverseTransformVector (rb.angularVelocity);

		float forceF = (rs.z - rbAVel.z) * rb.inertiaTensor.z;
		float forceH = (rs.x - rbAVel.x) * rb.inertiaTensor.x;
		float forceV = (rs.y - rbAVel.y) * rb.inertiaTensor.y;

		forceF = Mathf.Clamp (forceF, -rollAccel*Time.fixedDeltaTime, rollAccel*Time.fixedDeltaTime);
		forceH = Mathf.Clamp (forceH, -rotateAccel*Time.fixedDeltaTime, rotateAccel*Time.fixedDeltaTime);
		forceV = Mathf.Clamp (forceV, -rotateAccel*Time.fixedDeltaTime, rotateAccel*Time.fixedDeltaTime);

		rb.AddTorque (forceF * transform.forward, ForceMode.Impulse);
		rb.AddTorque (forceH * transform.right, ForceMode.Impulse);
		rb.AddTorque (forceV * transform.up, ForceMode.Impulse);

	}

	public void Dodge(Vector3 dir) {
		if (canDodge) {
			rb.AddForce (dir.normalized * dodgeStrength, ForceMode.Impulse);
			heat += dodgeHeat;
			dodgeTimer = 0;
		}
	}

	public void Thrust(float input) {

		if (input < 0) {
			rb.AddForce (-transform.forward * reverseAccel*engineThrottle);
			heat += engineHeatOutput * Time.fixedDeltaTime * engineThrottle * -input * (reverseAccel/accel);
		} else if (input > 0) {
			rb.AddForce (transform.forward * accel*engineThrottle);
			heat += engineHeatOutput * Time.fixedDeltaTime * engineThrottle * input;
		}

	}

	public void HorizThrust(float input) {
		rb.AddForce (input * sideAccel * transform.right*engineThrottle);
		heat += engineHeatOutput * Time.fixedDeltaTime * engineThrottle * Mathf.Abs(input) * (sideAccel/accel);
	}

	public void VertThrust(float input) {
		rb.AddForce (input * sideAccel * transform.up*engineThrottle);
		heat += engineHeatOutput * Time.fixedDeltaTime * engineThrottle * Mathf.Abs(input) * (sideAccel/accel);
	}

	public void Pitch(float amount) {
		float input = Mathf.Clamp (amount, -1f, 1f);
		transform.Rotate(Vector3.right * input * rotateSpeed * Time.fixedDeltaTime, Space.Self);
	}

	public void Yaw(float amount) {
		float input = Mathf.Clamp (amount, -1f, 1f);
		transform.Rotate(Vector3.up * input * rotateSpeed * Time.fixedDeltaTime, Space.Self);
	}

	public void Roll(float amount) {
		float input = Mathf.Clamp (amount, -1f, 1f);
		transform.Rotate(-Vector3.forward * input * rotateSpeed * Time.fixedDeltaTime, Space.Self);
	}

	public BaseShip WhoAttackedMe() {
		return attackedBy;
	}

	public float GetShields() {
		return shields;
	}

	public float GetHealth() {
		return hp;
	}

	public float GetHeat() {
		return heat;
	}

	public void AddHeat(float amount) {
		heat += amount;
	}

	public void SetThrottle (float amount) {
		if (amount > 1f) {
			engineThrottle = 0;
		} else if (amount < 0f) {
			engineThrottle = 1f;
		} else {
			engineThrottle = amount;
		}
	}

	public float GetThrottle() {
		return engineThrottle;
	}

	public void SetShieldLevel (float amount) {
		if (amount > 1f) {
			shieldLevel = 0;
		} else if (amount < 0f) {
			shieldLevel = 1f;
		} else {
			shieldLevel = amount;
		}
	}

	public float GetShieldLevel() {
		return shieldLevel;
	}
		
}
