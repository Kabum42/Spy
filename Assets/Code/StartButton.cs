using UnityEngine;
using System.Collections;

public class StartButton : MonoBehaviour {

	private Main main;
	private SpriteRenderer sRenderer;
	private Color targetColor;

	void Start() {

		main = Camera.main.GetComponent<Main> ();
		sRenderer = this.GetComponent<SpriteRenderer> ();
		targetColor = sRenderer.color;

	}

	void OnMouseDown() {

		//main.roundUp ();

	}

	void Update() {

		sRenderer.color = Color.Lerp (sRenderer.color, targetColor, Time.deltaTime * 5f);

	}

	void OnMouseEnter() {

		targetColor = new Color (1f, 1f, 1f);

	}

	void OnMouseExit() {

		targetColor = new Color (120f / 255f, 120f / 255f, 120f / 255f);

	}
}
