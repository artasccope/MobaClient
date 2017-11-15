using GameFW.NetClient.Fight;
using GameFW.NetClient.Login;
using GameFW.NetClient.Match;
using GameFW.NetClient.Select;
using GameFW.NetClient.Time;
using GameFW.NetClient.User;
using Protocol;
using UnityEngine;

namespace GameFW.NetClient
{
    /// <summary>
    /// 网络消息分发中心
    /// </summary>
    public class NetMsgCenter : MonoBehaviour, IHandler
    {
        #region 单例
        private static NetMsgCenter instance;
        private static System.Object obj = new System.Object();
        public static NetMsgCenter Instance
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
                            instance = o.AddComponent<NetMsgCenter>();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion

        private IHandler loginHandler;
        private IHandler userHandler;
        private IHandler selectHandler;
        private IHandler matchHandler;
        private IHandler fightHandler;
        private IHandler timeHandler;

        #region 初始化
        public void Init()
        {
            loginHandler = gameObject.AddComponent<LoginHandler>();
            userHandler = gameObject.AddComponent<UserHandler>();
            selectHandler = gameObject.AddComponent<SelectHandler>();
            matchHandler = gameObject.AddComponent<MatchHandler>();
            fightHandler = gameObject.AddComponent<FightHandler>();
            timeHandler = gameObject.AddComponent<TimeHandler>();
        }

        #endregion

        #region 从网络连接那里拿出消息
        private void Update()
        {
            while (NetCONClient.Instance.HasMessage())
            {
                SocketModel sm = NetCONClient.Instance.GetMessage();
                OnMessageReceived(sm);
            }
        }

        #endregion

        #region 网络消息分发

        /// <summary>
        /// 网络消息分发
        /// </summary>
        /// <param name="sm"></param>
        public void OnMessageReceived(SocketModel sm)
        {
            switch (sm.type)
            {
                case Protocol.Protocol.TYPE_USER:
                    userHandler.OnMessageReceived(sm);
                    break;
                case Protocol.Protocol.TYPE_SELECT:
                    selectHandler.OnMessageReceived(sm);
                    break;
                case Protocol.Protocol.TYPE_MATCH:
                    matchHandler.OnMessageReceived(sm);
                    break;
                case Protocol.Protocol.TYPE_LOGIN:
                    loginHandler.OnMessageReceived(sm);
                    break;
                case Protocol.Protocol.TYPE_FIGHT:
                    fightHandler.OnMessageReceived(sm);
                    break;
                case Protocol.Protocol.TYPE_TIME:
                    timeHandler.OnMessageReceived(sm);
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}
