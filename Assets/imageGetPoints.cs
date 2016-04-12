using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class imageGetPoints : MonoBehaviour {

	public List<Vector2> points = new List<Vector2>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void getMouseCoords()
	{
		var rect = GetComponent<RectTransform> ();

		Debug.Log ("Mouse = " + Input.mousePosition.ToString () + "rect = " + rect.position.ToString());

		var mousePos = Input.mousePosition;

		var pos = new Vector2 ();

		pos.x = mousePos.x - rect.position.x;
		pos.y = mousePos.y - rect.position.y;

		Debug.Log ("ImagePixel = " + pos.ToString());

		var texture = GetComponent<buttonLoadImage> ().currentTexture;

		var finalPos = new Vector2 ();

		finalPos.x = (pos.x * texture.width) / rect.rect.width;
		finalPos.y = (pos.y * texture.height) / rect.rect.height;

		Debug.Log ("TexturePixel = " + finalPos.ToString ());

		points.Add (finalPos);

		if (points.Count >= 12) {
			GameObject.Find("fifaface_head").GetComponent<DelauneyApp>().makeFaceText(texture, points);
			points.Clear ();
		}


		texture.SetPixel ((int)finalPos.x, (int)finalPos.y, Color.green);
		texture.Apply ();
		Debug.Log ("points count: " + points.Count);


	}
}
