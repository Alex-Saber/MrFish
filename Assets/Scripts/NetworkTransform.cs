using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class NetworkTransform : MonoBehaviour {

	private Vector3 oldPosition;
	private int oldDirection;

	private NetworkIdentity networkIdentity;
	private Player player;

	private float skillCounter = 0;

	public Animator playerAnim;
	public PlayerController playerCtrl;

	private bool running;
	private bool moving;
	private bool inCombat;
	private bool attacking;
	private int direction;

	// Use this for initialization
	void Start () {
		networkIdentity = GetComponent<NetworkIdentity> ();
		playerCtrl = GetComponent<PlayerController> ();

		oldPosition = transform.position;
		oldDirection = playerCtrl.direction;

		player = new Player ();
		player.id = networkIdentity.GetID ();
		player.position = new Position ();

		player.direction = 0;
		player.position.x = 0;
		player.position.y = 0;

		player.animation = new AnimationState ();
		playerAnim = GetComponent<Animator> ();

		inCombat = playerCtrl.inCombat;
		moving = playerCtrl.moving;
		running = playerCtrl.running;
		attacking = playerCtrl.attacking;
		direction = playerCtrl.direction;

		if (!networkIdentity.IsControlling ()) {
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (networkIdentity.IsControlling ()) {
			if (oldPosition != transform.position) {
				oldPosition = transform.position;
				sendPositionData ();
			}

//			if (oldDirection != playerCtrl.direction) {
//				oldDirection = playerCtrl.direction;
//				sendDirectionData();
//			}
				
			if (moving != playerCtrl.moving ||
				running != playerCtrl.running ||
				inCombat != playerCtrl.inCombat ||
				attacking != playerCtrl.attacking ||
				direction != playerCtrl.direction) {

				moving = playerCtrl.moving;
				running = playerCtrl.running;
				inCombat = playerCtrl.inCombat;
				attacking = playerCtrl.attacking;
				direction = playerCtrl.direction;

				sendAnimationData ();
			}
		}
	}

	private void sendPositionData() {
		player.position.x = transform.position.x;
		player.position.y = transform.position.y;

		networkIdentity.GetSocket().Emit("updatePosition", 
			new JSONObject(JsonUtility.ToJson(player))
		);
	}

	private void sendDirectionData() {
		player.direction = playerCtrl.direction;

		networkIdentity.GetSocket().Emit("updateDirection", 
			new JSONObject(JsonUtility.ToJson(player))
		);
	}

	private void sendAnimationData() {
		player.animation.inCombat = playerCtrl.inCombat;
		player.animation.moving = playerCtrl.moving;
		player.animation.running = playerCtrl.running;
		player.animation.attacking = playerCtrl.attacking;
		player.direction = playerCtrl.direction;

		networkIdentity.GetSocket().Emit("updateAnimation", 
			new JSONObject(JsonUtility.ToJson(player))
		);
	}
}
