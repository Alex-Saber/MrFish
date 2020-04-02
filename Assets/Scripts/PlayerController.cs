using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public Animator animator;
	public Camera cam;

	private int combatTime = 200;
	private int inCombatTimer = 0;

	public float running_speed = 0.2f;
	public float walking_speed = 0.11f;
	public float vertical_running_speed = 0.18f;
	public float vertical_walking_speed = 0.09f;

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
//		oldDirection = direction;
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
			if (Right()) {
				newDirection = 0;
				moving = true;
			} 
			else if (Left()) {
				newDirection = 1;
				moving = true;
			} 
			else if (Up()) {
				newDirection = 2;
				moving = true;
			} 
			else if (Down()) {
				newDirection = 3;
				moving = true;
			} 
			else {
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
		float horizontal_speed = 0.0f;
		float vertical_speed = 0.0f;

		int runMultiplier = 1;
		if (running) {
			runMultiplier = 2;
		} 

		if (moving) {
			// If moving and running multiply speed by 2
			if (Up()) {
				vertical_speed += vertical_walking_speed * runMultiplier;
			} 
			if (Down()) {
				vertical_speed -= vertical_walking_speed * runMultiplier;
			}
			if (Right ()) {
				horizontal_speed += walking_speed * runMultiplier;
			}
			if (Left ()) {
				horizontal_speed -= walking_speed * runMultiplier;
			}
		}

		// Check which direction the character is facing before moving...

		if (!isAttacking()) {

			Vector3 newPosition = new Vector3 (
				transform.position.x + horizontal_speed, 
				transform.position.y + vertical_speed, 
				transform.position.z
			);
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

	private bool Right() {
		return Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow);
	}

	private bool Left() {
		return Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow);
	}

	private bool Up() {
		return Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow);
	}

	private bool Down() {
		return Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.DownArrow);
	}
}

