using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_controller : MonoBehaviour {

	public Animator animator;
	public int direction;
	public float running_speed = 0.5f;
	public float walking_speed = 0.05f;
	// Use this for initialization
	void Start () {
		direction = 1;
	}

	// Update is called once per frame
	void Update () {

		float Horizontal = Input.GetAxis ("Horizontal");

		bool [] state = handleAnimations (Horizontal);
		handleFlipCharacter (Horizontal);

		// Compute current speed
		float curr_speed = 0.0f;
		if (Mathf.Abs(Horizontal) > 0.1) {
			if (state [0] && state [1]) {
				curr_speed = running_speed;
			} else if (state [0] && !state [1]) {
				curr_speed = walking_speed;
			}
		}

		handleMovement (curr_speed);

	}

	void handleMovement(float speed) {
		transform.position = new Vector3(
			transform.position.x + speed * direction, 
			transform.position.y,
			transform.position.z
		);
	}

	void handleFlipCharacter(float horizontal) {
		if (horizontal > 0.0) {
			if (direction == -1) {
				flipCharacter ();
				direction = 1;
			}
		} else if (horizontal < 0.0) {
			if (direction == 1) {
				flipCharacter ();
				direction = -1;
			}
		}
	}

	void flipCharacter() {
		transform.localScale = new Vector3 (
			transform.localScale.x * -1,
			transform.localScale.y,
			transform.localScale.z
		);
	}

	bool [] handleAnimations(float horizontal) {

		bool run = false;
		bool walk = false;

		if (horizontal != 0) {
			walk = true;
		}

		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
			run = true;
		}

		animator.SetBool ("moving", walk);
		animator.SetBool ("running", run);

		bool[] state = new bool[2];
		state [0] = walk;
		state [1] = run;
		return state;
	}
}

