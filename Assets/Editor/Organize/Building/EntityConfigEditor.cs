using GameFW;
using GameFW.OrganizeData.Entity;
using GameFW.Ultility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(EntityConfig))]
[CanEditMultipleObjects]
public class EntityConfigEditor : Editor
{
    protected EntityConfig entityConfig;
    private GameObject go;

    private void OnEnable()
    {
        entityConfig = target as EntityConfig;
        go = entityConfig.gameObject;
        AddBuildingSaveOption(go, 0, 0, 0, true, true, false, null, null, null);
    }

    private int categoryIndex = 1;
    private int specieIndex = 0;
    private int teamIndex = 1;
    private string assetName;
    private bool ifSaveHierachy = false;
    private bool ifSaveTrans = false;
    private bool ifLoadRuntime =false;

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("导入数据(xlsx)", GUI.skin.GetStyle("LargeButton"), GUILayout.Height(30)))
        {
            ReadNameExcel();
        }

        if (GUILayout.Button("清空数据", GUI.skin.GetStyle("LargeButton"), GUILayout.Height(30)))
        {
            if (categoryNames != null)
                categoryNames.Clear();
            if (specieNames != null)
                specieNames.Clear();
            if (entitySaveOptions != null)
                entitySaveOptions.Clear();
        }
        if (GUILayout.Button("保存数据", GUI.skin.GetStyle("LargeButton"), GUILayout.Height(30)))
        {
            SaveNameExcel();
            SaveRuntimeLoadXml();
        }

        GUILayout.EndHorizontal();
        GUILayout.Box("", GUI.skin.GetStyle("TL LoopSection"), GUILayout.Width(6000), GUILayout.Height(1));
        GUILayout.Space(5f);

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("类别名:", GUI.skin.GetStyle("SelectionRect"), GUILayout.Width(45), GUILayout.Height(22));

        int categoryId = 0;
        if (!entitySaveOptions.ContainsKey(go))
        {
            categoryIndex = EditorGUILayout.Popup(categoryIndex, categoryNames.Values.ToArray(), GUI.skin.GetStyle("Popup"), GUILayout.Width(90));
            categoryId = categoryIndex + 1;
        }
        else
        {
            entitySaveOptions[go].categoryIndex = EditorGUILayout.Popup(entitySaveOptions[go].categoryIndex, categoryNames.Values.ToArray(), GUI.skin.GetStyle("Popup"), GUILayout.Width(90));
            categoryId = entitySaveOptions[go].categoryIndex + 1;
        }

        int specieId = 0;
        if (specieNames.ContainsKey(categoryId)&&specieNames[categoryId].ContainsKey(specieIndex+1)) {
            EditorGUILayout.LabelField("种类名:", GUI.skin.GetStyle("SelectionRect"), GUILayout.Width(45), GUILayout.Height(22));
            if (!entitySaveOptions.ContainsKey(go))
            {
                specieIndex = EditorGUILayout.Popup(specieIndex, specieNames[categoryId].Values.ToArray(), GUI.skin.GetStyle("Popup"), GUILayout.Width(90));
                specieId = specieIndex + 1;
            }
            else {
                entitySaveOptions[go].specieIndex = EditorGUILayout.Popup(entitySaveOptions[go].specieIndex, specieNames[categoryId].Values.ToArray(), GUI.skin.GetStyle("Popup"), GUILayout.Width(90));
                specieId = entitySaveOptions[go].specieIndex + 1;
                specieIndex = entitySaveOptions[go].specieIndex;
            }
        }
        EditorGUILayout.LabelField("队伍:", GUI.skin.GetStyle("SelectionRect"), GUILayout.Width(35), GUILayout.Height(22));
        if (!entitySaveOptions.ContainsKey(go))
        {
            teamIndex = EditorGUILayout.Popup(teamIndex, teamArr, GUI.skin.GetStyle("Popup"), GUILayout.Width(90));
        }
        else {
            entitySaveOptions[go].teamIndex = EditorGUILayout.Popup(entitySaveOptions[go].teamIndex, teamArr, GUI.skin.GetStyle("Popup"), GUILayout.Width(90));
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Asset资源名:", GUI.skin.GetStyle("SelectionRect"), GUILayout.Width(80), GUILayout.Height(22));
        if (categoryNames.ContainsKey(categoryId) && specieNames[categoryId].ContainsKey(specieId))
        {
            assetName = GetBuildingAssetName(categoryId, specieId);
            EditorGUILayout.LabelField(assetName);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (!entitySaveOptions.ContainsKey(go))
        {
            ifSaveHierachy = EditorGUILayout.ToggleLeft("保存层次关系", ifSaveHierachy, GUILayout.Width(90));
            ifSaveTrans = EditorGUILayout.ToggleLeft("保存变换信息", ifSaveTrans, GUILayout.Width(90));
        }
        else {
            entitySaveOptions[go].ifSaveHierachy = EditorGUILayout.ToggleLeft("保存层次关系", entitySaveOptions[go].ifSaveHierachy, GUILayout.Width(90));
            entitySaveOptions[go].ifSaveTrans = EditorGUILayout.ToggleLeft("保存变换信息", entitySaveOptions[go].ifSaveTrans, GUILayout.Width(90));
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (!entitySaveOptions.ContainsKey(go))
        {
            ifLoadRuntime = EditorGUILayout.ToggleLeft("运行时加载", ifLoadRuntime, GUILayout.Width(90));
        }
        else {
            entitySaveOptions[go].ifLoadRuntime = EditorGUILayout.ToggleLeft("运行时加载", entitySaveOptions[go].ifLoadRuntime, GUILayout.Width(90));
        }

        foreach (GameObject obj in Selection.gameObjects)
        {
            if (!entitySaveOptions.ContainsKey(obj))
            {
                AddBuildingSaveOption(obj, categoryIndex, specieIndex, teamIndex, ifSaveHierachy, ifSaveTrans, ifLoadRuntime, "", "", assetName);
            }

            entitySaveOptions[obj].categoryIndex = entitySaveOptions[go].categoryIndex;
            entitySaveOptions[obj].specieIndex = entitySaveOptions[go].specieIndex;
            entitySaveOptions[obj].teamIndex = entitySaveOptions[go].teamIndex;
            entitySaveOptions[obj].ifSaveHierachy = entitySaveOptions[go].ifSaveHierachy;
            entitySaveOptions[obj].ifSaveTrans = entitySaveOptions[go].ifSaveTrans;
            entitySaveOptions[obj].ifLoadRuntime = entitySaveOptions[go].ifLoadRuntime;
            if(categoryNames.ContainsKey(categoryId))
                entitySaveOptions[obj].categoryName = categoryNames[categoryId];
            if(specieNames.ContainsKey(categoryId)&& specieNames[categoryId].ContainsKey(specieId))
                entitySaveOptions[obj].name = specieNames[categoryId][specieId];
            entitySaveOptions[obj].assetName = assetName;
        }

        GUILayout.EndHorizontal();

        GUILayout.Box("", GUI.skin.GetStyle("TL LoopSection"), GUILayout.Width(6000), GUILayout.Height(1));
    }

    private static void SaveRuntimeLoadXml()
    {
        Dictionary<string, RuntimeAssetLoadInfo> runtimeAssetLoadInfos = new Dictionary<string, RuntimeAssetLoadInfo>();
        foreach (KeyValuePair<GameObject, EntitySaveOption> p in entitySaveOptions) {
            if (!runtimeAssetLoadInfos.ContainsKey(p.Value.assetName)) {
                runtimeAssetLoadInfos.Add(p.Value.assetName, new RuntimeAssetLoadInfo(p.Value.categoryIndex, p.Value.specieIndex, p.Value.assetName, p.Key.activeSelf));
            }
        }

        XmlAccessor.WriteRuntimePrefabXml(PathTool.RuntimePrefabXmlPath(), runtimeAssetLoadInfos);
        FileTool.CopyFileToDestFolder(PathTool.RuntimePrefabXmlPath(), PathTool.GetAssetBundlePath());
    }

    private static string filePath;

    public static Dictionary<int, string> categoryNames = new Dictionary<int, string>();
    public static Dictionary<int, Dictionary<int, string>> specieNames = new Dictionary<int, Dictionary<int, string>>();

    public static Dictionary<GameObject, EntitySaveOption> entitySaveOptions = new Dictionary<GameObject, EntitySaveOption>();

    public static string[] teamArr = { "1", "2" };

    private Vector2 m_ScrollPosition;

    private static string GetBuildingAssetName(int category, int specie)
    {
        if (specieNames.ContainsKey(category) && specieNames[category].ContainsKey(specie))
        {
            return new StringBuilder(SceneManager.GetActiveScene().name.ToLower()).Append("/").Append(categoryNames[category].ToLower()).Append(".ld ").Append(specieNames[category][specie]).Append(".prefab").ToString();
        }
        return null;
    }

    private static void ReadNameExcel()
    {
        string path = EditorUtility.OpenFilePanel("导入建筑数据Excel", "", "xlsx");
        filePath = path;

        if (string.IsNullOrEmpty(path))
            return;

        ExcelAccessor.ReadCatgoryNames(path ,ref categoryNames, ref specieNames);
    }

    private static void SaveNameExcel()
    {
        ExcelAccessor.SaveBuildingInfos(filePath, entitySaveOptions);
    }

    private void AddBuildingSaveOption(GameObject go, int categoryId, int specieId, int teamId, bool ifSaveHierachy, bool ifSaveTrans, bool ifLoadRuntime, string categoryName, string name, string assetName)
    {
        if (!entitySaveOptions.ContainsKey(go))
        {
            EntitySaveOption entitySaveOption = new EntitySaveOption(categoryId, specieId, teamId, ifSaveHierachy, ifSaveTrans, ifLoadRuntime, categoryName, name, assetName);
            entitySaveOptions.Add(go, entitySaveOption);
            go.GetComponent<EntityConfig>().entitySaveOption = entitySaveOption;
        }
    }
}
