using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nav
{
    public class NavMeshProcessor
    {
        private static BinaryTree3D binaryTree3d = new BinaryTree3D();
        private static List<Vector3> points = new List<Vector3>();
        private static List<PointIndexPair> pointsAndIndexes = new List<PointIndexPair>();
        private static List<int> triangles = new List<int>();

        private static float GetFloat(string s) {
            return Convert.ToSingle(s);
        }

        private static int GetInt(string s)
        {
            return Convert.ToInt32(s);
        }

        public static NavMesh GetNavMeshFromRawNavFile(string fileName, bool ifPreCalculatePath) {
            if (!File.Exists(fileName))
                return null;

            points.Clear();
            pointsAndIndexes.Clear();
            binaryTree3d.Clear();
            triangles.Clear();

            ReadRawNavFile(fileName);
            MergeSamePoint();

            NavMesh navMesh = new NavMesh(points, triangles, ifPreCalculatePath);
            return navMesh;
        }
        /// <summary>
        /// 得到处理过后的导航文件,包含信息(顶点、多边形、多边形连接、预计算路径信息、多边形的AOI信息)
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="ifPreCalculatePath"></param>
        public static void WriteProcessedNavFile(NavMesh navMesh, string fileName, bool ifPreCalculatePath) {
            if (File.Exists(fileName))
                File.Delete(fileName);

            StreamWriter sw = new StreamWriter(fileName);
            StringBuilder lBuilder;

            for (int i = 0; i < navMesh.Points.Count; i++)//顶点信息
            {
                lBuilder = new StringBuilder("v").Append(navMesh.Points[i].x.ToString()).Append(" ").Append(navMesh.Points[i].y.ToString()).Append(" ").Append(navMesh.Points[i].z);
                sw.WriteLine(lBuilder.ToString());
            }

            SortedDictionary<int, Poly>.Enumerator polyIter = navMesh.Polys.GetEnumerator();
            while (polyIter.MoveNext()) {//Poly 信息
                lBuilder = new StringBuilder("p").Append(polyIter.Current.Key.ToString()).Append(" ");
                Poly p = polyIter.Current.Value;
                foreach (GeoEdge e in p.Edges) {
                    lBuilder.Append("|").Append(e.A.ToString()).Append(" ").Append(e.B);
                }
                sw.WriteLine(lBuilder.ToString());
            }
            polyIter.Dispose();

            SortedDictionary<int, HashSet<PolyConnection>>.Enumerator iter = navMesh.NearPolys.GetEnumerator();
            while (iter.MoveNext())//Poly connection信息
            {
                //有连接的poly的序号
                lBuilder = new StringBuilder("c ").Append(iter.Current.Key.ToString());
                HashSet<PolyConnection> cons = iter.Current.Value;

                foreach (PolyConnection con in cons)
                {
                    lBuilder.Append(" ").Append(con.Other(iter.Current.Key).ToString()).Append(" ").Append(con.Weight.ToString()).Append(" ").Append(con.ConnectionEdge.A.ToString()).Append(" ").Append(con.ConnectionEdge.B.ToString());
                    //每个连接有4个数据，分别是:连向的Poly的序号、此连接的权重、对应此连接的边的两个顶点
                }
                sw.WriteLine(lBuilder.ToString());
            }
            iter.Dispose();

            if (ifPreCalculatePath)
            {//预存储的路径信息, 存在文件的后部
                List<int>[,] paths = navMesh.Paths;
                for (int m = 0; m < navMesh.Polys.Count; m++)
                {
                    for (int n = 0; n < navMesh.Polys.Count; n++)
                    {
                        List<int> pth = paths[m, n];
                        if (pth != null && pth.Count > 0) {
                            lBuilder = new StringBuilder("from ").Append(m.ToString()).Append(" to ").Append(n.ToString());
                            for (int x = 0; x < pth.Count; x++)
                                lBuilder.Append(" ").Append(pth[x].ToString());
                            sw.WriteLine(lBuilder.ToString());
                        }
                    }
                }
            }

            sw.WriteLine("componentCount " + navMesh.ComponentCount);

            //这里把AOI信息也写进来
            NavAOI navAOI = NavAOIProcesser.ProcessPolyAOI(navMesh.Polys, navMesh);
            sw.WriteLine("navAOI:");
            lBuilder = new StringBuilder("AOIInfo ").Append(navAOI.Left.ToString()).Append(" ").Append(navAOI.Bottom).Append(" ").Append(navAOI.TileSize).Append(" ").Append(navAOI.Width).Append(" ").Append(navAOI.Height);
            sw.WriteLine(lBuilder.ToString());
            for (int i = 0; i < navAOI.Width; i++) {
                for (int j = 0; j < navAOI.Height; j++) {
                    List<int> aoiList = navAOI.NavPolyAOI[i, j];
                    if (aoiList != null) {
                        lBuilder = new StringBuilder(i.ToString()).Append(",").Append(j.ToString()).Append(":");
                        for (int k = 0; k < aoiList.Count; k++)
                            lBuilder.Append(" ").Append(aoiList[k]);

                        sw.WriteLine(lBuilder.ToString());
                    }
                }
            }

            sw.Close();
            sw.Dispose();
        }
        /// <summary>
        /// 将原始的NavMesh顶点信息(包含重复顶点)读入
        /// </summary>
        /// <param name="fileName"></param>
        public static void ReadRawNavFile(string fileName) {
            // 将文件中的信息读入这两个List
            FileStream fs = File.Open(fileName, FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            string l;
            string[] data;
            while ((l = sr.ReadLine()) != null)
            {
                if (l.Contains('v'))
                {
                    data = l.Split(' ');
                    Vector3 point = new Vector3(GetFloat(data[1]), GetFloat(data[2]), GetFloat(data[3]));
                    points.Add(point);
                    pointsAndIndexes.Add(new PointIndexPair(points.Count-1, point));//索引从0开始
                }
                else if (l.Contains('f'))
                {
                    data = l.Split(' ');
                    triangles.Add(GetInt(data[1])-1);//triangle对应的point的index是从0开始的
                    triangles.Add(GetInt(data[2])-1);
                    triangles.Add(GetInt(data[3])-1);
                }
            }

            sr.Close();
            sr.Dispose();
            fs.Close();
            fs.Dispose();
        }

        /// <summary>
        /// 相同的点全部存入了binaryTree3d
        /// </summary>
        public static void MergeSamePoint()
        {
            //命名写得更清楚点
            for (int i = pointsAndIndexes.Count - 1; i > 0; i--)
            {//将所有点存入binaryTree3d(使用随机化存储来使树均衡)
                int randomIndex = new System.Random(i).Next(0, i);
                PointIndexPair tmp = pointsAndIndexes[randomIndex];
                pointsAndIndexes[randomIndex] = pointsAndIndexes[i];
                pointsAndIndexes[i] = tmp;

                binaryTree3d.Add(tmp.Point, tmp.Index);//将相同点的索引更新为最小值，这样那些重复点的索引就被合并了
            }

            if (pointsAndIndexes.Count > 0)
                binaryTree3d.Add(pointsAndIndexes[0].Point, pointsAndIndexes[0].Index);

            for (int j = triangles.Count - 1; j >= 0; j--)
            {//更新三角形的索引到合并后的顶点
                triangles[j] = binaryTree3d[points[triangles[j]]];
            }

            List<Vector3> tmpPoints = new List<Vector3>();
            List<int> tmpTriangles = new List<int>();
            binaryTree3d.Clear();


            Debug.Log("triangle after process.");
            //接下来，把三角形对应的顶点从前到后插入这两个对应数据结构，tmpPo
            for (int k = 0; k < triangles.Count; k++) {
                if (!binaryTree3d.Contains(points[triangles[k]]))//如果没有存储这个point,说明是新的
                {
                    int index = binaryTree3d.Count;//先得到当前的索引
                    tmpPoints.Add(points[triangles[k]]);//tmpPoints存储point的Vector3
                    binaryTree3d.Add(points[triangles[k]], index);//binaryTree3d存储point的Vector3，对应的索引
                    tmpTriangles.Add(index);//三角形的索引也存储下来
                }
                else {//如果存储了，说明点是重复的，那么相应地把索引也更新到之前存储的point对应的索引上去
                    int oldIndex = triangles[k];
                    Vector3 pointVec = points[oldIndex];
                    int newIndex = binaryTree3d[pointVec];
                    tmpTriangles.Add(newIndex);
                }
            }//这是O(n2)的算法，可考虑优化

            points = tmpPoints;
            triangles = tmpTriangles;
            //到这里,points就都是去重的了
        }
    }
}
