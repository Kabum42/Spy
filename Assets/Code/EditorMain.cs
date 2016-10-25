using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System.Linq;

public class EditorMain : MonoBehaviour {

	public GameObject cursorTile;
	private List<GameObject> cursorChilds = new List<GameObject> ();
	private int selectedChild = 0;
	public static float distancePerMatrixPosition = 4f;
	private float zoom = 1f;
	private List<GameObject> tiles = new List<GameObject> ();
	public Text layerText;
	public Text selectedText;
	public Text rotationText;
	private static int minLayer = 1;
	private static int maxLayer = 5;
	private int currentLayer = minLayer;
	private int currentRotation = 0;

	public InputField mapName;

	private bool saving = false;
	private bool loading = false;

	public static char tileChar = '@';
	public static char infoChar = '#';

	// Use this for initialization
	void Start () {


		GameObject[] prefabs = Resources.LoadAll("Prefabs", typeof(GameObject)).Cast<GameObject>().ToArray();

		foreach (GameObject prefab in prefabs){
			addCursorChild(prefab.name);
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		selectedText.text = "Selected: <b>" + cursorChilds [selectedChild].name + "</b>";
		rotationText.text = "Rotation: <b>" + currentRotation + "</b>";

		if (saving) {
			handleSaving ();
		} else if (loading) {
			handleLoading();
		}  else {


			handleZoom ();
			
			Vector3 cursorInMatrix = GetCursorInMatrix ();

			cursorTile.transform.position = MatrixToWorld (cursorInMatrix);

			handleChangingChild ();
			handleRotation ();
			handleChangingLayer ();
			handleClicks ();

			if (Input.GetKey(KeyCode.G)) {
				saving = true;
				mapName.gameObject.SetActive(true);
				layerText.text = "GUARDANDO";
			} else if (Input.GetKey(KeyCode.C)) {
				loading = true;
				mapName.gameObject.SetActive(true);
				layerText.text = "CARGANDO";
			}
		}


	}

	void handleRotation() {

		if (Input.GetKeyDown (KeyCode.R)) {
			currentRotation += 90;
			if (currentRotation >= 360) {
				currentRotation -= 360;
			}
		}

		cursorChilds [selectedChild].transform.eulerAngles = new Vector3 (0, 0, currentRotation);

	}

	void handleSaving() {

		EventSystem.current.SetSelectedGameObject (mapName.gameObject);

		if (Input.GetKeyDown (KeyCode.Escape)) {
			mapName.gameObject.SetActive (false);
			saving = false;
		} else if (Input.GetKeyDown (KeyCode.Return)) {
			mapName.gameObject.SetActive (false);
			saving = false;
			saveMap();
		}

	}

	void handleLoading() {

		EventSystem.current.SetSelectedGameObject (mapName.gameObject);

		if (Input.GetKeyDown (KeyCode.Escape)) {
			mapName.gameObject.SetActive(false);
			loading = false;
		} else if (Input.GetKeyDown (KeyCode.Return)) {
			mapName.gameObject.SetActive(false);
			loading = false;
			loadMap();
		}

	}

	void saveMap() {

		string path = "Maps/"+ mapName.text +".txt";

		string mapInfo = "";
		bool firstTile = true;

		foreach (GameObject tile in tiles) {

			if (!firstTile) {
				mapInfo += tileChar;
			}

			mapInfo += tile.name;

			Vector3 matrixPos = WorldToMatrix(tile.transform.position);

			mapInfo += infoChar;
			mapInfo += matrixPos.x;

			mapInfo += infoChar;
			mapInfo += matrixPos.y;

			mapInfo += infoChar;
			mapInfo += matrixPos.z;

			mapInfo += infoChar;
			mapInfo += tile.transform.eulerAngles.z;

			firstTile = false;
		}

		System.IO.File.WriteAllText (path, mapInfo);

	}

	void loadMap() {

		DestroyAllTiles ();

		string path = "Maps/"+ mapName.text +".txt";
		
		string mapInfo = System.IO.File.ReadAllText (path);

		string[] stringTiles = mapInfo.Split (tileChar);

		foreach (string stringTile in stringTiles) {

			string[] info = stringTile.Split(infoChar);

			AddTileAtMatrixPos(info[0], new Vector3(float.Parse(info[1]), float.Parse(info[2]), float.Parse(info[3])), float.Parse(info[4]));

		}


	}

	void handleChangingLayer() {

		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			currentLayer++;
			if (currentLayer > maxLayer) {
				currentLayer = maxLayer;
			}
		} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
			currentLayer--;
			if (currentLayer < minLayer) {
				currentLayer = minLayer;
			}
		}

		layerText.text = "Current Layer: " + currentLayer;

	}

	void handleChangingChild() {

		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			selectedChild--;
			if (selectedChild < 0) {
				selectedChild = cursorChilds.Count - 1;
			}
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			selectedChild++;
			if (selectedChild > cursorChilds.Count - 1) {
				selectedChild = 0;
			}
		}

		selectChild(selectedChild);

	}

	void addCursorChild(string name) {

		GameObject child = Instantiate (Resources.Load ("Prefabs/"+name) as GameObject);
		Destroy (child.GetComponent<SpyScript> ());
		child.transform.SetParent (cursorTile.transform);
		child.transform.localPosition = Vector3.zero;
		cursorChilds.Add (child);
		child.name = name;

	}

	void selectChild (int pos) {

		foreach (GameObject child in cursorChilds) {
			child.SetActive (false);
		}

		cursorChilds [pos].SetActive (true);

	}

	void handleZoom() {

		zoom -= Input.mouseScrollDelta.y * 0.05f;
		zoom = Mathf.Clamp (zoom, 1f/3f, 3f);
		Camera.main.orthographicSize = zoom*zoom*30f;

	}

	private Vector3 GetCursorInMatrix() {

		Vector3 cursorInMatrix;
		Vector3 cursorWorld = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		cursorInMatrix = WorldToMatrix (new Vector3(cursorWorld.x, cursorWorld.y, -currentLayer));

		return cursorInMatrix;

	}

	void handleClicks() {

		Vector3 cursorInMatrix = GetCursorInMatrix ();

		if (Input.GetMouseButton(0)) {
			// ADD TILE
			DestroyTileAtMatrixPos(cursorInMatrix);
			AddTileAtMatrixPos (cursorChilds [selectedChild].name, cursorInMatrix, currentRotation);
		} else if (Input.GetMouseButton(1)) {
			// REMOVE TILE
			DestroyTileAtMatrixPos(cursorInMatrix);
		}

	}

	void DestroyTileAtMatrixPos(Vector3 matrixPos) {

		GameObject tile = GetTileAtMatrixPos (matrixPos);
		if (tile != null) {
			tiles.Remove (tile);
			Destroy (tile);
		}

	}

	void DestroyAllTiles() {

		while (tiles.Count > 0) {
			GameObject tile = tiles[0];
			tiles.RemoveAt(0);
			Destroy (tile);
		}

	}

	void AddTileAtMatrixPos(string tileName, Vector3 matrixPos, float rotationZ) {

		GameObject originalTile = Resources.Load("Prefabs/"+tileName) as GameObject;
		GameObject tile = Instantiate (originalTile);
		tile.transform.position = MatrixToWorld (matrixPos);
		tile.transform.eulerAngles = new Vector3 (0, 0, rotationZ);
		tile.name = tileName;
		Destroy (tile.GetComponent<SpyScript> ());
		tiles.Add (tile);

	}

	GameObject GetTileAtMatrixPos(Vector3 matrixPos) {

		foreach (GameObject tile in tiles) {
			if (WorldToMatrix (tile.transform.position) == matrixPos) {
				return tile;
			}
		}

		return null;

	}

	Vector3 WorldToMatrix(Vector3 worldPosition) {

		Vector3 matrixPos = Vector3.zero;
		int xPos = (int) Mathf.Floor((worldPosition.x +distancePerMatrixPosition/2f) / distancePerMatrixPosition);
		int yPos = (int) Mathf.Floor((worldPosition.y +distancePerMatrixPosition/2f) / distancePerMatrixPosition);
		int zPos = (int) Mathf.Floor((worldPosition.z +distancePerMatrixPosition/2f) / distancePerMatrixPosition);
		matrixPos = new Vector3 (xPos, yPos, zPos);

		return matrixPos;

	}

	Vector3 MatrixToWorld(Vector3 matrixPosition) {

		Vector3 worldPos = Vector3.zero;
		worldPos = new Vector3 (matrixPosition.x, matrixPosition.y, matrixPosition.z) * distancePerMatrixPosition;

		return worldPos;

	}

}
