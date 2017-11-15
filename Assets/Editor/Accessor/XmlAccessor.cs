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

public class XmlAccessor
{
    public static void WriteLoadRecordXml(string fileSavePath, Dictionary<string, AssetLoadInfo> assetLoadInfos)
    {
        if (File.Exists(fileSavePath)) {
            SortedDictionary<string, AssetLoadInfo> prefabNames = GetScenePrefabNames(fileSavePath);
            int count = assetLoadInfos.Count;
            foreach (KeyValuePair<string, AssetLoadInfo> p in prefabNames)
            {
                if (!assetLoadInfos.ContainsKey(p.Key))
                {
                    AssetLoadInfo assetLoadInfo = p.Value;
                    assetLoadInfo.initialOrder = count;
                    assetLoadInfos.Add(p.Key, assetLoadInfo);
                    count++;
                }
            }
            File.Delete(fileSavePath);
        }

        XElement root = new XElement("root");
        foreach (AssetLoadInfo info in assetLoadInfos.Values) {
            Debug.Log("recorded a load element:" + info.prefabABName);
            root.Add(new XElement("LoadItem", new XAttribute("PrefabABPath", info.prefabABName), new XAttribute("OrganizeDataABPath", info.organizeDataABName), new XAttribute("InitialOrder", info.initialOrder), new XAttribute("IsActive", info.activeSelf)));
        }
        root.Save(fileSavePath);
    }

    public static void WriteRuntimePrefabXml(string filePath ,Dictionary<string, RuntimeAssetLoadInfo> assetLoadInfos)
    {
        if (File.Exists(filePath))
        {
            Dictionary<string, RuntimeAssetLoadInfo> runtimePrefabNames = GetRuntimePrefabNames(filePath);

            foreach (KeyValuePair<string, RuntimeAssetLoadInfo> p in runtimePrefabNames)
            {
                if (!assetLoadInfos.ContainsKey(p.Key))
                {
                    assetLoadInfos.Add(p.Key, p.Value);
                }
            }
            File.Delete(filePath);
        }

        XElement root = new XElement("root");
        foreach (RuntimeAssetLoadInfo info in assetLoadInfos.Values)
        {
            root.Add(new XElement("RuntimeLoadItem", new XAttribute("CategoryId", info.categoryId + 1), new XAttribute("SpecieId", info.specieId + 1), new XAttribute("PrefabABPath", info.prefabABName), new XAttribute("ActiveSelf", info.activeSelf)));
        }
        root.Save(filePath);
    }

    private static SortedDictionary<string, AssetLoadInfo> GetScenePrefabNames(string filePath)
    {
        SortedDictionary<string, AssetLoadInfo> prefabs = new SortedDictionary<string, AssetLoadInfo>();
        XmlReader reader = XmlReader.Create(filePath);

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("LoadItem"))
            {
                XElement ele = XElement.ReadFrom(reader) as XElement;
                int initialOrder = int.Parse(ele.Attribute("InitialOrder").Value);
                bool isActive = bool.Parse(ele.Attribute("IsActive").Value);
                string prefabABName = ele.Attribute("PrefabABPath").Value;
                string organizeDataABName = ele.Attribute("OrganizeDataABPath").Value;
                AssetLoadInfo loadItem = new AssetLoadInfo(prefabABName, organizeDataABName, initialOrder, isActive);
                prefabs.Add(loadItem.prefabABName, loadItem);
            }
        }
        reader.Close();

        return prefabs;
    }

    private static Dictionary<string, RuntimeAssetLoadInfo> GetRuntimePrefabNames(string filePath)
    {

        Dictionary<string, RuntimeAssetLoadInfo> prefabs = new Dictionary<string, RuntimeAssetLoadInfo>();
        XmlReader reader = XmlReader.Create(filePath);

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("RuntimeLoadItem"))
            {
                XElement ele = XElement.ReadFrom(reader) as XElement;
                RuntimeAssetLoadInfo runtimeItem = new RuntimeAssetLoadInfo();
                runtimeItem.categoryId = int.Parse(ele.Attribute("CategoryId").Value);
                runtimeItem.specieId = int.Parse(ele.Attribute("SpecieId").Value);
                runtimeItem.prefabABName = ele.Attribute("PrefabABPath").Value;
                runtimeItem.activeSelf = bool.Parse(ele.Attribute("ActiveSelf").Value);
                prefabs.Add(runtimeItem.prefabABName, runtimeItem);
            }
        }

        reader.Close();

        return prefabs;
    }
}
