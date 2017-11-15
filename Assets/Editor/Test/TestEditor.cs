using GameFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Editor
{
    public class TestEditor
    {
        [MenuItem("Tools/Instantiate a prefab")]
        private static void InstantiatePrefab()
        {
            string path = PathTool.SceneResourcesPath() + "/UI/mask.prefab";
            Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            GameObject go = PrefabUtility.InstantiatePrefab(obj) as GameObject;
        }


        /// <summary>
        /// This function is the Validate Function to Function Selected GameObject
        /// </summary>
        /// <returns></returns>
        [MenuItem("MenuItem/Selected GameObject", true)]
        private static bool CheckObjectType()
        {
            Object selectedObject = Selection.activeObject;
            if (selectedObject != null &&
            selectedObject.GetType() == typeof(GameObject))
            {
                return true;
            }
            return false;
        }
    }
}
