using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {

	private List<GameObject> guards = new List<GameObject>();
	private Vector2 movingGuards = Vector2.zero;
	private int rounds = 1;
	private int maxRounds = 5;
	public bool readingArduino = false;
	private ArduinoHandler arduino;

	public SpriteRenderer title;
	public SpriteRenderer startButton;
	public SpriteRenderer roundsButton;
	public TextMesh roundsText;
	public SpriteRenderer arrowUp;
	public SpriteRenderer arrowDown;

	public SpriteRenderer calibrado;
	public SpriteRenderer calibradoDown;
	public SpriteRenderer calibradoLeft;
	public SpriteRenderer calibradoRight;
	public SpriteRenderer calibradoUp;
	public Color colorToAssign;
	public Color colorAssigned;

	private Vector3 originalTitlePos;
	private Vector3 originalStartPos;
	private Vector3 originalRoundsPos;
	private Vector3 originalRoundsTextPos;
	private Vector3 originalArrowUpPos;
	private Vector3 originalArrowDownPos;

	public AudioSource menuMusic;

	public State state = State.Menu;

	public enum State {
		Menu,
		MenuToCalibrate,
		Calibrating
	}

	// Use this for initialization
	void Start () {

		arduino = this.GetComponent<ArduinoHandler> ();
		readingArduino = false;

		originalTitlePos = title.transform.position;
		originalStartPos = startButton.transform.position;
		originalRoundsPos = roundsButton.transform.position;
		originalRoundsTextPos = roundsText.transform.position;
		originalArrowUpPos = arrowUp.transform.position;
		originalArrowDownPos = arrowDown.transform.position;

		/*
		for (int i = 0; i < 10; i++) {

			GameObject guard = Instantiate (Resources.Load("Prefabs/Guard") as GameObject);
			guard.SetActive (true);
			guard.transform.position = new Vector3 (Random.Range (-1f, 1f), Random.Range (-1f, 1f), 0f);
			guards.Add (guard);

		}
		*/
	
	}
	
	// Update is called once per frame
	void Update () {

		if (state == State.Menu || state == State.MenuToCalibrate || state == State.Calibrating) {
			menuMusic.volume = Mathf.Lerp (menuMusic.volume, 0.3f, Time.deltaTime);
		}

		if (state == State.MenuToCalibrate && title.gameObject.activeInHierarchy) {

			disappear (title, originalTitlePos + new Vector3 (0f, 0.1f, 0f), -0.1f, Time.deltaTime * 5f);
			disappear (startButton, originalStartPos + new Vector3 (0f, -0.1f, 0f), -0.1f, Time.deltaTime * 5f);
			disappear (roundsButton, originalRoundsPos + new Vector3 (0f, -0.1f, 0f), -0.1f, Time.deltaTime * 5f);
			disappear (arrowUp, originalArrowUpPos + new Vector3 (0f, -0.1f, 0f), -0.1f, Time.deltaTime * 5f);
			disappear (arrowDown, originalArrowDownPos + new Vector3 (0f, -0.1f, 0f), -0.1f, Time.deltaTime * 5f);

			Hacks.GameObjectLerp (roundsText.gameObject, originalRoundsTextPos + new Vector3(0f, -0.1f, 0f), Time.deltaTime*5f);
			roundsText.color = Hacks.ColorLerpAlpha (roundsText.color, -0.1f, Time.deltaTime * 5f);

			//roundsButton.transform.position = Vector3.Lerp


			if (title.color.a <= 0f) {
				title.gameObject.SetActive (false);
				startButton.gameObject.SetActive (false);
				roundsButton.gameObject.SetActive (false);
				roundsText.gameObject.SetActive (false);
				arrowUp.gameObject.SetActive (false);
				arrowDown.gameObject.SetActive (false);

				state = State.Calibrating;
				calibrado.gameObject.SetActive (true);
				readingArduino = true;
			}

		}

		if (state == State.Calibrating) {
			
			Hacks.SpriteRendererAlpha (calibrado, 1.1f, Time.deltaTime * 5f);

			adjustColorCalibradoArrow (calibradoDown, 0);
			adjustColorCalibradoArrow (calibradoLeft, 1);
			adjustColorCalibradoArrow (calibradoRight, 2);
			adjustColorCalibradoArrow (calibradoUp, 3);

		}

		handleMovingGuards ();
	
	}

	public void handleArduinoInput(string direction) {

		Debug.Log (direction);

	}

	void adjustColorCalibradoArrow(SpriteRenderer s, int threshold) {

		if (arduino.currentToMap == threshold) {
			Hacks.SpriteRendererColor (s, colorToAssign, Time.deltaTime * 5f);
		} else if (arduino.currentToMap > threshold) {
			Hacks.SpriteRendererColor (s, colorAssigned, Time.deltaTime * 5f);
		}

	}

	void disappear (SpriteRenderer s, Vector3 destination, float a, float t) {

		Hacks.GameObjectLerp (s.gameObject, destination, t);
		Hacks.SpriteRendererAlpha (s, a, t);

	}

	public void pressedStart() {

		if (state == State.Menu) {
			state = State.MenuToCalibrate;
		}

	}

	public void roundUp() {

		if (rounds < maxRounds) {
			rounds++;
			arrowDown.gameObject.SetActive(true);
			if (rounds == maxRounds) {
				arrowUp.GetComponent<ArrowUp> ().Disable ();
			}
			AudioSource a = Hacks.GetAudioSource ("Audio/Sonidos/boton_menu");
			a.volume = 0.5f;
			float modifier = ((float)rounds / (float)maxRounds);
			a.pitch = 0.7f + modifier * 0.3f;
			a.Play ();
		}

		roundsText.text = "" + rounds;

	}

	public void roundDown() {

		if (rounds > 1) {
			rounds--;
			arrowUp.gameObject.SetActive(true);
			if (rounds == 1) {
				arrowDown.GetComponent<ArrowDown> ().Disable ();
			}
			AudioSource a = Hacks.GetAudioSource ("Audio/Sonidos/boton_menu_v2");
			a.volume = 0.5f;
			float modifier = ((float)rounds / (float)maxRounds);
			a.pitch = 1f - (1f - modifier)*0.3f ;
			a.Play ();
		}

		roundsText.text = "" + rounds;

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
