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
		speed = 0.1f;
		followAhead = 3f;
		smoothing = 2f;
		followTarget = true;
		target = transform.root.gameObject;
		transform.gameObject.name = target.name + " " + transform.gameObject.name;
		gameObject.transform.parent = null;
	}

	// Update is called once per frame
	void Update () {
		if (followTarget) {
			targetPosition = new Vector3 (target.transform.position.x, target.transform.position.y, -10f);


			float distance_scale = Mathf.Abs (transform.position.x - targetPosition.x);

			//transform.position = targetPosition;
			if (Mathf.Abs(transform.position.x - targetPosition.x) > 1) {
				transform.position = new Vector3 (
					transform.position.x + speed * distance_scale * (Mathf.Sign (targetPosition.x - transform.position.x)), 
					transform.position.y,
					transform.position.z
				);
			}
		}

	}
}