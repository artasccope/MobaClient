using GameFW.Core.Base;
using GameFW.Core.Msg;
using GameFW.NetClient;
using Protocol.DTO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameFW.UI.User
{
    /// <summary>
    /// 用户UI模块
    /// </summary>
    public class UserUI : UIBase
    {

        #region 应用内消息注册

        public override void Regist()
        {
            base.Regist();
            ushort[] newMsgIds = new ushort[6] {
                (ushort)NetEventUser.HasNoUser,
                (ushort)NetEventUser.GetUserInfo,
                (ushort)NetEventUser.CreateUserFailed,
                (ushort)NetEventUser.CreateUserSuccess,
                (ushort)NetEventMatch.StartMatchSuccess,
                (ushort)NetEventMatch.CancelMatchSuccess,
            };

            AddNewMsgIds(newMsgIds);
            this.RegistSelf();
            //发送请求用户信息的消息
            MgrCenter.Instance.SendMsg(Msgs.GetMsgBase((ushort)NetEventUser.RequestUserInfo));
        }
        #endregion

        #region 初始化
        private void Start()
        {
            Initial();
        }

        private GameObject createUserPanel;//创建角色面板
        private InputField userNameInputField;//角色名称输入框
        private ErrorUI errorPanel;//提示面板

        protected void Initial()
        {
            GetUserInfoUIWidgets();
            GetMatchUIWidgets();

            errorPanel = GetUIWidgetByWidgetName(UIWidgetNames.ErrorPanelName).GetComponent<ErrorUI>();
            createUserPanel = GetUIWidgetByWidgetName(UIWidgetNames.CreateUserPanelName);
            userNameInputField = GetUIWidgetByWidgetName(UIWidgetNames.CreateUserPanelTextName).GetComponent<InputField>();
            //创建角色按钮注册点击事件
            GetUIWidgetByWidgetName(UIWidgetNames.CreateUserEnsureBtnName).GetComponent<UIRegister>().AddButtonListener(OnUserCreateBtnPressed);
        }

        private Text userNameText;//用户名称Text
        private Slider userExpSlider;//用户经验值bar
        private Text userLevelText;//用户等级Text

        /// <summary>
        /// 获得用户信息UI控件
        /// </summary>
        private void GetUserInfoUIWidgets()
        {
            userNameText = GetUIWidgetByWidgetName(UIWidgetNames.InfoPanelNameTextName).GetComponent<Text>();
            userExpSlider = GetUIWidgetByWidgetName(UIWidgetNames.InfoPanelExpSliderName).GetComponent<Slider>();
            userLevelText = GetUIWidgetByWidgetName(UIWidgetNames.InfoPanelLevelTextName).GetComponent<Text>();
        }

        private Image matchBtnImg;//匹配按钮的图片控件
        private Button matchBtn;//匹配按钮
        private Text matchBtnText;//匹配按钮的文本
        private Text matchHintText;//匹配提示的文本
        /// <summary>
        /// 获得匹配UI相关的控件
        /// </summary>
        private void GetMatchUIWidgets()
        {
            matchBtnImg = GetUIWidgetByWidgetName(UIWidgetNames.MatchBtnName).GetComponent<Image>();
            matchBtn = GetUIWidgetByWidgetName(UIWidgetNames.MatchBtnName).GetComponent<Button>();
            matchHintText = GetUIWidgetByWidgetName(UIWidgetNames.MatchTextHintName).GetComponent<Text>();
            matchHintText.text = "";
            matchBtn.GetComponent<UIRegister>().AddButtonListener(OnMatchButtonPressed);
            matchBtnText = GetUIWidgetByWidgetName(UIWidgetNames.MatchBtnTextName).GetComponent<Text>();
        }

        #endregion

        #region 应用内消息处理

        public override void ProcessEvent(MsgBase msg)
        {
            switch (msg.MsgId)
            {
                case (ushort)NetEventUser.HasNoUser://没有角色，显示创建面板
                    ShowCreateUserPanel();
                    break;
                case (ushort)NetEventUser.GetUserInfo://请求角色信息，并更新
                    MsgUserDTO msgUserDTO = msg as MsgUserDTO;
                    UpdateUserInfo(msgUserDTO.userDTO);
                    break;
                case (ushort)NetEventMatch.StartMatchSuccess://开始匹配成功
                    ActiveMatchBtn();
                    break;
                case (ushort)NetEventMatch.CancelMatchSuccess://取消匹配成功
                    ActiveMatchBtn();
                    break;
                case (ushort)NetEventUser.CreateUserFailed://创建角色失败
                    errorPanel.Show("创建角色失败,已存在此角色");
                    break;
                case (ushort)NetEventUser.CreateUserSuccess://创建角色成功
                    HideCreateUserPanel();
                    MgrCenter.Instance.SendMsg(Msgs.GetMsgBase((ushort)NetEventUser.RequestOnline));
                    break;
            }
        }

        #endregion

        #region 角色信息UI相关

        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <param name="userDTO"></param>
        private void UpdateUserInfo(UserDTO userDTO)
        {
            userNameText.text = userDTO.name;
            userLevelText.text = new StringBuilder("等级").Append(userDTO.level).ToString();
            userExpSlider.value = (float)(userDTO.exp / 10000);
        }

        /// <summary>
        /// 显示创建角色面板
        /// </summary>
        private void ShowCreateUserPanel()
        {
            Debug.Log("create user panel");
            if (createUserPanel.activeSelf == false)
            {
                createUserPanel.SetActive(true);
            }
        }

        /// <summary>
        /// 创建角色逻辑
        /// </summary>
        private void OnUserCreateBtnPressed()
        {
            string name = userNameInputField.text;
            if (name.Length <= 0)
            {
                errorPanel.Show("输入名字不能为空");
                return;
            }
            else if (name.Length > 6)
            {
                errorPanel.Show("输入名字长度不能大于6");
                return;
            }

            MgrCenter.Instance.SendMsg(Msgs.GetMsgString((ushort)NetEventUser.CreateUser, name));
            //TODO set mask hint
        }

        /// <summary>
        /// 隐藏创建角色面板
        /// </summary>
        private void HideCreateUserPanel()
        {
            if (createUserPanel.activeSelf == true)
            {
                createUserPanel.SetActive(false);
            }
        }

        #endregion

        #region 匹配相关
        private bool isMatching = false;//是否正在匹配
        /// <summary>
        /// 匹配按钮:匹配和取消匹配
        /// </summary>
        private void OnMatchButtonPressed()
        {
            if (isMatching == false)
            {
                isMatching = true;
                matchHintText.text = "正在匹配游戏\n匹配成功后自动进入\n请等待...".Replace("\\n", "\n");
                matchBtnImg.color = matchBtnImg.color * 0.5f;
                matchBtnText.text = "取消匹配";
                MgrCenter.Instance.SendMsg(Msgs.GetMsgBase((ushort)NetEventMatch.StartMatch));
            }
            else
            {
                isMatching = false;
                matchHintText.text = "";
                matchBtnImg.color = matchBtnImg.color * 2f;
                matchBtnText.text = "开始匹配";
                MgrCenter.Instance.SendMsg(Msgs.GetMsgBase((ushort)NetEventMatch.CancelMatch));
            }
            ForbideMatchBtn();
        }
        /// <summary>
        /// 禁用匹配按钮
        /// </summary>
        private void ForbideMatchBtn()
        {
            if (matchBtn.enabled == true)
            {
                matchBtn.enabled = false;
            }
        }
        /// <summary>
        /// 激活匹配按钮
        /// </summary>
        private void ActiveMatchBtn()
        {
            matchBtn.enabled = true;
        }
        #endregion
    }
}
