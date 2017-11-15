using GameFW.Core;
using GameFW.Core.Base;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFW.UI
{
    /// <summary>
    /// UI基础模块
    /// </summary>
    public abstract class UIBase : ModuleBase
    {

        protected override void SetMsgType()
        {
            this.msgType = MsgType.UI;
        }

        /// <summary>
        /// 通过UI名字获得UI控件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject GetUIWidgetByWidgetName(string name) {
            return MgrCenter.UIMgr.GetItem((SceneManager.GetActiveScene().name + name).GetHashCode());
        }
    }
}
