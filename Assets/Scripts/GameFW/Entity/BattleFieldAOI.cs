using System.Collections.Generic;
using UnityEngine;
using GameFW.AOI;
using CommonTools;

namespace GameFW.Entity
{
    /// <summary>
    /// 战场AOI
    /// </summary>
    public class BattleFieldAOI
    {
        #region 单例
        private static readonly BattleFieldAOI instance = new BattleFieldAOI();

        public static BattleFieldAOI Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        #region 构造及初始化
        private BattleFieldAOI() { }

        private Iaoi aoiCenter = null;

        /// <summary>
        /// 初始化AOI
        /// </summary>
        /// <param name="gridSize"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Init(float gridSize, int width, int height)
        {
            aoiCenter = new HashAOICenter(gridSize, width, height);
        }

        #endregion

        #region 增、删、改

        /// <summary>
        /// 增加entity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        public void AddEntity(int id, Vector3 pos)
        {
            if (!aoiCenter.ContainsKey(id))
            {
                aoiCenter.AddEntity(id, ref pos);
            }
        }

        /// <summary>
        /// 移除entity
        /// </summary>
        /// <param name="id"></param>
        public void RemoveEntity(int id)
        {
            if (aoiCenter.ContainsKey(id))
            {
                aoiCenter.RemoveEntity(id);
            }
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        public void UpdatePos(int id, Vector3 pos)
        {
            aoiCenter.UpdatePos(id, ref pos);
        }

        #endregion

        #region 获取最近的entity

        /// <summary>
        /// 获取最近的entity列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public List<RBTree<int, Vector3>> GetNearEntities(int id, Vector3 pos, float range)
        {
            return aoiCenter.GetInterestEntities(id, ref pos, range);
        }

        /// <summary>
        /// 获取最近的entity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public int GetNearestEntity(int id, Vector3 pos, float range)
        {
            int entityId = id;
            float sqrMagnitude = Mathf.Infinity;
            foreach (RBTree<int, Vector3> entities in aoiCenter.GetInterestEntities(id, ref pos, range))
            {
                for (RBTree<int, Vector3>.RBNode node = entities.First; entities.HasNext(node); node = entities.MoveNext(node))
                {
                    if (node.key != id && Vector3.SqrMagnitude(pos - node.value) < sqrMagnitude)
                    {
                        entityId = node.key;
                    }
                }
            }

            return entityId;
        }


        #endregion
    }
}
