using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nav
{
    public class NodeEdge
    {
        public int A { get; set; }
        public int B { get; set; }
        public int PointA { get; set; }
        public int PointB { get; set; }
        public float Weight { get; set; }
        public int Other(int p) {
            return p == A ? B : A;
        }

        public int OtherPoint(int p) {
            return p == PointA ? B : A;
        }
    }
}
