using GameFW.Core.Msg;
using UnityEngine;
using UnityEngine.UI;

namespace GameFW.UI
{
    /// <summary>
    /// 错误提示界面
    /// </summary>
    public class ErrorUI : UIBase
    {
        private GameObject errorPanel;//错误提示面板
        private Button ensureBtn;//确认按钮

        #region 初始化
        private void Start()
        {
            Initial();
        }

        protected void Initial()
        {
            ensureBtn = GetUIWidgetByWidgetName(UIWidgetNames.ErrorEnsureButtonName).GetComponent<Button>();
            ensureBtn.GetComponent<UIRegister>().AddButtonListener(OnEnsureClicked);
        }
        #endregion

        #region 显示相关
        /// <summary>
        /// 显示错误提示面板
        /// </summary>
        /// <param name="str"></param>
        public void Show(string str)
        {
            if (gameObject.activeSelf == false)
            {
                gameObject.SetActive(true);
                GetUIWidgetByWidgetName(UIWidgetNames.ErrorTextName).GetComponent<Text>().text = str;
            }
        }

        /// <summary>
        /// 隐藏错误提示面板
        /// </summary>
        public void Hide()
        {
            if (gameObject.activeSelf == true)
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 确认按钮按下，隐藏面板
        /// </summary>
        private void OnEnsureClicked()
        {
            Hide();
        }

        #endregion

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="msg"></param>
        public override void ProcessEvent(MsgBase msg)
        {
        }
    }
}
