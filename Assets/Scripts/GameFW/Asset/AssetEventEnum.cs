using GameFW.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameFW.Asset
{
    /// <summary>
    /// 资源加载卸载事件
    /// </summary>
    public enum AssetLoadEvent {
        LoadRequest = MsgType.Asset + 1,
        BundleLoadProgress,
        BundleLoaded,
        AssetLoadProgress,
        AssetLoaded,
        SceneAssetInstantiated,

        UnloadSingleAsset,
        UnloadBundleAsset,
        UnloadSceneAsset,

        UnloadSingleBundle,
        UnloadSceneBundle,
        UnloadBundles,
        UnloadAllBundle,
        MaxValue,
    }
    /// <summary>
    /// 场景加载事件
    /// </summary>
    public enum SceneLoadEvent {
        LoadScene = AssetLoadEvent.MaxValue,

        MaxValue,
    }
}
