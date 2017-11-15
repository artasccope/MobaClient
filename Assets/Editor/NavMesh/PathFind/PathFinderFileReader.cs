using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nav
{
    public class PathFinderFileReader
    {
        public static PathFinder Read(string fileName) {
            StreamReader sr = new StreamReader(fileName);
            string l;

            float left = 0f;
            float bottom = 0f;
            float tileSize = 0f;
            int width = 0;
            int height = 0;
            int componentCount = 0;
            List<Vector3> points = new List<Vector3>();
            Dictionary<int, Poly> polys = new Dictionary<int, Poly>();
            Dictionary<int, List<NodeEdge>> edges = new Dictionary<int, List<NodeEdge>>();
            List<int>[,] pths = null;
            List<int>[,] polyAOI = null;
            while ((l = sr.ReadLine()) != null) {
                string[] info = l.Split(' ');
                if (info[0] == "v")
                {
                    Vector3 p = new Vector3(Convert.ToSingle(info[1]), Convert.ToSingle(info[2]), Convert.ToSingle(info[3]));
                    points.Add(p);
                }
                else if (info[0] == "p")
                {
                    string[] infos = l.Split('|');
                    Poly poly = new Poly();
                    int index = Convert.ToInt32(info[1]);
                    poly.Index = Convert.ToInt32(info[1]);
                    for (int i = 1; i < infos.Length; i++) {
                        string[] edge = infos[i].Split(' ');
                        poly.AddEdge(new GeoEdge(Convert.ToInt32(edge[0]), Convert.ToInt32(edge[1]), points));
                    }
                    polys.Add(index, poly);
                }
                else if (info[0] == "c")
                {
                    int index = Convert.ToInt32(info[1]);
                    List<NodeEdge> eEdges = new List<NodeEdge>();
                    for (int i = 2; i < info.Length - 3; i += 4) {
                        NodeEdge nEdge = new NodeEdge();
                        nEdge.A = index;
                        nEdge.B = Convert.ToInt32(info[i]);
                        nEdge.Weight = Convert.ToSingle(info[i+1]);
                        nEdge.PointA = Convert.ToInt32(info[i+2]);
                        nEdge.PointB = Convert.ToInt32(info[i+3]);
                    }
                    edges.Add(index, eEdges);
                }
                else if (info[0] == "from")
                {
                    if (pths == null)
                        pths = new List<int>[polys.Count, polys.Count];

                    List<int> path = new List<int>();
                    int from = Convert.ToInt32(info[1]);
                    int to = Convert.ToInt32(info[3]);
                    for (int i = 4; i < info.Length; i++) {
                        path.Add(Convert.ToInt32(info[i]));
                    }
                    pths[from, to] = path;
                }
                else if (info[0] == "componentCount")
                {
                    componentCount = Convert.ToInt32(info[1]);
                }
                else if (info[0] == "AOIInfo")
                {
                    left = Convert.ToSingle(info[1]);
                    bottom = Convert.ToSingle(info[2]);
                    tileSize = Convert.ToSingle(info[3]);
                    width = Convert.ToInt32(info[4]);
                    height = Convert.ToInt32(info[5]);
                }
                else if (info[0] == "aoiOf") {
                    if (polyAOI == null)
                        polyAOI = new List<int>[width, height];

                    List<int> aoi = new List<int>();
                    int i = Convert.ToInt32(info[1]);
                    int j = Convert.ToInt32(info[2]);

                    for (int k = 3; k < info.Length; k++) {
                        aoi.Add(Convert.ToInt32(info[k]));
                    }

                    polyAOI[i, j] = aoi;
                }
            }

            sr.Close();
            sr.Dispose();

            if (pths == null)
                return new PathFinderRealtime(points, polys, edges, left, bottom, tileSize, width, height, componentCount, polyAOI);
            else
                return new PathFinderPre(points, polys,edges, left, bottom, tileSize, width, height, componentCount, polyAOI);
        }
    }
}
