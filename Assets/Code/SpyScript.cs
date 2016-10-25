using UnityEngine;
using System.Collections;

public class SpyScript : MonoBehaviour {

	public static float speed = 30f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		handleSpyMovement ();

	}

	void handleSpyMovement() {

		Vector2 direction = Vector2.zero;
		bool usingKeyboard = false;


		// KEYBOARD
		if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow)) {
			direction = new Vector2 (direction.x - 1, direction.y);
			usingKeyboard = true;
		}
		if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow)) {
			direction = new Vector2 (direction.x + 1, direction.y);
			usingKeyboard = true;
		}
		if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow)) {
			direction = new Vector2 (direction.x, direction.y +1);
			usingKeyboard = true;
		}
		if (Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.DownArrow)) {
			direction = new Vector2 (direction.x, direction.y -1);
			usingKeyboard = true;
		}

		if (usingKeyboard) {
			if (direction != Vector2.zero) {
				direction.Normalize ();
			} 
		} else {
			// XBOX CONTROLLER
			direction = new Vector2(Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical"));
			direction.Normalize ();
		}
			
		Vector2 change = direction * speed * Time.deltaTime;

		this.GetComponent<Rigidbody2D> ().MovePosition (this.transform.position + new Vector3 (change.x, change.y, 0f));

	}

}
