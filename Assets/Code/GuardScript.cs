using UnityEngine;
using System.Collections;

public class GuardScript : MonoBehaviour {

	private Main main;

	// Use this for initialization
	void Start () {
		
		main = Camera.main.GetComponent<Main> ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}
		

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.GetComponent<SpyScript>() != null) 
		{

			main.TouchedGuard ();

			/*

			//Ha detectado al espia...
			//Guardamos la direccion del guardia al espia
			Vector2 direction = new Vector2 (other.gameObject.transform.position.x - transform.position.x,
				                    other.gameObject.transform.position.y - transform.position.y);
			
			//Pintamos el rayo porque mola
			Debug.DrawRay(transform.position,direction);

			//Creamos el rayo y vemos si le da a el o se encuentra con algo antes
			RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1000f, LayerMask.NameToLayer("NotGuard"));
			if (hit != null && hit.transform.gameObject == other.gameObject) {
				main.TouchedGuard ();
				//Destroy (other.gameObject);
				Debug.Log ("El guardia " + this.name + " caza al espia");
			} else {
				Debug.Log ("El guardia " + this.name + " no encuentra al espia por culpa de " + hit.transform.gameObject.name );
			}
			*/
		}
	}

	void OnCollisionEnter2D(Collision2D other) {

		if (other.gameObject.GetComponent<SpyScript> () != null) {
			
			main.TouchedGuard ();

		}

	}

}
