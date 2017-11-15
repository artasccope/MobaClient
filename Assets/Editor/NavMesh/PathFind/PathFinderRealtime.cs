using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nav
{
    public class PathFinderRealtime : PathFinder
    {
        public PathFinderRealtime(List<Vector3> points, Dictionary<int, Poly> polys, Dictionary<int, List<NodeEdge>> edges, float left, float bottom, float tileSize, int width, int height, int componentCount, List<int>[,] aoiList, List<int>[,] pths = null) : base(points, polys, edges, left, bottom, tileSize, width, height, componentCount, aoiList, pths)
        {
        }

        public override List<int> GetPaths(Vector2 pos, Vector2 target)
        {
            int tarNode = GetPoly(target, navAOI.GetPolyListInAOI(target.x, target.y));
            int srcNode = GetPoly(pos, navAOI.GetPolyListInAOI(pos.x, pos.y));

            return PathAStar(srcNode, tarNode);
        }

        private List<int> PathAStar(int sourceNode, int targetNode) {
            List<float> realDist = new List<float>(polys.Count);
            List<int> from = new List<int>(polys.Count);
            List<bool> marked = new List<bool>(polys.Count);
            for (int i = 0; i < polys.Count; i++) {
                realDist[i] = Mathf.Infinity;
                from[i] = -1;
                marked[i] = false;
            }

            realDist[sourceNode] = 0f;
            from[sourceNode] = sourceNode;
            SortedList<float, int> queue = new SortedList<float, int>();
            queue.Add(Vector2.Distance(polys[sourceNode].Center, polys[targetNode].Center), sourceNode);
            marked[sourceNode] = true;
            while (queue.Count > 0) {
                KeyValuePair<float, int> pair = queue.Min();
                int v = pair.Value;
                queue.Remove(pair.Key);
                marked[v] = true;
                if (v == targetNode)
                    break;

                foreach (NodeEdge e in edges[v]) {
                    int w = e.Other(v);
                    if (!marked[w])
                    {
                        float HCost = Vector2.Distance(polys[targetNode].Center, polys[w].Center);
                        float GCost = realDist[v] + e.Weight;

                        if (GCost < realDist[w] || from[w] == -1) {
                            realDist[w] = GCost;
                            from[w] = v;

                            if (!queue.ContainsValue(w))
                                queue.Add(HCost + GCost, w);
                            else {
                                queue.RemoveAt(queue.IndexOfValue(w));
                                queue.Add(HCost+GCost, w);
                            }
                        }
                    }
                }
            }

            return Path(sourceNode, targetNode, from);
        }

        /// <summary>
        /// 得到从s到d的路径
        /// </summary>
        /// <param name="s"></param>
        /// <param name="d"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        private List<int> Path(int s, int d, List<int> from)
        {
            if (from[d] == -1)
                return null;

            List<int> pth = new List<int>();
            while (d != s)
            {
                pth.Add(d);
                d = from[d];
            }
            pth.Add(s);

            pth.Reverse();
            return pth;
        }
    }
}
