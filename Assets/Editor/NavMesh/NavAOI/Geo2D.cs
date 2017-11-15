using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nav
{
    public class Geo2D
    {
        private List<Vector2> points = new List<Vector2>();
        private List<GeoEdge2D> edges = new List<GeoEdge2D>();

        public Geo2D() {
            MinX = Mathf.Infinity;
            MinY = Mathf.Infinity;
            MaxX = Mathf.NegativeInfinity;
            MaxY = Mathf.NegativeInfinity;
        }

        public float MinX { get; set; }
        public float MaxX { get; set; }
        public float MinY { get; set; }
        public float MaxY { get; set; }

        public bool AABBTest(Geo2D other) {
            if (this.MaxX < other.MinX || this.MaxY < other.MinY || other.MaxX < this.MinX || other.MaxY < this.MinY)
                return false;

            return true;
        }

        public Geo2D(Vector2 a, Vector2 b, Vector2 c, Vector2 d) {
            MinX = Mathf.Infinity;
            MinY = Mathf.Infinity;
            MaxX = Mathf.NegativeInfinity;
            MaxY = Mathf.NegativeInfinity;

            AddPoint(a);
            AddPoint(b);
            AddPoint(c);
            AddPoint(d);

            AddEdge(a, b);
            AddEdge(b, c);
            AddEdge(c, d);
            AddEdge(d, a);

            points = ArrangePoints();
        }

        public void AddPoint(Vector2 point) {
            if (!points.Contains(point)) {
                points.Add(point);

                if (point.x > MaxX)
                    MaxX = point.x;
                if (point.y > MaxY)
                    MaxY = point.y;
                if (point.x < MinX)
                    MinX = point.x;
                if (point.y < MinY)
                    MinY = point.y;
            }
        }

        public void AddEdge(Vector2 a, Vector2 b) {
            GeoEdge2D edge = new GeoEdge2D(a,b);
            edges.Add(edge);
        }

        public List<GeoEdge2D> Edges {
            get {
                return edges;
            }
        }

        public List<Vector2> Points {
            get { return points; }
        }

        private Vector2 center;

        public Vector2 Center {
            get {
                center = Vector2.zero;

                foreach (Vector2 p in points) {
                    center += p;
                }

                center /= points.Count;

                return center;
            }
        }

        public void ArrangeThisGeo() {
            points = ArrangePoints();
        }

        private List<Vector2> ArrangePoints() {
            List<Vector2> newPoints = new List<Vector2>();
            GeoEdge2D edge = edges[0];
            Vector2 startPoint = edge.PointA;
            newPoints.Add(startPoint);
            while (newPoints.Count < edges.Count) {
                Vector2 endPoint = edge.Other(startPoint);
                newPoints.Add(endPoint);
                startPoint = endPoint;
                foreach (GeoEdge2D e in edges) {
                    if (e.IsRelatedToPoint(endPoint) || e != edge) {
                        edge = e;
                        break;
                    }    
                }
            }

            return newPoints;
        }

        public float GetArea() {
            float maxX = -Mathf.Infinity;
            float maxY = -Mathf.Infinity;
            foreach (Vector2 p in points) {
                if (p.x > maxX)
                    maxX = p.x;
                if (p.y > maxY)
                    maxY = p.y;
            }

            Vector2 pP = new Vector2(maxX+1f, maxY+1f);
            ArrangePoints();
            float area = 0f;
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 a = points[i] - pP;
                Vector2 b = points[(i + 1) % points.Count] - pP;
                area = area + a.x * b.y - b.x * a.y;
            }

            return Mathf.Abs(area*0.5f);
        }
    }
}
