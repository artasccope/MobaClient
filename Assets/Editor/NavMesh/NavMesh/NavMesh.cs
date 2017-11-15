using CommonTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nav
{
    public class NavMesh
    {
        private List<Vector3> points;//存储顶点
        private HashSet<Triangle> triangles = new HashSet<Triangle>();//存储初始读进来的三角形
        private Dictionary<GeoEdgeIndex, List<Triangle>> edgeTriangleMap = new Dictionary<GeoEdgeIndex, List<Triangle>>();//存储初始读进来的和边相连的三角形这两者之间的映射关系
        private SortedDictionary<int, Poly> polys = new SortedDictionary<int, Poly>();//存储处理过后的poly
        private HashSet<PolyConnection> polyConnections = new HashSet<PolyConnection>();//存储poly之间的connection
        private SortedDictionary<int, HashSet<PolyConnection>> nearPolys = new SortedDictionary<int, HashSet<PolyConnection>>();//存储和每个poly相连的connection
        private List<int>[,] paths;

        private int componentCount;

        public int ComponentCount { get { return this.componentCount; } }

        public SortedDictionary<int, Poly> Polys { get { return polys; } }

        public List<Vector3> Points { get { return points; } }

        public SortedDictionary<int, HashSet<PolyConnection>> NearPolys { get { return nearPolys; } }

        private bool ifPreCaculatePath;
        public List<int>[,] Paths
        {
            get
            {
                if (ifPreCaculatePath)
                    return paths;
                else
                    return null;
            }
        }
        /// <summary>
        /// 从三角网格建立导航网格
        /// </summary>
        /// <param name="points"></param>
        /// <param name="tris"></param>
        /// <param name="preCaculatePath"></param>
        public NavMesh(List<Vector3> points, List<int> tris, bool preCaculatePath)
        {
            this.points = points;
            Debug.Log("points count: " + points.Count);
            Debug.Log("tris count: " + tris.Count / 3);
            for (int i = 0; i < tris.Count - 2; i += 3)
            {
                Triangle t = new Triangle(new int[3] { tris[i], tris[i + 1], tris[i + 2] }, this.points);
                triangles.Add(t);//建立三角形
                foreach (GeoEdge e in t.Edges)
                {
                    if (!edgeTriangleMap.ContainsKey(e.Index))
                        edgeTriangleMap.Add(e.Index, new List<Triangle>());
                    if (!edgeTriangleMap[e.Index].Contains(t))
                    {
                        edgeTriangleMap[e.Index].Add(t);
                    }
                }
            }
            this.ifPreCaculatePath = preCaculatePath;

            ProcessMeshToGraph();

            if (ifPreCaculatePath)
            {
                CalculateAllPaths();
            }
        }

        public void ProcessMeshToGraph()
        {
            Dictionary<Triangle, bool> visited = new Dictionary<Triangle, bool>();
            foreach (Triangle tri in triangles)
            {
                visited.Add(tri, false);
            }
            Queue<Poly> processEdgeQueue = new Queue<Poly>();

            int i = 0;
            componentCount = 0;

            int mergedCout = 0;
            while (triangles.Count > 0)
            {
                componentCount++;//整张图的联通分量计数++
                Triangle tri = triangles.First();//拿出一个三角形
                tri.Index = i++;//为这个poly分配索引
                visited[tri] = true;
                processEdgeQueue.Enqueue(tri);
                triangles.Remove(tri);
                foreach (GeoEdge e in tri.Edges)
                {
                    if (edgeTriangleMap.ContainsKey(e.Index) && edgeTriangleMap[e.Index].Contains(tri))
                    {
                        edgeTriangleMap[e.Index].Remove(tri);//移除这个三角形和它的边的映射
                    }
                }

                while (processEdgeQueue.Count > 0)
                {
                    Poly poly = processEdgeQueue.Dequeue();

                    bool hasConnection = false;
                    for (int j = 0; j < poly.Edges.Count; j++)
                    {//对于这个多边形的每一条边
                        GeoEdge e = poly.Edges[j];

                        for (int k = 0; k < edgeTriangleMap[e.Index].Count; k++)
                        {//如果有与之相连的其他三角形
                            Triangle t = edgeTriangleMap[e.Index][k];
                            if (triangles.Contains(t) && t != poly && PolyProcesser.HasSharedEdge(poly, t))
                            {
                                GeoEdge sharedEdge = PolyProcesser.SharedEdge(poly, t);
                                if (PolyProcesser.TryMerge(ref poly, t, ref sharedEdge))//如果可以成功合并
                                {//这里ref Poly处理过后，里面就有t的边了
                                    hasConnection = true;
                                    mergedCout++;
                                }
                                else
                                {//相连但不可成功合并，说明是相连的多边形
                                    if (!visited[t]) {
                                        processEdgeQueue.Enqueue(t);
                                        t.Index = i++;
                                        visited[t] = true;
                                        //设置polyConnection,这里polyConnection还不用计算Weight,因为可能poly和t的顶点都没有全部得到
                                        PolyConnection con = new PolyConnection(poly.Index, t.Index, sharedEdge);
                                        if (!polyConnections.Contains(con))
                                        {
                                            polyConnections.Add(con);
                                            InsertNearPoly(con.A, con);
                                            InsertNearPoly(con.B, con);
                                        }
                                    }
                                }

                                edgeTriangleMap[e.Index].Remove(t);
                                triangles.Remove(t);
                            }
                        }
                    }

                    if (hasConnection)
                        processEdgeQueue.Enqueue(poly);//说明还有连接，可能这个poly还没有处理完
                    else
                    {//Poly没有连接了，说明处理完了，对poly做最后的处理
                        if (!polys.ContainsKey(poly.Index))
                            polys.Add(poly.Index, poly);
                    }
                }


            }
            //对polyConnection计算权值
            foreach (PolyConnection polyCon in polyConnections)
            {
                Vector3 pointA = CenterPoint(polys[polyCon.A].GetPoints());
                Vector3 pointB = CenterPoint(polys[polyCon.B].GetPoints());
                polyCon.Weight = Vector3.Distance(pointA, pointB);
            }

            Debug.Log("merged count :" + mergedCout);
            Debug.Log("component count :" + componentCount);
        }
        /// <summary>
        /// 计算所有点到所有点的路径
        /// </summary>
        public void CalculateAllPaths()
        {
            paths = new List<int>[polys.Count, polys.Count];
            for (int j = 0; j < polys.Count; j++)
            {
                List<int> path = CalculatePath(j);
                for (int k = 0; k != j && k < polys.Count; k++)
                {
                    paths[j, k] = Path(j, k, path);
                }
            }
        }
        /// <summary>
        /// 计算从点s到其他所有点的路径
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public List<int> CalculatePath(int s)
        {
            List<bool> visited = new List<bool>(polys.Count);
            List<float> length = new List<float>(polys.Count);
            List<int> from = new List<int>(polys.Count);
            for (int i = 0; i < polys.Count; i++)
            {
                visited.Add(false);
                length.Add(Single.MaxValue);
                from.Add(-1);
            }

            RBTree<float, int> queue = new RBTree<float, int>();
            visited[s] = true;
            length[s] = 0f;
            queue.Add(0f, s);
            while (queue.Count > 0)
            {
                KeyValuePair<float, int> vPair = queue.Min();
                int v = vPair.Value;
                queue.Remove(vPair.Key);
                //每次拿出来的最小值一定属于最短路径(贪心性质)
                visited[v] = true;

                if (NearPolys.ContainsKey(v))
                {
                    foreach (PolyConnection e in nearPolys[v])
                    {
                        int w = e.Other(v);
                        if (!visited[w])
                        {
                            float l = length[v] + e.Weight;
                            if (from[w] == -1 || l < length[w])
                            {
                                length[w] = l;
                                from[w] = v;

                                if (queue.ContainsValue(w))
                                {//这里用sortedList的更新操作(删除再插入)来实现优先队列的更新
                                    queue.Remove(queue.KeyOfValue(w));
                                    queue.Add(l, w);
                                }
                                else
                                    queue.Add(l, w);
                            }
                        }
                    }
                }
            }
            return from;
        }
        /// <summary>
        /// 得到从s到d的路径
        /// </summary>
        /// <param name="s"></param>
        /// <param name="d"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public List<int> Path(int s, int d, List<int> from)
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
        /// <summary>
        /// 插入一个poly和其对应的polyConnection的映射
        /// </summary>
        /// <param name="A"></param>
        /// <param name="connection"></param>
        public void InsertNearPoly(int index, PolyConnection connection)
        {
            if (!nearPolys.ContainsKey(index))
                nearPolys.Add(index, new HashSet<PolyConnection>());

            if (!nearPolys[index].Contains(connection))
                nearPolys[index].Add(connection);
        }
        /// <summary>
        /// 根据一组点的索引,得到这些点的中点
        /// </summary>
        /// <param name="pointIndexes"></param>
        /// <returns></returns>
        public Vector3 CenterPoint(List<int> pointIndexes)
        {
            Vector3 center = Vector3.zero;
            foreach (int i in pointIndexes)
            {
                center += points[i];
            }

            return center / pointIndexes.Count;
        }
        /// <summary>
        /// 得到三条边构成的总角度
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public float TotalAngleForThreeSeg(int a, int b, int c, int d)
        {
            Vector3 lineA = points[a] - points[b];
            Vector3 lineB = points[c] - points[b];
            Vector3 lineC = points[d] - points[b];//中间那条线

            return Vector3.Angle(lineA, lineC) + Vector3.Angle(lineC, lineB);
        }
        /// <summary>
        /// 根据索引得到一个点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 Point(int index)
        {
            return points[index];
        }
    }
}
