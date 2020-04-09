using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
// Test change
public class NetworkClient : SocketIOComponent {

	public GameObject playerPrefab;
	public GameObject cameraPrefab;

	private Transform objectContainer;

	private Dictionary<string, GameObject> serverPlayers;

	public static string ClientID { get; private set; }

	private int numPlayers;

	// Use this for initialization
	public override void Start () {
		base.Start ();
		numPlayers = 0;
		initialize ();
		setupEvents ();
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}

	private void setupEvents() {
		On("open", (E) => {
			print("connection made");
		});
			
		On("register", (E) => {
			ClientID = E.data["id"].ToString();
		});

		On ("spawn", (E) => {
			spawnPlayers(E);
		});
			
		On ("disconnected", (E) => {
			disconnectPlayer(E);
		});

		On ("updatePosition", (E) => {
			string id = E.data ["id"].ToString ();
			float x = float.Parse(E.data ["position"] ["x"].ToString());
			float y = float.Parse(E.data ["position"] ["y"].ToString());

			serverPlayers [id].transform.position = new Vector3 (x, y, serverPlayers [id].transform.position.z);
		});

		On ("updateDirection", (E) => {
			string id = E.data ["id"].ToString ();
			int dir = int.Parse(E.data["direction"].ToString());
			PlayerController playCtrl = serverPlayers[id].GetComponent<PlayerController>();
			playCtrl.direction = dir;
		});

		On ("updateAnimation", (E) => {
			Debug.Log("reached");
			string id = E.data ["id"].ToString ();

			Animator anim = serverPlayers[id].GetComponent<Animator>();

			bool inCombat = E.data["inCombat"].ToString() == "true";
			bool moving = E.data["moving"].ToString() == "true";
			bool running = E.data["running"].ToString() == "true";
			bool attacking = E.data["attacking"].ToString() == "true";
			int direction = int.Parse(E.data["direction"].ToString());

			anim.SetBool("inCombat", inCombat);
			anim.SetBool("moving", moving);
			anim.SetBool("running", running);
			anim.SetInteger("direction", direction);

			PlayerController playCtrl = serverPlayers[id].GetComponent<PlayerController>();
			playCtrl.moving = moving;
			playCtrl.running = running;
			playCtrl.inCombat = inCombat;
			playCtrl.attacking = attacking;
			playCtrl.direction = direction;

			playCtrl.handleAnimations();
		});

	}

	private void initialize() {
		serverPlayers = new Dictionary<string, GameObject>();
		objectContainer = GameObject.Find ("ObjectContainer").transform;
	}
		
	private void spawnPlayers(SocketIOEvent E) {
		// Spawn the Player

		string id = E.data ["player"]["id"].ToString ();
		Vector3 playerLocation = new Vector3 (
			float.Parse(E.data ["player"] ["position"] ["x"].ToString()), 
			float.Parse(E.data ["player"] ["position"] ["y"].ToString()),
			7.5f
		);
			
		GameObject newPlayer = Instantiate(playerPrefab, playerLocation, Quaternion.identity) as GameObject;
		newPlayer.name = "player_" + id;

		PlayerController controller = (PlayerController) newPlayer.GetComponent(typeof(PlayerController));
		controller.direction = 0;

		if (ClientID == id) {
			GameObject newCam = Instantiate (cameraPrefab, new Vector3 (0, 0, -1), Quaternion.identity) as GameObject;
			newCam.name = "camera_" + id;

			controller.setCam(newCam);

			CameraController camController = (CameraController) newCam.GetComponent(typeof(CameraController));
			camController.setTarget(newPlayer);
		}
			
		newPlayer.transform.SetParent(objectContainer);
		serverPlayers.Add(id, newPlayer);
		NetworkIdentity ni = newPlayer.GetComponent<NetworkIdentity> ();
		ni.SetControllerID (id);
		ni.SetSocketReference (this);
	}

	private void disconnectPlayer(SocketIOEvent E) {
		string id = E.data ["id"].ToString ();
		GameObject go = serverPlayers[id];
		Destroy(go);
		serverPlayers.Remove(id);
	}

	public string getClientID() {
		return ClientID;
	}
}

[System.Serializable]
public class Player {
	public string id;
	public Position position;
	public Scale scale;
	public int direction;
	public AnimationState animation;
}

[System.Serializable]
public class Position {
	public float x;
	public float y;
}

[System.Serializable]
public class Scale {
	public float x;
	public float y;
}

[System.Serializable]
public class AnimationState {
	public bool moving;
	public bool running;
	public bool inCombat;
	public bool attacking;
}

