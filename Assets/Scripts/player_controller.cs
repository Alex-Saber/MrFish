using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_controller : MonoBehaviour {

	public Animator animator;
	public Camera cam;

	private int combatTime = 300;
	private int inCombatTimer = 0;

	private float running_speed = 0.7f;
	private float walking_speed = 0.3f;
	private float curr_speed;

	private int direction;
	private bool moving;
	private bool running;
	private bool attacking;
	private bool inCombat;

	// Use this for initialization
	void Start () {
		direction = 1;
		moving = false;
		curr_speed = walking_speed;
	}

	// Update is called once per frame. This is the "main" function.
	void Update () {

		// Gathers the user inputs and assigns the appropriate state variables.
		int newDirection = handleInputs ();

		// Handles the player's inCombat status (timer that counts down)
		handleCombatTimer();

		// Handles which animations play as well as animation parameters.
		handleAnimations ();

		// Handle which direction the character is facing.
		handleCharacterDirection (newDirection);

		// Handle player movement.
		handleMovement ();

		print (Input.mousePosition + " " + transform.position);
	}

	int handleInputs() {
		
		int newDirection = direction;
		if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow)) {
			newDirection = -1;
			moving = true;
		} else if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow)) {
			newDirection = 1;
			moving = true;
		} else {
			moving = false;
		}

		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
			running = true;
		} else {
			running = false;
		}

		if (Input.GetMouseButton (0)) {
			attacking = true;
			// Find location of mouse to determine which direction to attack in.
			Vector3 mouseLoc = cam.ScreenToWorldPoint(Input.mousePosition);
			if (mouseLoc.x >= transform.position.x) {
				newDirection = 1;
			} else {
				newDirection = -1;
			}

			print (newDirection);
		} else {
			attacking = false;
		}

		return newDirection;
	}

	void handleCombatTimer() {

		if (isAttacking ()) {
			inCombatTimer = combatTime;
		}
			
		if (inCombatTimer > 0) {
			inCombat = true;
			inCombatTimer -= 1;
		} else {
			inCombat = false;
		}
	}

	void handleMovement() {
		float curr_speed = 0.0f;
		if (moving) {
			// If moving and running set speed to running_speed
			if (moving && running) {
				curr_speed = running_speed;
			} 
			// If moving but not running set speed to walking speed
			else if (moving && !running) {
				curr_speed = walking_speed;
			}
		}
			
		if (!isAttacking()) {
			transform.position = new Vector3 (
				transform.position.x + curr_speed * direction, 
				transform.position.y,
				transform.position.z
			);
		}
	}

	void handleCharacterDirection(int newDirection) {
		if (direction != newDirection) {
			flipCharacter ();
			direction = newDirection;
		}
	}

	void flipCharacter() {
		transform.localScale = new Vector3 (
			transform.localScale.x * -1,
			transform.localScale.y,
			transform.localScale.z
		);
	}

	void handleAnimations() {

		if (attacking) {
			animator.Play ("side_slash_01");
		}
			
		animator.SetBool ("moving", moving);
		animator.SetBool ("running", running);
		animator.SetBool ("inCombat", inCombat);
	}


	bool isAttacking() {
		return animator.GetCurrentAnimatorStateInfo (0).IsName ("side_slash_01") || animator.GetCurrentAnimatorStateInfo (0).IsName ("side_slash_02");
	}

}

