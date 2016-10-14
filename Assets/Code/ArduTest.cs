using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;

public class ArduTest : MonoBehaviour {

	public static SerialPort stream;

	// Use this for initialization
	void Start () {

		stream = new SerialPort("COM6", 9600);
		stream.ReadTimeout = 100;
		stream.Open();

	}
	
	// Update is called once per frame
	void Update () {

		ReadArduino (100);

	}

	void ReadArduino(int timeout) {

		DateTime initialTime = DateTime.Now;
		DateTime nowTime;
		TimeSpan diff = default(TimeSpan);
		
		string dataString = null;

		do {
			try {
				dataString = stream.ReadLine();
			}
			catch (TimeoutException) {
				dataString = null;
			}
			
			if (dataString != null)
			{
				handleData(dataString);
			}
			
			nowTime = DateTime.Now;
			diff = nowTime - initialTime;
			
		} while (diff.Milliseconds < timeout);

	}

	void OnApplicationQuit() {
		stream.Close ();
	}

	void handleData(string s) {
			Debug.Log (s);
	}
	
}
