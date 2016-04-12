using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Delauney {

	public class Triangle {

		public HashSet<Vector2> points;
		public HashSet<Edge> edges;
		public float radius;
		public Vector2 center;

		public List<Vector2> ordered_points;
		public List<int> vertices_index;

		public Triangle(Vector2 p1, Vector2 p2, Vector2 p3)
		{

			points = new HashSet<Vector2>();

			points.Add(p1);
			points.Add(p2);
			points.Add(p3);

			if (this.points.Count < 3) {
				Debug.Log("Triangle vertex count < 3");
			}

			this.center = new Vector2 (0, 0);
			this.radius = 0;

			this.calcCircle();

			edges = new HashSet<Edge> ();
			edges.Add (new Edge (p1, p2));
			edges.Add (new Edge (p2, p3));
			edges.Add (new Edge (p3, p1));

			ordered_points = new List<Vector2> ();
			ordered_points.Add (p1);
			ordered_points.Add (p2);
			ordered_points.Add (p3);

			vertices_index = new List<int> ();

		}

		// Calculate the triangle's circle
		public void calcCircle()
		{
			List<Vector2> temp = new List<Vector2>(3);

			foreach (Vector2 v in this.points) {
				temp.Add(v);
			}

			Vector2 A = temp [0];
			Vector2 B = temp [1];
			Vector2 C = temp [2];

			float D = 2*(A.x*(B.y - C.y) + B.x*(C.y - A.y) + C.x*(A.y - B.y));

			this.center.x = (
					(A.x*A.x + A.y*A.y)*(B.y - C.y) +
					(B.x*B.x + B.y*B.y)*(C.y - A.y) +
					(C.x*C.x + C.y*C.y)*(A.y - B.y))/D;

			this.center.y = (
				(A.x*A.x + A.y*A.y)*(C.x - B.x) +
				(B.x*B.x + B.y*B.y)*(A.x - C.x) +
				(C.x*C.x + C.y*C.y)*(B.x - A.x))/D;

			this.radius = (this.center - A).magnitude;

		}

		public bool hasPointInCircle(Vector2 p)
		{
			float dist = (p - center).magnitude;

			if (dist > radius)
				return false;
			else
				return true;
		}

		public Vector2 getBarycentricCoords(Vector2 p)
		{

			Vector2 v0 = ordered_points [0] - ordered_points [2];
			Vector2 v1 = ordered_points [1] - ordered_points [2];
			Vector2 v2 = p - ordered_points [2];

			float d00 = Vector2.Dot (v0, v0);
			float d10 = Vector2.Dot (v0, v1);
			float d01 = d10;
			float d11 = Vector2.Dot (v1, v1);

			float d02 = Vector2.Dot (v2, v0);
			float d12 = Vector2.Dot (v2, v1);

			float det = d00 * d11 - d10 * d01;

			float alpha = (d02 * d11 - d01 * d12)/det;
			float beta = (d00 * d12 - d02 * d10) / det;

			return new Vector2 (alpha, beta); 

		}

		public Vector2 getCartesianCoords(Vector2 bar)
		{
			Vector2 a = ordered_points [0];
			Vector2 b = ordered_points [1];
			Vector2 c = ordered_points [2];

			return bar[0]*a + bar[1]*b + (1 - bar[0] - bar[1])*c;
		}

		public static Triangle MakeTriangle(Edge e, Vector2 p)
		{
			List<Vector2> temp = new List<Vector2> (2);

			foreach (Vector2 v in e.points)
				temp.Add (v);

			return new Triangle (temp [0], temp [1], p);
		}
		                                 

		public bool isPointInside(Vector2 p)
		{
			var b = getBarycentricCoords (p);

			if (((b [0] + b [1]) > 1) || (b [0] < 0) || (b [1] < 0))
				return false;
			else 
				return true;
		}

		public bool isCompatible(Triangle t)
		{
			if ((t.vertices_index [0] == vertices_index [0]) && (t.vertices_index [1] == vertices_index [1]) && (t.vertices_index [2] == vertices_index [2]))
				return true;
			else
				return false;
		}


		public bool isNeighbour(Triangle t)
		{
			int count = 0;

			foreach (var i in t.vertices_index) {

				if (vertices_index.Contains(i))
					count++;

			}

			if (count == 2)
				return true;
			else
				return false;

		}


		public void print()
		{
			string s = "Triangle = { center: " + center.ToString() + ", radius: " + radius + ", points: ";

			foreach (Vector2 v in this.points) {
				s += v.ToString();
			}

			s += "ordered_vertices: ";

			foreach (Vector2 v in this.ordered_points) {
				s += v.ToString();
			}

			s += "vertices_index: ";

			foreach (var v in this.vertices_index) {
				s += v + " ";
			}

			s += "}";

			Debug.Log (s);

		}

		public void printEdges()
		{
			Debug.Log ("Triangle edges:");

			foreach (Edge e in edges) {
				e.print();
			}
		}

		public void drawDebug()
		{
			foreach (var e in edges)
				e.drawDebug ();
		}

		public List<Vector2> getPoints()
		{
			var temp = new List<Vector2> (3);

			foreach (var v in points)
				temp.Add (v);

			return temp;
		}


		public bool hasVertex(Vector2 v)
		{
			return points.Contains (v);
		}

		public bool hasAnyVertex(List<Vector2> vertices)
		{
			bool ret = false;

			foreach (var v in vertices) {
				if (points.Contains(v))
					ret = true;
			}

			return ret;
		}

	}


	public class TriangleEquality : IEqualityComparer<Triangle>
	{
		public bool Equals(Triangle t1, Triangle t2)
		{
			bool ret = true;

			foreach (Vector2 v in t1.points) {

				if (!t2.points.Contains(v))
				{
					ret = false;
				}
			}

			return ret;
		}

		public int GetHashCode(Triangle t)
		{
			int code = 0;

			foreach (Vector2 v in t.points) {
				
				code += (int) Math.Ceiling(v.x);
			}

			return code.GetHashCode();
		}
	}



}





