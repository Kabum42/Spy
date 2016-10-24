using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Hacks {

	private static List<AudioSource> hacksAudioSources = new List<AudioSource> ();
	private static GameObject parentAudioSources;
	public static GameObject gameObject1 = new GameObject();
	public static GameObject gameObject2 = new GameObject();

    // TEXT ALPHA
    public static void TextAlpha(GameObject g, float a)
    {
        g.GetComponent<TextMesh>().color = new Color(g.GetComponent<TextMesh>().color.r, g.GetComponent<TextMesh>().color.g, g.GetComponent<TextMesh>().color.b, a);
    }

    public static void TextAlpha(TextMesh t, float a)
    {
        t.color = new Color(t.color.r, t.color.g, t.color.b, a);
    }

    // SPRITE RENDERER ALPHA
    public static void SpriteRendererAlpha(GameObject g, float a)
    {
        SpriteRendererAlpha(g.GetComponent<SpriteRenderer>(), a);
    }

    public static void SpriteRendererAlpha(SpriteRenderer s, float a)
    {
        s.color = new Color(s.color.r, s.color.g, s.color.b, a);
    }

	public static void SpriteRendererAlpha(SpriteRenderer s, float a, float t)
	{
		s.color = Color.Lerp (s.color, new Color (s.color.r, s.color.g, s.color.b, a), t);
	}

    // SPRITE RENDERER COLOR
    public static void SpriteRendererColor(GameObject g, Color c)
    {
        SpriteRendererColor(g.GetComponent<SpriteRenderer>(), c);
    }

    public static void SpriteRendererColor(SpriteRenderer s, Color c)
    {
        s.color = new Color(c.r, c.g, c.b, s.color.a);
    }

	public static void SpriteRendererColor(SpriteRenderer s, Color c, float t)
	{
		s.color = Color.Lerp (s.color, c, t);
	}

	// COLOR ALPHA
	public static Color ColorLerpAlpha(Color c, float alpha, float t) {

		c = Color.Lerp (c, new Color (c.r, c.g, c.b, alpha), t);
		return c;

	}

	// LERP VECTOR
	public static Vector3 LerpVector3(Vector3 origin, Vector3 target, float delta) {

		float resultX = Mathf.Lerp(origin.x, target.x, delta);
		float resultY = Mathf.Lerp(origin.y, target.y, delta);
		float resultZ = Mathf.Lerp(origin.z, target.z, delta);

		return new Vector3 (resultX, resultY, resultZ);

	}

	public static Vector3 LerpVector3Angle(Vector3 origin, Vector3 target, float delta) {
		
		float resultX = Mathf.LerpAngle(origin.x, target.x, delta);
		float resultY = Mathf.LerpAngle(origin.y, target.y, delta);
		float resultZ = Mathf.LerpAngle(origin.z, target.z, delta);
		
		return new Vector3 (resultX, resultY, resultZ);
		
	}

	// POSITION GAMEOBJECT
	public static void GameObjectLerp(GameObject g, Vector3 position, float t) {
		g.transform.position = Vector3.Lerp (g.transform.position, position, t);
	}

    // BINARY PERLIN
    public static int BinaryPerlin(int bits, float seedX, float seedY)
    {
        int result = 0;
        float aux;

        for (int i = 1; i <= bits; i++)
        {
            aux = (Mathf.Clamp(Mathf.PerlinNoise(seedX, seedY), 0f, 1f));
            seedX += 1.573576868f;
            if (aux > 0.5f) { result += (int) Mathf.Pow(2, i-1); }
        }

        return result;
    }

    public static float BinaryPerlin(float min, float max, int bits, float seedX, float seedY)
    {
        float result = min + ((float)Hacks.BinaryPerlin(bits, seedX, seedY)) / (Mathf.Pow(2, bits) -1) * (max - min);

        return result;
    }

    // XBOX CONTROLLER
    public static bool ControllerAnyConnected()
    {
        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            if (Input.GetJoystickNames()[i] != "")
            {
                return true;
            }
        }
        return false;
    }


	// TEXT
	public static string TextMultilineCentered(GameObject g, string s) {


        string previousString = g.GetComponent<TextMesh>().text;

		string result = "";
		string[] lines = s.Split('\n');
		float maxSizeX = 0f;

		for (int i = 0; i < lines.Length; i++) {
			g.GetComponent<TextMesh>().text = lines[i];
			if (g.GetComponent<Renderer>().bounds.size.x > maxSizeX) {
				maxSizeX = g.GetComponent<Renderer>().bounds.size.x;
			}
		}

		for (int i = 0; i < lines.Length; i++) {
			g.GetComponent<TextMesh>().text = lines[i];
			while (g.GetComponent<Renderer>().bounds.size.x < maxSizeX) {
				lines[i] = " "+lines[i]+" "; 
				g.GetComponent<TextMesh>().text = lines[i];
			}
			result += lines[i];
			if (i < lines.Length-1) { result += "\n"; }
		}

        g.GetComponent<TextMesh>().text = previousString;

		return result;

	}

	// DETECT MOUSE OVER GAMEOBJECT

	public static bool isOver(GameObject target)
	{

		Ray aimingRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast (aimingRay, out hit)) {

			if (hit.collider.gameObject == target) {
				return true;
			}

		} else if (target == null) {
			return true;
		}
		
		return false;
		
	}

	// SOUND
	public static AudioSource GetAudioSource() {

		if (parentAudioSources != Camera.main.gameObject) {
			hacksAudioSources.Clear ();
			parentAudioSources = Camera.main.gameObject;
		}

		AudioSource aux = null;

		for (int i = 0; i < hacksAudioSources.Count; i++) {
			if (!hacksAudioSources [i].isPlaying) {
				aux = hacksAudioSources [i];
				break;
			}
		}

		if (aux == null) {
			aux = Camera.main.gameObject.AddComponent<AudioSource> ();
			aux.spatialBlend = 0f;
			aux.loop = false;
			aux.playOnAwake = false;
			hacksAudioSources.Add (aux);
		}

		return aux;
	}

	public static AudioSource GetAudioSource(string resourcesPath) {

		AudioSource aux = GetAudioSource ();
		aux.clip = Resources.Load(resourcesPath) as AudioClip;
		return aux;

	}

	// APPLICATION
	public static IEnumerator LockCursor(float time)
	{
		if (time < 0f) { 
			// USE DEFAULT
			time = 0.1f;
		}

		yield return new WaitForSeconds(time);
		// Code to execute after the delay
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.SetCursor (null, Vector2.zero, CursorMode.Auto);
	}

	// TEXTURE2D
	public static Texture2D GlitchTexture2D(Texture2D sourceTex) {

		bool corrupted = false;
		Texture2D newTex = sourceTex;

		while (!corrupted) {

			byte[] bytes = sourceTex.EncodeToJPG();
			string data = System.Convert.ToBase64String(bytes);

			string newData = data;
			int iterations = UnityEngine.Random.Range(1, 100);

			for (int i = 0; i < iterations; i++)
			{
				newData = modifyInfo(newData);
			}

			byte[] newBytes = System.Convert.FromBase64String(newData);

			newTex = MonoBehaviour.Instantiate(sourceTex);
			newTex.LoadImage(newBytes);

			if (newTex.height != 8)
			{
				corrupted = true;
			}

		}

		return newTex;

	}
		
	private static string modifyInfo(string data)
	{
		int delete = 1;
		int position = UnityEngine.Random.Range(0, data.Length -delete);

		string prefix = data.Substring(0, position);
		string sufix = data.Substring(position + delete, data.Length - (position + delete));

		string randomChar = UnityEngine.Random.Range(1, 9).ToString();

		return (prefix + randomChar + sufix);
	}


	public static void SetLayerRecursively(GameObject obj, int newLayer)
	{
		obj.layer = newLayer;

		foreach (Transform child in obj.transform)
		{
			SetLayerRecursively(child.gameObject, newLayer);
		}
	}

	public static float GetMedianVolume(AudioSource audio) {
		float[] spectrum = audio.GetSpectrumData(1024, 0, FFTWindow.BlackmanHarris);
		float median_volume = 0f;
		for (int i = 0; i < spectrum.Length; i++) {
			median_volume += spectrum[i];
		}
		median_volume = median_volume / spectrum.Length;
		return median_volume;
	}

	public static string ColorToHexString(Color color) {

		string hexString = "";

		int r = (int) Mathf.Min(255, (color.r * 255f));
		int g = (int) Mathf.Min(255, (color.g * 255f));
		int b = (int) Mathf.Min(255, (color.b * 255f));
		int a = (int) Mathf.Min(255, (color.a * 255f));

		hexString = r.ToString ("X2") + g.ToString ("X2") + b.ToString ("X2") + a.ToString ("X2");

		return hexString;

	}

	public static Vector3 EulerAnglesLookAt(Vector3 origin, Vector3 destination) {

		if (gameObject1 == null) {
			gameObject1 = new GameObject ();
		}

		if (gameObject2 == null) {
			gameObject2 = new GameObject ();
		}

		gameObject1.transform.position = origin;
		gameObject2.transform.position = destination;

		gameObject1.transform.LookAt (gameObject2.transform);

		return gameObject1.transform.eulerAngles;

	}

	public static Vector3 GetBSpline (Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t) {

		double[] a = new double[4];
		double[] b = new double[4];
		double[] c = new double[4];

		a[0] = (-p1.x + 3 * p2.x - 3 * p3.x + p4.x) / 6.0;
		a[1] = (3 * p1.x - 6 * p2.x + 3 * p3.x) / 6.0;
		a[2] = (-3 * p1.x + 3 * p3.x) / 6.0;
		a[3] = (p1.x + 4 * p2.x + p3.x) / 6.0;

		b[0] = (-p1.y + 3 * p2.y - 3 * p3.y + p4.y) / 6.0;
		b[1] = (3 * p1.y - 6 * p2.y + 3 * p3.y) / 6.0;
		b[2] = (-3 * p1.y + 3 * p3.y) / 6.0;
		b[3] = (p1.y + 4 * p2.y + p3.y) / 6.0;

		c[0] = (-p1.z + 3 * p2.z - 3 * p3.z + p4.z) / 6.0;
		c[1] = (3 * p1.z - 6 * p2.z + 3 * p3.z) / 6.0;
		c[2] = (-3 * p1.z + 3 * p3.z) / 6.0;
		c[3] = (p1.z + 4 * p2.z + p3.z) / 6.0;

		float value_x = (float) ( (a[2] + t * (a[1] + t * a[0]))*t+a[3] );
		float value_y = (float) ( (b[2] + t * (b[1] + t * b[0]))*t+b[3] );
		float value_z = (float) ( (c[2] + t * (c[1] + t * c[0]))*t+c[3] );

		return new Vector3 (value_x, value_y, value_z);

	}

}
