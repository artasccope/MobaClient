using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nav
{
    public class GeoEdge2D
    {
        public Vector2 PointA { get; set; }
        public Vector2 PointB { get; set; }

        public GeoEdge2D(Vector2 pointA, Vector2 PointB) {
            this.PointA = pointA;
            this.PointB = PointB;
        }

        public Vector2 Vec {
            get { return PointB - PointA; }
        }

        public bool IsRelatedToPoint(Vector2 p)
        {
            if (p == PointA || p == PointB)
                return true;
            else
                return false;
        }

        public static bool IsParallel(GeoEdge2D a, GeoEdge2D b) {
            return Mathf.Abs(Vector2.Angle(a.Vec, b.Vec) % 180f) < 1e-06;
        }

        public static bool IsIntersected(GeoEdge2D a, GeoEdge2D b)
        {
            return CheckCross(a, b) && CheckCross(b, a);
        }

        private static bool CheckCross(GeoEdge2D line1, GeoEdge2D line2)
        {
            Vector2 v1, v2, v3;
            v1.x = line2.PointA.x - line1.PointB.x;
            v1.y = line2.PointA.y - line1.PointB.y;

            v2.x = line2.PointB.x - line1.PointB.x;
            v2.y = line2.PointB.y - line1.PointB.y;

            v3.x = line1.PointA.x - line1.PointB.x;
            v3.y = line1.PointA.y - line1.PointB.y;

            return CrossMult(v1, v3) * CrossMult(v2, v3) <= 0;//叉乘之积为负，说明在两侧
        }

        private static float CrossMult(Vector2 a, Vector2 b)
        {
            return a.x * b.y - b.x * a.y;
        }

        public static bool IsOnline(Vector2 point, GeoEdge2D edge)
        {
            Vector2 pointA = edge.PointA;
            Vector2 pointB = edge.PointB;

            return Mathf.Abs(Vector2.Distance(point, pointA) + Vector2.Distance(point, pointB) - Vector2.Distance(pointA, pointB)) < 1e-06;
        }

        public Vector2 Other(Vector2 startPoint)
        {
            if (startPoint == PointA)
                return PointB;
            else
                return PointA;
        }
    }
}
