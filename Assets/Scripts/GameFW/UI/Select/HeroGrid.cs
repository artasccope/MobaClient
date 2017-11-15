using GameFW.Core.Base;
using GameFW.Core.Msg;
using UnityEngine;
using UnityEngine.UI;

namespace GameFW.UI.Select
{
    /// <summary>
    /// 英雄选择列表里的选择Grid
    /// </summary>
    public class HeroGrid : MonoBehaviour
    {
        private int heroIndex;//此Grid对应的英雄索引
        public int HeroIndex { set { this.heroIndex = value; } }

        /// <summary>
        /// 注册按钮事件
        /// </summary>
        private void Start()
        {
            gameObject.GetComponent<UIRegister>().AddButtonListener(OnHeroPresses);
        }

        /// <summary>
        /// 按下时选择英雄
        /// </summary>
        private void OnHeroPresses()
        {
            MgrCenter.Instance.SendMsg(Msgs.GetMsgInt((ushort)SelectUIEvent.HeroPressed, this.heroIndex));
        }


        #region 显示相关
        /// <summary>
        /// 设置此Grid的英雄头像
        /// </summary>
        /// <param name="sprite"></param>
        public void SetSprite(Sprite sprite)
        {
            if (transform.childCount > 0)
            {
                Transform childSprite = transform.GetChild(0);
                Image img = childSprite.GetComponent<Image>();
                if (img != null)
                {
                    img.sprite = sprite;
                }
            }
        }
        /// <summary>
        /// 放大
        /// </summary>
        public void ZoomUp()
        {
            transform.localScale = Vector3.one * 1.25f;
        }
        /// <summary>
        /// 变为原先状态
        /// </summary>
        public void ZoomDown()
        {
            transform.localScale = Vector3.one;
        }
        #endregion
    }
}
