using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Main : MonoBehaviour {

	private List<GameObject> elements = new List<GameObject> ();
	private SpyScript spy;
	private List<GuardScript> guards = new List<GuardScript>();
	private FinnishScript finnish;
	private Vector2 movingGuards = Vector2.zero;
	private int rounds = 1;
	private int maxRounds = 5;
	public static int currentRound = 1;
	public static int currentPlayer = 0;
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

	public Color[] playerColor = new Color[2];

	public Text[] playerText = new Text[2];

	public Image[] crowns = new Image[2];
	public Color goldColor;
	public Color silverColor;

	public Text countDown;
	private float countDownTime = 4f;

	public AudioSource menuMusic;
	public AudioSource playingMusic;

	public State state = State.Menu;

	private ArduinoInput lastInput = ArduinoInput.Null;
	private float maxCooldown = 1f;
	private float cooldown = 0f;
	private float calibrateToPlayTime = 0.5f;

	private float guardsDistance = 5f;
	private float guardsLockDown = 1f;

	public enum State {
		Menu,
		MenuToCalibrate,
		Calibrating,
		CalibrateToPlay,
		PreparingLevel,
		CountDown,
		Play,
		End
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

		playerText [0].color = Hacks.ColorLerpAlpha (playerColor [0], 0f, 1f);
		playerText [1].color = Hacks.ColorLerpAlpha (playerColor [1], 0f, 1f);

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

		if (cooldown > 0) {
			cooldown -= Time.deltaTime;
			if (cooldown <= 0f) {
				cooldown = 0f;
				lastInput = ArduinoInput.Null;
			}
		}

		if (state == State.Menu || state == State.MenuToCalibrate || state == State.Calibrating) {
			menuMusic.volume = Mathf.Lerp (menuMusic.volume, 0.3f, Time.deltaTime);
		}

		if (state == State.MenuToCalibrate) {

			disappear (title, originalTitlePos + new Vector3 (0f, 3f, 0f), -0.1f, Time.deltaTime * 5f);
			disappear (startButton, originalStartPos + new Vector3 (0f, -3f, 0f), -0.1f, Time.deltaTime * 5f);
			disappear (roundsButton, originalRoundsPos + new Vector3 (0f, -3f, 0f), -0.1f, Time.deltaTime * 5f);
			disappear (arrowUp, originalArrowUpPos + new Vector3 (0f, -3f, 0f), -0.1f, Time.deltaTime * 5f);
			disappear (arrowDown, originalArrowDownPos + new Vector3 (0f, -3f, 0f), -0.1f, Time.deltaTime * 5f);

			Hacks.GameObjectLerp (roundsText.gameObject, originalRoundsTextPos + new Vector3 (0f, -3f, 0f), Time.deltaTime * 5f);
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

		} else if (state == State.Calibrating) {
			
			Hacks.SpriteRendererAlpha (calibrado, 1.1f, Time.deltaTime * 5f);

			adjustColorCalibradoArrow (calibradoDown, 0);
			adjustColorCalibradoArrow (calibradoLeft, 1);
			adjustColorCalibradoArrow (calibradoRight, 2);
			adjustColorCalibradoArrow (calibradoUp, 3);

			if (arduino.currentToMap >= 4) {
				state = State.CalibrateToPlay;
			}

		} else if (state == State.CalibrateToPlay) {

			menuMusic.volume = Mathf.Lerp (menuMusic.volume, -0.1f, Time.deltaTime * 5f);
			playingMusic.volume = Mathf.Lerp (playingMusic.volume, 0.25f, Time.deltaTime * 5f);

			changeArrowsAccordingToInput ();

			if (calibrateToPlayTime > 0f) {
				calibrateToPlayTime -= Time.deltaTime;
				if (calibrateToPlayTime <= 0f) {
					buildMap ();
				}
			} 

		} else if (state == State.PreparingLevel) {

			ChangeAlphaElements (1.1f, Time.deltaTime * 5f);
			playerText [0].color = Hacks.ColorLerpAlpha (playerText [0].color, 1.1f, Time.deltaTime * 5f);
			playerText [1].color = Hacks.ColorLerpAlpha (playerText [1].color, 1.1f, Time.deltaTime * 5f);

			if (elements [0].GetComponent<SpriteRenderer> ().color.a >= 1f) {
				countDown.color = Hacks.ColorLerpAlpha (countDown.color, 0f, 1f);
				countDown.gameObject.SetActive (true);
				countDownTime = 4f;
				state = State.CountDown;
			}

		} else if (state == State.CountDown) {
			
			int lastFloor = Mathf.FloorToInt (countDownTime);
			countDownTime -= Time.deltaTime;
			if (countDownTime < 0f) {
				countDownTime = 0f;
			}

			if (lastFloor != Mathf.FloorToInt (countDownTime)) {
				countDown.transform.localScale = new Vector3 (1f, 1f, 1f);
				countDown.color = Hacks.ColorLerpAlpha (countDown.color, 1f, 1f);
			}

			countDown.transform.localScale = Vector3.Lerp (countDown.transform.localScale, Vector3.zero, Time.deltaTime);
			countDown.color = Hacks.ColorLerpAlpha (countDown.color, 0f, Time.deltaTime);
			countDown.text = "" + Mathf.FloorToInt (countDownTime);

			if (countDownTime == 0f) {
				countDown.transform.localScale = new Vector3 (1f, 1f, 1f);
				countDown.color = Hacks.ColorLerpAlpha (countDown.color, 1f, 1f);
				countDown.text = "GO!";
				state = State.Play;
			}

		} else if (state == State.Play) {

			countDown.color = Hacks.ColorLerpAlpha (countDown.color, 0f, Time.deltaTime * 10f);
			changeArrowsAccordingToInput ();
			handleMovingGuards ();

		} else if (state == State.End) {

			playerText [0].rectTransform.anchoredPosition = Vector2.Lerp (playerText [0].rectTransform.anchoredPosition, new Vector2 (playerText [0].rectTransform.anchoredPosition.x, 0f), Time.deltaTime * 5f);
			playerText [1].rectTransform.anchoredPosition = Vector2.Lerp (playerText [1].rectTransform.anchoredPosition, new Vector2 (playerText [1].rectTransform.anchoredPosition.x, 0f), Time.deltaTime * 5f);
			playerText [0].transform.localScale = Vector3.Lerp (playerText [0].transform.localScale, new Vector3 (1f, 1f, 1f), Time.deltaTime * 10f);
			playerText [1].transform.localScale = Vector3.Lerp (playerText [1].transform.localScale, new Vector3 (1f, 1f, 1f), Time.deltaTime * 10f);

			if (playerText [0].transform.localScale.x > 0.99f) {
				
				int points1 = int.Parse (playerText [0].text);
				int points2 = int.Parse (playerText [1].text);

				if (points1 > points2) {

					crowns [0].color = Hacks.ColorLerpAlpha (crowns [0].color, 1f, Time.deltaTime*5f);
					crowns [0].rectTransform.anchoredPosition = playerText [0].rectTransform.anchoredPosition + new Vector2 (0f, 150f);

				} else if (points2 > points1) {

					crowns [1].color = Hacks.ColorLerpAlpha (crowns [1].color, 1f, Time.deltaTime*5f);
					crowns [1].rectTransform.anchoredPosition = playerText [1].rectTransform.anchoredPosition + new Vector2 (0f, 150f);

				} else {

					crowns [0].color = Hacks.ColorLerpAlpha (crowns [0].color, 1f, Time.deltaTime*5f);
					crowns [0].rectTransform.anchoredPosition = playerText [0].rectTransform.anchoredPosition + new Vector2 (0f, 150f);
					crowns [1].color = Hacks.ColorLerpAlpha (crowns [1].color, 1f, Time.deltaTime*5f);
					crowns [1].rectTransform.anchoredPosition = playerText [1].rectTransform.anchoredPosition + new Vector2 (0f, 150f);

				}

			}

			if (Input.GetKeyDown (KeyCode.Return)) {

				currentPlayer = 0;
				currentRound = 1;
				crowns [0].color = Hacks.ColorLerpAlpha (crowns [0].color, 0f, 1f);
				crowns [1].color = Hacks.ColorLerpAlpha (crowns [0].color, 0f, 1f);

				playerText [0].transform.localScale = Vector3.one * 0.5f;
				playerText [0].rectTransform.anchorMin = new Vector2 (0.5f, 1f);
				playerText [0].rectTransform.anchorMax = new Vector2 (0.5f, 1f);
				playerText [0].rectTransform.anchoredPosition = new Vector2 (-200f, -70f);
				playerText [1].transform.localScale = Vector3.one * 0.5f;
				playerText [1].rectTransform.anchorMin = new Vector2 (0.5f, 1f);
				playerText [1].rectTransform.anchorMax = new Vector2 (0.5f, 1f);
				playerText [1].rectTransform.anchoredPosition = new Vector2 (200f, -70f);

				calibrado.gameObject.SetActive (true);

				buildMap ();

			}

			if (Input.GetKeyDown (KeyCode.Escape)) {

				Application.LoadLevel ("Main");

			}

		}
	
	}

	void buildMap() {

		foreach (GameObject element in elements) {
			Destroy (element);
		}

		elements.Clear ();
		spy = null;
		guards.Clear ();

		loadMap ("mapa1");

		spy.GetComponent<SpriteRenderer> ().color = playerColor [currentPlayer];
		finnish.GetComponent<SpriteRenderer> ().color = Hacks.ColorLerpAlpha (playerColor [currentPlayer], 0.5f, 1f);

		int otherPlayer = GetOtherPlayer ();

		foreach (GuardScript guard in guards) {
			guard.GetComponent<SpriteRenderer> ().color = playerColor [otherPlayer];
			guard.transform.FindChild ("vision").GetComponent<SpriteRenderer> ().color = Hacks.ColorLerpAlpha(playerColor [otherPlayer], 50f/255f, 1f);
		}

		state = State.PreparingLevel;
			
	}

	int GetOtherPlayer() {

		int otherPlayer = currentPlayer + 1;
		if (otherPlayer > 1) {
			otherPlayer = 0;
		}

		return otherPlayer;

	}

	void loadMap(string mapName) {

		DestroyAllElements ();

		string path = "Maps/"+ mapName +".txt";

		string mapInfo = System.IO.File.ReadAllText (path);

		string[] stringTiles = mapInfo.Split (EditorMain.tileChar);

		foreach (string stringTile in stringTiles) {

			string[] info = stringTile.Split(EditorMain.infoChar);

			AddTileAtMatrixPos(info[0], new Vector3(float.Parse(info[1]), float.Parse(info[2]), float.Parse(info[3])), float.Parse(info[4]));

		}

		ChangeAlphaElements (0f, 1f);

	}

	void DestroyAllElements() {

		while (elements.Count > 0) {
			GameObject element = elements[0];
			elements.RemoveAt(0);
			Destroy (element);
		}

	}

	void ChangeAlphaElements(float a, float t) {

		foreach (GameObject element in elements) {
			if (element == finnish.gameObject && a > 0.5f) {
				a = 0.5f;
			}
			Hacks.SpriteRendererAlpha (element.GetComponent<SpriteRenderer> (), a, t);
		}

	}


	void AddTileAtMatrixPos(string tileName, Vector3 matrixPos, float rotationZ) {

		GameObject originalTile = Resources.Load("Prefabs/"+tileName) as GameObject;
		GameObject tile = Instantiate (originalTile);
		tile.transform.position = MatrixToWorld (matrixPos);
		tile.transform.eulerAngles = new Vector3 (0, 0, rotationZ);
		tile.name = tileName;
		elements.Add (tile);

		if (tile.GetComponent<SpyScript> () != null) {
			spy = tile.GetComponent<SpyScript> ();
		}

		if (tile.GetComponent<GuardScript> () != null) {
			guards.Add (tile.GetComponent<GuardScript> ());
		}

		if (tile.GetComponent<FinnishScript> () != null) {
			finnish = tile.GetComponent<FinnishScript> ();
		}

	}

	Vector3 MatrixToWorld(Vector3 matrixPosition) {

		Vector3 worldPos = Vector3.zero;
		worldPos = new Vector3 (matrixPosition.x, matrixPosition.y, matrixPosition.z) * EditorMain.distancePerMatrixPosition;

		return worldPos;

	}

	public void TouchedFinnish() {

		VictoryCondition (currentPlayer);

	}

	public void TouchedGuard() {

		VictoryCondition (GetOtherPlayer ());

	}

	void VictoryCondition(int player) {

		int points = int.Parse (playerText [player].text) + 1;
		playerText[player].text = ""+ points;

		if (currentPlayer == 1) {
			currentRound++;
		}

		currentPlayer = GetOtherPlayer ();

		if (currentRound > rounds) {
			
			foreach (GameObject element in elements) {
				Destroy (element);
			}

			calibrado.gameObject.SetActive (false);

			Vector3 previous0 = playerText [0].transform.position;
			Vector3 previous1 = playerText [1].transform.position;
			playerText [0].rectTransform.anchorMin = new Vector2 (0.5f, 0.5f);
			playerText [0].rectTransform.anchorMax = new Vector2 (0.5f, 0.5f);
			playerText [0].transform.position = previous0;
			playerText [1].rectTransform.anchorMin = new Vector2 (0.5f, 0.5f);
			playerText [1].rectTransform.anchorMax = new Vector2 (0.5f, 0.5f);
			playerText [1].transform.position = previous1;

			int points1 = int.Parse (playerText [0].text);
			int points2 = int.Parse (playerText [1].text);

			if (points1 > points2) {

				crowns [0].color = Hacks.ColorLerpAlpha (goldColor, 0f, 1f);

			} else if (points2 > points1) {

				crowns [1].color = Hacks.ColorLerpAlpha (goldColor, 0f, 1f);

			} else {

				crowns [0].color = Hacks.ColorLerpAlpha (silverColor, 0f, 1f);
				crowns [1].color = Hacks.ColorLerpAlpha (silverColor, 0f, 1f);

			}

			state = State.End;

		} else {
			
			buildMap ();

		}

	}

	void changeArrowsAccordingToInput() {

		calibrado.transform.position = Vector3.Lerp (calibrado.transform.position, new Vector3 (0f, 25.5f, 0f), Time.deltaTime * 10f);
		calibrado.transform.localScale = Vector3.Lerp (calibrado.transform.localScale, new Vector3 (0.3f, 0.3f, 0.3f), Time.deltaTime * 10f);

		// DOWN
		changeArrowAccordingToInput(calibradoDown, ArduinoInput.Down);

		// LEFT
		changeArrowAccordingToInput(calibradoLeft, ArduinoInput.Left);

		// RIGHT
		changeArrowAccordingToInput(calibradoRight, ArduinoInput.Right);

		// UP
		changeArrowAccordingToInput(calibradoUp, ArduinoInput.Up);

	}

	void changeArrowAccordingToInput(SpriteRenderer calibradoArrow, ArduinoInput associatedInput) {
		
		if (lastInput == associatedInput) {
			Hacks.SpriteRendererColor (calibradoArrow, colorToAssign, Time.deltaTime * 10f);
			calibradoArrow.transform.localScale = Vector3.Lerp (calibradoArrow.transform.localScale, Vector3.one * 3f, Time.deltaTime * 10f);
		} else {
			Hacks.SpriteRendererColor (calibradoArrow, colorAssigned, Time.deltaTime * 10f);
			calibradoArrow.transform.localScale = Vector3.Lerp (calibradoArrow.transform.localScale, Vector3.one * 1.5f, Time.deltaTime * 10f);
		}

	}

	public void handleArduinoInput(ArduinoInput direction) {

		if (lastInput == ArduinoInput.Null) {
			cooldown = maxCooldown;
			lastInput = direction;
			Debug.Log (direction.ToString());
		}

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

		if (movingGuards == Vector2.zero && lastInput == ArduinoInput.Null) {

			if (Input.GetKeyDown (KeyCode.I)) {

				movingGuards = new Vector2 (0, guardsLockDown);

			} else if (Input.GetKeyDown (KeyCode.K)) {

				movingGuards = new Vector2 (0, -guardsLockDown);

			} else if (Input.GetKeyDown (KeyCode.J)) {

				movingGuards = new Vector2 (-guardsLockDown, 0);

			} else if (Input.GetKeyDown (KeyCode.L)) {

				movingGuards = new Vector2 (guardsLockDown, 0);

			}

		} else {

			foreach (GuardScript guard in guards) {

				Vector2 direction = new Vector2 (movingGuards.x, movingGuards.y).normalized;
				float amountX = 0f;
				float amountY = 0f;

				if (movingGuards.x != 0) {
					amountX = direction.x * guardsDistance * (1f/guardsLockDown) * Time.deltaTime;
				}
				if (movingGuards.y != 0) {
					amountY = direction.y * guardsDistance * (1f/guardsLockDown) * Time.deltaTime;
				}

				guard.GetComponent<Rigidbody2D>().MovePosition(guard.transform.position + new Vector3 (amountX, amountY, 0f));


				float arrowAngle = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
				guard.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(guard.transform.localEulerAngles.z, arrowAngle, Time.deltaTime*10f));

			}

			movingGuards = Vector2.MoveTowards (movingGuards, Vector2.zero, Time.deltaTime);

		}

	}

}
