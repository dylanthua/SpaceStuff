using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherHM : Weapons {

	public override void Fire1() {
		if (canFireP) {
			for (int i = 0; i < weps1.Length; i++) {
				if (batt >= 1) {
					batt -= 1;
					launch1 = Instantiate (primaryObj, weps1[i].transform.position+transform.forward*10f, weps1[i].transform.rotation);
					if (launch1.GetComponent<Homing> () != null) {
						Rigidbody rb = GetComponentInParent<Rigidbody> ();
						Vector3 localVel = rb.gameObject.transform.InverseTransformVector (rb.velocity);
						float addedSpeed = 0;
						if (localVel.z > 0) {
							addedSpeed = localVel.z;
						}
						launch1.GetComponent<Homing> ().speed += addedSpeed;
						launch1.GetComponent<Homing> ().target = target;
					}
					launch1 = null;
				}
			}
			primaryTimer = 0;
		}
	}

	public override void AdditionalStart ()
	{
		primarySpeed = primaryObj.GetComponent<Homing> ().speed;
		primaryRange = primaryObj.GetComponent<Homing> ().speed * primaryObj.GetComponent<Homing> ().lifetime;
	}
}
