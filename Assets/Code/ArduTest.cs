using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class ArduTest : MonoBehaviour {

	public static SerialPort stream;

	// Use this for initialization
	void Start () {

		stream = new SerialPort("COM2", 9600);
		stream.ReadTimeout = 50;
		stream.Open();

	}
	
	// Update is called once per frame
	void Update () {
	
	}



	public void WriteToArduino(string message) {
		stream.WriteLine(message);
		stream.BaseStream.Flush();
	}


	public string ReadFromArduino (int timeout = 0) {
		stream.ReadTimeout = timeout;        
		try {
			return stream.ReadLine();
		}
		catch (UnityException) {
			return null;
		}
	}

}
