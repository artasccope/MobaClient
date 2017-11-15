using CommonTools;
using GameFW.Core.Base;
using GameFW.Entity.Driver;
using Protocol.DTO.Fight;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFW.Entity
{
    /// <summary>
    /// Entity管理器
    /// </summary>
    public class EntityMgr : Mgr<GameObject>
    {
        #region 初始化、清空
        /// <summary>
        /// 初始化AOI和entity loader
        /// </summary>
        public override void Start()
        {
            if (SceneManager.GetActiveScene().name == "Fight")
            {
                BattleFieldAOI.Instance.Init(25f, 10, 10);
                EntityLoader.Instance.Init();
            }
        }

        public override void ClearAll()
        {
            base.ClearAll();
            //TODO
        }

        #endregion

        #region 注册Entity到AOI、获取AOI信息

        /// <summary>
        /// 注册一个Entity到AOI里
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="obj"></param>
        public void RegistItemToAOI(int instanceId, GameObject obj)
        {
            BattleFieldAOI.Instance.AddEntity(instanceId, obj.transform.position);
        }

        /// <summary>
        /// 更新AOI
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="pos"></param>
        public void UpdateAOIPos(int instanceId, Vector3 pos) {
            BattleFieldAOI.Instance.UpdatePos(instanceId, pos);
        }

        /// <summary>
        /// 获取最近的Entity
        /// </summary>
        /// <param name="queryEntityId"></param>
        /// <param name="queryEntityPos"></param>
        /// <param name="range"></param>
        /// <param name="selfLayer"></param>
        /// <returns></returns>
        public GameObject GetNearestEnemy(int queryEntityId, Vector3 queryEntityPos, float range, int selfLayer)
        {
            int nearestEnemyId = queryEntityId;
            List<RBTree<int, Vector3>> nearestEntities = BattleFieldAOI.Instance.GetNearEntities(queryEntityId, queryEntityPos, range);

            GameObject go;
            float sqrMagnitude = Mathf.Infinity;
            float tmpSqrMagnitude = 0;
            foreach (RBTree<int, Vector3> entities in nearestEntities)
            {
                for (RBTree<int, Vector3>.RBNode node = entities.First; entities.HasNext(node); node = entities.MoveNext(node))
                {
                    go = GetItem(node.key);

                    if (go != null && go.layer != selfLayer)
                    {
                        tmpSqrMagnitude = Vector3.SqrMagnitude(go.transform.position - queryEntityPos);
                        if (tmpSqrMagnitude < sqrMagnitude)
                        {
                            sqrMagnitude = tmpSqrMagnitude;
                            nearestEnemyId = node.key;
                        }
                    }
                }
            }

            return (nearestEnemyId == queryEntityId || range * range < sqrMagnitude) ? null : GetItem(nearestEnemyId);
        }


        /// <summary>
        /// 获取范围内的单位数
        /// </summary>
        /// <param name="queryEntityId">查询者的id</param>
        /// <param name="queryEntityPos">查询的地点</param>
        /// <param name="range">查询范围</param>
        /// <param name="selfLayer">自己的layer</param>
        /// <param name="isFriend">是否为友方</param>
        /// <returns></returns>
        public int GetPartEntityCountInRange(int queryEntityId, Vector3 queryEntityPos, float range, int selfLayer, bool isFriend)
        {
            int entityCount = 0;
            List<RBTree<int, Vector3>> nearestEntities = BattleFieldAOI.Instance.GetNearEntities(queryEntityId, queryEntityPos, range);

            GameObject go;
            foreach (RBTree<int, Vector3> entities in nearestEntities)
            {
                for (RBTree<int, Vector3>.RBNode node = entities.First; entities.HasNext(node); node = entities.MoveNext(node))
                {
                    go = GetItem(node.key);

                    if (go != null)
                    {
                        if (isFriend && (go.layer == selfLayer))
                            entityCount++;
                        else if ((!isFriend) && (go.layer != selfLayer))
                            entityCount++;
                    }
                }
            }

            return entityCount;
        }

        /// <summary>
        /// 获取最近的敌方英雄数量
        /// </summary>
        /// <param name="queryEntityId"></param>
        /// <param name="queryEntityPos"></param>
        /// <param name="range"></param>
        /// <param name="selfLayer"></param>
        /// <returns></returns>
        public int GetPartHeroCountInRange(int queryEntityId, Vector3 queryEntityPos, float range, int selfLayer, bool isFriend)
        {
            int heroCount = 0;
            List<RBTree<int, Vector3>> nearestEntities = BattleFieldAOI.Instance.GetNearEntities(queryEntityId, queryEntityPos, range);

            GameObject go;
            FightDriver fightDriver;
            foreach (RBTree<int, Vector3> entities in nearestEntities)
            {
                for (RBTree<int, Vector3>.RBNode node = entities.First; entities.HasNext(node); node = entities.MoveNext(node))
                {
                    go = GetItem(node.key);

                    if (go != null && ((fightDriver = go.GetComponent<FightDriver>()) != null) &&
                            fightDriver.GetModelType() == ModelType.Hero)
                    {

                        if (isFriend && go.layer == selfLayer)
                            heroCount++;
                        else if ((!isFriend && go.layer != selfLayer))
                            heroCount++;
                    }
                }
            }

            return heroCount;
        }

        #endregion
    }
}
