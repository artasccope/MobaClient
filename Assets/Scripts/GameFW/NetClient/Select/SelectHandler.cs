using Protocol;
using Protocol.DTO;
using GameFW.Core.Msg;
using GameFW.Asset;
using GameFW.Core.Base;

namespace GameFW.NetClient.Select
{
    /// <summary>
    /// 选择消息处理器
    /// </summary>
    public class SelectHandler : NetBase, IHandler
    {
        #region 应用内消息注册、处理

        /// <summary>
        /// 注册应用内信息
        /// </summary>
        protected override void Awake()
        {
            playerHasEnter = false;
            base.Awake();
            Regist();
            ushort[] newMsgIds = new ushort[3] {
                (ushort)NetEventSelect.EnterRequest,
                (ushort)NetEventSelect.SelectRequest,
                (ushort)NetEventSelect.ReadyRequest,
            };
            AddNewMsgIds(newMsgIds);
            RegistSelf();
        }

        /// <summary>
        /// 处理应用内信息
        /// </summary>
        /// <param name="msg"></param>
        public override void ProcessEvent(MsgBase msg)
        {
            switch (msg.MsgId)
            {
                case (ushort)NetEventSelect.EnterRequest:
                    Send(SelectProtocol.ENTER_CREQ);
                    break;
                case (ushort)NetEventSelect.SelectRequest:
                    MsgInt msgInt = msg as MsgInt;
                    Send(SelectProtocol.SELECT_CREQ, msgInt.Int);
                    break;
                case (ushort)NetEventSelect.ReadyRequest:
                    Send(SelectProtocol.READY_CREQ);
                    break;
            }
        }

        #endregion

        #region 网络消息处理

        private bool playerHasEnter = false;//玩家是否已经进入选择

        /// <summary>
        /// 选择网络消息处理
        /// </summary>
        /// <param name="sm"></param>
        public override void OnMessageReceived(SocketModel sm)
        {
            switch (sm.command)
            {
                case SelectProtocol.ENTER_SRES:
                    SelectRoomDTO selectRoom = sm.GetMessage<SelectRoomDTO>();
                    playerHasEnter = true;
                    MgrCenter.Instance.SendMsg(Msgs.GetMsgSelectRoom((ushort)NetEventSelect.EnterSres, selectRoom));
                    break;
                case SelectProtocol.ENTER_EXBRO:
                    if (playerHasEnter)
                    {
                        int enterUserId = sm.GetMessage<int>();
                        MgrCenter.Instance.SendMsg(Msgs.GetMsgInt((ushort)NetEventSelect.SomeOneEntered, enterUserId));
                    }
                    break;
                case SelectProtocol.SELECT_BRO:
                    if (playerHasEnter)
                    {
                        SelectDTO selectDTO = sm.GetMessage<SelectDTO>();
                        MgrCenter.Instance.SendMsg(Msgs.GetMsgSelectDTO((ushort)NetEventSelect.SomeOneSelected, selectDTO));
                    }
                    break;
                case SelectProtocol.SELECT_SRES:

                    break;
                case SelectProtocol.READY_BRO:
                    if (playerHasEnter)
                    {
                        SelectDTO select = sm.GetMessage<SelectDTO>();
                        MgrCenter.Instance.SendMsg(Msgs.GetMsgSelectDTO((ushort)NetEventSelect.SomeOneReady, select));
                    }
                    break;
                case SelectProtocol.FIGHT_BRO:
                    if (playerHasEnter)
                    {
                        MgrCenter.Instance.SendMsg(Msgs.GetMsgString((ushort)SceneLoadEvent.LoadScene, "Fight"));
                    }
                    break;
            }
        }

        /// <summary>
        /// 网络消息类型
        /// </summary>
        /// <returns></returns>
        public override byte GetType()
        {
            return Protocol.Protocol.TYPE_SELECT;
        }

        #endregion
    }
}
