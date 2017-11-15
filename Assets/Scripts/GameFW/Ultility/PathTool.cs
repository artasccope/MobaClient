using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFW
{
    /// <summary>
    /// 返回系统相关的各种路径
    /// </summary>
    public class PathTool
    {
        /// <summary>
        /// Behaviac的导出文件的路径
        /// </summary>
        public static string ExportedFilePath
        {
            get
            {
                string relativePath = "/Resources/behaviac/exported";

                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    return Application.dataPath + relativePath;
                }
                else if (Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    return Application.dataPath + relativePath;
                }
                else
                {
                    return "Assets" + relativePath;
                }
            }
        }

        /// <summary>
        /// 加载记录文件的所在路径
        /// </summary>
        /// <returns></returns>
        public static string GetLoadRecordFilePath()
        {
            return new StringBuilder(Application.dataPath).Append("/Resources/").Append(SceneManager.GetActiveScene().name).Append("_loadRecord.xml").ToString();
        }

        /// <summary>
        /// 记录实时加载prefab信息的xml文件路径
        /// </summary>
        /// <returns></returns>
        public static string RuntimePrefabXmlPath()
        {
            return new StringBuilder(ResourcesPath()).Append('/').Append(SceneManager.GetActiveScene().name).Append("_runtimePrefabRecord.xml").ToString();
        }

        /// <summary>
        /// 资源路径
        /// </summary>
        /// <returns></returns>
        public static string ResourcesPath()
        {
            return Application.dataPath + "/Resources";
        }

        /// <summary>
        /// 场景对应的文件路径
        /// </summary>
        /// <returns></returns>
        public static string SceneResourcesPath()
        {
            return new StringBuilder("Assets/Resources/").Append(SceneManager.GetActiveScene().name).ToString();
        }

        /// <summary>
        /// 对应平台上导出AssetBundle的路径
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static string GetAssetBundleLoadPath(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "jar:file://" + Application.dataPath + "!/assets/";
                case RuntimePlatform.IPhonePlayer:
                    return Application.dataPath + "/Raw/";
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return new StringBuilder("file://").Append(Application.dataPath).Append("/StreamingAssets/").Append(GetPlatformFolderName(platform)).Append("/").ToString();

                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "OSX";

                default:
                    return null;
            }
        }

        /// <summary>
        /// 平台对应的文件夹名字
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static string GetPlatformFolderName(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return "Windows";

                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "OSX";

                default:
                    return null;
            }
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        /// <returns></returns>
        public static string GetAppFilePath()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                return Application.streamingAssetsPath;
            }
            else
            {
                return Application.persistentDataPath;
            }
        }

        /// <summary>
        /// AssetBundle路径
        /// </summary>
        /// <returns></returns>
        public static string GetAssetBundlePath()
        {
            return new StringBuilder(GetAppFilePath()).Append("/").Append(GetPlatformFolderName(Application.platform)).ToString();
        }

        /// <summary>
        /// 当前场景对应的资源记录路径
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static string GetRecordFileName(string sceneName)
        {
            return new StringBuilder(GetAssetBundlePath()).Append("/").Append(sceneName).Append("_record.txt").ToString();
        }

        /// <summary>
        /// 组织数据文件的路径
        /// </summary>
        /// <returns></returns>
        public static string GetOrganizeDataPath()
        {
            return new StringBuilder("Assets/Resources/").Append(SceneManager.GetActiveScene().name).Append("/OrganizeData/").ToString();
        }

        /// <summary>
        /// 组织数据文件对应的包的路径
        /// </summary>
        /// <returns></returns>
        public static string GetOrganizeDataABName()
        {
            return SceneManager.GetActiveScene().name + "/OrganizeData/";
        }

        /// <summary>
        /// 根据包名得到包的路径
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public static string GetBundlePathByName(string bundleName)
        {
            return GetAssetBundleLoadPath(Application.platform) + bundleName;
        }
    }
}
