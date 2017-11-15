using UnityEngine;
using UnityEngine.UI;

namespace GameFW.UI.Fight.HUD
{
    /// <summary>
    /// 实体HUD
    /// </summary>
    public class EntityHUD : MonoBehaviour
    {
        #region 获取相关组件

        private Text nameText;//名字Text
        private Image hpBarImg;//生命值bar的img
        private RectTransform rectTrans;//RectTrans组件

        void Start()
        {
            this.nameText = transform.GetChild(2).GetComponent<Text>();
            this.hpBarImg = transform.GetChild(1).GetComponent<Image>();
            rectTrans = GetComponent<RectTransform>();
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="name"></param>
        /// <param name="percent"></param>
        public void Initial(string name, float percent)
        {
            nameText.name = name;
            UpdateHpBar(percent);
        }
        #endregion

        #region 更新方法

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="pos"></param>
        public void UpdatePos(Vector3 pos)
        {
            rectTrans.position = pos;
        }

        /// <summary>
        /// 更新Up条
        /// </summary>
        /// <param name="percent"></param>
        public void UpdateHpBar(float percent)
        {
            this.hpBarImg.fillAmount = percent;
            if (percent <= 0.3f)
            {
                this.hpBarImg.color = Color.red;
            }
            else if (percent <= 0.6f)
            {
                this.hpBarImg.color = Color.yellow;
            }
            else
            {
                this.hpBarImg.color = Color.green;
            }
        }

        #endregion

        #region 显示、隐藏、清空

        /// <summary>
        /// 显示
        /// </summary>
        public void Show()
        {
            if (transform.GetChild(0).gameObject.activeSelf == false)
            {
                transform.GetChild(0).gameObject.SetActive(true);
                transform.GetChild(1).gameObject.SetActive(true);
                transform.GetChild(2).gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public void Hide()
        {
            if (transform.GetChild(0).gameObject.activeSelf == true)
            {
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(false);
                transform.GetChild(2).gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            this.nameText.text = string.Empty;
            this.hpBarImg.fillAmount = 0f;
        }

        #endregion
    }
}
