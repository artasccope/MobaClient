using System;
using GameFW.Core.Base;
using GameFW.Core.Msg;
using GameFW.UI;
using GameFW.Ultility;
using Protocol.DTO.Fight.Instance;
using UnityEngine;
using Protocol.DTO.Fight;

namespace GameFW.Entity.Driver
{
    /// <summary>
    /// 战斗单位驱动类
    /// </summary>
    public class FightDriver : MonoBehaviour
    {
        protected AbsFightInstance fightInstance;

        public virtual void Initial(AbsFightInstance buildingInstance)
        {
            this.fightInstance = buildingInstance;
            MgrCenter.Instance.SendMsg(Msgs.GetMsgHUD((ushort)HUDEvent.CreateHUD, fightInstance.instanceId, transform.position, fightInstance.name, 1f));
        }

        #region 属性

        public ModelType GetModelType() {
            return (ModelType)fightInstance.fightModel.category;
        }

        #endregion

        #region HUD通知相关

        protected Renderer thisRenderer;//渲染器

        protected virtual void Start()
        {
            thisRenderer = GetComponent<Renderer>();
        }

        protected virtual void Update()
        {
            //有renderer、在视锥内、活的
            if (thisRenderer != null)
            {
                if (thisRenderer.IsVisibleFrom(Camera.main) && fightInstance.hp > 0)
                {
                    MgrCenter.Instance.SendMsg(Msgs.GetMsgEntityPos((ushort)HUDEvent.UpdateHUDPos, fightInstance.instanceId, transform.position));
                }
                else
                {
                    MgrCenter.Instance.SendMsg(Msgs.GetMsgInt((ushort)HUDEvent.HideHUD, fightInstance.instanceId));
                }
            }
        }
        #endregion

        /// <summary>
        /// 清空以回收
        /// </summary>
        public void Clear()
        {
            MgrCenter.Instance.SendMsg(Msgs.GetMsgInt((ushort)HUDEvent.HideHUD, fightInstance.instanceId));
        }

        #region 战斗单位通用方法

        /// <summary>
        /// 显示伤害
        /// </summary>
        /// <param name="damageCount"></param>
        /// <param name="isAlive"></param>
        public void Damage(int damageCount, bool isAlive)
        {
            fightInstance.hp -= damageCount;
            float percent = fightInstance.hp / fightInstance.fightModel.maxHp;
            MgrCenter.Instance.SendMsg(Msgs.GetMsgIntFloat((ushort)HUDEvent.UpdateHUDHp, fightInstance.instanceId, percent));
        }

        private void Dead()
        {

        }

        #endregion
    }
}
