using UnityEngine;
using System.Collections;

public class SpyScript : MonoBehaviour {

	public static float speed = 20f;
	private GameObject arrow;
	private float arrowAngle = 0f;
	private Main main;

	// Use this for initialization
	void Start () {
	
		main = Camera.main.GetComponent<Main> ();

		arrow = transform.FindChild ("flecha").gameObject;
		arrowAngle = 0f;
		arrow.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(arrow.transform.localEulerAngles.z, arrowAngle, 1f));
		arrow.transform.localPosition = -arrow.transform.right * 2.5f;
		arrow.SetActive (true);

	}
	
	// Update is called once per frame
	void Update () {

		if (main.state == Main.State.Play) {

			handleSpyMovement ();

		}

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
			
		if (!usingKeyboard) {
			// XBOX CONTROLLER
			direction = new Vector2(Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical"));
		}

		if (direction != Vector2.zero) {
			direction.Normalize ();
			arrowAngle = 180f + Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
		} 

		arrow.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(arrow.transform.localEulerAngles.z, arrowAngle, Time.deltaTime*10f));
		arrow.transform.localPosition = -arrow.transform.right * 2.5f;
			
		Vector2 change = direction * speed * Time.deltaTime;

		this.GetComponent<Rigidbody2D> ().MovePosition (this.transform.position + new Vector3 (change.x, change.y, 0f));

	}

}
