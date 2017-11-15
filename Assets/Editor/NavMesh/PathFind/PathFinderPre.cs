using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nav
{
    public class PathFinderPre : PathFinder
    {
        public PathFinderPre(List<Vector3> points, Dictionary<int, Poly> polys, Dictionary<int, List<NodeEdge>> edges, float left, float bottom, float tileSize, int width, int height, int componentCount, List<int>[,] aoiList, List<int>[,] pths = null) : base(points, polys, edges, left, bottom, tileSize, width, height, componentCount, aoiList, pths)
        {
        }

        public override List<int> GetPaths(Vector2 pos, Vector2 target)
        {
            int tarNode = GetPoly(target, navAOI.GetPolyListInAOI(target.x, target.y));
            int curNode = GetPoly(pos, navAOI.GetPolyListInAOI(pos.x, pos.y));

            return paths[curNode, tarNode];
        }
    }
}
