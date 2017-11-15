using GameFW.Core.Msg;
using GameFW.NetClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameFW.UI.Login
{
    /// <summary>
    /// 登录UI模块
    /// </summary>
    public class LoginUI : UIBase
    {
        private InputField loginAccountInputField;//登录账号输入域
        private InputField loginPasswordInputField;//登录密码输入域
        private GameObject registerPanel;//注册面板
        private ErrorUI errorPanel;//提示面板

        #region 注册、处理消息

        /// <summary>
        /// 注册消息
        /// </summary>
        public override void Regist()
        {
            base.Regist();
            ushort[] newMsgIds = new ushort[1] {
                (ushort)NetEventLogin.LoginRes,
            };

            AddNewMsgIds(newMsgIds);
            this.RegistSelf();
        }

        /// <summary>
        /// 应用内消息处理
        /// </summary>
        /// <param name="msg"></param>
        public override void ProcessEvent(MsgBase msg)
        {
            switch (msg.MsgId)
            {
                case (ushort)NetEventLogin.LoginRes:
                    MsgString msgString = msg as MsgString;
                    errorPanel.Show(msgString.Str);
                    break;
            }
        }

        #endregion

        #region 初始化
        private void Start()
        {
            Initial();
        }

        protected void Initial()
        {
            errorPanel = GetUIWidgetByWidgetName(UIWidgetNames.ErrorPanelName).GetComponent<ErrorUI>();
            registerPanel = GetUIWidgetByWidgetName(UIWidgetNames.RegisterPanelName);
            if (registerPanel == null)
                Debug.Log("register panel is null");
            loginAccountInputField = GetUIWidgetByWidgetName(UIWidgetNames.LoginAccountInputfieldName).GetComponent<InputField>();
            loginPasswordInputField = GetUIWidgetByWidgetName(UIWidgetNames.LoginPasswordInputfieldName).GetComponent<InputField>();

            GetUIWidgetByWidgetName(UIWidgetNames.LoginButtonName).GetComponent<UIRegister>().AddButtonListener(OnLoginClicked);
            GetUIWidgetByWidgetName(UIWidgetNames.RegisterButtonName).GetComponent<UIRegister>().AddButtonListener(OnRegisterClicked);
        }
        #endregion

        #region 登录、注册按钮调用方法

        void OnLoginClicked()
        {
            string account = loginAccountInputField.text;
            string password = loginPasswordInputField.text;

            SendMsg(Msgs.GetMsgAccount((ushort)NetEventLogin.LoginRequest, account, password));
        }

        void OnRegisterClicked()
        {
            if (registerPanel.activeSelf == false)
            {
                registerPanel.SetActive(true);
            }
        }
        #endregion
    }
}
