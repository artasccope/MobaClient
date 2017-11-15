using UnityEngine;
using UnityEditor;
using Nav;
using System.Collections.Generic;

namespace Assets.Editor
{
    public class NavMeshExportTest
    {
        [MenuItem("NavMesh/Draw Mesh")]
        public static void DrawNavMesh() {
            NavMeshExporter.ExportNavMesh();

            NavMesh navMesh = NavMeshProcessor.GetNavMeshFromRawNavFile(EditorPaths.exportedRawNavMeshPath, false);
            Debug.Log("poly Count:" + navMesh.Polys.Count);
            NavMeshProcessor.WriteProcessedNavFile(navMesh, EditorPaths.processedNavMeshPath, false);

            HashSet<int> polyIndex = new HashSet<int>();
            foreach (HashSet<PolyConnection> connections in navMesh.NearPolys.Values) {
                foreach (PolyConnection connection in connections) {
                    if (!polyIndex.Contains(connection.A))
                        polyIndex.Add(connection.A);
                    if (!polyIndex.Contains(connection.B))
                        polyIndex.Add(connection.B);
                }
            }

            foreach (int i in polyIndex) {
                Poly poly = navMesh.Polys[i];
                foreach (GeoEdge edge in poly.Edges) {
                    Debug.DrawLine(edge.PointA, edge.PointB, new Color(30f/255,185f/255,246f/255));
                }
            }
        }
    }
}
