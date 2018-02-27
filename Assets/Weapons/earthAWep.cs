using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class earthAWep : Weapons {

	public override void Fire1() {
		if (canFireP) {
			GetComponentInParent<BaseShip> ().AddHeat (heatCost);
			Rigidbody rb = GetComponentInParent<Rigidbody> ();
			for (int i = 0; i < weps1.Length; i++) {
				if (batt >= 0) {
					batt -= 0;
					launch1 = Instantiate (primaryObj, weps1[i].transform.position, weps1[i].transform.rotation);
					if (launch1.GetComponent<Rigidbody> () != null) {
						launch1.GetComponent<Rigidbody> ().velocity = rb.velocity;
					}
					if (launch1.GetComponent<BaseShip> () != null) {
						launch1.GetComponent<BaseShip> ().isPlayerSide = GetComponent<BaseShip>().isPlayerSide;
					}
					launch1 = null;
				}
			}
			primaryTimer = 0;
		}
	}

	public override void Fire2() {
		if (canFireS) {
			GetComponentInParent<BaseShip> ().AddHeat (heatCost);
			Rigidbody rb = GetComponentInParent<Rigidbody> ();
			for (int i = 0; i < weps2.Length; i++) {
				if (batt >= 0) {
					batt -= 0;
					launch2 = Instantiate (secondaryObj, weps2[i].transform.position, weps2[i].transform.rotation);
					if (launch2.GetComponent<Rigidbody> () != null) {
						launch2.GetComponent<Rigidbody> ().velocity = rb.velocity;
					}
					if (launch2.GetComponent<BaseShip> () != null) {
						launch2.GetComponent<BaseShip> ().isPlayerSide = GetComponent<BaseShip>().isPlayerSide;
					}
					launch2 = null;
				}
			}
			secondaryTimer = 0;
		}
	}

	public override void AdditionalStart ()
	{
		primarySpeed = primaryObj.GetComponent<Bullet> ().speed;
		primaryRange = primaryObj.GetComponent<Bullet> ().speed * primaryObj.GetComponent<Bullet> ().lifetime;

		secondarySpeed = primaryObj.GetComponent<Bullet> ().speed;
		secondaryRange = primaryObj.GetComponent<Bullet> ().speed * primaryObj.GetComponent<Bullet> ().lifetime;
	}

}
