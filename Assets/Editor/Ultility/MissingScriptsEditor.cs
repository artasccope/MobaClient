using GameFW;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissingScriptsEditor
{
    private static List<GameObject> lstTmp = new List<GameObject>();



    [MenuItem("Tools/Clear all prefab missing scripts _%#&c")]
    private static void CleanAllAssetMissingScripts() {
        lstTmp.Clear();
        string[] pths = AssetDatabase.GetAllAssetPaths();

        for(int i = 0; i < pths.Length; i++) {
            if (pths[i].Contains(".prefab")) {
                Debug.Log(pths[i]);
                EditorUtility.DisplayProgressBar("Checking", "逐个分析中，请勿退出！", (float)i / (float)pths.Length);
                Object obj = AssetDatabase.LoadAssetAtPath(pths[i], typeof(GameObject));
                CleanUpAsset(obj);
            }
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();

        foreach (var go in lstTmp)
        {
            GameObject.DestroyImmediate(go);
        }
        lstTmp.Clear();
    }

    [MenuItem("Tools/Clear selected prefab missing scripts _%#c"), MenuItem("Assets/Clear selected prefab missing scripts")]
    private static void CleanUpSelection()
    {
        var lstSelection = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets);

        for (int i = 0; i < lstSelection.Length; ++i)
        {
            EditorUtility.DisplayProgressBar("Checking", "逐个分析中，请勿退出！", (float)i / (float)lstSelection.Length);
            var gameObject = lstSelection[i] as GameObject;

            CleanUpAsset(gameObject);
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();

        foreach (var go in lstTmp)
        {
            GameObject.DestroyImmediate(go);
        }
        lstTmp.Clear();
    }

    private static void CleanUpAsset(Object asset)
    {
        GameObject go = PrefabUtility.InstantiatePrefab(asset) as GameObject;

        Queue<Transform> transQueue = new Queue<Transform>();
        transQueue.Enqueue(go.transform);
        while (transQueue.Count > 0) {
            GameObject trans = transQueue.Dequeue().gameObject;
            SerializedObject serializedObject = new SerializedObject(trans);
            // 获取组件列表属性
            SerializedProperty prop = serializedObject.FindProperty("m_Component");

            var components = trans.GetComponents<Component>();
            int r = 0;
            for (int j = 0; j < components.Length; j++)
            {
                // 如果组建为null
                if (components[j] == null)
                {
                    // 按索引删除
                    prop.DeleteArrayElementAtIndex(j - r);
                    r++;
                }
            }
            // 应用修改到对象
            serializedObject.ApplyModifiedProperties();
            for (int i = 0; i < trans.transform.childCount; i++) {
                transQueue.Enqueue(trans.transform.GetChild(i));
            }
        }

        // 将数据替换到asset
        // PrefabUtility.ReplacePrefab(go, asset);
        PrefabUtility.CreatePrefab(AssetDatabase.GetAssetPath(asset), go);

        go.hideFlags = HideFlags.HideAndDontSave;

        // 删除临时实例化对象
        lstTmp.Add(go);
    }



}