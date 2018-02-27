using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject movementToken;
	public LayerMask mask;
	public GameObject commandGrid;
	public float speed;
	public float zoomSpeed;

	private LargeShipAI currSelected;
	private GameObject currGrid;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (transform.forward * speed * Input.GetAxis ("Vertical") * Time.deltaTime);
		transform.Translate (transform.right * speed * Input.GetAxis ("Horizontal") * Time.deltaTime);
		Vector3 camForward = Camera.main.transform.forward;

		if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
		{
			transform.Translate (camForward * zoomSpeed * Time.deltaTime);
		}
		if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
		{
			transform.Translate (-camForward * zoomSpeed * Time.deltaTime);
		}

		if (Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			if (currGrid != null) {
				Destroy (currGrid);
			}
			if (currSelected != null) {
				currSelected.isSelected = false;
			}
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity)) {
				LargeShipAI ls = hit.collider.gameObject.GetComponent<LargeShipAI> ();
				Transform tr = hit.collider.gameObject.transform;
				if (ls != null && hit.collider.gameObject.GetComponent<BaseShip>().isPlayerSide) {
					ls.isSelected = true;
					currGrid = Instantiate (commandGrid, tr.position, transform.rotation);
					currSelected = ls;
				}
			}
		}

		if (Input.GetMouseButtonDown (1)) {
			int layermask = 1 << 9;
			RaycastHit hit;

			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, Mathf.Infinity, mask)) {
				if (currSelected != null && hit.collider.gameObject.GetComponent<BaseShip>() != null) {
					currSelected.target = hit.collider.gameObject.GetComponent<BaseShip> ();
				}
			} else if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, Mathf.Infinity, layermask)) {
				if (currSelected != null) {
					currSelected.ClearWaypoints ();
					currSelected.AddWaypoint (hit.point);
				}
			}
				
		}
	}
}
