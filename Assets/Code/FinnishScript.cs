using UnityEngine;
using System.Collections;

public class FinnishScript : MonoBehaviour {

	private Main main;

	// Use this for initialization
	void Start () {
	
		main = Camera.main.GetComponent<Main> ();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay2D(Collider2D collider) {

		if (collider.gameObject.GetComponent<SpyScript> () != null) {
			main.TouchedFinnish ();
		}

	}

}
