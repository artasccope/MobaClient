using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nav
{
    public class GeoEdge:IComparable<GeoEdge>
    {
        public int A { get; set; }
        public int B { get; set; }
        private List<Vector3> points;

        public Vector3 PointA {
            get { return points[A]; }
        }

        public Vector3 PointB
        {
            get { return points[B]; }
        }

        public GeoEdge(int a, int b, List<Vector3> points) {
            A = a;
            B = b;
            this.points = points;
            this.Index = new GeoEdgeIndex(a, b);
        }

        public int Other(int p) {
            if (p == A)
                return B;
            else
                return A;
        }

        public Vector3 OtherPoint(int p) {
            return points[Other(p)];
        }

        public bool IsRelatedToPoint(int p) {
            if (p == A || p == B)
                return true;
            else
                return false;
        }

        public int CompareTo(GeoEdge other)
        {
            if ((this.A == other.A && this.B == other.B) || (this.A == other.B && this.B == other.A))
                return 0;
            else if (this.A > other.A && this.B > other.B)
                return 1;
            else
                return -1;
        }

        public GeoEdgeIndex Index {
            get;set;
        }

        public void PrintEdge() {
            Debug.Log(A + " " + B);
        }
    }

    public struct GeoEdgeIndex {
        public int a;
        public int b;

        public GeoEdgeIndex(int a, int b) {
            this.a = Mathf.Min(a, b);
            this.b = ((this.a == a) ? b : a);
        }
    }
}
