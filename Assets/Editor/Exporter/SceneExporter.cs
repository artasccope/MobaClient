using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;


/*
public class SceneExporter
{
    private static AutomicInt atInt = new AutomicInt(1);
    private static Dictionary<int, HierachyInfo> UIElementsList = new Dictionary<int, HierachyInfo>();
    private static Dictionary<int, HierachyInfo> mapElementsList = new Dictionary<int, HierachyInfo>();

    //-------------------------------------------csv----------------------------------------------------//
    static void changeLuaToUTF8(string luaPath)
    {
        DirectoryInfo dir = new DirectoryInfo(luaPath);
        foreach (FileInfo file in dir.GetFiles("*.lua.*", SearchOption.AllDirectories))
        {
            if (file.Extension != ".meta")
            {
                string path = file.FullName.Replace('\\', '/');
                string content = File.ReadAllText(path);
                content = content.Replace("\r\n", "\n");
                //UTF8Encoding的参数一定要为false，表示不省略BOM，即Byte Order Mark，也即字节流标记，它是用来让应用程序识别所用的编码的
                using (var sw = new StreamWriter(path, false, new UTF8Encoding(false)))
                {
                    sw.Write(content);
                    sw.Close();
                }
                Debug.Log("Encode file::>>" + path + " OK!");
            }
        }
        Debug.Log("转换完成");
    }

    [MenuItem("Export/配置表/更新")]
    static void updateCsv()
    {
        //转换为Utf8编码，存入临时文件夹
        string tmpDir = Application.dataPath + "/_lang/tmp/";
        if (Directory.Exists(tmpDir))
            Directory.Delete(tmpDir, true);
        Directory.CreateDirectory(tmpDir);

        DirectoryInfo dir = new DirectoryInfo(PathUtil.csvPath);
        foreach (FileInfo file in dir.GetFiles("*.csv", SearchOption.AllDirectories))
        {
            using (StreamReader sr = new StreamReader(file.FullName, Encoding.Default, false))
            {
                byte[] bytes = Encoding.Default.GetBytes(sr.ReadToEnd());
                sr.Close();
                byte[] data = Encoding.Convert(Encoding.Default, Encoding.UTF8, bytes);
                string str = Encoding.UTF8.GetString(data);

                string writePath = tmpDir + file.Name;
                using (StreamWriter sw = new StreamWriter(writePath, false, Encoding.UTF8))//以新的编码格式写入
                {
                    sw.Write(str);
                    sw.Close();
                }
                Debug.Log("write to " + writePath);
            }
        }

        //转为lua表
        Debug.Log("start to pack to lua ！！！");
        string path = Application.dataPath + "/_lang/PackTool.bat";
        System.Diagnostics.Process proc = System.Diagnostics.Process.Start(path);
        proc.WaitForExit();
        //删除临时文件
        Directory.Delete(tmpDir, true);
        changeLuaToUTF8(PathUtil.luaPath + "game/configs/");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Export/Test editor scene name")]
    static void testEditorSceneName()
    {
        Debug.Log(EditorSceneManager.GetActiveScene().name);
    }

    /*    [MenuItem("Export/将记录UI信息的CSV文件转为小写")]
        static void castCsvFilesToLower()
        {
            string path = Application.dataPath + "/_lang/csv/";
            string[] csvFileNames = Directory.GetFiles(path);
            foreach (string fileName in csvFileNames)
            {
                using (StreamReader sr = new StreamReader(fileName, Encoding.Default, false))
                {
                    string block = sr.ReadToEnd().ToLower();
                    sr.Close();
                    File.Delete(fileName);
                    using (StreamWriter sw = new StreamWriter(fileName, false, Encoding.Default))//以新的编码格式写入
                    {
                        sw.Write(block);
                        sw.Close();
                    }
                    Debug.Log("write to " + fileName);
                }
            }
        }

    
    [MenuItem("Export/UI和地图预制")]
    static void exportToLoadElements()
    {
        GameObject UIRoot = GameObject.FindObjectOfType<Canvas>().gameObject;
        if (UIRoot != null)
        {
            UIElementsList.Clear();
            BuildHierachyPrefabsNCatalog(UIRoot, UIElementsList);
            if (UIElementsList.Count > 0)
            {
                string catalogFilePath = Application.dataPath + "/_lang/csv/UIcatalog.csv";
                WriteLineToCatalog(catalogFilePath, EditorSceneManager.GetActiveScene().name, UIRoot.name);
            }
        }

        GameObject MapRoot = GameObject.FindGameObjectWithTag("mapRoot");
        if (MapRoot != null)
        {
            mapElementsList.Clear();
            BuildHierachyPrefabsNCatalog(MapRoot, mapElementsList);
            if (mapElementsList.Count > 0)
            {
                string catalogFilePath = Application.dataPath + "/_lang/csv/MapCatalog.csv";
                WriteLineToCatalog(catalogFilePath, MapRoot.name, MapRoot.name);
            }
        }
    }

    private static void BuildHierachyPrefabsNCatalog(GameObject go, Dictionary<int, HierachyInfo> elementList)
    {
        string eleName = go.name;
        clearNodesInfo(go.transform, eleName);
        recurseNodes(go.transform, eleName);
        recursePrefabs(go.transform, eleName, elementList);
        writeElementCatalog(eleName, elementList);
    }

    private static void writeElementCatalog(string indexName, Dictionary<int, HierachyInfo> elementList)
    {
        string listFilePath = Application.dataPath + "/_lang/csv/" + indexName + ".csv";
        if (File.Exists(listFilePath))
        {
            File.Delete(listFilePath);
        }

        if (elementList.Count > 0)
        {
            FileStream fs = new FileStream(listFilePath, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs);

            string header = "sid,name,parentId,siblingId";
            sw.WriteLine(header);
            string types = "int,string,int,int";
            sw.WriteLine(types);
            string introduction = "序号,名字,父节点Id,顺序Id";
            sw.WriteLine(introduction);
            foreach (KeyValuePair<int, HierachyInfo> kv in elementList)
            {
                string name = kv.Value.name;
                if (name.Contains(","))
                {
                    name.Replace(",", "用于占位的逗号");
                }
                string element = kv.Key + "," + kv.Value.name + "," + kv.Value.parentId + "," + kv.Value.siblingId;
                sw.WriteLine(element);
            }

            sw.Close();
            fs.Close();
        }
    }

    /// <summary>
    /// 将当前UI名及其对应的场景id记录进文件
    /// </summary>
    /// <param name="catalogFilePath"></param>
    /// <param name="name"></param>
    public static void WriteLineToCatalog(string catalogFilePath, string indexStr, string name)
    {
        string[] lines = new string[0];
        if (File.Exists(catalogFilePath))
        {
            lines = File.ReadAllLines(catalogFilePath);
        }

        Dictionary<string, string> indexAndNames = new Dictionary<string, string>();
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Split(',').Length == 2)
            {
                if (lines[i] != "sid,name" && lines[i] != "string,string" && lines[i] != "索引名,目录名")
                {
                    string[] indexAndName = lines[i].Split(',');
                    indexAndNames.Add(indexAndName[0], indexAndName[1]);
                }
            }
        }

        if (indexAndNames.ContainsKey(indexStr))
        {
            indexAndNames[indexStr] = name;
        }
        else
        {
            indexAndNames.Add(indexStr, name);
        }

        FileStream fs = new FileStream(catalogFilePath, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        sw.WriteLine("sid,name");
        sw.WriteLine("string,string");
        sw.WriteLine("索引名,目录名");

        foreach (KeyValuePair<string, string> kv in indexAndNames)
        {
            sw.WriteLine(kv.Key + ',' + kv.Value);
        }

        sw.Close();
        fs.Close();
    }
    /// <summary>
    /// 清除场景中物体的节点信息
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="indexName"></param>
    private static void clearNodesInfo(Transform trans, string indexName)
    {
        InfoNode[] nodes = trans.GetComponentsInChildren<InfoNode>();

        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].removeNodeInfoOfName(indexName);
            if ((nodes[i].transform.tag == "uiPrefab") || (nodes[i].transform.tag == "commonUI"))
            {
                RecTransformInfo[] recs = nodes[i].transform.GetComponents<RecTransformInfo>();
                foreach (RecTransformInfo info in recs)
                {
                    if (info.IndexName == indexName)
                    {
                        GameObject.DestroyImmediate(info);
                    }
                }
            }
            else if (nodes[i].transform.tag == "mapElements")
            {
                GameObjInfo[] goInfos = nodes[i].transform.GetComponents<GameObjInfo>();
                foreach (GameObjInfo goInfo in goInfos)
                {
                    if (goInfo.IndexName == indexName)
                    {
                        GameObject.DestroyImmediate(goInfo);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 递归遍历所有子节点，将每个节点的hierachy信息存储到InfoNode脚本，并将脚本挂载上去
    /// </summary>
    /// <param name="trans"></param>
    private static void recurseNodes(Transform trans, string indexName)
    {
        int childCount = trans.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = trans.GetChild(i);
            if (child.GetComponent<InfoNode>() == null)
            {
                child.gameObject.AddComponent<InfoNode>();
            }
            InfoNode node = child.gameObject.GetComponent<InfoNode>();
            int parentId = 0;
            if (trans.GetComponent<InfoNode>() != null)
            {
                parentId = trans.GetComponent<InfoNode>().GetNodeIdOfName(indexName);
            }
            node.SetNodeOfName(indexName, atInt.GetIncrease(), parentId, child.GetSiblingIndex());

            recurseNodes(child, indexName);
        }
    }

    private static void recordPrefabs(Transform trans, string indexName, Dictionary<int, HierachyInfo> elementList)
    {
        elementList.Clear();
        recursePrefabs(trans, indexName, elementList);
    }

    /// <summary>
    /// 递归遍历所有子节点，将每个需要做成预制物体的预制信息存储到对应的脚本，并将脚本挂载上去
    /// </summary>
    /// <param name="trans"></param>
    private static void recursePrefabs(Transform trans, string indexName, Dictionary<int, HierachyInfo> elementList)
    {
        int childCount = trans.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = trans.GetChild(i);
            string filePath = string.Empty;
            string fileName = string.Empty;
            if ((child.tag == "uiPrefab") || (child.tag == "commonUI"))
            {
                RecTransformInfo info = child.gameObject.AddComponent<RecTransformInfo>();
                RectTransform rec = child.GetComponent<RectTransform>();

                info.IndexName = indexName;
                info.SiblingIndex = child.GetSiblingIndex();
                info.Pivot = rec.pivot;
                info.SizeDelta = rec.sizeDelta;
                info.AnchoredPosition = rec.anchoredPosition;
                info.AnchoredPosition3D = rec.anchoredPosition3D;
                info.AnchorMax = rec.anchorMax;
                info.AnchorMin = rec.anchorMin;
                info.LocalEulerAngles = rec.localEulerAngles;
                info.LocalPosition = rec.localPosition;
                info.LocalScale = rec.localScale;
                info.OffsetMax = rec.offsetMax;
                info.OffsetMin = rec.offsetMin;

                if (child.tag != "commonUI")
                {
                    filePath = "Assets/DynamicResources/UIPrefabs/UI/" + indexName + '/';
                }
                else
                {
                    filePath = "Assets/DynamicResources/UIPrefabs/UI/COMMON/";
                }



                fileName = child.name;

            }
            else if (child.tag == "mapElements")
            {
                GameObjInfo goInfo = child.gameObject.AddComponent<GameObjInfo>();

                goInfo.IndexName = indexName;
                goInfo.LocalPosition = child.localPosition;
                goInfo.LocalScale = child.localScale;
                goInfo.LocalEulerAngles = child.localEulerAngles;
                goInfo.SiblingId = child.GetSiblingIndex();

                GameObject[] mapElements = GameObject.FindGameObjectsWithTag("mapElements");
                bool isChild = false;
                foreach (GameObject o in mapElements)
                {
                    if ((child != o.transform) && child.IsChildOf(o.transform))
                    {
                        isChild = true;
                    }
                }

                if (isChild)
                {
                    filePath = "Assets/prefabs/map/" + indexName + "/childs/";
                    fileName = child.name;
                }
                else
                {
                    filePath = "Assets/prefabs/map/" + indexName + "/";
                    fileName = child.name + "#" + child.GetComponent<InfoNode>().GetNodeIdOfName(indexName);
                }
            }

            if (!string.IsNullOrEmpty(filePath) && !Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(filePath))
            {
                string preFileName = filePath + fileName + ".prefab";
                if (File.Exists(preFileName))
                {
                    File.Delete(preFileName);
                }

                Object pre = PrefabUtility.CreateEmptyPrefab(preFileName);
                PrefabUtility.ReplacePrefab(child.gameObject, pre);

                if (!preFileName.Contains("/childs/"))
                {
                    InfoNode nInfo = child.gameObject.GetComponent<InfoNode>();
                    HierachyInfo hInfo = new HierachyInfo();
                    hInfo.name = preFileName.ToLower();
                    hInfo.parentId = nInfo.GetParentIdOfName(indexName);
                    hInfo.siblingId = nInfo.GetSiblingIdOfName(indexName);
                    elementList.Add(nInfo.GetNodeIdOfName(indexName), hInfo);
                }
            }

            recursePrefabs(child, indexName, elementList);
        }
    }


    private static string getVectorStringFiltered(Vector3 vec)
    {
        return vec.ToString().Replace(',', '|').Replace('(', ' ').Replace(')', ' ');
    }
}

    */