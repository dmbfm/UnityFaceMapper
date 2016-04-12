using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Delauney {

	public class Edge {

		public HashSet<Vector2> points;


		public Edge(Vector2 p1, Vector2 p2)
		{
			points = new HashSet<Vector2>();

			points.Add (p1);
			points.Add (p2);

			if (points.Count < 2) {
				Debug.Log("Edge vertex count < 2");
			}
		}


		public bool hasPoint(Vector2 p)
		{
			return points.Contains(p);
		}


		public List<Vector2> getVertices()
		{
			var ret = new List<Vector2> (2);

			foreach (var p in points)
				ret.Add (p);

			return ret;

		}

		public void print()
		{
			string s = "Edge = {";

			foreach (Vector2 v in points) {
				s += v.ToString();
			}

			s += "}";
			Debug.Log (s);
		}

		public void drawDebug()
		{
			var vert = new List<Vector2> (2);

			foreach (var v in points)
				vert.Add (v);

			Debug.DrawLine (vert [0], vert [1]);
		}

	}



	public class EdgeEquality : IEqualityComparer<Edge>
	{
		public bool Equals(Edge e1, Edge e2)
		{
			bool ret = true;

			foreach (Vector2 v in e1.points) {
				if (!e2.points.Contains(v))
					ret = false;
			}

			return ret;
		}

		public int GetHashCode(Edge e)
		{
			int code = 0;
			
			foreach (Vector2 v in e.points) {
				
				code += (int) Math.Ceiling(v.x);
			}
			
			return code.GetHashCode();
		}
	}

}