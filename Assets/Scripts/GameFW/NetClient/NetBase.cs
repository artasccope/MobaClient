using GameFW.Core;
using GameFW.Core.Base;
using Protocol;
using System.Collections.Generic;

namespace GameFW.NetClient
{
    /// <summary>
    /// 网络模块基类
    /// </summary>
    public abstract class NetBase : ModuleBase
    {
        #region 网络消息体socketModel的设置相关
        protected MessageEncode mEncode;//消息编码
        public void SetMessageEncode(MessageEncode encode)
        {
            this.mEncode = encode;
        }
        private byte type;//此模块对应的消息类型
        private int area;//区域号

        public void SetType(byte type)
        {
            this.type = type;
        }

        public abstract new byte GetType();


        public void SetArea(int area)
        {
            this.area = area;
        }

        public virtual int GetArea()
        {
            return area;
        }
        #endregion

        #region 通过连接对象发送
        public void Send(int command)
        {
            Send(GetArea(), command, null);
        }

        public void Send(int command, object message)
        {
            Send(GetType(), GetArea(), command, message);
        }

        public void Send(int area, int command, object message)
        {
            Send(GetType(), area, command, message);
        }

        public void Send(byte type, int area, int command, object message)
        {
            MgrCenter.NetMgr.Send(mEncode(NewSocketModel(type, area, command, message)));
        }

        /// <summary>
        /// 创建相应的消息包
        /// </summary>
        /// <param name="type"></param>
        /// <param name="area"></param>
        /// <param name="command"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private SocketModel NewSocketModel(byte type, int area, int command, object message)
        {
            return new SocketModel(type, area, command, message);
        }
        #endregion

        #region 模块相关
        protected override void Awake()
        {
            base.Awake();
            SetMessageEncode(MessageEncoder.Encode);
        }

        protected override void SetMsgType()
        {
            this.msgType = MsgType.Net;
        }
        #endregion

        #region 网络消息缓存处理
        /// <summary>
        /// 消息缓存
        /// </summary>
        protected HashSet<SocketModel> smCache = new HashSet<SocketModel>();

        protected void CacheSocketModel(SocketModel sm)
        {
            if (!smCache.Contains(sm))
            {
                smCache.Add(sm);
            }
        }

        protected bool HasCacheSocketModel()
        {
            return smCache.Count > 0;
        }

        public abstract void OnMessageReceived(SocketModel sm);

        protected void ProcessCacheSM()
        {
            foreach (SocketModel sm in smCache)
            {
                OnMessageReceived(sm);
            }
            smCache.Clear();

            //TODO 开协程来处理
        }
        #endregion
    }
}
