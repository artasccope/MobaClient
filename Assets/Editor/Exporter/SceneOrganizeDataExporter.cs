using GameFW;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using GameFW.UI;
using UnityEngine.UI;
using System.Text;
using GameFW.Entity;
using GameFW.Ultility;
using GameFW.OrganizeData.UI;
using GameFW.ID;
using GameFW.OrganizeData.Entity;

public class SceneOrganizeDataExporter
{
    [MenuItem("Tools/Log presistent data path")]
    public static void DebugPersistentDataPath()
    {
        Debug.Log(Application.persistentDataPath);
        Debug.Log(Application.platform.ToString());
    }


    [MenuItem("Tools/Add registers to all _%#r")]
    public static void AddRegisterToAll()
    {
        GameObject[] gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        XElement nameRecord = new XElement("infoRecord");
        foreach (GameObject o in gameObjects)
        {
            AddRegisterRecursively(o);
            RecordElementName(ref nameRecord, o);
        }
        nameRecord.Save(new StringBuilder(Application.dataPath).Append("/Resources/").Append(SceneManager.GetActiveScene().name).Append("_nameRecord.xml").ToString());
    }

    private static void RecordElementName(ref XElement ele, GameObject root)
    {
        if (root.GetComponent<UIWidgetConfig>() != null || root.GetComponent<Button>() != null || root.GetComponent<InputField>() != null || root.GetComponent<Text>() != null
                || root.GetComponent<Slider>() != null)
        {
            ele.Add(new XElement("elementRecord", new XAttribute("name", IDCaculater.TransformNameInHierachy(root.transform, ""))));
        }
        for (int i = 0; i < root.transform.childCount; i++)
        {
            RecordElementName(ref ele, root.transform.GetChild(i).gameObject);
        }
    }

    private static void AddRegisterRecursively(GameObject root)
    {
        switch (LayerMask.LayerToName(root.layer))
        {
            case "UI":
                if (root.GetComponent<UIRegister>() == null)
                    root.AddComponent<UIRegister>();
                break;
            case "Building":
            case "NPC":
                if (root.GetComponent<EntityRegister>() == null)
                    root.AddComponent<EntityRegister>();
                break;
        }

        for (int i = 0; i < root.transform.childCount; i++)
        {
            AddRegisterRecursively(root.transform.GetChild(i).gameObject);
        }
    }

    [MenuItem("Tools/Export all selected Organize Data _%#&l")]
    public static void ExportOrganizeData()
    {//改为树形遍历
        AddRegisterToAll();

        Stack<EntityConfig> configs = new Stack<EntityConfig>();
        Queue<GameObject> objs = new Queue<GameObject>();
        foreach (GameObject o in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            objs.Enqueue(o);
        }

        while (objs.Count > 0)
        {
            GameObject o = objs.Dequeue();
            if (o.GetComponent<EntityConfig>() != null)
            {
                configs.Push(o.GetComponent<EntityConfig>());
            }
            for (int k = 0; k < o.transform.childCount; k++)
            {
                objs.Enqueue(o.transform.GetChild(k).gameObject);
            }
        }

        Dictionary<string, AssetLoadInfo> loadDic = new Dictionary<string, AssetLoadInfo>();
        Stack<KeyValuePair<GameObject, Transform>> parentCache = new Stack<KeyValuePair<GameObject, Transform>>();

        int i = 0;
        while (configs.Count > 0)
        {
            EditorUtility.DisplayProgressBar("Exporting", "逐个导出，请勿退出！", (float)i / (float)configs.Count);
            EntityConfig config = configs.Pop();
            GameObject go = config.gameObject;
            config.RecordInfo();

            string catName = "";
            if (config.entitySaveOption != null)
                catName = config.entitySaveOption.categoryName;
            else
                catName = LayerMask.LayerToName(go.layer);

            string name = null;
            if (config.entitySaveOption.ifLoadRuntime)
                name = config.entitySaveOption.name;
            else
                name = go.name;


            string prefabSavePath = new StringBuilder(PathTool.SceneResourcesPath()).Append("/").Append(catName).Append("/").Append(name).Append(".prefab").ToString();
            string prefabABName = new StringBuilder(SceneManager.GetActiveScene().name.ToLower()).Append("/").Append(catName.ToLower()).Append(".ld ").Append(name).Append(".prefab").ToString();
            string organizeDataABName = new StringBuilder(PathTool.GetOrganizeDataABName().ToLower()).Append(catName.ToLower()).Append(".ld ").Append(name).Append(".asset").ToString();

            if (config.entitySaveOption.ifLoadRuntime) {
                parentCache.Push(new KeyValuePair<GameObject, Transform>(go, go.transform.parent));
                go.transform.SetParent(null, true);
                PrefabUtility.CreatePrefab(prefabSavePath, go, ReplacePrefabOptions.Default);
            }
            else if (!loadDic.ContainsKey(prefabABName)) {
                loadDic.Add(prefabABName, new AssetLoadInfo(prefabABName, organizeDataABName, i, go.activeSelf));

                string organizeDataPath = new StringBuilder(PathTool.GetOrganizeDataPath()).Append(catName).Append("/").ToString();
                ExportInfo(config.entityInfo, organizeDataPath, name);
                parentCache.Push(new KeyValuePair<GameObject, Transform>(go, go.transform.parent));
                go.transform.SetParent(null, true);
                PrefabUtility.CreatePrefab(prefabSavePath, go, ReplacePrefabOptions.Default);
            }
            else
                continue;

            i++;
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        while (parentCache.Count > 0) {
            KeyValuePair<GameObject, Transform> p = parentCache.Pop();
            p.Key.transform.SetParent(p.Value, true);
        }

        XmlAccessor.WriteLoadRecordXml(PathTool.GetLoadRecordFilePath(), loadDic);
        FileTool.CopyFileToDestFolder(PathTool.GetLoadRecordFilePath(), PathTool.GetAssetBundlePath());

        EditorUtility.ClearProgressBar();
    }

    private static string GetPrefabName(string name, string goName) {
        if (string.IsNullOrEmpty(name))
            return goName;
        else
            return name;
    }

    public static void ExportInfo(ScriptableObject obj, string path, string name)
    {
        string assetPath = path;
        string resPath = path + name + ".asset";
        if (!Directory.Exists(assetPath))
            Directory.CreateDirectory(assetPath);

        if (File.Exists(resPath))
        {
            AssetDatabase.DeleteAsset(resPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        ScriptableObject info = GameObject.Instantiate(obj);
        AssetDatabase.CreateAsset(info, resPath);
    }



    [MenuItem("Tools/Add OrganizeData to selections _%#_l"), MenuItem("GameObject/UI/Add OrganizeData")]
    private static void AddOrganizeDataToGOs()
    {
        GameObject[] gos = Selection.gameObjects;
        for (int i = 0; i < gos.Length; i++)
        {
            AddOrganizeDataToGO(gos[i]);
            Debug.Log("Add organize data to GO : " + gos[i].name);
        }
    }

    private static void AddOrganizeDataToGO(GameObject go)
    {
        if (go.GetComponent<RectTransform>() != null && go.CompareTag("uiPrefab"))
        {
            if (go.GetComponent<UIWidgetConfig>() == null)
            {
                go.AddComponent<UIWidgetConfig>().RecordInfo();
            }
        }
        else
        {
            if (go.GetComponent<EntityConfig>() == null)
            {
                go.AddComponent<EntityConfig>().RecordInfo();
            }
            else
            {
                go.GetComponent<EntityConfig>().RecordInfo();
            }
        }
    }
}
