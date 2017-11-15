using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditorPaths
{
        public static string GetBuildFolderName(BuildTarget target) {
            switch (target) {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                    return "OSX";

                default:
                    return null;
            }
        }

    public static string exportedRawNavMeshPath = new StringBuilder(Application.dataPath).Append("/Scenes/").Append(SceneManager.GetActiveScene().name).Append("/").Append(SceneManager.GetActiveScene().name).Append("_NavMesh_Origin.obj").ToString();
    public static string processedNavMeshPath = new StringBuilder(Application.dataPath).Append("/Scenes/").Append(SceneManager.GetActiveScene().name).Append("/").Append(SceneManager.GetActiveScene().name).Append("_NavMesh.obj").ToString();
}
