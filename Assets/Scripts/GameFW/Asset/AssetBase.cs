using GameFW.Core;
using GameFW.Core.Base;
using GameFW.Core.Msg;
using GameFW.Ultility;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace GameFW.Asset
{
    /// <summary>
    /// 当请求的资源加载完成时的回调
    /// </summary>
    /// <param name="abAndAssetName"></param>
    /// <param name="obj"></param>
    public delegate void RequesterOnObjLoaded(string abAndAssetName, Object obj);
    /// <summary>
    /// 资源模块基类
    /// </summary>
    public abstract class AssetBase : ModuleBase
    {
        #region 加载完成的回调声明、设置
        protected RequesterOnObjLoaded requesterOnObjLoaded;//加载完成回调

        public void SetOnObjLoaded(RequesterOnObjLoaded requesterOnObjLoaded)
        {
            this.requesterOnObjLoaded = requesterOnObjLoaded;
        }
        #endregion

        #region 资源加载请求与加载完成后的回调方法

        protected Dictionary<string, int> loadRequest = new Dictionary<string, int>();
        /// <summary>
        /// 加载资源申请
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="assetName"></param>
        public void LoadObjRequest(string bundleName, string assetName)
        {
            //↓发送加载资源请求给Asset Bundle的load模块
            MgrCenter.Instance.SendMsg(Msgs.GetMsgAssetLoadRequest((ushort)AssetLoadEvent.LoadRequest, bundleName, assetName));
            string uniqueName = NameTool.GetUniqueAssetStr(bundleName, assetName);
            //↓记录有多少个针对此资源的加载调用 
            if (!loadRequest.ContainsKey(uniqueName))
                loadRequest.Add(uniqueName, 1);
            else
                loadRequest[uniqueName]++;
        }

        /// <summary>
        /// 加载完成后，根据字典记录的数目进行回调调用
        /// </summary>
        /// <param name="abAndAssetName"></param>
        /// <param name="obj"></param>
        public virtual void OnObjLoaded(string abAndAssetName, Object obj)
        {
            if (loadRequest.ContainsKey(abAndAssetName))
            {
                for (int i = 0; i < loadRequest[abAndAssetName]; i++)
                {
                    requesterOnObjLoaded(abAndAssetName, obj);
                }
                loadRequest.Remove(abAndAssetName);
            }
        }
        #endregion

        protected override void SetMsgType()
        {
            this.msgType = MsgType.Asset;
        }
    }
}
