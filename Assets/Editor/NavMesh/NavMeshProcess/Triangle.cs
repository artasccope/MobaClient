using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nav
{
    public class Triangle:Poly
    {
        public Triangle(int[] points, List<Vector3> pts):base(pts) {
            outerEdges.Add(new GeoEdge(points[0], points[1], pts));
            outerEdges.Add(new GeoEdge(points[1], points[2], pts));
            outerEdges.Add(new GeoEdge(points[2], points[0], pts));
        }
    }
}
