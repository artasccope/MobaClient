using Protocol;
using GameFW.Utility;
using UnityEngine;
using GameFW.Core.Msg;
using GameFW.Asset;
using GameFW.Core.Base;

namespace GameFW.NetClient.Match
{
    /// <summary>
    /// 匹配消息处理器
    /// </summary>
    public class MatchHandler : NetBase, IHandler
    {
        #region 应用内消息注册、处理
        /// <summary>
        /// 注册msg ids
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Regist();
            ushort[] newMsgIds = new ushort[2] {
                (ushort)NetEventMatch.StartMatch,
                (ushort)NetEventMatch.CancelMatch,
            };
            AddNewMsgIds(newMsgIds);
            RegistSelf();
        }
        /// <summary>
        /// 应用内消息处理
        /// </summary>
        /// <param name="msg"></param>
        public override void ProcessEvent(MsgBase msg)
        {
            switch (msg.MsgId)
            {
                case (ushort)NetEventMatch.StartMatch:
                    Send(Protocol.Protocol.TYPE_MATCH, 0, MatchProtocol.ENTER_CREQ, null);
                    break;
                case (ushort)NetEventMatch.CancelMatch:
                    Send(Protocol.Protocol.TYPE_MATCH, 0, MatchProtocol.LEAVE_CREQ, null);
                    break;
            }
        }

        #endregion

        #region 网络消息处理

        /// <summary>
        /// 匹配网络消息处理
        /// </summary>
        /// <param name="sm"></param>
        public override void OnMessageReceived(SocketModel sm)
        {
            switch (sm.command)
            {
                case MatchProtocol.ENTER_SRES:
                    MgrCenter.Instance.SendMsg(Msgs.GetMsgBase((ushort)NetEventMatch.StartMatchSuccess));
                    break;
                case MatchProtocol.LEAVE_SRES:
                    MgrCenter.Instance.SendMsg(Msgs.GetMsgBase((ushort)NetEventMatch.CancelMatchSuccess));
                    break;
                case MatchProtocol.ENTER_SELECT_BRO:
                    int teamPlayerCount = sm.GetMessage<int>();

                    PlayerPrefs.SetInt("TeamPlayerCount", teamPlayerCount);
                    MgrCenter.Instance.SendMsg(Msgs.GetMsgString((ushort)SceneLoadEvent.LoadScene, "Select"));
                    break;
            }
        }

        /// <summary>
        /// 网络消息类型
        /// </summary>
        /// <returns></returns>
        public override byte GetType()
        {
            return Protocol.Protocol.TYPE_MATCH;
        }
        #endregion
    }
}
