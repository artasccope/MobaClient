using Nav._2DGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nav
{
    public abstract class PathFinder
    {
        protected List<Vector3> points;
        protected Dictionary<int, Poly> polys;
        protected Dictionary<int, List<NodeEdge>> edges;
        protected List<int>[,] paths;
        protected NavAOI navAOI;
        protected bool isPathPreCalculate;
        protected int componentCount;

        public PathFinder(List<Vector3> points, Dictionary<int, Poly> polys, Dictionary<int, List<NodeEdge>> edges, float left, float bottom, float tileSize, int width, int height, int componentCount, List<int>[,] aoiList, List<int>[,] pths = null) {
            this.points = points;
            this.edges = edges;
            this.polys = polys;
            this.componentCount = componentCount;

            navAOI = new NavAOI(left, bottom, width, height, tileSize, aoiList);

            this.isPathPreCalculate = (pths != null);
            if (pths != null) {
                this.paths = pths;
            }
        }

        public abstract List<int> GetPaths(Vector2 pos, Vector2 target);


        protected int GetPoly(Vector2 pos, List<int> plys) {
            foreach (int i in plys) {
                Poly p = polys[i];
                if (GraphTester2D.IsInside(pos, p.GetGeo2D()))
                    return i;
            }

            return -1;
        }
    }
}
