using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AFFlak : Weapons {

	public override void AdditionalStart() {
		primaryCool = 0.2f;
		primaryRange = primaryObj.GetComponent<Bullet> ().lifetime * primaryObj.GetComponent<Bullet> ().speed;
		primarySpeed = primaryObj.GetComponent<Bullet> ().speed;
	}

	public override void Fire1() {
		if (canFireP) {
			Rigidbody rb = GetComponentInParent<Rigidbody> ();
			for (int i = 0; i < weps1.Length; i++) {
				launch1 = Instantiate (primaryObj, weps1[i].transform.position, weps1[i].transform.rotation);
				if (launch1.GetComponent<Rigidbody> () != null) {
					launch1.GetComponent<Rigidbody> ().velocity = rb.velocity;
				}
			}
			primaryTimer = 0;
			batt --;
		}
	}

}
