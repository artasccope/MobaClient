using GameFW.Core.Msg;
using System.Collections.Generic;
using UnityEngine;

namespace GameFW.Core.Base
{
    /// <summary>
    /// 处理消息的模块基类
    /// </summary>
    public abstract class ModuleBase : MonoBehaviour, IMessageProcess
    {
        /// <summary>
        /// 这个模块关注的消息
        /// </summary>
        protected List<ushort> msgIds = new List<ushort>();

        #region 消息类型及消息初始化
        protected MsgType msgType;//消息类型
        /// <summary>
        /// 这个方法在模块被实例化之后会马上调用
        /// </summary>
        public virtual void Regist()
        {
            SetMsgType();
        }

        protected virtual void Awake()
        {
            SetMsgType();
        }

        /// <summary>
        /// 注销自己
        /// </summary>
        protected virtual void OnDestroy()
        {
            UnRegistSelf();
        }

        protected abstract void SetMsgType();

        /// <summary>
        /// 增加新的消息Id到原有消息列表(继承时可用)
        /// </summary>
        /// <param name="newMsgIds"></param>
        protected void AddNewMsgIds(params ushort[] newMsgIds)
        {
            if (this.msgIds == null)
                this.msgIds = new List<ushort>();
            for (int i = 0; i < newMsgIds.Length; i++)
            {
                if (!this.msgIds.Contains(newMsgIds[i]))
                    this.msgIds.Add(newMsgIds[i]);
            }
        }

        #endregion

        #region 注册、注销自己对某些消息的监听

        /// <summary>
        /// 将自己注册到相应的Mgr
        /// </summary>
        public void RegistSelf()
        {
            MgrCenter.Instance.RegistMsgs(this, this.msgIds);
        }

        /// <summary>
        /// 从相应的Mgr中将自己注销
        /// </summary>
        public void UnRegistSelf()
        {
            MgrCenter.Instance.UnRegistMsgs(this, this.msgIds);
        }

        /// <summary>
        /// 将一些消息注册到相应的Mgr
        /// </summary>
        public void RegistMsgs(MsgType msgType, List<ushort> msgIds)
        {
            MgrCenter.Instance.RegistMsgs(this, msgIds);
        }

        /// <summary>
        /// 从相应的Mgr中将一些消息注销
        /// </summary>
        public void UnRegistMsgs(MsgType msgType, List<ushort> msgIds)
        {
            MgrCenter.Instance.UnRegistMsgs(this, msgIds);
        }

        #endregion

        #region 发送、处理消息
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendMsg(MsgBase msg)
        {
            MgrCenter.Instance.SendMsg(msg);
        }

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="msg"></param>
        public abstract void ProcessEvent(MsgBase msg);

        #endregion
    }
}
