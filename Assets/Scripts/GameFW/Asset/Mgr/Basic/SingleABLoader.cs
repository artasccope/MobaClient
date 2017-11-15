using System.Collections;
using System.Text;
using UnityEngine;

namespace GameFW.Asset.Mgr.Basic
{
    /// <summary>
    /// 包加载器
    /// </summary>
    public class SingleABLoader : IUnload
    {
        protected AssetBundle assetBundle;//包
        protected string bundleName;//包名
        protected ABResLoader resLoader;//此包的资源加载器

        protected bool isLoading = false;//是否正在加载
        protected bool isLoadFinished = false;//是否加载结束了
        public bool IsLoadFinished { get { return this.isLoadFinished; } }
        public bool IsLoading { get { return this.isLoading; } }

        #region 构造及释放方法

        public SingleABLoader(string bundleName)
        {
            assetBundle = null;
            this.bundleName = bundleName;
        }

        /// <summary>
        /// 释放这个包
        /// </summary>
        /// <param name="ifWithObj">是否将包加载出的Obj也一起释放</param>
        public void UnloadBundle(bool ifWithObj)
        {
            isLoading = false;
            isLoadFinished = false;
            if (ifWithObj)
            {
                resLoader.Dispose();
                assetBundle.Unload(true);
            }
            else
                assetBundle.Unload(false);

            resLoader = null;
        }

        #endregion

        #region 查询方法

        /// <summary>
        /// 得到此资源
        /// </summary>
        /// <param name="objName"></param>
        /// <returns></returns>
        public UnityEngine.Object GetObj(string objName)
        {
            return resLoader.GetObj(objName);
        }

        /// <summary>
        /// 是否已经加载了此资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public bool HasLoadedObj(string assetName)
        {
            return isLoadFinished && resLoader.HasLoadedObj(assetName);
        }

        /// <summary>
        /// 是否正在或已经加载了此资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public bool HasLoadingOrLoadedObj(string assetName)
        {
            return isLoadFinished && resLoader.HasLoadingOrLoadedObj(assetName);
        }

        /// <summary>
        /// 是否正在加载资源
        /// </summary>
        public bool IsLoadingAsset
        {
            get { return resLoader != null && resLoader.IsLoading; }
        }
        #endregion

        #region 加载
        /// <summary>
        /// 加载此包
        /// </summary>
        /// <returns></returns>
        public IEnumerator LoadBundle()
        {
            WWW bundleLoader = new WWW(PathTool.GetBundlePathByName(bundleName));
            while (!bundleLoader.isDone)
            {
                //TODO MgrCenter.Instance.SendMsg(MgrCenter.AssetMgr.GetBundleLoadProgressMsg((ushort)AssetLoadEvent.AssetLoadProgress, bundleName, bundleLoader.progress));
                isLoading = true;
                yield return null;
            }

            if (string.IsNullOrEmpty(bundleLoader.error) && bundleLoader.progress >= 1.0f)
            {
                //TODO Send load finished msg
                isLoadFinished = true;
                isLoading = false;
                assetBundle = bundleLoader.assetBundle;

                SelfOnLoadFinished();
                //加载资源的loader
                resLoader = new ABResLoader(assetBundle, bundleName);
            }
            else
            {
                Debug.LogError(new StringBuilder("asset bundle load error, bundle name : ").Append(bundleName).Append(bundleLoader.error).ToString());
            }
        }

        protected virtual void SelfOnLoadFinished()
        {
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public IEnumerator LoadAsset(string assetName)
        {
            yield return resLoader.LoadAsset(assetName);
        }

        #endregion
    }
}
