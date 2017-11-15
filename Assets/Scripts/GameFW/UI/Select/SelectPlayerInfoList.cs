using Protocol.DTO;
using System.Collections.Generic;
using UnityEngine;

namespace GameFW.UI.Select
{
    /// <summary>
    /// 玩家信息格列表
    /// </summary>
    public class SelectPlayerInfoList:MonoBehaviour
    {
        /// <summary>
        /// 根据一组玩家信息，初始化一组玩家信息格列表
        /// </summary>
        /// <param name="selectDTOs"></param>
        /// <param name="heroHeads"></param>
        /// <returns></returns>
        public Dictionary<int, SelectPlayerInfoGrid> InitialPlayerInfo(SelectDTO[] selectDTOs, Sprite[] heroHeads) {
            Dictionary<int, SelectPlayerInfoGrid> infoGridMap = new Dictionary<int, SelectPlayerInfoGrid>();
            for (int i = 0; i < selectDTOs.Length; i++) {
                Transform grid = transform.GetChild(i);
                SelectDTO select = selectDTOs[i];
                if (grid != null) {
                    SelectPlayerInfoGrid selectPlayerInfoGrid = grid.GetComponent<SelectPlayerInfoGrid>();
                    if (selectPlayerInfoGrid == null) {
                        selectPlayerInfoGrid = grid.gameObject.AddComponent<SelectPlayerInfoGrid>();
                    }
                    selectPlayerInfoGrid.Init(select.name, select.heroId, select.isEnter, select.isReady, heroHeads);
                    if (!infoGridMap.ContainsKey(select.userId)) {
                        infoGridMap.Add(select.userId, selectPlayerInfoGrid);
                    }
                }
            }

            return infoGridMap;
        }
    }
}
