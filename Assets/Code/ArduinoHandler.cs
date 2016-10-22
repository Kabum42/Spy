using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Collections.Generic;

public class ArduinoHandler : MonoBehaviour {

	public static SerialPort stream;
	private string[] order = new string[4];
	private string[] mapping = new string[4];
	private List<int> inputsToMap = new List<int> ();
	private int currentToMap = 0;
	private Main main;
	
	// Use this for initialization
	void Start () {

		order [0] = "down";
		order [1] = "left";
		order [2] = "right";
		order [3] = "up";

		for (int i = 0; i < mapping.Length; i++) {
			mapping[i] = "null";
		}

		inputsToMap.Add (0);
		inputsToMap.Add (1);
		inputsToMap.Add (2);
		inputsToMap.Add (3);

		stream = new SerialPort("COM8", 9600);
		stream.ReadTimeout = 100;
		stream.Open();

		main = Camera.main.GetComponent<Main> ();
		
	}

	void Update() {

		if (main.readingArduino) {
			ReadArduino (100);
		}

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
				stream.DiscardInBuffer();
				stream.DiscardOutBuffer();
			}
			
			nowTime = DateTime.Now;
			diff = nowTime - initialTime;
			
		} while (diff.Milliseconds < timeout);
		
	}
	
	void OnApplicationQuit() {
		if (stream != null && stream.IsOpen) {
			stream.Close ();
		}
	}
	
	void handleData(string s) {

		int input = int.Parse (s);

		if (inputsToMap.Count > 0) {

			if (inputsToMap.Contains(input)) {

				Debug.Log ("ASSIGNED " + order[currentToMap]);
				mapping[input] = order[currentToMap];
				currentToMap++;
				inputsToMap.Remove(input);

				// ASSIGN LAST ONE
				if (inputsToMap.Count == 1) {
					Debug.Log ("ASSIGNED " + order[currentToMap]);
					mapping[inputsToMap[0]] = order[currentToMap];
					inputsToMap.RemoveAt(0);
				}
			}

		} else {
			// SEND DATA TO MAIN

			Debug.Log (mapping[input]);

		}

	}
	
}
