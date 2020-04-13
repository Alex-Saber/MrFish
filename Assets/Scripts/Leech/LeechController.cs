using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeechController : MonoBehaviour {

	public Animator anim;

	public float walking_speed;
	public float vertical_walking_speed;

	public int direction;

	public bool moving;

    public int stateTimer;

	// Use this for initialization
	void Start () {
		walking_speed = 0.11f;
		vertical_walking_speed = 0.09f;

        // facing right
        direction = 0;
		moving = false;

        stateTimer = 60;

	}
	
	// Update is called once per frame
	void Update () {


        if (stateTimer <= 0)
        {
            // 1
            // What state are we in
            // Are we moving? and in which direction?
            composeState();
            stateTimer = 60;
        }
        else
        {
            stateTimer--;
        }

        // 2
        // Move the dude
        handleMovement();

        // 3
        // play the appropriate animations
        handleAnimations();
	}

    void composeState()
    {

        int isMoving = Random.Range(0, 2);

        if (isMoving == 0)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }

        if (moving)
        {
            direction = Random.Range(0, 4);
        }
    }

    void handleMovement()
    {
        float currentHorizontalSpeed = 0.0f;
        float currentVerticalSpeed = 0.0f;

        if (moving)
        {
            switch (direction)
            {
                case 0: // Moving Right
                    currentHorizontalSpeed += walking_speed;
                    break;
                case 1: // Moving Left
                    currentHorizontalSpeed -= walking_speed;
                    break;
                case 2: // Moving Up
                    currentVerticalSpeed += vertical_walking_speed;
                    break;
                case 3: // Moving Down
                    currentVerticalSpeed -= vertical_walking_speed;
                    break;
            }
        }

        transform.position = new Vector3(
            transform.position.x + currentHorizontalSpeed,
            transform.position.y + currentVerticalSpeed,
            transform.position.z
        );
    }

    void handleAnimations()
    {
        anim.SetBool("moving", moving);
        anim.SetInteger("direction", direction);

        if (moving)
        {
            switch (direction)
            {
                case 0: // Moving Right
                    anim.Play("walk_right");
                    break;
                case 1: // Moving Left
                    anim.Play("walk_left");
                    break;
                case 2: // Moving Up
                    anim.Play("walk_up");
                    break;
                case 3: // Moving Down
                    anim.Play("walk_down");
                    break;
            }
        }
    }
}






