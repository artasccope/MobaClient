using GameFW.Core.Base;
using GameFW.Core.Msg;
using GameFW.NetClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameFW.UI
{
    /// <summary>
    /// 注册UI模块
    /// </summary>
    public class RegisterUI : UIBase
    {

        private GameObject registerPanel;//注册面板
        private InputField registerAccountInputField;//注册账号输入域
        private InputField registerPwdInputField;//注册密码输入域
        private InputField registerPwdAgainInputField;//注册密码重复输入域
        private ErrorUI errorPanel;//提示面板

        #region 应用内消息注册、处理
        /// <summary>
        /// 消息注册
        /// </summary>
        public override void Regist()
        {
            base.Regist();
            ushort[] newMsgIds = new ushort[1] {
                (ushort)NetEventRegister.RegisterRes,
            };

            AddNewMsgIds(newMsgIds);
            RegistSelf();
        }

        /// <summary>
        /// 消息处理
        /// </summary>
        /// <param name="msg"></param>
        public override void ProcessEvent(MsgBase msg)
        {
            switch (msg.MsgId)
            {
                case (ushort)NetEventRegister.RegisterRes:
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

        /// <summary>
        /// 拿到各个组件
        /// </summary>
        protected void Initial()
        {
            errorPanel = GetUIWidgetByWidgetName(UIWidgetNames.ErrorPanelName).GetComponent<ErrorUI>();
            registerPanel = GetUIWidgetByWidgetName(UIWidgetNames.RegisterPanelName);
            registerAccountInputField = GetUIWidgetByWidgetName(UIWidgetNames.RegisterAccountInputfieldName).GetComponent<InputField>();
            registerPwdInputField = GetUIWidgetByWidgetName(UIWidgetNames.RegisterPasswordInputfieldName).GetComponent<InputField>();
            registerPwdAgainInputField = GetUIWidgetByWidgetName(UIWidgetNames.RegisterPwdAgainInputfieldName).GetComponent<InputField>();

            GetUIWidgetByWidgetName(UIWidgetNames.RegisterPanelRegisterButtonName).GetComponent<UIRegister>().AddButtonListener(OnRegisterClicked);
            GetUIWidgetByWidgetName(UIWidgetNames.RegisterQuitButtonName).GetComponent<UIRegister>().AddButtonListener(OnQuitClicked);
        }

        #endregion

        #region 注册、退出注册按钮功能
        /// <summary>
        /// 注册按钮
        /// </summary>
        void OnRegisterClicked()
        {
            string accountStr = registerAccountInputField.text;
            string pwdStr = registerPwdInputField.text;
            string pwdAgainStr = registerPwdAgainInputField.text;

            if (string.IsNullOrEmpty(accountStr))
            {
                errorPanel.Show("账号不能为空!");
                return;
            }
            else if (accountStr.Length < 6)
            {
                errorPanel.Show("账号字符不能少于6个!");
                return;
            }
            if (string.IsNullOrEmpty(pwdStr))
            {
                errorPanel.Show("密码不能为空!");
                return;
            }
            else if (pwdStr.Length < 8 || pwdStr.Length > 20)
            {
                errorPanel.Show("密码长度必须在8-20之间");
                return;
            }
            else if (pwdStr != pwdAgainStr)
            {
                errorPanel.Show("重复密码不一致!");
                return;
            }

            SendMsg(Msgs.GetMsgAccount((ushort)NetEventRegister.RegisterRequest, accountStr, pwdStr));
        }

        /// <summary>
        /// 退出注册按钮
        /// </summary>
        void OnQuitClicked()
        {
            if (registerPanel.activeSelf == true)
            {
                registerPanel.SetActive(false);
            }
        }
        #endregion

    }
}