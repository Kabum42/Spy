using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EditorMain : MonoBehaviour {

	public GameObject cursorTile;
	private List<GameObject> cursorChilds = new List<GameObject> ();
	private int selectedChild = 0;
	private static float distancePerMatrixPosition = 0.2f;
	private float zoom = 1f;
	private List<GameObject> tiles = new List<GameObject> ();
	public Text layerText;
	private static int minLayer = 1;
	private static int maxLayer = 5;
	private int currentLayer = minLayer;

	// Use this for initialization
	void Start () {

		addCursorChild ("Spy");
		addCursorChild ("End");
		addCursorChild ("Wall");
		addCursorChild ("Guard");
	
	}
	
	// Update is called once per frame
	void Update () {

		handleZoom ();

		Vector2 cursorInMatrix = WorldToMatrix (Camera.main.ScreenToWorldPoint(Input.mousePosition));
		cursorTile.transform.position = MatrixToWorld (cursorInMatrix);
		cursorTile.transform.position = new Vector3 (cursorTile.transform.position.x, cursorTile.transform.position.y, -1f);

		handleChangingChild ();
		handleChangingLayer ();
		handleClicks ();

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
		Camera.main.orthographicSize = zoom*zoom;

	}

	void handleClicks() {

		Vector2 cursorInMatrix = WorldToMatrix (Camera.main.ScreenToWorldPoint(Input.mousePosition));


		if (Input.GetMouseButton(0)) {
			// ADD TILE
			DestroyTileAtMatrixPos(cursorInMatrix);
			AddTileAtMatrixPos (cursorInMatrix);
		} else if (Input.GetMouseButton(1)) {
			// REMOVE TILE
			DestroyTileAtMatrixPos(cursorInMatrix);
		}

	}

	void DestroyTileAtMatrixPos(Vector2 matrixPos) {
		if (GetTileAtMatrixPos (matrixPos) != null) {
			GameObject tile = GetTileAtMatrixPos (matrixPos);
			tiles.Remove (tile);
			Destroy (tile);
		}
	}

	void AddTileAtMatrixPos(Vector2 matrixPos) {
		GameObject tile = Instantiate ((cursorChilds [selectedChild]) as GameObject);
		tile.transform.position = MatrixToWorld (matrixPos);
		tiles.Add (tile);
	}

	GameObject GetTileAtMatrixPos(Vector2 matrixPos) {

		foreach (GameObject tile in tiles) {
			if (WorldToMatrix (tile.transform.position) == matrixPos) {
				return tile;
			}
		}

		return null;

	}

	Vector2 WorldToMatrix(Vector3 worldPosition) {

		Vector2 matrixPos = Vector2.zero;
		int xPos = (int) Mathf.Floor((worldPosition.x +distancePerMatrixPosition/2f) / distancePerMatrixPosition);
		int yPos = (int) Mathf.Floor((worldPosition.y +distancePerMatrixPosition/2f) / distancePerMatrixPosition);
		matrixPos = new Vector2 (xPos, yPos);

		return matrixPos;

	}

	Vector3 MatrixToWorld(Vector2 matrixPosition) {

		Vector3 worldPos = Vector3.zero;
		worldPos = new Vector3 (matrixPosition.x, matrixPosition.y, 0f) * distancePerMatrixPosition;

		return worldPos;

	}

}
