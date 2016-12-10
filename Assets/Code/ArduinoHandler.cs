using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Collections.Generic;

public class ArduinoHandler : MonoBehaviour {

	public static SerialPort stream;
	private ArduinoInput[] order = new ArduinoInput[4];
	private ArduinoInput[] mapping = new ArduinoInput[4];
	private List<int> inputsToMap = new List<int> ();
	public int currentToMap = 0;
	private Main main;
	private bool listenArduino = true;

	// Use this for initialization
	void Start () {

		main = Camera.main.GetComponent<Main> ();

		currentToMap = 0;

		order [0] = ArduinoInput.Down;
		order [1] = ArduinoInput.Left;
		order [2] = ArduinoInput.Right;
		order [3] = ArduinoInput.Up;

		for (int i = 0; i < mapping.Length; i++) {
			mapping[i] = ArduinoInput.Null;
		}

		inputsToMap.Add (0);
		inputsToMap.Add (1);
		inputsToMap.Add (2);
		inputsToMap.Add (3);

		stream = new SerialPort("COM8", 9600);
		stream.ReadTimeout = 100;
		stream.Open();
		
	}

	void Update() {

		if (Input.GetKeyDown (KeyCode.Tab)) {
			listenArduino = !listenArduino;
		}

		if (main.readingArduino) {

			if (listenArduino) {
				ReadArduino ();
			} else {
				SimulateArduino();
			}

		}

	}

	void SimulateArduino() {

		if (Input.GetKeyDown (KeyCode.K)) {
			handleData ("0");
		}
		if (Input.GetKeyDown (KeyCode.J)) {
			handleData ("1");
		}
		if (Input.GetKeyDown (KeyCode.L)) {
			handleData ("2");
		}
		if (Input.GetKeyDown (KeyCode.I)) {
			handleData ("3");
		}

	}
	
	void ReadArduino() {
		
		DateTime initialTime = DateTime.Now;
		DateTime nowTime;
		TimeSpan diff = default(TimeSpan);

		string dataString = null;
		
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
					currentToMap++;
					inputsToMap.RemoveAt(0);
				}
			}

		} else {
			
			// SEND DATA TO MAIN
			main.handleArduinoInput (mapping [input]);

		}

	}
	
}

public enum ArduinoInput {
	Null,
	Down,
	Left,
	Right,
	Up
}
