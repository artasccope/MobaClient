using UnityEngine;
using UnityEngine.UI;

namespace GameFW.UI.Select
{
    /// <summary>
    /// 玩家选择时的信息格
    /// </summary>
    public class SelectPlayerInfoGrid : MonoBehaviour
    {

        private Image playerHeadImg;//玩家头像Image
        private Text playerName;//玩家名称文本
        private Text playerStatus;//玩家的状态文本
        private Sprite[] heroHeads;//可用的玩家头像Sprites

        #region 获取组件、初始化
        void Start()
        {
            playerHeadImg = transform.GetChild(2).GetComponent<Image>();
            playerName = transform.GetChild(0).GetComponent<Text>();
            playerStatus = transform.GetChild(1).GetComponent<Text>();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="name"></param>
        /// <param name="heroId"></param>
        /// <param name="isEnter"></param>
        /// <param name="isReady"></param>
        /// <param name="heroHeads"></param>
        public void Init(string name, int heroId, bool isEnter, bool isReady, Sprite[] heroHeads)
        {
            this.heroHeads = heroHeads;
            playerName.text = name;
            if (heroId <= 0 || heroId > heroHeads.Length)
            {
                playerHeadImg.sprite = null;
                playerHeadImg.color = Color.clear;
            }
            else
            {
                playerHeadImg.sprite = heroHeads[heroId - 1];
                playerHeadImg.color = Color.white;
            }

            if (isEnter == false)
            {
                playerStatus.text = "未进入";
                return;
            }
            if (isReady == false)
            {
                playerStatus.text = "选择中...";
            }
            else
            {
                playerStatus.text = "就绪";
                playerHeadImg.color = Color.green * 0.75f;
            }
        }

        #endregion

        #region 更新信息
        /// <summary>
        /// 根据是否进入更新信息格
        /// </summary>
        /// <param name="isEnter"></param>
        public void UpdateInfo(bool isEnter)
        {

            if (isEnter == false)
            {
                playerStatus.text = "未进入";
                return;
            }

            playerStatus.text = "选择中...";
        }

        /// <summary>
        /// 根据是否准备好更新信息格
        /// </summary>
        /// <param name="isReady"></param>
        public void UpdateStatusInfo(bool isReady)
        {
            if (isReady)
            {
                playerStatus.text = "就绪";
                playerHeadImg.color = Color.green * 0.75f;
            }
            else
            {
                playerStatus.text = "未准备";
                playerHeadImg.color = Color.white;
            }
        }

        /// <summary>
        /// 更新玩家所选的英雄头像Sprite
        /// </summary>
        /// <param name="index"></param>
        public void UpdateSprite(int index)
        {
            playerHeadImg.sprite = heroHeads[index - 1];
            playerHeadImg.color = Color.white;
        }

        #endregion
    }
}