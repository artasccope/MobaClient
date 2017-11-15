using GameFW.Asset.Load;
using GameFW.Asset.Mgr.Basic;
using GameFW.Core.Base;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace GameFW.Asset.Mgr
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    public class AssetMgr : Mgr<Object>
    {
        public AssetMgr() : base() { }

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        public override void Start()
        {
            //1.读取本场景资源记录
            ReadRecords();
            //2.读取Manifest
            LoadMgr.Instance.LoadManifest();
            //3.初始化场景资源加载
            SceneLoader.Instance.Init();
        }
        #endregion

        #region 读取资源记录
        private HashSet<string> assetNames = new HashSet<string>();//资源名称记录:包和资源的对应

        /// <summary>
        /// 读取本场景资源记录
        /// </summary>
        private void ReadRecords()
        {
            string recordFileName = PathTool.GetRecordFileName(SceneManager.GetActiveScene().name);

            FileStream fs = new FileStream(recordFileName, FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            string l;
            while ((l = sr.ReadLine()) != null)
            {
                if (!assetNames.Contains(l))
                    assetNames.Add(l);
            }
            sr.Close();
            fs.Close();
            sr.Dispose();
            fs.Dispose();
        }

        /// <summary>
        /// 得到本场景所有资源的记录
        /// </summary>
        /// <returns></returns>
        public HashSet<string> GetSceneAssets()
        {
            return assetNames;
        }

        #endregion

        #region 加载资源
        /// <summary>
        /// 根据包名和资源名加载资源
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public Object LoadAsset(string bundleName, string assetName)
        {
            return LoadMgr.Instance.LoadAssetAsync(bundleName, assetName);
        }

        #endregion
    }
}
