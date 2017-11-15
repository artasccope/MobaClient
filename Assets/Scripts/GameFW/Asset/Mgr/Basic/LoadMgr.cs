using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFW.Asset.Mgr.Basic
{

    /// <summary>
    /// 资源加载管理器,全部使用异步加载
    /// </summary>
    public class LoadMgr : MonoBehaviour
    {
        #region 单例
        private static LoadMgr instance;
        private static System.Object obj = new System.Object();
        public static LoadMgr Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            GameObject o = GameObject.Find("mgrGameObject");
                            if (o == null)
                                o = new GameObject("mgrGameObject");
                            instance = o.AddComponent<LoadMgr>();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion
        /// <summary>
        /// 各个包加载器
        /// </summary>
        private Dictionary<string, SingleABLoader> abLoaders = new Dictionary<string, SingleABLoader>();
        /// <summary>
        /// 记录了有直接资源请求、有包依赖的活动包(不应当被卸载)
        /// </summary>
        private HashSet<string> activingABs = new HashSet<string>();
        /// <summary>
        /// 各个包对其他包的依赖记录
        /// </summary>
        private Dictionary<string, HashSet<string>> dependencyOnOthers = new Dictionary<string, HashSet<string>>();
        /// <summary>
        /// 其他包对各个包的依赖记录
        /// </summary>
        private Dictionary<string, HashSet<string>> dependencyOnSelf = new Dictionary<string, HashSet<string>>();
        /// <summary>
        /// manifest加载器
        /// </summary>
        private static ManifestLoader manifestLoader = new ManifestLoader(PathTool.GetPlatformFolderName(Application.platform));

        #region 加载Manifest

        /// <summary>
        /// 开启加载manifest协程
        /// </summary>
        public void LoadManifest()
        {
            StartCoroutine(GetManifest());
        }

        /// <summary>
        /// manifest加载协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator GetManifest()
        {
            if (!manifestLoader.IsLoadFinished)
            {
                if (!manifestLoader.IsLoading)
                    yield return manifestLoader.LoadBundle();
            }
        }

        #endregion

        #region 异步加载资源

        /// <summary>
        /// 异步加载asset,先看有没有，没有就开始加载的携程
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public Object LoadAssetAsync(string bundleName, string assetName)
        {
            //1.如果此资源已经被加载了，那么直接返回
            if (abLoaders.ContainsKey(bundleName) && abLoaders[bundleName].HasLoadedObj(assetName))
                return abLoaders[bundleName].GetObj(assetName);
            //2.有了来自外部的请求，那么这个包就是活动包了
            if (!activingABs.Contains(bundleName))
                activingABs.Add(bundleName);
            //3.开始加载资源协程
            StartCoroutine(LoadAsset(bundleName, assetName));
            return null;
        }

        /// <summary>
        /// 加载资源协程
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        private IEnumerator LoadAsset(string bundleName, string assetName)
        {
            //1.首先检查Manifest是否加载完成
            while (!manifestLoader.IsLoadFinished)
                yield return null;
            //2.Manifest加载完成后，就可以拿到并注册包依赖关系了
            if (!dependencyOnOthers.ContainsKey(bundleName))
                RegisterBundleRely(bundleName, manifestLoader.GetDependencies(bundleName));
            //3.先加载所有依赖的包
            foreach (string rely in dependencyOnOthers[bundleName])
            {
                if (!abLoaders.ContainsKey(rely))
                {
                    SingleABLoader relyABLoader = new SingleABLoader(rely);
                    abLoaders.Add(rely, relyABLoader);
                    yield return relyABLoader.LoadBundle();
                }
            }
            while (!IsPreLoadFinished(dependencyOnOthers[bundleName]))
                yield return null;
            //4.再加载此包
            if (!abLoaders.ContainsKey(bundleName))
            {
                SingleABLoader loader = new SingleABLoader(bundleName);
                abLoaders.Add(bundleName, loader);
                yield return loader.LoadBundle();
            }
            SingleABLoader abLoader = abLoaders[bundleName];
            //5.如果此包还没加载完成、或者正在加载资源、或者已经在加载此资源了，就不要申请加载
            while (!abLoader.IsLoadFinished || abLoader.IsLoadingAsset || abLoader.HasLoadingOrLoadedObj(assetName))
                yield return null;
            //6.最后加载需要的资源
            yield return abLoader.LoadAsset(assetName);
        }

        /// <summary>
        /// 注册依赖关系
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="dependencies"></param>
        private void RegisterBundleRely(string bundleName, string[] dependencies)
        {
            if (!dependencyOnOthers.ContainsKey(bundleName))
                dependencyOnOthers.Add(bundleName, new HashSet<string>());

            for (int i = 0; i < dependencies.Length; i++)
            {
                if (!dependencyOnOthers[bundleName].Contains(dependencies[i]))
                    dependencyOnOthers[bundleName].Add(dependencies[i]);

                if (!dependencyOnSelf.ContainsKey(dependencies[i]))
                    dependencyOnSelf.Add(dependencies[i], new HashSet<string>());

                if (!dependencyOnSelf[dependencies[i]].Contains(bundleName))
                    dependencyOnSelf[dependencies[i]].Add(bundleName);
            }
        }

        /// <summary>
        /// 依赖的所有包是否加载完了
        /// </summary>
        /// <param name="relyBundles"></param>
        /// <returns></returns>
        private bool IsPreLoadFinished(HashSet<string> relyBundles)
        {
            bool finished = true;
            foreach (string rely in relyBundles)
            {
                finished &= (abLoaders.ContainsKey(rely) && abLoaders[rely].IsLoadFinished);
            }
            return finished;
        }

        #endregion

        #region 卸载包

        /// <summary>
        /// 卸载一批包
        /// </summary>
        /// <param name="bundleNames"></param>
        /// <param name="ifWithObjs"></param>
        public void UnloadAssetBundles(string[] bundleNames, bool ifWithObjs)
        {
            for (int i = 0; i < bundleNames.Length; i++)
                UnloadAssetBundle(bundleNames[i], ifWithObjs);
        }

        /// <summary>
        /// 卸载一个包
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="ifWithObjs">是否连此包的资源也一起卸载</param>
        public void UnloadAssetBundle(string bundleName, bool ifWithObjs)
        {
            //1.收到了卸载请求，那么说明它就不是活动的ab了
            if (activingABs.Contains(bundleName))
                activingABs.Remove(bundleName);

            if (abLoaders.ContainsKey(bundleName))
            {
                //2.如果没有活动的ab依赖这个ab，那么才可以卸载
                if (CanBeTruelyDisposed(bundleName))
                {
                    //3.卸载这个包
                    abLoaders[bundleName].UnloadBundle(ifWithObjs);
                    abLoaders.Remove(bundleName);
                    if (dependencyOnOthers.ContainsKey(bundleName))
                    {
                        HashSet<string> relyBundles = dependencyOnOthers[bundleName];

                        foreach (string relyBundle in relyBundles)
                        {
                            //4.对于此ab依赖的ab，如果因此不再活动了，那么也应当卸载
                            if (!activingABs.Contains(relyBundle))
                                UnloadAssetBundle(relyBundle, ifWithObjs);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 一个包是否能被卸载，如果没有活动的ab依赖这个ab，那么才可以卸载
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        private bool CanBeTruelyDisposed(string bundleName)
        {
            if (dependencyOnSelf.ContainsKey(bundleName))
            {
                HashSet<string> relyOnSelfs = dependencyOnSelf[bundleName];
                foreach (string relyOnself in relyOnSelfs)
                {
                    if (activingABs.Contains(relyOnself))
                        return false;
                }
            }

            return true;
        }

        #endregion
    }
}
