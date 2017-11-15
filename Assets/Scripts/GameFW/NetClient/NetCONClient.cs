using System;
using System.Collections.Generic;
using System.Net.Sockets;
using GameFW.Net;
using GameFW.Utility;
using Protocol;
using UnityEngine;

namespace GameFW.NetClient
{
    /// <summary>
    /// 客户端使用的异步连接类,有一个UserToken,并且是单例,所有的网络交互从这里走
    /// </summary>
    public class NetCONClient : NetCONBase
    {
        static NetCONClient()
        {

        }

        #region 客户端上用的是单例
        public static NetCONClient Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        private class Nested
        {
            static Nested() { }

            internal readonly static NetCONClient instance = new NetCONClient();
        }


        #endregion
        /// <summary>
        /// 接收到并解码出的可用消息队列（MonoBehaviour可以通过单例直接来拿）
        /// </summary>
        private Queue<SocketModel> receivedQueue = new Queue<SocketModel>();
        /// <summary>
        /// 先建一个UserToken,设置UserToken的一些方法,再连接,再开始接收
        /// </summary>

        #region 构造器、连接初始化
        protected NetCONClient() : base()
        {
        }

        public void Start()
        {
            if (!socket.Connected)
            {
                Connect();
            }
        }

        #endregion

        /// <summary>
        /// 连接Socket
        /// </summary>
        private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        #region 连接与关闭连接
        /// <summary>
        /// 客户端连接到服务器
        /// </summary>
        /// <returns></returns>
        private bool Connect()
        {
            try
            {
                if (!socket.Connected)
                    socket.Connect("192.168.31.218", 6630);
                Tools.debuger.Log("连接服务器成功");

                token.Connection = socket;//在这里将新建的socket传进去
                token.StartReceive();
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("连接服务器失败,错误信息:" + e.Message);
                return false;
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseConnect()
        {
            if (socket.Connected)
            {
                socket.Close();
            }
        }

        /// <summary>
        /// 关闭连接的回调
        /// </summary>
        /// <param name="token"></param>
        /// <param name="error"></param>
        protected override bool CloseCON(UserToken token, string error)
        {
            return false;
        }

        #endregion

        /// <summary>
        /// 消息发送成功的回调
        /// </summary>
        /// <param name="e"></param>
        protected override void MessageSended(SocketAsyncEventArgs e)
        {
        }

        public bool HasMessage()
        {
            return receivedQueue.Count > 0;
        }

        /// <summary>
        /// 接收到消息后的处理，这里直接放入队列
        /// </summary>
        /// <param name="token"></param>
        /// <param name="message"></param>
        protected override void MessageProcess(UserToken token, byte[] message)
        {
            receivedQueue.Enqueue(MessageEncoder.Decode(message));
        }

        #region 拿出消息
        /// <summary>
        /// 拿出一个可用的消息
        /// </summary>
        /// <returns></returns>
        public SocketModel GetMessage()
        {
            if (receivedQueue.Count == 0)
            {
                Tools.debuger.Log("没有新收的消息,请在调用前先用HasMessage检查消息是否存在.");
                return null;
            }

            return receivedQueue.Dequeue();
        }
        #endregion
    }
}
