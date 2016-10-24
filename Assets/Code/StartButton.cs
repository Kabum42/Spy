using UnityEngine;
using System.Collections;

public class StartButton : MonoBehaviour {

	private Main main;
	private SpriteRenderer sRenderer;
	private Color originalColor;
	private Color targetColor;

	void Start() {

		main = Camera.main.GetComponent<Main> ();
		sRenderer = this.GetComponent<SpriteRenderer> ();
		originalColor = sRenderer.color;
		targetColor = sRenderer.color;

	}

	void OnMouseDown() {

		main.pressedStart ();

	}

	void Update() {

		if (main.state == Main.State.Menu) {
			sRenderer.color = Color.Lerp (sRenderer.color, targetColor, Time.deltaTime * 5f);
		}

	}

	void OnMouseEnter() {

		targetColor = new Color (1f, 1f, 1f);

	}

	void OnMouseExit() {

		targetColor = originalColor;

	}
}
