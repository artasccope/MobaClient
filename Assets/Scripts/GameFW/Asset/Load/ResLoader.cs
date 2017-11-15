using GameFW.Asset.Mgr.Basic;
using GameFW.Core.Base;
using GameFW.Core.Msg;
using Object = UnityEngine.Object;

namespace GameFW.Asset.Load
{
    /// <summary>
    /// 资源加载器，接受加载事件
    /// </summary>
    public class ResLoader : AssetBase
    {
        /// <summary>
        /// 注册加载事件
        /// </summary>
        protected virtual void Start()
        {
            base.Regist();
            ushort[] newMsgIds = new ushort[9] {
                (ushort)AssetLoadEvent.LoadRequest,
                (ushort)AssetLoadEvent.AssetLoaded,
                (ushort)AssetLoadEvent.UnloadSingleAsset,
                (ushort)AssetLoadEvent.UnloadBundleAsset,
                (ushort)AssetLoadEvent.UnloadSceneAsset,

                (ushort)AssetLoadEvent.UnloadBundles,
                (ushort)AssetLoadEvent.UnloadSingleBundle,
                (ushort)AssetLoadEvent.UnloadSceneBundle,
                (ushort)AssetLoadEvent.UnloadAllBundle,
            };

            AddNewMsgIds(newMsgIds);
            RegistSelf();
        }

        /// <summary>
        /// 处理加载事件
        /// </summary>
        /// <param name="msg"></param>
        public override void ProcessEvent(MsgBase msg)
        {
            switch (msg.MsgId) {
                case (ushort)AssetLoadEvent.LoadRequest://请求资源

                    MsgAssetLoadRequest msgLoadReq = msg as MsgAssetLoadRequest;
                    Object obj = MgrCenter.AssetMgr.LoadAsset(msgLoadReq.bundleName, msgLoadReq.assetName);
                    //说明已经加载完成了，就直接返回给请求者
                    if(obj != null)
                        MgrCenter.Instance.SendMsg(Msgs.GetMsgAssetLoaded(
                            (ushort)AssetLoadEvent.AssetLoaded, msgLoadReq.bundleName, msgLoadReq.assetName, obj));

                    break;
                case (ushort)AssetLoadEvent.AssetLoaded://资源加载完成后的处理

                    MsgAssetLoaded msgAssetLoaded = msg as MsgAssetLoaded;
                    OnObjLoaded(msgAssetLoaded.GetAssetAndABName(), msgAssetLoaded.asset);

                    break;
                case (ushort)AssetLoadEvent.UnloadSingleAsset:
                    break;
                case (ushort)AssetLoadEvent.UnloadBundleAsset:
                    break;
                case (ushort)AssetLoadEvent.UnloadSceneAsset:
                    break;
                case (ushort)AssetLoadEvent.UnloadBundles://卸载所有bundle

                    MsgUnloadAssetBundles msgUnloadAssetBundles = msg as MsgUnloadAssetBundles;
                    LoadMgr.Instance.UnloadAssetBundles(msgUnloadAssetBundles.bundleNames, msgUnloadAssetBundles.ifUnloadObjs);

                    break;
                case (ushort)AssetLoadEvent.UnloadSingleBundle:
                    break;
                case (ushort)AssetLoadEvent.UnloadSceneBundle:
                    break;
                case (ushort)AssetLoadEvent.UnloadAllBundle:
                    break;
            }
        }
    }
}
