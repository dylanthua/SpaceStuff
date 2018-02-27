using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lrBeam : Weapons {

	public float laserDuration;
	public float damage;

	private RaycastHit hit;
	private Vector3 endPos;
	private LineRenderer line;
	private float laserTimer;

	public override void Fire1() {
		if (canFireP) {
			laserTimer = 0;
			int layermask = 1 << 8;
			layermask = ~layermask;
			if (Physics.Raycast (transform.position, transform.forward, out hit, primaryRange, layermask)) {
				BaseShip hitShip = hit.collider.gameObject.GetComponent<BaseShip> ();
				Destroyable hitObstacle = hit.collider.gameObject.GetComponent<Destroyable> ();
				if (hitShip != null) {
					hitShip.Damage (damage, GetComponentInParent<BaseShip> ());
				} else if (hitObstacle != null) {
					hitObstacle.Damage (damage);
				}
			}
			primaryTimer = 0;
		}
	}

	public override void AdditionalUpdate() {

		int layermask = 1 << 8;
		layermask = ~layermask;

		if (Physics.Raycast (transform.position, transform.forward, out hit, primaryRange, layermask)) {
			endPos = hit.point;
		} else {
			endPos = transform.forward * primaryRange;
		}

		line.SetPosition (0, transform.position);
		line.SetPosition (1, endPos);

		laserTimer += Time.deltaTime;
		laserTimer = Mathf.Clamp (laserTimer, 0, laserDuration);
		if (laserTimer < laserDuration) {
			line.enabled = true;
		} else {
			line.enabled = false;
		}
	}

	public override void AdditionalStart() {
		line = GetComponent<LineRenderer> ();
		laserTimer = laserDuration;
		primarySpeed = Mathf.Infinity;
	}
}
