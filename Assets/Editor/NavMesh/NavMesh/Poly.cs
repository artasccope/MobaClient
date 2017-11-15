using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nav
{
    /// <summary>
    /// NavMesh
    /// </summary>
    public class Poly
    {
        protected List<GeoEdge> outerEdges;
        protected List<Vector3> points;

        private int index;

        public int Index {
            get { return index; }
            set { index = value; }
        }

        public int EdgeCount
        {
            get
            {
                return outerEdges.Count;
            }
        }

        public Vector3 Point(int index) {
            return points[index];
        }

        public Poly(List<Vector3> points)
        {
            this.points = points;
            outerEdges = new List<GeoEdge>();
        }

        public Poly() {
            outerEdges = new List<GeoEdge>();
        }

        public List<GeoEdge> Edges
        {
            get { return outerEdges; }
        }

        public void RemoveEdge(GeoEdge edge)
        {
            if (ContainsEdge(edge)) {
                for (int i = 0; i < outerEdges.Count; i++) {
                    if(outerEdges[i].CompareTo(edge) == 0)
                        outerEdges.RemoveAt(i);
                }
            }
        }

        public void AddEdges(List<GeoEdge> edges)
        {
            for (int i = 0; i < edges.Count; i++) {
                if(!ContainsEdge(edges[i]))
                    outerEdges.Add(edges[i]);
            }
        }

        public bool ContainsEdge(GeoEdge edge) {
            for (int i = 0; i < outerEdges.Count; i++) {
                if (outerEdges[i].CompareTo(edge) == 0)
                    return true;
            }

            return false;
        }

        public void AddEdge(GeoEdge edge) {
            if (!ContainsEdge(edge)) {
                outerEdges.Add(edge);
            }
        }

        public GeoEdge RelatedEdgeExtrude(int p, GeoEdge extrudeEdge) {

            for (int i = 0; i < outerEdges.Count; i++) {
                if (outerEdges[i].IsRelatedToPoint(p) && (outerEdges[i].CompareTo(extrudeEdge) != 0))
                    return outerEdges[i];
            }

            return null;
        }

        public List<int> GetPoints() {
            List<int> points = new List<int>(outerEdges.Count);
            foreach (GeoEdge edge in outerEdges) {
                if (!points.Contains(edge.A))
                    points.Add(edge.A);
                if (!points.Contains(edge.B))
                    points.Add(edge.B);
            }

            return points;
        }

        public bool ContainsSomeEdge(List<GeoEdge> restEdge)
        {
            for (int i = 0; i < restEdge.Count; i++) {
                if (ContainsEdge(restEdge[i]))
                    return true;
            }
            return false;
        }

        public List<GeoEdge> RestEdge(GeoEdge edge)
        {
            List<GeoEdge> restEdge = new List<GeoEdge>();
            for (int i = 0; i < outerEdges.Count; i++)
            {
                if (outerEdges[i].CompareTo(edge) != 0)
                {
                    restEdge.Add(outerEdges[i]);
                }
            }

            return restEdge;
        }

        private Vector2 center;

        public Vector2 Center
        {
            get
            {
                center = Vector2.zero;

                foreach (int p in GetPoints())
                {
                    Vector3 point = points[p];
                    center += new Vector2(point.x, point.z);
                }

                center /= points.Count;

                return center;
            }
        }

        public void PrintPoly() {
            Debug.Log("poly:");
            for (int i = 0; i < outerEdges.Count; i++) {
                Debug.Log(outerEdges[i].A + " "+ outerEdges[i].B);
            }
            Debug.Log("poly end.");
        }

        #region Geo 2d
        private Geo2D geo2d;

        public Geo2D GetGeo2D() {
            if (geo2d == null)
            {
                geo2d = new Geo2D();
                foreach (int i in GetPoints())
                {
                    Vector3 p = Point(i);
                    geo2d.AddPoint(new Vector2(p.x, p.z));
                }

                foreach (GeoEdge e in outerEdges)
                {
                    Vector3 a = Point(e.A);
                    Vector3 b = Point(e.B);
                    geo2d.AddEdge(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
                }
            }
            return geo2d;
        }
#endregion
    }
}
