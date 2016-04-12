using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Delauney {
	

	public class Triangulate {

		public HashSet<Triangle> triangles;
		public List<Vector2> vertices;
		public float width;
		public float height;
		public Triangle super_triangle;

		public Triangulate(List<Vector2> input, float w, float h)
		{
			vertices = input;

			triangles = new HashSet<Triangle>(new TriangleEquality());

			this.width = w;
			this.height = h;

			// Add super triangle 
			superTriangle ();

			int counter = 0;

			// Process each vertex
			foreach (Vector2 vertex in vertices) {

				// Create edge buffer
				Dictionary<Edge, int> edge_buffer = new Dictionary<Edge, int>(new EdgeEquality());

				// Check each triangle to see if point is in circle
				foreach (Triangle triangle in triangles)
				{
					// If so, store the edges in buffer, and remove the triangles from the list
					if (triangle.hasPointInCircle(vertex))
					{
						// Store edge as keys, with number of occurences as values
						foreach (Edge triangle_edge in triangle.edges)
						{
							if (edge_buffer.ContainsKey(triangle_edge))
								edge_buffer[triangle_edge] += 1;
							else
								edge_buffer[triangle_edge] = 1;
						}

						// Remove the triangle from list
						//triangles.Remove(triangle);

					}
				}

				// Remove triangles from list
				triangles.RemoveWhere( t => t.hasPointInCircle(vertex));

				var edge_remove  = new List<Edge>();

				// Remove doubled edges from buffer
				foreach (KeyValuePair<Edge, int> e in edge_buffer)
				{
					if (e.Value > 1)
						//edge_buffer.Remove(e.Key);
						edge_remove.Add(e.Key);
				}

				foreach (Edge e in edge_remove)
					edge_buffer.Remove(e);



				// For each remaining edge, add a new triangle that connects to the vertex
				foreach (Edge e in edge_buffer.Keys)
				{
					triangles.Add(Triangle.MakeTriangle(e, vertex));
				}

				//Debug.Log ("Vertex " + counter);
				//printTriangles();
				//counter++;

			}

			// Remove any triangle with supre triangle vertices
			triangles.RemoveWhere( t=> t.hasAnyVertex( super_triangle.getPoints()) );

			// Set triangle vertex ordering based on input vertex index order
			orderTrianglesVertices ();

			printTriangles ();

		}

		void orderTrianglesVertices ()
		{

			// Clear orderings
			foreach (var t in triangles) {
				t.ordered_points.Clear();
			}

			// For each vertex
			foreach (var v in vertices) {

				// And each triangle
				foreach (var t in triangles)
				{
					// If v is a vertex of the triangle, add it to its ordered vertex list
					if (t.hasVertex(v))
					{
						t.ordered_points.Add(v);
						t.vertices_index.Add(vertices.IndexOf(v));
					}
				}

			}


			// Check ordering
			foreach (var t in triangles) {
				if (t.ordered_points.Count != 3)
					Debug.Log("Invalid triangle ordering");
			}

		}


		public Triangle findCompatible(Triangle t)
		{

			Triangle ret = null;

			foreach (var triangle in triangles) {

				if (triangle.isCompatible(t) == true)
					ret = triangle;

			}

			return ret;
		}


		public void superTriangle()
		{
			float factor = 3;
			Triangle super_tri = new Triangle (
				new Vector2(-factor*width,0),
				new Vector2(0, factor*width),
				new Vector2(factor*width, 0));

			triangles.Add (super_tri);
			super_triangle = super_tri;
		}


		public void flipQuadEdges(Triangle t1, Triangle t2)
		{
			// Check if the triangles are in the triangulation
			if (!triangles.Contains (t1) || !triangles.Contains (t2)) {
				Debug.Log("flipQuadEdges: triangle not in triangulation");
				return;
			}

			Dictionary<int, int> indices_count = new Dictionary<int, int> ();
			var indices = t1.vertices_index.Concat (t2.vertices_index);

			// Count indices occurances
			foreach (var i in indices) {

				if (! indices_count.ContainsKey(i))
				{
					indices_count[i] = 1;
				}
				else
				{
					indices_count[i]++;
				}

			}

			Debug.Log ("Before:");
			t1.print ();
			t2.print ();


			var l = new List<Triangle> ();
			l.Add (t1);
			l.Add (t2);

			// Buffer to store used indexes
			var used_buffer = new List<int> ();

			foreach (var pair in indices_count) {
				Debug.Log( pair.Key + ", " + pair.Value);
			}

			// For each triangle
			foreach (var t in l) {
				int count = -1;

				// For each vertex index in the triangle
				foreach (var vert_index in t.vertices_index)
				{
					//Debug.Log("vert_index = "+vert_index);
					count++;

					// If it's a double index, not used yet
					if (indices_count[vert_index] == 2 && !used_buffer.Contains(vert_index))
					{
						Debug.Log ("indices_count[vert_index] == 2 && !used_buffer.Contains(vert_index)");

						bool quit = false;

						// Scan index count
						foreach (var pair in indices_count)
						{
							// If we find a simple unsused index, make the switch
							if (pair.Value == 1 && !used_buffer.Contains(pair.Key) &&!t.vertices_index.Contains(pair.Key))
							{
								Debug.Log ("pair.Value == 1 && !used_buffer.Contains(pair.Key) -> Key = " + pair.Key + " Val = " + pair.Value);

								// Switch the index
								t.vertices_index[count] = pair.Key;

								// Add both to used index buffer and break loop
								used_buffer.Add(pair.Key);
								used_buffer.Add (vert_index);
								quit = true;
								break;
							}
						}

						// If switched, break from loop
						if (quit)						
							break;
					}
				}

				// Sort the triangle's indexes
				t.vertices_index.Sort();

				// Set the points acording to the indexes
				setTriangleFromIndices(t);
			}




			foreach (var x in used_buffer)
				Debug.Log ("used: " + x);

			Debug.Log ("After:");
			t1.print ();
			t2.print ();


		}

		public void setTriangleFromIndices(Triangle t)
		{
			Vector2 p1 = this.vertices[t.vertices_index[0]];
			Vector2 p2 = this.vertices[t.vertices_index[1]];
			Vector2 p3 = this.vertices[t.vertices_index[2]];

			// Clear points and add new ones
			t.points.Clear ();
			t.points.Add (p1);
			t.points.Add (p2);
			t.points.Add (p3);

			t.edges.Clear ();
			t.edges.Add (new Edge (p1, p2));
			t.edges.Add (new Edge (p2, p3));
			t.edges.Add (new Edge (p3, p1));

			t.ordered_points.Clear ();
			t.ordered_points.Add (p1);
			t.ordered_points.Add (p2);
			t.ordered_points.Add (p3);
		}

		public void printTriangles()
		{
			Debug.Log ("Triangles: ");

			foreach (Triangle t in triangles)
				t.print ();
		}

		public void drawTriangles()
		{
			foreach (var t in triangles)
				t.drawDebug ();
		}

	}

}
