using UnityEngine;
using System.Collections;

public class ArrowDown : MonoBehaviour {

	private Main main;
	private SpriteRenderer sRenderer;
	private Color targetColor;
	
	void Start() {
		
		main = Camera.main.GetComponent<Main> ();
		sRenderer = this.GetComponent<SpriteRenderer> ();
		targetColor = sRenderer.color;
		
	}

	public void Disable() {

		sRenderer.color = new Color (60f / 255f, 60f / 255f, 60f / 255f);
		targetColor = sRenderer.color;
		this.gameObject.SetActive (false);

	}
	
	void OnMouseDown() {
		
		main.roundDown ();
		
	}

	void Update() {

		sRenderer.color = Color.Lerp (sRenderer.color, targetColor, Time.deltaTime * 5f);

	}

	void OnMouseEnter() {

		targetColor = new Color (1f, 1f, 1f);

	}

	void OnMouseExit() {

		targetColor = new Color (60f / 255f, 60f / 255f, 60f / 255f);

	}

}
