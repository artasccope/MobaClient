using Protocol;
using Protocol.DTO;
using UnityEngine;
using GameFW.Core.Msg;
using GameFW.Core.Base;

namespace GameFW.NetClient.User
{
    /// <summary>
    /// 用户消息处理器
    /// </summary>
    public class UserHandler : NetBase, IHandler
    {
        #region 应用内消息注册、处理
        /// <summary>
        /// 注册msg ids
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Regist();
            ushort[] newMsgIds = new ushort[3] {
                (ushort)NetEventUser.RequestUserInfo,
                (ushort)NetEventUser.CreateUser,
                (ushort)NetEventUser.RequestOnline,
            };
            AddNewMsgIds(newMsgIds);
            RegistSelf();
        }

        /// <summary>
        /// 处理应用内消息
        /// </summary>
        /// <param name="msg"></param>
        public override void ProcessEvent(MsgBase msg)
        {
            switch (msg.MsgId)
            {
                case (ushort)NetEventUser.CreateUser:
                    MsgString msgString = msg as MsgString;
                    Send(Protocol.Protocol.TYPE_USER, 0, UserProtocol.CREATE_CREQ, msgString.Str);
                    break;
                case (ushort)NetEventUser.RequestUserInfo:
                    Send(Protocol.Protocol.TYPE_USER, 0, UserProtocol.INFO_CREQ, null);
                    break;
                case (ushort)NetEventUser.RequestOnline:
                    Send(Protocol.Protocol.TYPE_USER, 0, UserProtocol.ONLINE_CREQ, null);
                    break;
            }
        }
        #endregion

        #region 网络消息处理

        /// <summary>
        /// 处理网络消息
        /// </summary>
        /// <param name="sm"></param>
        public override void OnMessageReceived(SocketModel sm)
        {
            switch (sm.command)
            {
                case UserProtocol.INFO_SRES:
                    UserDTO userDTO = sm.GetMessage<UserDTO>();
                    Debug.LogWarning("get userDTO");
                    if (userDTO == null)
                        OnHasNoUser();
                    else
                        Send(Protocol.Protocol.TYPE_USER, 0, UserProtocol.ONLINE_CREQ, null);
                    break;
                case UserProtocol.CREATE_SRES:
                    bool isCreateSuccess = sm.GetMessage<bool>();
                    if (isCreateSuccess)
                        OnCreateSuccess();
                    else
                        OnCreateFailed();
                    break;
                case UserProtocol.ONLINE_SRES:
                    UserDTO user = sm.GetMessage<UserDTO>();
                    if (user == null)
                        OnlineFailed();
                    else
                        Online(user);
                    break;
            }
        }

        /// <summary>
        /// 网络消息类型
        /// </summary>
        /// <returns></returns>
        public override byte GetType()
        {
            return Protocol.Protocol.TYPE_USER;
        }

        #endregion

        #region 创建角色、创建失败、不存在角色、上线、上线失败

        /// <summary>
        /// 成功创建角色
        /// </summary>
        public void OnCreateSuccess()
        {
            MgrCenter.Instance.SendMsg(Msgs.GetMsgBase((ushort)NetEventUser.CreateUserSuccess));
        }

        /// <summary>
        /// 创建角色失败
        /// </summary>
        public void OnCreateFailed()
        {
            MgrCenter.Instance.SendMsg(Msgs.GetMsgBase((ushort)NetEventUser.CreateUserFailed));
        }

        /// <summary>
        /// 不存在角色
        /// </summary>
        public void OnHasNoUser()
        {
            MgrCenter.Instance.SendMsg(Msgs.GetMsgBase((ushort)NetEventUser.HasNoUser));
        }

        /// <summary>
        /// 上线成功
        /// </summary>
        /// <param name="userDTO"></param>
        public void Online(UserDTO userDTO)
        {
            MgrCenter.Instance.SendMsg(Msgs.GetMsgUserDTO((ushort)NetEventUser.GetUserInfo, userDTO));
        }

        /// <summary>
        /// 上线失败
        /// </summary>
        public void OnlineFailed()
        {
            //TODO 上线失败
            Debug.LogWarning("上线失败");
        }

        #endregion
    }
}
