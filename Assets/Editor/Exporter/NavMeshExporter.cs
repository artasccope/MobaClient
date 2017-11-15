using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using System.IO;

namespace Assets.Editor
{
    public class NavMeshExporter
    {
        [MenuItem("NavTools/Export NavMesh to obj")]
        public static void ExportNavMesh() {
            Debug.Log("exporting NavMesh...");

            //Create NavMesh Obj file.
            NavMeshTriangulation tris = UnityEngine.AI.NavMesh.CalculateTriangulation();
            int[] areas = tris.areas;
            Vector3[] verts = tris.vertices;
            int[] indices = tris.indices;

            if (File.Exists(EditorPaths.exportedRawNavMeshPath))
                File.Delete(EditorPaths.exportedRawNavMeshPath);

            StreamWriter sw = new StreamWriter(EditorPaths.exportedRawNavMeshPath);
            for (int i = 0; i < verts.Length; i++) {
                sw.WriteLine("v "+ verts[i].x + " "+ verts[i].y + " " + verts[i].z);
            }

            sw.WriteLine("g obj ");

            for (int i = 0; i < indices.Length;) {
                sw.WriteLine("f "+ (indices[i+0]+1) + " " + (indices[i+1]+1) + " " + (indices[i+2]+1));
                i = i + 3;
            }
            sw.Flush();
            sw.Close();
            sw.Dispose();
            Debug.Log("NavMesh exported.");
        }



    }
}
