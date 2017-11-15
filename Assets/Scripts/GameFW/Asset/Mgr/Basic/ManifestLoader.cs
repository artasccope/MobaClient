using UnityEngine;

namespace GameFW.Asset.Mgr.Basic
{
    /// <summary>
    /// Manifest文件加载器
    /// </summary>
    public class ManifestLoader : SingleABLoader
    {
        public ManifestLoader(string bundleName) : base(bundleName) { }

        private AssetBundleManifest mainifest = null;

        /// <summary>
        /// 加载完成后，得到manifest
        /// </summary>
        protected override void SelfOnLoadFinished()
        {
            this.mainifest = assetBundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
        }

        /// <summary>
        /// 得到资源依赖
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public string[] GetDependencies(string bundleName) {
            return this.mainifest.GetAllDependencies(bundleName);
        }
    }
}
