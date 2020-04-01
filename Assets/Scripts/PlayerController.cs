using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public Animator animator;
	public Camera cam;

	private int combatTime = 200;
	private int inCombatTimer = 0;

	public float running_speed = 0.2f;
	public float walking_speed = 0.1f;
	public float vertical_walking_speed = 0.19f;
	public float vertical_running_speed = 0.09f;
	private float curr_speed;

	public int direction;
	private int oldDirection;
	public bool moving;
	public bool running;
	public bool attacking = false;
	public bool inCombat;

	private NetworkIdentity networkIdentity;

	// Use this for initialization
	void Start () {
		direction = 0;
		oldDirection = direction;
		moving = false;
		networkIdentity = GetComponent<NetworkIdentity> ();
	}

	// Update is called once per frame. This is the "main" function.
	void Update () {

		if (networkIdentity.IsControlling ()) {
			// Gathers the user inputs and assigns the appropriate state variables.
			handleInputs ();
		}

		// Handles the player's inCombat status (timer that counts down)
		handleCombatTimer ();

		// Handles which animations play as well as animation parameters.
		handleAnimations ();

		handleMovement ();
	}

	void handleInputs() {
		int newDirection = direction;
		oldDirection = direction;

		if (!isAttacking ()) {
			if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow)) {
				newDirection = 1;
				moving = true;
			} else if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow)) {
				newDirection = 0;
				moving = true;
			} else if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow)) {
				newDirection = 2;
				moving = true;
			} else if (Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.DownArrow)) {
				newDirection = 3;
				moving = true;
			} else {
				moving = false;
			}
		}


		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
			running = true;
		} else {
			running = false;
		}

		if (Input.GetMouseButton (0)) {
			attacking = true;
			moving = false;
			// Find location of mouse to determine which direction to attack in.
			Vector3 mouseLoc = cam.ScreenToWorldPoint(Input.mousePosition);
			if (mouseLoc.x >= transform.position.x) {
				newDirection = 0;
			} else {
				newDirection = 1;
			}
		} else {
			attacking = false;
		}

		direction = newDirection;
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

	public void handleMovement() {
		curr_speed = 0.0f;
		if (moving) {
			// If moving and running set speed to running_speed
			if (moving && running) {
				if (direction == 2 || direction == 3) {
					curr_speed = vertical_running_speed;
				} else {
					curr_speed = running_speed;
				}
			} 
			// If moving but not running set speed to walking speed
			else if (moving && !running) {
				if (direction == 2 || direction == 3) {
					curr_speed = vertical_walking_speed;
				} else {
					curr_speed = walking_speed;
				}
			}
		}

		// Check which direction the character is facing before moving...

		if (!isAttacking()) {

			Vector3 newPosition = new Vector3 (0f, 0f, transform.position.z);

			float new_x = transform.position.x;
			float new_y = transform.position.y;
			switch (direction) {
			case 0:
				new_x += curr_speed;
				break;
			case 1:
				new_x -= curr_speed;
				break;
			case 2:
				new_y += curr_speed;
				break;
			case 3:
				new_y -= curr_speed;
				break;
			}

			newPosition.x = new_x;
			newPosition.y = new_y;
			transform.position = newPosition;
		}
	}

	public void handleAnimations() {
		animator.SetInteger ("direction", direction);
		animator.SetBool ("moving", moving);
		animator.SetBool ("running", running);
		animator.SetBool ("inCombat", inCombat);

		if (attacking) {
			switch (direction) {
			case 0:
				animator.Play ("right_slash_01");
				break;
			case 1:
				animator.Play ("left_slash_01");				
				break;
			case 2:
				animator.Play ("right_slash_01");				
				break;
			case 3:
				animator.Play ("right_slash_01");				
				break;
			}
		} 
			
		if (!isAttacking ()) {
			if (moving) {
				string animState = "walk";
				if (running)
					animState = "run";
				switch (direction) {
				case 0:
					animator.Play (animState + "_right");
					break;
				case 1:
					animator.Play (animState + "_left");
					break;
				case 2:
					animator.Play (animState + "_up");
					break;
				case 3:
					animator.Play (animState + "_down");
					break;
				}
			}
		}
	}


	bool isAttacking() {
		List<string> attackStates = new List<string>() {   
			"right_slash_01",
			"right_slash_02",
			"left_slash_01",
			"left_slash_02"
		};

		foreach (string state in attackStates) {
			if (animator.GetCurrentAnimatorStateInfo (0).IsName (state)) {
				return true;
			}
		}
		return false;
	}

	public void setCam(GameObject camera) {
		cam = (Camera) camera.GetComponent(typeof(Camera));
	}
}

