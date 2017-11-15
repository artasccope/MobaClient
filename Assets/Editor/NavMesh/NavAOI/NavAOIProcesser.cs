using Nav._2DGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nav
{
    public class NavAOIProcesser
    {
        private static float left, bottom, width, height;
        private static float tileSize;

        public static NavAOI ProcessPolyAOI(SortedDictionary<int, Poly> polys, NavMesh mesh) {
            float l = Mathf.Infinity;
            float r = Mathf.NegativeInfinity;
            float b = Mathf.Infinity;
            float t = Mathf.NegativeInfinity;
            float area = 0f;

            SortedDictionary<int, Poly>.Enumerator polyIter = polys.GetEnumerator();
            while (polyIter.MoveNext())
            {//Poly 信息
                Poly poly = polyIter.Current.Value;

                foreach (int pIndex in poly.GetPoints())
                {
                    Vector3 point = mesh.Point(pIndex);
                    if (point.x < l)
                        l = point.x;
                    if (point.z < b)
                        b = point.z;
                    if (point.x > r)
                        r = point.x;
                    if (point.z > t)
                        t = point.z;
                }
                area += poly.GetGeo2D().GetArea();
            }
            polyIter.Dispose();
            area /= polys.Count;

            left = l;
            bottom = b;
            width = r - l;
            height = t - b;
            tileSize = Mathf.Sqrt(area);
            int w = Mathf.CeilToInt(width / tileSize);
            int h = Mathf.CeilToInt(height / tileSize);

            NavAOI navAOI = new NavAOI(left, bottom, width, height, tileSize);

            polyIter = polys.GetEnumerator();
            while (polyIter.MoveNext())
            {
                Poly poly = polyIter.Current.Value;

                Geo2D geo2D = poly.GetGeo2D();
                int xStart = Mathf.FloorToInt(geo2D.MinX/tileSize);
                int xEnd = Mathf.CeilToInt(geo2D.MaxX/tileSize);
                int yStart = Mathf.FloorToInt(geo2D.MinY / tileSize);
                int yEnd = Mathf.CeilToInt(geo2D.MaxY / tileSize);

                for (int i = xStart; i < xEnd; i++) {
                    for (int j = yStart; j < yEnd; j++) {
                        if (IsIntersect(poly, i, j)) {
                            navAOI.AddPolyToAOI(i,j, polyIter.Current.Key);
                        }
                    }
                }
            }
            polyIter.Dispose();

            return navAOI;
        }

        public static bool IsIntersect(Poly poly, int x, int z) {
            Vector2 pA = new Vector2(left + tileSize * x, bottom + tileSize * z);
            Vector2 pB = new Vector2(left + tileSize * (x + 1), bottom + tileSize * z);
            Vector2 pC = new Vector2(left + tileSize * (x + 1), bottom + tileSize * (z + 1));
            Vector2 pD = new Vector2(left + tileSize * x, bottom + tileSize * (z + 1));

            Geo2D poly2d = poly.GetGeo2D();
            Geo2D rect2d = new Geo2D(pA, pB, pC, pD);

            return GraphTester2D.IsIntersect(poly2d, rect2d);
        }
    }
}
