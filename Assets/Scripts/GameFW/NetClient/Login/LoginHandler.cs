using Protocol;
using Protocol.Result;
using GameFW.Utility;
using GameFW.Core.Msg;
using GameFW.Asset;
using GameFW.Core.Base;

namespace GameFW.NetClient.Login
{
    /// <summary>
    /// 登录消息处理器
    /// </summary>
    public class LoginHandler : NetBase, IHandler
    {

        #region 应用内消息的注册与处理
        /// <summary>
        /// 注册msg ids
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Regist();
            ushort[] newMsgIds = new ushort[2] {
                (ushort)NetEventLogin.LoginRequest,
                (ushort)NetEventRegister.RegisterRequest,
            };
            AddNewMsgIds(newMsgIds);
            RegistSelf();

        }

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="msg"></param>
        public override void ProcessEvent(MsgBase msg)
        {
            switch (msg.MsgId)
            {
                case (ushort)NetEventLogin.LoginRequest:
                    MsgAccount accountMsg = msg as MsgAccount;
                    AccountInfoDTO accountDTO = new AccountInfoDTO();
                    accountDTO.account = accountMsg.Account;
                    accountDTO.password = accountMsg.Password;

                    Tools.debuger.Log("申请登录.");
                    Send(Protocol.Protocol.TYPE_LOGIN, 0, (int)LoginProtocol.LOGIN_CREQ, accountDTO);
                    break;
                case (ushort)NetEventRegister.RegisterRequest:
                    MsgAccount msgAccount = msg as MsgAccount;
                    AccountInfoDTO dtoAccount = new AccountInfoDTO();
                    dtoAccount.account = msgAccount.Account;
                    dtoAccount.password = msgAccount.Password;

                    Tools.debuger.Log("申请新建一个账号.");
                    Send(Protocol.Protocol.TYPE_LOGIN, 0, (int)LoginProtocol.REG_CREQ, dtoAccount);
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
                case LoginProtocol.REG_SRES:
                    int resMsg = sm.GetMessage<int>();
                    string regResHint = GetAccountResHint(resMsg);
                    MgrCenter.Instance.SendMsg(Msgs.GetMsgString((ushort)NetEventRegister.RegisterRes, regResHint));

                    break;
                case LoginProtocol.LOGIN_SRES:
                    int loginMsg = sm.GetMessage<int>();
                    if (loginMsg == (int)AccountResult.LoginSuccess)
                        MgrCenter.Instance.SendMsg(Msgs.GetMsgString((ushort)SceneLoadEvent.LoadScene, "Match"));

                    break;
            }
        }

        /// <summary>
        /// 根据服务器返回值打开提示对话框
        /// 提示对话框不是一个好设计
        /// </summary>
        /// <param name="resMsg"></param>
        /// <returns></returns>
        private string GetAccountResHint(int resMsg)
        {
            switch (resMsg)
            {
                case (int)AccountResult.HasAccountCantCreate:
                    return "已经存在同名账号了，无法创建！";
                case (int)AccountResult.CreateSuccess:
                    return "账号创建成功！";
                case (int)AccountResult.AccountNotExistedCantLogin:
                    return "账号不存在，无法登录！";
                case (int)AccountResult.AccountPwdNotMatch:
                    return "密码不正确，无法登录！";
                case (int)AccountResult.LoginSuccess:
                    return "登录成功!";
                case (int)AccountResult.AlreadyOnlineCantLogin:
                    return "该账号已经在线上了，无法上线！";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 网络消息类型
        /// </summary>
        /// <returns></returns>
        public override byte GetType()
        {
            return Protocol.Protocol.TYPE_LOGIN;
        }

        #endregion
    }
}
