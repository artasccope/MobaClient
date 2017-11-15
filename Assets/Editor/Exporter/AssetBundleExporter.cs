using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using GameFW;

public class AssetBundleBuilder
{
    private static Dictionary<string, List<string>> assetABRefs = new Dictionary<string, List<string>>();

    [MenuItem("Build/Build AssetBundle Android _%#&k")]
    public static void BuildAndroidAssetBundle() {
        BuildAssetBundle(BuildTarget.Android);
    }

    [MenuItem("Build/Build AssetBundle Windows")]
    public static void BuildWindowsAssetBundle()
    {
        BuildAssetBundle(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Build/Build AssetBundle iOS")]
    public static void BuildIOSAssetBundle()
    {
        BuildAssetBundle(BuildTarget.iOS);
    }

    public static void BuildAssetBundle(BuildTarget buildTarget)
    {
        MarkAssetBundle();

        string outPath = Path.Combine(PathTool.GetAppFilePath(), EditorPaths.GetBuildFolderName(buildTarget));
        if (!Directory.Exists(outPath))
            Directory.CreateDirectory(outPath);

        BuildPipeline.BuildAssetBundles(outPath, 0, buildTarget);

        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Mark AssetBundle _%#k")]
    public static void MarkAssetBundle()
    {
        AssetDatabase.RemoveUnusedAssetBundleNames();
        string path = PathTool.ResourcesPath();
        DirectoryInfo dir = new DirectoryInfo(path);
        FileSystemInfo[] fileSystemInfo = dir.GetFileSystemInfos();
        for (int i = 0; i < fileSystemInfo.Length; i++)
        {
            if (fileSystemInfo[i] is DirectoryInfo)
            {
                MarkSceneABs(fileSystemInfo[i].FullName, fileSystemInfo[i].Name);
            }
        }

        AssetDatabase.Refresh();
    }

    private static void MarkSceneABs(string sceneFolderPath, string sceneName)
    {
        assetABRefs.Clear();
        DirectoryInfo dir = new DirectoryInfo(sceneFolderPath);
        MarkPath(dir);
        RecordAssetToTxt(PathTool.GetRecordFileName(sceneName));
    }

    private static void RecordAssetToTxt(string recordFileName)
    {
        if (File.Exists(recordFileName))
            File.Delete(recordFileName);

        FileStream fs = new FileStream(recordFileName, FileMode.OpenOrCreate);
        StreamWriter bw = new StreamWriter(fs);

        foreach (KeyValuePair<string, List<string>> p in assetABRefs)
        {
            foreach (string asset in p.Value)
            {
                string l = p.Key + " " + asset;

                bw.WriteLine(l);
            }
        }
        bw.Close();
        fs.Close();
    }

    private static void MarkPath(DirectoryInfo dir)
    {
        FileSystemInfo[] fileInfos = dir.GetFileSystemInfos();

        for (int i = 0; i < fileInfos.Length; i++)
        {
            FileSystemInfo file = fileInfos[i];

            if (file is DirectoryInfo)
            {
                MarkPath(file as DirectoryInfo);
            }
            else if (IsFileUsable(file))
            {
                MarkAssetBundle(FixedWindowsPath(dir.FullName), file);
            }
        }
    }

    private static void MarkAssetBundle(string path, FileSystemInfo fileInfo)
    {
        string bundleName = GetAssetBundleName(path).Substring(1);

        string fullPath = fileInfo.FullName;
        int assetCount = fullPath.IndexOf("Assets");

        string assetPath = fullPath.Substring(assetCount, fullPath.Length - assetCount);
        AssetImporter importer = AssetImporter.GetAtPath(assetPath);

        importer.assetBundleName = bundleName;

        if (fileInfo.Extension == ".unity")
        {
            importer.assetBundleVariant = "u3d";
        }
        else
        {
            importer.assetBundleVariant = "ld";
        }

        Debug.Log("file name == " + fileInfo.Name);
        Debug.Log("bundle name == " + bundleName);

        RecordToDic(bundleName.ToLower(), fileInfo.Name.ToLower());
    }

    public static void RecordToDic(string bundleName, string fileName)
    {
        if (!assetABRefs.ContainsKey(bundleName))
            assetABRefs.Add(bundleName, new List<string>());

        if (!assetABRefs[bundleName].Contains(fileName))
            assetABRefs[bundleName].Add(fileName);
    }

    public static string FixedWindowsPath(string path)
    {
        return path.Replace("\\", "/");
    }

    private static bool IsFileUsable(FileSystemInfo fileInfo)
    {
        string extension = fileInfo.Extension;
        return extension != ".meta";
    }

    private static string GetAssetBundleName(string path)
    {
        string resPath = PathTool.ResourcesPath();
        if (path.Contains(resPath))
        {
            return path.Substring(path.IndexOf(resPath) + resPath.Length);
        }
        else
        {
            Debug.LogError("path don't contain resources Path!");
            return null;
        }
    }

    //TODO clear assetBundle
    //TODO build player
    //TODO Read config
    //TODO Write binary
}