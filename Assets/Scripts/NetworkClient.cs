using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class NetworkClient : SocketIOComponent {

	public GameObject playerPrefab;
	public GameObject cameraPrefab;

	private Transform objectContainer;

	private Dictionary<string, GameObject> serverPlayers;
	private Dictionary<string, GameObject> serverCameras;

	// Use this for initialization
	public override void Start () {
		base.Start ();
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
			string id = E.data["id"].ToString();
		});

		On ("spawn", (E) => {
			spawnPlayer(E);
		});

		On ("disconnected", (E) => {
			disconnectPlayer(E);
		});
	}

	private void initialize() {
		serverPlayers = new Dictionary<string, GameObject>();
		serverCameras = new Dictionary<string, GameObject> ();
		objectContainer = GameObject.Find ("ObjectContainer").transform;
	}


	private void spawnPlayer(SocketIOEvent E) {
		// Spawn the Player
		string id = E.data ["player"]["id"].ToString ();

		GameObject newCam = Instantiate(cameraPrefab, new Vector3(0, 0, -2), Quaternion.identity) as GameObject;
		newCam.name = "camera_" + id;
		GameObject newPlayer = Instantiate(playerPrefab, new Vector3(0, 0, -1), Quaternion.identity) as GameObject;
		newPlayer.name = "player_" + id;


		PlayerController controller = (PlayerController) newPlayer.GetComponent(typeof(PlayerController));
		controller.setCam(newCam);

		CameraController camController = (CameraController) newCam.GetComponent(typeof(CameraController));
		camController.setTarget(newPlayer);

		newPlayer.transform.SetParent(objectContainer);
		serverPlayers.Add(id, newPlayer);
		serverCameras.Add(id, newCam);
	}


	private void disconnectPlayer(SocketIOEvent E) {
		string id = E.data ["id"].ToString ();
		GameObject go = serverPlayers[id];
		Destroy(go);
		serverPlayers.Remove(id);
		serverCameras.Remove(id);
	}
}
