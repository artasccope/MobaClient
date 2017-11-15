using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nav._2DGraph
{
    public class GraphTester2D
    {
        public static bool IsIntersect(Geo2D poly2d, Geo2D rect2d)
        {
            if (!poly2d.AABBTest(rect2d))
                return false;

            foreach (GeoEdge2D e1 in poly2d.Edges)
            {
                foreach (GeoEdge2D e2 in rect2d.Edges)
                {
                    if (GeoEdge2D.IsIntersected(e1, e2))
                        return true;
                }
            }

            return IsInside(poly2d.Center, rect2d) || IsInside(rect2d.Center, poly2d);
        }

        public static bool IsInside(Vector2 point, Geo2D geo2d)
        {
            Vector2 lineEnd = new Vector2(-Mathf.Infinity, point.y);
            GeoEdge2D edge = new GeoEdge2D(point, lineEnd);

            int intersectCount = 0;
            foreach (GeoEdge2D e in geo2d.Edges)
            {
                if (GeoEdge2D.IsParallel(e, edge))
                    continue;

                if (GeoEdge2D.IsOnline(point, e))
                    return true;

                if (GeoEdge2D.IsIntersected(edge, e))
                    intersectCount++;
            }

            return intersectCount % 2 != 0;
        }

        /// <summary>
        /// 得到三条边构成的总角度
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static float TotalAngleForThreeSeg(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            Vector3 lineA = a - b;
            Vector3 lineB = c - b;
            Vector3 lineC = d - b;//中间那条线

            return Vector3.Angle(lineA, lineC) + Vector3.Angle(lineC, lineB);
        }
    }
}
