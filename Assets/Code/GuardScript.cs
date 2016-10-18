using UnityEngine;
using System.Collections;

public class GuardScript : MonoBehaviour {

	public bool Spydetected;

	enum Direction {RIGHT,LEFT,UP,DOWN}

	Direction direction ;

	// Use this for initialization
	void Start () {
		Spydetected = false;

		direction = Direction.RIGHT;
	}
	
	// Update is called once per frame
	void Update () {
		//if (Spydetected)
			//Debug.Log ("Detectado");
	}
		

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag== "Spy") 
		{
			//Ha detectado al espia...
			//Guardamos la direccion del guardia al espia
			Vector2 direction = new Vector2 (other.gameObject.transform.position.x - transform.position.x,
				                    other.gameObject.transform.position.y - transform.position.y);
			
			//Pintamos el rayo porque mola
			Debug.DrawRay(transform.position,direction);

			//Creamos el rayo y vemos si le da a el o se encuentra con algo antes
			RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);
			if (hit != null) {
				Debug.Log ("Me choco con " + hit.transform.gameObject.name);
			}
			if (hit != null && hit.transform.gameObject.tag=="Spy") {
				Spydetected = true;
				//Destroy (other.gameObject);
				Debug.Log ("El guardia " + this.name + " caza al espia");
			} else {
				Debug.Log ("El guardia " + this.name + " no encuentra al espia por culpa de " + hit.transform.gameObject.name );
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag== "Spy") 
		{
			Spydetected = false;
		}
	}

	public void  ChangeRotation(Vector2 moving )
	{
		//FALTA ACTUAR DEPENDIENDO DE SI HAY O NO HAY PARED, PERO NO ESTOY SEGURO DE COMO HARAS LAS PAREDES

		if (moving.x>0) 
		{
			//comprobar muro
			direction = Direction.RIGHT;

			Quaternion rotation = Quaternion.Euler (0, 0, 0);
			transform.rotation = rotation;
		}

		else if (moving.x<0) 
		{
			//comprobar muro
			direction = Direction.LEFT;

			Quaternion rotation = Quaternion.Euler (0, 0, 180);
			transform.rotation = rotation;

		}else if (moving.y>0) 
		{
			//comprobar muro
			direction = Direction.UP;

			Quaternion rotation = Quaternion.Euler (0, 0, 90);
			transform.rotation = rotation;

		}else if (moving.y<0) 
		{
			//comprobar muro
			direction = Direction.DOWN;

			Quaternion rotation = Quaternion.Euler (0, 0, 270);
			transform.rotation = rotation;

		}

		//Debug.Log (direction);
	}
}
