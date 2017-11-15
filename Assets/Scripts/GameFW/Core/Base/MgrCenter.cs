using GameFW.Asset.Mgr;
using GameFW.Audio;
using GameFW.Core.Msg;
using GameFW.Entity;
using GameFW.NetClient.Mgr;
using System.Collections.Generic;
using UnityEngine;


namespace GameFW.Core.Base
{
    /// <summary>
    /// 所有的管理类放在这里
    /// </summary>
    public class MgrCenter : MonoBehaviour
    {
        #region 单例
        private static MgrCenter instance;
        private static System.Object obj = new System.Object();
        public static MgrCenter Instance
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
                            instance = o.AddComponent<MgrCenter>();
                            Init();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion

        #region 所有管理类
        public static Mgr<GameObject> UIMgr { get { return uiMgr; } }
        public static EntityMgr EntityMgr { get { return entityMgr; } }
        public static AudioMgr AudioMgr { get { return audioMgr; } }
        public static NetMgr NetMgr { get { return netMgr; } }
        public static AssetMgr AssetMgr { get { return assetMgr; } }

        private static Mgr<GameObject> uiMgr = new Mgr<GameObject>();
        private static EntityMgr entityMgr = new EntityMgr();
        private static NetMgr netMgr = new NetMgr();
        private static AudioMgr audioMgr = new AudioMgr();
        private static AssetMgr assetMgr = new AssetMgr();
        #endregion

        #region 初始化、启动、清空方法
        /// <summary>
        /// 所有管理类的初始化方法
        /// </summary>
        private static void Init()
        {
            uiMgr.MessageType = MsgType.UI;
            entityMgr.MessageType = MsgType.Entity;
            netMgr.MessageType = MsgType.Net;
            audioMgr.MessageType = MsgType.Audio;
            assetMgr.MessageType = MsgType.Asset;
        }

        /// <summary>
        /// 启动管理类
        /// </summary>
        public void StartMgrs()
        {
            assetMgr.Start();
            netMgr.Start();
            audioMgr.Start();
            entityMgr.Start();
            uiMgr.Start();
        }

        /// <summary>
        /// 清空所有管理类
        /// </summary>
        public void ClearAll()
        {
            uiMgr.ClearAll();
            entityMgr.ClearAll();
            netMgr.ClearAll();
            audioMgr.ClearAll();
            assetMgr.ClearAll();
        }

        #endregion

        #region 注册、注销、发送消息

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="msgIds"></param>
        public void RegistMsgs(IMessageProcess mono, List<ushort> msgIds)
        {
            for (int i = 0; i < msgIds.Count; i++)
            {
                switch (MsgBase.GetMsgType(msgIds[i]))
                {
                    case MsgType.UI:
                        uiMgr.RegistMsg(mono, msgIds[i]);
                        break;
                    case MsgType.Net:
                        netMgr.RegistMsg(mono, msgIds[i]);
                        break;
                    case MsgType.Entity:
                        entityMgr.RegistMsg(mono, msgIds[i]);
                        break;
                    case MsgType.Audio:
                        audioMgr.RegistMsg(mono, msgIds[i]);
                        break;
                    case MsgType.Asset:
                        assetMgr.RegistMsg(mono, msgIds[i]);
                        break;
                }
            }
        }

        /// <summary>
        /// 注销消息
        /// </summary>
        public void UnRegistMsgs(IMessageProcess mono, List<ushort> msgIds)
        {
            for (int i = 0; i < msgIds.Count; i++)
            {
                switch (MsgBase.GetMsgType(msgIds[i]))
                {
                    case MsgType.UI:
                        uiMgr.UnRegistMsg(mono, msgIds[i]);
                        break;
                    case MsgType.Net:
                        netMgr.UnRegistMsg(mono, msgIds[i]);
                        break;
                    case MsgType.Entity:
                        entityMgr.UnRegistMsg(mono, msgIds[i]);
                        break;
                    case MsgType.Audio:
                        audioMgr.UnRegistMsg(mono, msgIds[i]);
                        break;
                    case MsgType.Asset:
                        assetMgr.UnRegistMsg(mono, msgIds[i]);
                        break;
                }
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendMsg(MsgBase msg)
        {
            switch (msg.GetMsgType())
            {
                case MsgType.UI:
                    uiMgr.ProcessEvent(msg);
                    break;
                case MsgType.Entity:
                    entityMgr.ProcessEvent(msg);
                    break;
                case MsgType.Player:
                    break;
                case MsgType.Net:
                    netMgr.ProcessEvent(msg);
                    break;
                case MsgType.Game:
                    break;
                case MsgType.AI:
                    break;
                case MsgType.Audio:
                    audioMgr.ProcessEvent(msg);
                    break;
                case MsgType.Asset:
                    assetMgr.ProcessEvent(msg);
                    break;
            }
        }
        #endregion

        #region 应用退出

        /// <summary>
        /// 当应用退出时调用
        /// </summary>
        public void OnApplicationQuit()
        {
            netMgr.CloseConnect();
        }
        #endregion
    }
}
