using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {

	public GameObject dir;
	public float rotRangeX;
	public float rotRangeY;
	public float mouseSensitivity;
	public float deadzoneSize;
	public Transform[] waypoints;
	public string[] wpNames;
	public int waypointIndex;
	public float desiredMaxSpeed;

	private bool flightAssistOn = true;
	private BaseShip bs;
	private Vector3 clampedMouse;
	private bool manControlOn = false;

	// Use this for initialization
	void Start () {
		bs = GetComponent<BaseShip> ();
		dir.transform.localEulerAngles = Vector3.zero;
		Cursor.lockState = CursorLockMode.Locked;
		wpNames = new string[waypoints.Length];
		for (int i = 0; i < waypoints.Length; i++) {
			wpNames [i] = waypoints [i].name;
		}
	}

	void Update()
	{

		// Rotate 
		dir.transform.localEulerAngles += new Vector3 (-Input.GetAxis ("Mouse Y"), Input.GetAxis ("Mouse X"), 0) * mouseSensitivity;
		dir.transform.localEulerAngles = ClampRot (dir.transform.localEulerAngles, rotRangeX, rotRangeY);

		// Rotate Ship
		clampedMouse = ClampRot(dir.transform.localEulerAngles, rotRangeX, rotRangeY);
		clampedMouse = Deadzone (clampedMouse, deadzoneSize);

		if (Input.GetButtonDown ("Cancel")) {
			Cursor.lockState = CursorLockMode.None;
		}

		if (Input.GetButtonDown ("CycleWaypoints")) {
			waypointIndex++;
			if (waypointIndex >= waypoints.Length) {
				waypointIndex = 0;
			}
		}

		if (Input.GetButtonDown ("FlightAssist")) {
			flightAssistOn = !flightAssistOn;
		}

		if (Input.GetButtonDown ("ManualControl")) {
			manControlOn = !manControlOn;
		}

		if (Input.GetButtonDown ("Throttle")) {
			bs.SetThrottle (bs.GetThrottle () + 0.25f);
		}

		if (Input.GetButtonDown ("Shield")) {
			bs.SetShieldLevel (bs.GetShieldLevel () + 0.25f);
		}
	}

	void FixedUpdate() {

		Vector3 faVector = (transform.forward * Input.GetAxis ("Vertical"));
		faVector += (transform.right * Input.GetAxis ("Horizontal"));
		faVector += (transform.up * Input.GetAxis ("Elevate"));

		if (faVector.magnitude != 0) {
			if (Input.GetButton ("Dodge")) {
				bs.Dodge (faVector.normalized);
			}
		}

		Vector3 dx = (clampedMouse.x / rotRangeX) * bs.rotateSpeed * Mathf.Deg2Rad * transform.right;
		Vector3 dy = (clampedMouse.y / rotRangeY) * bs.rotateSpeed * Mathf.Deg2Rad * transform.up;
		Vector3 dz = -Input.GetAxis ("Roll") * bs.rollSpeed * Mathf.Deg2Rad * transform.forward;
		bs.TorqueAssist (dx + dy + dz);

		if (flightAssistOn) {
			bs.FlightAssist (faVector.normalized * desiredMaxSpeed);
		} else {
			bs.Thrust (Input.GetAxis ("Vertical"));
			bs.HorizThrust (Input.GetAxis ("Horizontal"));
			bs.VertThrust (Input.GetAxis ("Elevate"));
		}

	}

	Vector3 ClampRot(Vector3 euler, float rangeX, float rangeY) {
		float newX;
		float newY;

		if (euler.x >= 180) {
			newX = euler.x - 360f;
		} else {
			newX = euler.x;
		}
		newX = Mathf.Clamp(newX, -rangeX, rangeX);

		if (euler.y >= 180) {
			newY = euler.y - 360f;
		} else {
			newY = euler.y;
		}
		newY = Mathf.Clamp(newY, -rangeY, rangeY);

		Vector3 returned = new Vector3 (newX, newY, 0);
		returned = Vector3.ClampMagnitude(returned, rotRangeY);

		return returned;
	}

	Vector3 Deadzone(Vector3 input, float size) {
		Vector3 newInput = input;
		if (input.x < size && input.x > -size) {
			newInput.x = 0;
		}
		if (input.y < size && input.y > -size) {
			newInput.y = 0;
		}
		return newInput;
	}

	public Vector3 currentWaypoint() {
		if (waypoints.Length > 0) {
			return waypoints [waypointIndex].position;
		} else {
			return Vector3.zero;
		}
	}

	public string currentWaypointName() {
		if (wpNames.Length > 0) {
			return wpNames[waypointIndex];
		} else {
			return "";
		}
	}

	public bool isFlightAssistOn() {
		return flightAssistOn;
	}

	public bool isManControl() {
		return manControlOn;
	}
		
}
