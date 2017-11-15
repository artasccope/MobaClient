using GameFW.Core.Msg;
using System.Collections.Generic;
using UnityEngine;

namespace GameFW.Core.Base
{
    /// <summary>
    /// 管理类的基类，提供消息集中处理的功能
    /// </summary>
    public class Mgr<T> : IMessageProcess,IGetObj
    {
        #region 构造、初始化与清空

        public Mgr() {
            eventTree = new Dictionary<ushort, HashSet<IMessageProcess>>[8];
            for (int i = 0; i < 8; i++) {
                eventTree[i] = new Dictionary<ushort, HashSet<IMessageProcess>>();
            }
        }

        public virtual void Start() { }

        /// <summary>
        /// 清空消息处理和对象
        /// </summary>
        public virtual void ClearAll()
        {
            ClearItems();
            ClearMsgs();
        }

#endregion

        #region 消息处理相关
        /// <summary>
        /// 这个消息树将《一种消息，关注这种消息的MonoBase》在这里注册,使得可以集中处理(一般一种mgr只关注自己种类的消息,比如UIMgr只关注MsgType==MsgType.UI的消息)
        /// 优先级有0-7  八个等级
        /// </summary>
        protected Dictionary<ushort, HashSet<IMessageProcess>>[] eventTree;
        protected MsgType msgType;//管理类对应的消息类型
        public MsgType MessageType { set { this.msgType = value; } }
        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="msg"></param>
        public virtual void ProcessEvent(MsgBase msg)
        {
            bool hasProcessor = false;

            for (int i = 0; i < eventTree.Length; i++)
            {
                if (eventTree[i] != null && eventTree[i].ContainsKey(msg.MsgId))
                {
                    hasProcessor = true;
                    foreach (IMessageProcess mono in eventTree[i][msg.MsgId])
                    {
                        mono.ProcessEvent(msg);
                    }
                }
            }

            if(!hasProcessor)
                Debug.LogError("msg isn't existed in eventTree, msgId == " + msg.MsgId + ", MsgType == " + msg.GetMsgType());
        }

        /// <summary>
        /// 查询一个消息是否有监听者
        /// </summary>
        /// <param name="msgId"></param>
        /// <returns></returns>
        public bool HasMgrListeners(ushort msgId)
        {
            bool hasListeners = false;
            for (int i = 0; i < eventTree.Length; i++)
            {
                if (eventTree[i] != null)
                {
                    if (eventTree[i].ContainsKey(msgId))
                    {
                        hasListeners = true;
                        break;
                    }
                }
            }

            return hasListeners;
        }

        #endregion

        #region 消息的注册、注销

        /// <summary>
        /// 不带优先级,那么就按最差情况处理，优先级为7
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="msgId"></param>
        /// <param name="priority"></param>
        public void RegistMsg(IMessageProcess mono, ushort msgId)
        {
            RegistMsg(mono, msgId, 7);
        }

        /// <summary>
        /// 注册一个消息,带优先级
        /// </summary>
        /// <param name="mono">关注msgId的Mono</param>
        /// <param name="msgId">消息id</param>
        public void RegistMsg(IMessageProcess mono, ushort msgId, byte priority)
        {
            priority = GetLegalPriority(priority);
            if (eventTree[priority] == null)
                eventTree[priority] = new Dictionary<ushort, HashSet<IMessageProcess>>();

            if (!eventTree[priority].ContainsKey(msgId))
            {
                HashSet<IMessageProcess> set = new HashSet<IMessageProcess>();
                eventTree[priority].Add(msgId, set);
            }

            if (!eventTree[priority][msgId].Contains(mono))
            {
                eventTree[priority][msgId].Add(mono);
                Debug.Log("mono registered, msgId == " + msgId);
            }
        }

        /// <summary>
        /// 得到合法的优先级(0-7)
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        private byte GetLegalPriority(byte priority)
        {
            if (priority > 7)
                return 7;
            else
                return priority;
        }

        /// <summary>
        /// 注销mono对msgId的关注，不带优先级，按优先级7处理
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="msgId"></param>
        public void UnRegistMsg(IMessageProcess mono, ushort msgId)
        {
            UnRegistMsg(mono, msgId, 7);
        }

        /// <summary>
        /// 注销mono对msgId的关注
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="msgId"></param>
        public void UnRegistMsg(IMessageProcess mono, ushort msgId, byte priority)
        {
            priority = GetLegalPriority(priority);
            if (eventTree[priority] != null && eventTree[priority].ContainsKey(msgId) &&  eventTree[priority][msgId].Contains(mono))
                eventTree[priority][msgId].Remove(mono);
        }

        /// <summary>
        /// 对一个脚本注册一组消息
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="msgIds"></param>
        public void RegistMsgs(IMessageProcess mono, params ushort[] msgIds)
        {
            for (int i = 0; i < msgIds.Length; i++)
            {
                RegistMsg(mono, msgIds[i]);
            }
        }

        /// <summary>
        /// 注销mono对一组消息的关注
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="msgIds"></param>
        public void UnRegistMsgs(IMessageProcess mono, params ushort[] msgIds)
        {
            for (int i = 0; i < msgIds.Length; i++)
            {
                UnRegistMsg(mono, msgIds[i]);
            }
        }

        /// <summary>
        /// 清空消息
        /// </summary>
        public void ClearMsgs()
        {
            for (int i = 0; i < eventTree.Length; i++) {
                eventTree[i].Clear();
            }
        }

        #endregion

        #region item管理相关

        /// <summary>
        /// id和item的dic
        /// </summary>
        private Dictionary<int, T> subUIGameObjects = new Dictionary<int, T>();

        /// <summary>
        /// 得到Item
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public virtual T GetItem(int instanceId)
        {
            if (subUIGameObjects.ContainsKey(instanceId))
                return subUIGameObjects[instanceId];
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// 注册对象
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="obj"></param>
        public virtual void RegistItem(int instanceId, T obj)
        {
            if (!subUIGameObjects.ContainsKey(instanceId))
                subUIGameObjects.Add(instanceId, obj);
        }

        /// <summary>
        /// 注销对象
        /// </summary>
        /// <param name="instanceId"></param>
        public virtual void UnRegistItem(int instanceId)
        {
            if (!subUIGameObjects.ContainsKey(instanceId))
                subUIGameObjects.Remove(instanceId);
        }

        /// <summary>
        /// 清空对象
        /// </summary>
        public virtual void ClearItems()
        {
            subUIGameObjects.Clear();
        }



        #endregion

        #region 获取对象相关

        protected Dictionary<ushort, HashSet<IGetObj>>[] getObjEventTree;

        public object GetObj(MsgBase msg)
        {
            bool hasProcessor = false;
            object retObj = null;
            object tmpObj = null;

            for (int i = 0; i < getObjEventTree.Length; i++)
            {
                if (getObjEventTree[i] != null && getObjEventTree[i].ContainsKey(msg.MsgId))
                {
                    hasProcessor = true;
                    foreach (IGetObj mono in eventTree[i][msg.MsgId])
                    {
                        tmpObj = mono.GetObj(msg);
                        if (tmpObj != null)
                            retObj = tmpObj;
                    }
                }
            }

            if (!hasProcessor)
                Debug.LogError("msg isn't existed in obj eventTree, msgId == " + msg.MsgId + ", MsgType == " + msg.GetMsgType());

            return retObj;
        }

#endregion
    }
}
