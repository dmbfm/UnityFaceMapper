using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class faceMask : MonoBehaviour {

	public List<Vector2> points  = new List<Vector2> {
		new Vector2(319, 110),
		new Vector2(398, 141),
		new Vector2(483, 203),
		new Vector2(519, 260),
		new Vector2(542, 338),
		new Vector2(548, 566),
		new Vector2(521, 649),
		new Vector2(480, 677),
		new Vector2(413, 693),
		new Vector2(234, 694),

		new Vector2(161, 679),
		new Vector2(117, 647),
		new Vector2(93, 567),
		new Vector2(97, 340),
		new Vector2(118, 266),
		new Vector2(158, 199),
		new Vector2(257, 131),
	};

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}


	public List<Vector2> getScreenPoints()
	{
		var worldPoints = new List<Vector2> ();

		var texture = GetComponent<Renderer> ().material.mainTexture;
		var width = texture.width;
		var height = texture.height;

		foreach (var point in points) {

			var newPoint = new Vector3();

			newPoint.x = point.x/width - 0.5f;
			newPoint.y = point.y/height - 0.5f;
			newPoint.z = GetComponent<Transform>().position.z;

			Vector2 finalPoint = Camera.main.WorldToScreenPoint(GetComponent<Transform>().localToWorldMatrix.MultiplyPoint3x4(newPoint));

			worldPoints.Add(finalPoint);

			Debug.Log("Original point: " + point + ", local coord: " + newPoint + ", finalPoint: " + finalPoint);

		}

		return worldPoints;
	}


	public List<Vector2> getWorldPoints()
	{
		var worldPoints = new List<Vector2> ();
		
		var texture = GetComponent<Renderer> ().material.mainTexture;
		var width = texture.width;
		var height = texture.height;
		
		foreach (var point in points) {
			
			var newPoint = new Vector3();
			
			newPoint.x = point.x/width - 0.5f;
			newPoint.y = point.y/height - 0.5f;
			newPoint.z = GetComponent<Transform>().position.z;
			
			Vector2 finalPoint = GetComponent<Transform>().TransformPoint(newPoint);
			
			worldPoints.Add(finalPoint);
			
			Debug.Log("Original point: " + point + ", local coord: " + newPoint + ", finalPoint: " + finalPoint + ", texture = " + width + ", " + height);
			
		}
		
		return worldPoints;
	}
}
