using GameFW.Core.Base;
using Protocol.DTO.Fight.Instance;
using System;
using UnityEngine;

namespace GameFW.Entity.Driver
{
    /// <summary>
    /// 防御塔驱动类
    /// </summary>
    public class DefenseTowerDriver : FightDriver
    {
        private Transform bullet;//防御塔发射的子弹
        private Vector3 originPos;//子弹初始位置
        private Transform curTarget = null;//当前的攻击目标
        private BuildingInstance buildingInstance;//防御塔数据类

        #region 初始化
        private void Start()
        {
            bullet = transform.GetChild(0);
            bullet.gameObject.SetActive(true);
            originPos = bullet.localPosition;
            isAttacking = false;
            atkTimeStamp = DateTime.Now.Ticks;
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="buildingInstance"></param>
        public override void Initial(AbsFightInstance buildingInstance)
        {
            base.Initial(buildingInstance);
            this.buildingInstance = buildingInstance as BuildingInstance;
            isAttacking = false;
        }

        #endregion

        #region 攻击、停止
        // id
        public int Id { get { return this.buildingInstance.instanceId; } }
        // 攻击范围
        public float AtkRange { get { return this.buildingInstance.atkRange; } }
        // 攻击速度
        public float AtkSpeed { get { return this.buildingInstance.atkSpeed; } }
        private bool isAttacking;//是否正在攻击
        private long atkTimeStamp;//上一次攻击的时间戳
        /// <summary>
        /// 收到攻击指令后根据id和攻击时间戳(这个时间戳已经换算到本地)进行播放
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="atkTimeStamp"></param>
        public virtual void Attack(int targetId, long atkTimeStamp)
        {
            this.atkTimeStamp = atkTimeStamp;
            GameObject tar = MgrCenter.EntityMgr.GetItem(targetId);
            if (tar != null)
            {
                curTarget = tar.transform;
                isAttacking = true;
            }
            else
                curTarget = null;
        }

        /// <summary>
        /// 停止攻击
        /// </summary>
        public virtual void Stop()
        {
            isAttacking = false;
            curTarget = null;
        }

        #endregion

        #region 子弹位置插值
        /// <summary>
        /// 子弹位置插值
        /// </summary>
        private void FixedUpdate()
        {
            if (isAttacking)
            {
                if ((DateTime.Now.Ticks - atkTimeStamp) * 0.0000001f >= fightInstance.atkSpeed * 0.66f)
                {
                    bullet.localPosition = originPos;
                }
                else if ((DateTime.Now.Ticks - atkTimeStamp) * 0.0000001f >= fightInstance.atkSpeed)
                {
                    atkTimeStamp = DateTime.Now.Ticks;
                }
                else
                {
                    bullet.position = Vector3.Lerp(bullet.position, curTarget.transform.position, ((DateTime.Now.Ticks - atkTimeStamp) * 0.0000001f) / (fightInstance.atkSpeed * 0.66f));
                }
            }
        }
        #endregion
    }
}
