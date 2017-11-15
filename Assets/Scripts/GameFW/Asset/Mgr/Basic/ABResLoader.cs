using GameFW.Core.Base;
using GameFW.Core.Msg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFW.Asset.Mgr.Basic
{
    /// <summary>
    /// 包的资源加载器
    /// </summary>
    public class ABResLoader : IDisposable
    {
        private AssetBundle ab;//包
        private string bundleName;//包名
        private Dictionary<string, Object> objs;//对应的资源
        private List<string> objsLoadingAndLoaded;//正在加载以及已经加载完成的资源

        #region 构造与清空
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ab"></param>
        /// <param name="bundleName"></param>
        public ABResLoader(AssetBundle ab, string bundleName)
        {
            this.ab = ab;
            this.bundleName = bundleName;
            objs = new Dictionary<string, Object>();
            objsLoadingAndLoaded = new List<string>();
        }

        /// <summary>
        /// 清空方法
        /// </summary>
        public void Dispose()
        {
            objs = null;
        }
        #endregion

        #region 查询方法

        /// <summary>
        /// 是否加载完成此名称的资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public bool HasLoadedObj(string assetName)
        {
            return objs.ContainsKey(assetName);
        }

        /// <summary>
        /// 是否正在加载或已经加载此资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public bool HasLoadingOrLoadedObj(string assetName)
        {
            return objsLoadingAndLoaded.Contains(assetName);
        }

        /// <summary>
        /// 获取资源
        /// </summary>
        /// <param name="objName"></param>
        /// <returns></returns>
        public Object GetObj(string objName)
        {
            return objs[objName];
        }

        #endregion

        #region 异步加载资源

        private bool isLoading;//是否正在加载
        public bool IsLoading
        {
            get { return isLoading; }
        }
        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public IEnumerator LoadAsset(string assetName)
        {
            AssetBundleRequest resReq = ab.LoadAssetAsync(assetName);
            objsLoadingAndLoaded.Add(assetName);
            while (!resReq.isDone)
            {
                isLoading = true;
                yield return resReq.progress;
            }

            isLoading = false;
            objs.Add(assetName, resReq.asset);
            //↓发送加载完成消息
            MgrCenter.Instance.SendMsg(Msgs.GetMsgAssetLoaded((ushort)AssetLoadEvent.AssetLoaded, bundleName, assetName, resReq.asset));
        }
#endregion
    }
}
