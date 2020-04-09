using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeechController : MonoBehaviour {

	public Animator animator;

	private int maxTimeInState;
	private int stateChangeTimer;

	public float walking_speed = 0.07f;
	public float vertical_walking_speed = 0.06f;

	public float followDistance;
	public int direction;
	public bool moving;

	private string currentState;

	private GameObject target;

	// Use this for initialization
	void Start()
	{
		direction = 0;
		moving = false;
        walking_speed = 0.11f;
        vertical_walking_speed = 0.09f;
		maxTimeInState = 20;
        stateChangeTimer = 0;
		target = null;
		currentState = "patrol";
		followDistance = 0.3f;
	}

	// Update is called once per frame. This is the "main" function.
	void Update()
	{
		if (target == null)
		{
			// Decides which patrol state the Leech is in
			if (stateChangeTimer <= 0)
			{
				composeState();
				stateChangeTimer = maxTimeInState;
			}
			else
			{
				stateChangeTimer--;
			}

			// Handles the movement of the Leech
			handlePatrolMovement();
		}
        else
        {
			// Follow the target somehow
			handleFollowMovement();
        }


		// Handles which animations play as well as animation parameters.
		handleAnimations();

	}


    public void composeState()
    {
		// Randomly decides if the leech is moving or not
		int isMoving = Random.Range(0, 15);
        if (isMoving <= 10)
        {
			moving = true;
        }
        else
        {
			moving = false;
        }

        // If the leech is moving figure out in which direction

        if (moving)
        {   
			direction = Random.Range(0, 4);
        }

    }

    public void handleFollowMovement()
    {
        if (Vector3.Distance(transform.position, target.transform.position) > followDistance)
        {
			// Figure out movement direction...
			int vertical = 1;
			int horizontal = 1;
			Vector3 targetLoc = target.transform.position;
			if (targetLoc.x < transform.position.x)
			{
				horizontal = -1;
			}

			if (targetLoc.y < transform.position.y)
			{
				vertical = -1;
			}

			// Now move the dude
			// Check which direction the character is facing before moving...
			Vector3 newPosition = new Vector3(
				transform.position.x + walking_speed * horizontal,
				transform.position.y + walking_speed * vertical,
				transform.position.z
			);
			transform.position = newPosition;


			// Figure out which direction the dude is supposed to face
			float verticalDistance = Mathf.Abs(targetLoc.y - transform.position.y);
			float horizontalDistance = Mathf.Abs(targetLoc.x - transform.position.x);

			if (verticalDistance > horizontalDistance)
			{
				// Check if target is below or above
				if (vertical == 1)
					direction = 2;
				else
					direction = 3;
			}
			else
			{
				// Check if target is to the right or the left
				if (horizontal == 1)
					direction = 0;
				else
					direction = 1;
			}
		}
    }

	public void handlePatrolMovement()
	{
		float horizontal_speed = 0.0f;
		float vertical_speed = 0.0f;

		if (moving)
		{
            switch(direction)
            {
				case 0:
					horizontal_speed += walking_speed;
					break;
				case 1:
					horizontal_speed -= walking_speed;
					break;
				case 2:
					vertical_speed += vertical_walking_speed;
					break;
				case 3:
					vertical_speed -= vertical_walking_speed;
					break;

			}
		}

		// Check which direction the character is facing before moving...
		Vector3 newPosition = new Vector3(
			transform.position.x + horizontal_speed,
			transform.position.y + vertical_speed,
			transform.position.z
		);
		transform.position = newPosition;

	}

	public void handleAnimations()
	{
		animator.SetInteger("direction", direction);
		animator.SetBool("moving", moving);

		if (moving)
		{
			string animState = "walk";

			switch (direction)
			{
				case 0:
					animator.Play(animState + "_right");
					break;
				case 1:
					animator.Play(animState + "_left");
					break;
				case 2:
					animator.Play(animState + "_up");
					break;
				case 3:
					animator.Play(animState + "_down");
					break;
			}
		}
	}

    public void targetAcquired(GameObject newTarget)
    {
		target = newTarget;
    }

    public void targetLost(GameObject oldTarget)
    {
		target = null;
    }
}
