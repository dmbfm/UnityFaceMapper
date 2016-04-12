using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class pointSelect : MonoBehaviour {

	// Set to true when doing the point selection
	private bool doPointSelection = false;

	// The number of points to be taken
	//private int numPoints = 23;
	private int numPoints = 25;

	// The list of points
	private List<Vector2> pointList = new  List<Vector2>();

	// The list of arrow sprites
	private List<GameObject> arrows = new List<GameObject> ();


	/**
	 *  Called when user presses button to start point taking
	 */
	public void startPointSelection()
	{
		doPointSelection = true;

		// Get the points corresponding to the face mask boundary
		addMaskPoints ();
	}

	public void endPointSelection()
	{
		doPointSelection = false;
	}


	void Start () {

	}

	void Update () {

		if (! this.doPointSelection || pointList.Count >= numPoints)
			return;

		if (Input.touchCount == 1 && (Input.GetTouch(0).phase == TouchPhase.Began)) {

			var touch = Input.GetTouch(0);

			// Screen position of touch
			var pos = touch.position;

			// Get the pixel position in the image
			var pixel = getImagePixel(pos);

			// If the pixel coordinate is outside image, don't add point
			if (!isInImage(pixel))
				return;

			// Add arrow sprite in point
			arrows.Add((GameObject) Instantiate(Resources.Load("arrow"), Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, Camera.main.nearClipPlane + 1)), Quaternion.identity));
			Instantiate(Resources.Load("arrow"), Camera.main.ScreenToWorldPoint(new Vector3(pixel.x, pixel.y, Camera.main.nearClipPlane + 1)), Quaternion.identity);

			// Add pixel coordinate to point list
			pointList.Add(pixel);

			Debug.Log("Picked pixel: " + pixel + ", count: " + pointList.Count);
			var tex = GetComponent<Renderer>().material.mainTexture as Texture2D;
			//tex.SetPixel((int) pixel.x, (int) pixel.y, Color.cyan);
			tex.Apply();

			// If all points were taken, enable the 'done' button
			if (pointList.Count >= numPoints)
			{
				GameObject.Find("Canvas/Panel/DoneBtn2").GetComponent<Button>().interactable = true;
			}
		}
	
	}

	/**
	 * Checks if pixel is in the image
	 */
	bool isInImage (Vector2 pixel)
	{
		var texture = GetComponent<Renderer> ().material.mainTexture;

		if (pixel.x > texture.width || pixel.y > texture.height || pixel.x < 0 || pixel.y < 0)
			return false;
		else
			return true;
	}

	public void removeLastPoint()
	{
		if (arrows.Count == 0)
			return;

		GameObject.Find("Canvas/Panel/DoneBtn2").GetComponent<Button>().interactable = false;

		pointList.RemoveAt (pointList.Count - 1);

		var arrow = arrows [arrows.Count - 1];
		arrows.RemoveAt (arrows.Count - 1);

		Destroy (arrow);
		Debug.Log ("removeLastPoint: point count = " + pointList.Count + ", arrow count = " + arrows.Count);

	}

	Vector2 getImagePixel(Vector2 screenPos)
	{
		// Get world position of screen point
		var worldPos = Camera.main.ScreenToWorldPoint (screenPos);

		var transform = GetComponent<Transform> ();
		var texture = GetComponent<Renderer> ().material.mainTexture;

		// Transform to quad's local coordinate
		var localPos = transform.worldToLocalMatrix.MultiplyPoint3x4 (worldPos);

		// Get local distance from bottom left corner
		var pixelLocalPos = new Vector2 (0.5f, 0.5f) + new Vector2 (localPos.x, localPos.y);

		// Go back to world coordinates
		var pixelWorldPos = transform.localToWorldMatrix.MultiplyPoint3x4(pixelLocalPos);

		// Subtract quad position and un-rotate
		Vector2 pixelPos = Quaternion.Inverse(transform.rotation) *  (new Vector2(pixelWorldPos.x, pixelWorldPos.y) - new Vector2(transform.position.x, transform.position.y));

		// Scale to image size
		pixelPos.x = pixelPos.x * ((texture.width) / (transform.localScale.x));
		pixelPos.y = pixelPos.y * ((texture.height) / (transform.localScale.y));

		return pixelPos;
	}


	public void runDelauney()
	{
		//var delauney = GameObject.Find ("fifaface_head").GetComponent<DelauneyApp> ();
		var delauney = GameObject.Find ("simpleMan2.6/uomo").GetComponent<DelauneyApp> ();

		delauney.makeFaceText (GetComponent<Renderer> ().material.mainTexture as Texture2D, pointList);
	}

	// Add all maks points
	void addMaskPoints ()
	{
		/*
		var maskPoints = GameObject.Find ("Quad").GetComponent<faceMask> ().getWorldPoints ();

		var transform = GetComponent<Transform> ();

		var texture = GetComponent<Renderer> ().material.mainTexture as Texture2D;

		foreach (var point in maskPoints) {

			var localPos = transform.InverseTransformPoint(new Vector3(point.x, point.y, transform.position.z));

			var localPosCorner = new Vector2(localPos.x, localPos.y) + new Vector2(0.5f, 0.5f);

			var pixelPos = new Vector2();

			pixelPos.x = localPosCorner.x * texture.width;
			pixelPos.y = localPosCorner.y * texture.height;


			Debug.Log("Texure = " + texture.width + ", " + texture.height +
			          "localPos = " + localPos +
			          "localPosCorner = " + localPosCorner + 
			          "pixelPos = " + pixelPos);

			pointList.Add(pixelPos);
			Instantiate(Resources.Load("arrow"), Camera.main.ScreenToWorldPoint(new Vector3(pixelPos.x, pixelPos.y, Camera.main.nearClipPlane + 1)), Quaternion.identity);

			texture.SetPixel((int) pixelPos.x, (int) pixelPos.y, Color.blue);
			texture.Apply();
		}*/


		// Get list of screen-coordinate mask points
		var maskPoints = GameObject.Find ("Quad").GetComponent<faceMask> ().getScreenPoints();

		foreach (var point in maskPoints) {
			Instantiate(Resources.Load("arrow"), Camera.main.ScreenToWorldPoint(new Vector3(point.x, point.y, Camera.main.nearClipPlane + 1)), Quaternion.identity);

			var pixelPoint = getImagePixel(point);
			pointList.Add(pixelPoint);
			Instantiate(Resources.Load("arrow"), Camera.main.ScreenToWorldPoint(new Vector3(pixelPoint.x, pixelPoint.y, Camera.main.nearClipPlane + 1)), Quaternion.identity);
		}
	}
}

