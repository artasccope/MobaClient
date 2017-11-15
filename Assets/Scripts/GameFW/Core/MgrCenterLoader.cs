using GameFW.Core.Base;
using GameFW.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFW.Core
{
    /// <summary>
    /// 消息中心，这个脚本必须挂到场景中去（一般来说，只用挂这一个脚本就可以了）
    /// </summary>
    public class MgrCenterLoader:MonoBehaviour
    {
        /// <summary>
        /// 唤醒时加载MgrCenter
        /// </summary>
        private void Awake()
        {
            Tools.debuger = ClientDebuger.Instance;
            MgrCenter.Instance.ClearAll();
            MgrCenter.Instance.StartMgrs();
        }

        //private void OnDestroy()
        //{
        //    MgrCenter.Instance.ClearAll();
        //}
    }
}
