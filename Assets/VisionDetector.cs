using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionDetector : MonoBehaviour {

	private LeechController leech;

	// Use this for initialization
	void Start () {
		leech = transform.parent.gameObject.GetComponent<LeechController>();
	}

    void OnTriggerEnter2D(Collider2D collision)
    {
		// Found a new player here
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("found dude :)");

            // Call a function from the parent to change from patroling to chasing.
            leech.targetAcquired(collision.gameObject);
            
        }




    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("dude left :(");
            leech.targetLost(collision.gameObject);
        }
    }
}
