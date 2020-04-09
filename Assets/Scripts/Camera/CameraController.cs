using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject target;

	public float followAhead;
	public float smoothing;

	private Vector3 targetPosition;

	public bool followTarget;

	public float speed;
	// Use this for initialization
	void Start () {
		speed = 0.05f;
		followAhead = 3f;
		smoothing = 2f;
		followTarget = true;
		transform.gameObject.name = target.name + " " + transform.gameObject.name;
		transform.SetParent (null);
	}

	// Update is called once per frame
	void Update () {
		if (followTarget) {
			targetPosition = new Vector3 (target.transform.position.x, target.transform.position.y, -10f);


			float distance_scale_x = Mathf.Abs (transform.position.x - targetPosition.x);
			float distance_scale_y = Mathf.Abs (transform.position.y - targetPosition.y);

			//transform.position = targetPosition;
			if (Mathf.Abs(transform.position.x - targetPosition.x) > 1) {
				transform.position = new Vector3 (
					transform.position.x + speed * distance_scale_x * (Mathf.Sign (targetPosition.x - transform.position.x)), 
					transform.position.y,
					transform.position.z
				);
			}

			if (Mathf.Abs(transform.position.y - targetPosition.y) > 1) {
				transform.position = new Vector3 (
					transform.position.x, 
					transform.position.y + speed * distance_scale_y * (Mathf.Sign (targetPosition.y - transform.position.y)),
					transform.position.z
				);
			}
		}

	}

	public void setTarget(GameObject targ) {
		target = targ;
		followTarget = true;
	}
}