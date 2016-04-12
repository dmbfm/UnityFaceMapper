using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Delauney;

/**
 * DelauneyApp: This class maps one texture into another by means of delauney triangulation.
 * 				
 * 				We first define set of points in the 'standard' texture, which is triangulated
 * 				using a Delayney incremental alghorithm.
 * 
 * 				The, an input texture, with a similar set of points, should be supplyed. The 
 * 				set of points in, then, triangulated.
 * 
 * 				After this, the input image is mapped onto a blank texture, via barycentric coordinates
 * 				of the user points triangulation onto the 'standard' triangulation.
 * 
 * 				Usage: DelauneyApp.makeFaceText( inputTexture, inputPoints);
 * 
 */
public class DelauneyApp : MonoBehaviour {

	// The face points of the 'standard' face, i.e., the dest. texture
	public List<Vector2> standardFacePoints = new List<Vector2> {

		new Vector2(257, 99),
		new Vector2(300, 119),
		new Vector2(340, 153), 
		new Vector2(355, 180),
		new Vector2(370, 217),
		new Vector2(365, 320),
		new Vector2(351, 351), 
		new Vector2(337, 363),
		new Vector2(306, 377), 
		new Vector2(210, 375),
		new Vector2(176, 361),
		new Vector2(160, 348), 
		new Vector2(140, 317),
		new Vector2(146, 212),
		new Vector2(162, 172),
		new Vector2(183, 143),
		new Vector2(215, 112),
		new Vector2(222, 283),
		new Vector2(289, 282),
		new Vector2(244, 211),
		new Vector2(267, 211),
		new Vector2(236, 163),
		new Vector2(273, 163)

	};

	// The face points of the source texture, i.e., the user image
	private List<Vector2> points;

	// The triangulation of the user face
	public Triangulate triangulation;

	// The triangulation of the standard face
	public Triangulate standardFaceTriangulation;

	// Debug flag
	private bool debug = false;

	// The dimensions of the dest. texture
	private int standardFaceWidth = 512;
	private int standardFaceHeight = 512;

	// The source (user image) texture
	private Texture2D inputTexture;

	// The output texture
	private Texture2D outputTexture;


	void Start () {

		// Create standard triangulation

		outputTexture = GetComponent<Renderer> ().material.mainTexture as Texture2D;

		standardFaceWidth = outputTexture.width;
		standardFaceHeight = outputTexture.height;

		standardFaceTriangulation = new Triangulate (standardFacePoints, standardFaceWidth, standardFaceHeight);	
	}
	

	/**
	 *	makeFaceText: Given an input texture and points, transfer texture to outoutTexture
	 */
	public void makeFaceText(Texture2D tex, List<Vector2> facePoints)
	{
		// Set input texture
		inputTexture = tex;

		// Set user points
		this.points = facePoints;

		// If debug, draw red dots on the original image points
		if (debug) {
			foreach (var p in points) {
				tex.SetPixel ((int)p.x, (int)p.y, Color.red);
				Debug.Log ("("+ (int) p[0] + ", " + (int) p[1] + ") ");
			}
			tex.Apply();
		}

		// Triangulate inpute texture
		triangulation = new Triangulate (points, tex.width, tex.height);

		// Create output texture
		//outputTexture = new Texture2D (standardFaceWidth, standardFaceHeight);

		// Make the transfer
		transferTexture (inputTexture, triangulation, outputTexture, standardFaceTriangulation);

		// Set as texture
		GetComponent<Renderer> ().material.mainTexture = outputTexture;
	}

	/**
	 * 	transferTexture: Transfer texture src with 'src' and triangulation 'srcTriangulation' to 
	 * 				     texture 'dst' with triangulation 'dstTriangulation'
	 */ 
	public static void transferTexture(Texture2D src, Triangulate srcTriangulation, Texture2D dst, Triangulate dstTriangulation)
	{
		int inCount = 0;
		int outCount = 0;

		// First, try to make triangulations compatible
		fixTriangulationIncompability (srcTriangulation, dstTriangulation);

		// For every pixel in destination texture
		for (int i = 0; i < dst.width; i++)
		{
			for (int j = 0; j < dst.height; j++)
			{
				// For each triangle in dest. triangulation
				foreach (var t in dstTriangulation.triangles)
				{
					// If the pixel is inside the triangle
					if (t.isPointInside( new Vector2( (float) i, (float) j)))
					{
						// Get the barycentric coordinates
						var barycentric = t.getBarycentricCoords( new Vector2( (float) i, (float) j));

						// Get the equivalent triangle in the source triangulation
						var src_triangle = srcTriangulation.findCompatible(t);

						if (src_triangle == null)
						{
							//Debug.Log("src triangle null");
							dst.SetPixel(i, j, Color.red);
							continue;
						}

						// Get the equivalent pixel coordinates in the src triangle
						Vector2 src_pixel = src_triangle.getCartesianCoords(barycentric);

						// Get the pixel value at source texture
						Color src_val = src.GetPixel( (int) src_pixel.x, (int) src_pixel.y );

						// Set the same value at dest. texture
						dst.SetPixel(i, j, src_val);
						//dst.SetPixel(i, j, Color.red);
						inCount++;
						break;

					}
					else
					{
						//dst.SetPixel(i, j, Color.blue);
						outCount++;
					}
				}
			}
		}	
		dst.Apply ();
		Debug.Log ("In = " + inCount + ", Out = " + outCount + ", triangles = " + dstTriangulation.triangles.Count);
	}

	/**
	 * 	fixTriangulationIncompability: Try to fix imcompatible triangulations. Only works 
	 * 								   if the imcompatible triangles are in pairs of neighbours, 
	 * 								   by flipping the edges between them.
	 */
	public static void fixTriangulationIncompability(Triangulate a, Triangulate b)
	{
		var incompatible_triangles = new List<Triangle> ();
		var complements = new List<Triangle> ();

		// For each triangle in triangulation A
		foreach (var t1 in a.triangles) {

			// Find if there is a compatible triangle in triangulation B
			var triangle = b.findCompatible(t1);

			// If no compatible triangle was found, add triangle to incompatible list
			if (triangle == null)
			{

				// If a neighbour is already in incompatible list, add to complement list
				bool flag = true;
				foreach (var tri in incompatible_triangles)
				{
					if (tri.isNeighbour(t1))
					{
						complements.Add(t1);
						flag = false;

						Debug.Log("Incompatible triangle (Complement):");
						t1.print();
					}
				}

				// Else, add to incompatible list
				if (flag)
				{
					incompatible_triangles.Add (t1);
					Debug.Log("Incompatible triangle:");
					t1.print();
				}
			}
		}	

		// For each triangle in incompatible list
		foreach (var triangle in incompatible_triangles) {

			// Find complement
			Triangle complement = null;
			foreach (var t in complements)
			{
				if (triangle.isNeighbour(t))
				{
					complement = t; 
					break;
				}
			}

			// If complement wasn't found, return
			if (complement == null)
			{
				Debug.Log("FixTriangulationIncompability: triangle complement is null");
				return;
			}

			// Flip the edges
			a.flipQuadEdges(triangle, complement);
		}
	}

	public static void DrawLine(int x0, int y0, int x1, int y1, Color color, Texture2D texture)
	{

		int _x0 = x0;
		int _x1 = x1;
		int _y0 = y0;
		int _y1 = y1;

		if (x0 > x1) {

			_x0 = x1;
			_y0 = y1;
			_x1 = x0;
			_y1 = y0;

		}

		float a = ((float)(_y1 - _y0)) / (_x1 - _x0);
		float b = ((float)(_x1 * _y0 - _x0 * _y1)) / (_x1 - _x0);


		for (int x = _x0; x <= _x1; x++) {

			int y = (int) (a * x + b);

			texture.SetPixel( x, y, color);

		}

		if (_y0 <= _y1) {


			for (int y = _y0; y <= _y1; y++) {
				int x = (int)((y - b) / a);
				texture.SetPixel (x, y, color);
			}

		} else {
			for (int y = _y1; y <= _y0; y++) {
				int x = (int)((y - b) / a);
				texture.SetPixel (x, y, color);
			}
		}



		texture.Apply ();

	}

	public static void DrawLine(Edge e, Color color, Texture2D texture)
	{
		Vector2 p1 = e.getVertices () [0];
		Vector2 p2 = e.getVertices () [1];

		DrawLine ((int) p1.x, (int) p1.y, (int) p2.x, (int) p2.y, color, texture);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
