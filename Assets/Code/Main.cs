using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {

	private List<GameObject> guards = new List<GameObject>();
	private Vector2 movingGuards = Vector2.zero;

	// Use this for initialization
	void Start () {

		for (int i = 0; i < 10; i++) {

			GameObject guard = Instantiate (Resources.Load("Prefabs/Guard") as GameObject);
			guard.SetActive (true);
			guard.transform.position = new Vector3 (Random.Range (-1f, 1f), Random.Range (-1f, 1f), 0f);
			guards.Add (guard);

		}
	
	}
	
	// Update is called once per frame
	void Update () {

		handleMovingGuards ();
	
	}

	void handleMovingGuards() {

		if (movingGuards == Vector2.zero) {

			if (Input.GetKeyDown (KeyCode.I)) {

				movingGuards = new Vector2 (0, 1);

			} else if (Input.GetKeyDown (KeyCode.K)) {

				movingGuards = new Vector2 (0, -1);

			} else if (Input.GetKeyDown (KeyCode.J)) {

				movingGuards = new Vector2 (-1, 0);

			} else if (Input.GetKeyDown (KeyCode.L)) {

				movingGuards = new Vector2 (1, 0);

			}

		} else {

			foreach (GameObject guard in guards) {
				
				float amountX = 0f;
				float amountY = 0f;

				if (movingGuards.x != 0) {
					amountX = (movingGuards.x / Mathf.Abs (movingGuards.x)) * Time.deltaTime;
				}
				if (movingGuards.y != 0) {
					amountY = (movingGuards.y / Mathf.Abs (movingGuards.y)) * Time.deltaTime;
				}

				guard.transform.position = guard.transform.position + new Vector3 (amountX, amountY, 0f);
			}

			movingGuards = Vector2.MoveTowards (movingGuards, Vector2.zero, Time.deltaTime);

		}

	}

}
