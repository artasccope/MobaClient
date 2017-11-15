using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFW.UI.Select
{
    /// <summary>
    /// 英雄选择表格
    /// </summary>
    public class HeroSelectGridBox : MonoBehaviour
    {
        #region 初始化
        /// <summary>
        /// 初始化英雄选择表格
        /// </summary>
        /// <param name="heroIndexes"></param>
        /// <param name="heroSprites"></param>
        public void InstantiateGrids(HashSet<int> heroIndexes, Sprite[] heroSprites)
        {
            this.heroSps = heroSprites;
            GameObject original = transform.GetChild(0).gameObject;
            Instantiate(original, heroIndexes);
        }

        private int left = 12;//水平起始位置
        private int up = -8;//垂直起始位置
        private Sprite[] heroSps;//英雄对应的Sprite
        /// <summary>
        /// 记录id对应的HeroGrid
        /// </summary>
        private Dictionary<int, HeroGrid> heroSelects = new Dictionary<int, HeroGrid>();

        /// <summary>
        /// 初始化单个格子
        /// </summary>
        /// <param name="heroGridOrigin"></param>
        /// <param name="heroIndexes"></param>
        private void Instantiate(GameObject heroGridOrigin, HashSet<int> heroIndexes)
        {
            foreach (int i in heroIndexes)
            {
                GameObject heroGrid = GameObject.Instantiate(heroGridOrigin) as GameObject;
                heroGrid.transform.parent = transform;
                heroGrid.name = i.ToString();
                RectTransform rec = heroGrid.GetComponent<RectTransform>();
                rec.anchoredPosition = new Vector2(left + 65 * ((i - 1) % 6), up - 65 * Mathf.CeilToInt((i - 1) / 6));

                rec.anchorMax = new Vector2(0.5f, 0.5f);
                rec.anchorMin = new Vector2(0.5f, 0.5f);
                rec.pivot = new Vector2(0.5f, 0.5f);
                rec.localScale = Vector3.one;
                rec.localEulerAngles = Vector3.zero;

                HeroGrid gridScript = heroGrid.GetComponent<HeroGrid>();
                gridScript.SetSprite(heroSps[i - 1]);
                gridScript.HeroIndex = i;
                if (!heroSelects.ContainsKey(i))
                {
                    heroSelects.Add(i, gridScript);
                }
            }
        }
#endregion

        #region 表格更新
        /// <summary>
        /// 根据选择状态更新英雄选择表格
        /// </summary>
        /// <param name="heroCouldSelected"></param>
        public void UpdateSelectHeroGrids(HashSet<int> heroCouldSelected)
        {
            foreach (KeyValuePair<int, HeroGrid> p in heroSelects)
            {
                if (!heroCouldSelected.Contains(p.Key))
                {
                    ForbideHeroGrid(p.Key);
                }
                else
                {
                    AllowHeroGrid(p.Key);
                }
            }
        }

        /// <summary>
        /// 禁止选择一个英雄
        /// </summary>
        /// <param name="heroIndex"></param>
        public void ForbideHeroGrid(int heroIndex)
        {
            if (heroSelects.ContainsKey(heroIndex))
            {
                Debug.LogWarning("禁止了英雄" + heroIndex);
                heroSelects[heroIndex].GetComponent<Button>().enabled = false;
            }
        }

        /// <summary>
        /// 允许选择一个英雄
        /// </summary>
        /// <param name="heroIndex"></param>
        public void AllowHeroGrid(int heroIndex)
        {
            if (heroSelects.ContainsKey(heroIndex))
            {
                Debug.LogWarning("解禁了英雄" + heroIndex);
                heroSelects[heroIndex].GetComponent<Button>().enabled = true;
            }
        }

        private int lastSelectHero = -1;//最后选择的英雄

        /// <summary>
        /// 当某个英雄被选择时UI更新
        /// </summary>
        /// <param name="index"></param>
        public void UIOnSomeHeroSelected(int index)
        {
            if (index != lastSelectHero)
            {
                if (heroSelects.ContainsKey(lastSelectHero))
                {
                    heroSelects[lastSelectHero].ZoomDown();
                }
                if (heroSelects.ContainsKey(index))
                {
                    heroSelects[index].ZoomUp();
                }
                lastSelectHero = index;
            }
        }
#endregion
    }
}