using Nav._2DGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nav
{
    public class PolyProcesser
    {
        public static bool TryMerge(ref Poly toPoly, Triangle mergeTri,ref GeoEdge sharedEdge)
        {
            if (CouldMerge(ref toPoly, mergeTri, sharedEdge))
            {
                return Merge(ref toPoly, mergeTri, ref sharedEdge);
            }
            else
                return false;
        }

        public static bool CouldMerge(ref Poly toPoly, Poly mergeTri, GeoEdge sharedEdge)
        {
            bool couldmerge = true;
            Vector3 a = toPoly.RelatedEdgeExtrude(sharedEdge.A, sharedEdge).OtherPoint(sharedEdge.A);
            Vector3 b = sharedEdge.PointA;
            Vector3 c = mergeTri.RelatedEdgeExtrude(sharedEdge.A, sharedEdge).OtherPoint(sharedEdge.A);
            Vector3 d = sharedEdge.OtherPoint(sharedEdge.A);
            couldmerge &= GraphTester2D.TotalAngleForThreeSeg(a, b, c, d) - 180f < 1e-06;

            a = toPoly.RelatedEdgeExtrude(sharedEdge.B, sharedEdge).OtherPoint(sharedEdge.B);
            b = sharedEdge.PointB;
            c = mergeTri.RelatedEdgeExtrude(sharedEdge.B, sharedEdge).OtherPoint(sharedEdge.B);
            d = sharedEdge.OtherPoint(sharedEdge.B);
            couldmerge &= GraphTester2D.TotalAngleForThreeSeg(a, b, c, d) - 180f < 1e-06;

            return couldmerge;
        }

        public static bool Merge(ref Poly toPoly, Poly mergeTri, ref GeoEdge sharedEdge)
        {
            List<GeoEdge> restEdge = mergeTri.RestEdge(sharedEdge);
            if (!toPoly.ContainsSomeEdge(restEdge))
            {
                toPoly.RemoveEdge(sharedEdge);
                toPoly.AddEdges(restEdge);
                return true;
            }
            else {
                return false;
            }
        }

        public static bool HasSharedEdge(Poly polyA, Poly polyB)
        {
            for (int i = 0; i < polyA.EdgeCount; i++)
            {
                for (int j = 0; j < polyB.EdgeCount; j++)
                {
                    if (AreEqual(polyA.Edges[i], polyB.Edges[j]))
                        return true;
                }
            }

            return false;
        }

        private static bool AreEqual(GeoEdge a, GeoEdge b) {
            return ((a.A == b.A && a.B == b.B) || (a.A == b.B && a.B == b.A));
        }

        public static GeoEdge SharedEdge(Poly polyA, Poly polyB)
        {
            for (int i = 0; i < polyA.EdgeCount; i++)
            {
                for (int j = 0; j < polyB.EdgeCount; j++)
                {
                    if (AreEqual(polyA.Edges[i], polyB.Edges[j]))
                        return polyA.Edges[i];
                }
            }

            return null;
        }
    }
}
