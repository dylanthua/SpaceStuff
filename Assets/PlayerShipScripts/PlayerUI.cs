using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

	public GameObject crosshairIcon;
	public GameObject targetIcon;
	public GameObject leadIcon;
	public GameObject waypointIcon;
	public GameObject velocityIcon;

	public Slider heatS;
	public Slider shieldS;
	public Slider healthS;
	public Slider throttleS;
	public Slider shieldLevelS;
	public Text flightAssistText;
	public Text manControlText;
	public Text turretNames;
	public Text enemyInfo;
	public Text waypointName;
	public Text velocityText;
	public Text heatWarning;
	public Text heatPercentage;
	public Transform targetArrow;
	public Transform waypointArrow;

	private string turretNameString;
	private string enemyNameString;
	private TurretAI[] tArray;
	private BaseShip bs;
	private PlayerWepController pw;
	private PlayerControls pc;

	// Use this for initialization
	void Start () {
		bs = GetComponentInParent<BaseShip> ();
		pw = GetComponentInParent<PlayerWepController> ();
		pc = GetComponentInParent<PlayerControls> ();
	}

	void OnGUI() {
		if (pc.waypoints.Length > 0) {
			waypointIcon.GetComponentInChildren<Renderer> ().enabled = true;
			waypointIcon.transform.position = pc.waypoints [0].position;
			waypointIcon.transform.localScale = (pc.waypoints [0].position - transform.position).magnitude * new Vector3(1, 1,1);
		} else {
			waypointIcon.GetComponentInChildren<Renderer> ().enabled = false;
		}

		if (pw.target != null) {
			targetIcon.GetComponentInChildren<Renderer> ().enabled = true;
			targetIcon.transform.position = pw.target.transform.position;
			targetIcon.transform.localScale = (pw.target.transform.position - transform.position).magnitude * new Vector3(1, 1,1);
		} else {
			targetIcon.GetComponentInChildren<Renderer> ().enabled = false;
		}

		Vector3 velDir = (bs.gameObject.GetComponent<Rigidbody> ().velocity.normalized * 500f) + transform.position;
		if (bs.gameObject.GetComponent<Rigidbody> ().velocity.magnitude >= 1f) {
			velocityIcon.GetComponentInChildren<Renderer> ().enabled = true;
			velocityIcon.transform.position = velDir;
			velocityIcon.transform.localScale = 500f * new Vector3(1, 1,1);
		} else {
			velocityIcon.GetComponentInChildren<Renderer> ().enabled = false;
		}

		if (pw.GetTargetLead () != Vector3.zero && pw.target != null) {
			leadIcon.GetComponentInChildren<Renderer> ().enabled = true;
			leadIcon.transform.position = pw.GetTargetLead();
			leadIcon.transform.localScale = (pw.GetTargetLead() - transform.position).magnitude * new Vector3(1, 1,1);
		} else {
			leadIcon.GetComponentInChildren<Renderer> ().enabled = false;
		}

		Vector3 crosshairPos = pc.dir.transform.position + pc.dir.transform.forward * 500f;
		crosshairIcon.transform.position = crosshairPos;
		crosshairIcon.transform.localScale = 500f * new Vector3(1, 1,1); 

	}
	
	// Update is called once per frame
	void Update () {

		if ((bs.GetHeat () / bs.maxHeat) >= 0.75f) {
			heatWarning.text = "HEAT";
		} else {
			heatWarning.text = "";
		}

		velocityText.text = Mathf.Round(bs.gameObject.GetComponent<Rigidbody> ().velocity.magnitude) + " m/s";

		heatPercentage.text = "H: " + Mathf.Round ((bs.GetHeat () / bs.maxHeat) * 100f) + "%";

		tArray = pw.GetTurrets ();
		turretNameString = "";

		shieldS.value = bs.GetShields() / bs.maxShields;
		healthS.value = bs.GetHealth () / bs.maxHp;
		heatS.value = bs.GetHeat () / bs.maxHeat;
		throttleS.value = bs.GetThrottle ();
		shieldLevelS.value = bs.GetShieldLevel ();

		for (int i = 0; i < tArray.Length; i++) {
			turretNameString += tArray [i].turretName;

			if (tArray [i].isDisabled) {
				turretNameString += " Offline ";
			} else {
				turretNameString += " Online ";
			}

			if (tArray [i].wep.primaryCost == 0) {
				turretNameString += " - ";
			} else {
				turretNameString += " " + tArray [i].wep.ReturnBatt () + " ";
			}

			if (tArray [i].wep.ReturnIsRdy () == 1f) {
				if ((tArray [i].wep.primaryCost == 0 || tArray [i].wep.ReturnBatt () > 0) && tArray[i].wep.primaryCool >= 1f) {
					turretNameString += " RDY ";
				} else {
					turretNameString += "  ";
				}
			} else {
				turretNameString += "  ";
			}

			turretNameString += "\n";
		}
		turretNames.text = turretNameString;

		if (pw.target != null) {
			enemyNameString = pw.target.shipName;
			enemyNameString += " - " + Mathf.Round((pw.target.transform.position-pw.gameObject.transform.position).magnitude) + "m\n";
			enemyNameString += "Hull: " + Mathf.Round (100f * (pw.target.GetHealth () / pw.target.maxHp)) + "%\n";
			enemyNameString += "Shield: " + Mathf.Round (100f * (pw.target.GetShields () / pw.target.maxShields)) + "%";
		} else {
			enemyNameString = "";
		}
		enemyInfo.text = enemyNameString;

		waypointName.text = pc.currentWaypointName();
		waypointName.text += " - " + Mathf.Round ((pc.currentWaypoint () - bs.transform.position).magnitude) + "m";

		if (pw.target != null && Camera.main != null) {
			float angle = Vector3.Angle (Camera.main.transform.forward, pw.target.transform.position - Camera.main.transform.position);
			Vector3 targetDir = transform.InverseTransformVector (pw.target.transform.position - targetArrow.position);
			targetDir.Normalize ();
			targetDir = new Vector3 (targetDir.x, targetDir.y, 0);
			targetDir = transform.TransformVector (targetDir);
			if (angle > 40f) {
				targetArrow.rotation = Quaternion.LookRotation (transform.forward, targetDir);
				targetArrow.gameObject.SetActive(true);
			} else {
				targetArrow.gameObject.SetActive(false);
			}
		} else {
			targetArrow.gameObject.SetActive(false);
		}

		if (pc.currentWaypoint () != Vector3.zero && Camera.main != null) {
			float angle = Vector3.Angle (Camera.main.transform.forward, pc.currentWaypoint() - Camera.main.transform.position);
			Vector3 targetDir = transform.InverseTransformVector (pc.currentWaypoint() - targetArrow.position);
			targetDir.Normalize ();
			targetDir = new Vector3 (targetDir.x, targetDir.y, 0);
			targetDir = transform.TransformVector (targetDir);
			if (angle > 30f) {
				waypointArrow.rotation = Quaternion.LookRotation (transform.forward, targetDir);
				waypointArrow.gameObject.SetActive(true);
			} else {
				waypointArrow.gameObject.SetActive(false);
			}
		} else {
			waypointArrow.gameObject.SetActive(false);
		}

		if (pc.isFlightAssistOn ()) {
			flightAssistText.text = "FA";
		} else {
			flightAssistText.text = "NA";
		}

		if (pc.isManControl()) {
			manControlText.text = "MC";
		} else {
			manControlText.text = "AC";
		}

	}
}
