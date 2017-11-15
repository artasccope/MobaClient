using System;
using GameFW.Core;
using GameFW.Core.Base;
using GameFW.Core.Msg;
using GameFW.Entity.Driver;
using Protocol.DTO.Fight;
using UnityEngine;

namespace GameFW.Entity.Logic
{
    /// <summary>
    /// 实体的logic模块
    /// </summary>
    public class EntityLogic : ModuleBase
    {

        #region 注册、处理消息
        /// <summary>
        /// 注册消息
        /// </summary>
        public override void Regist()
        {
            base.Regist();
            ushort[] newMsgIds = new ushort[5] {
                (ushort)MoveEvent.PosSync,
                (ushort)SoilderFightEvent.SoilderIdleServerAllow,
                (ushort)SoilderFightEvent.SoilderAtkToPosServerCmd,
                (ushort)MoveEvent.MoveServerRes,
                (ushort)FightEvent.Damage,
            };
            AddNewMsgIds(newMsgIds);
            this.RegistSelf();
        }

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="msg"></param>
        public override void ProcessEvent(MsgBase msg)
        {
            switch (msg.MsgId)
            {
                case (ushort)MoveEvent.PosSync://同步处理
                    PosSync(msg as MsgPosSync);

                    break;
                case (ushort)SoilderFightEvent.SoilderIdleServerAllow://发呆处理
                    Idle(msg as MsgInt);

                    break;
                case (ushort)SoilderFightEvent.SoilderAtkToPosServerCmd://A向某地处理
                    AtkToPos(msg as MsgT<PosDTO>);

                    break;
                case (ushort)MoveEvent.MoveServerRes://服务器返回移动位置处理
                    SmallMove(msg as MsgT<PosDTO>);

                    break;

                case (ushort)FightEvent.Damage:
                    EntityDamage((msg as MsgIntArr).Int);

                    break;
            }
        }

        private void EntityDamage(int[] targetDamage)
        {
            GameObject go = MgrCenter.EntityMgr.GetItem(targetDamage[0]);
            if (go != null && go.GetComponent<FightDriver>() != null) {
                go.GetComponent<FightDriver>().Damage(targetDamage[1], targetDamage[2] == 1);
            }
        }

        #endregion

        #region entity逻辑方法(移动、攻击、发呆)

        /// <summary>
        /// 根据服务器返回的小移动点，执行移动
        /// </summary>
        /// <param name="msgSTar"></param>
        private void SmallMove(MsgT<PosDTO> msgSTar)
        {
            SoilderAgent soilder = GetSoilderAgent(msgSTar.msgData.instanceId);
            if (soilder != null)
            {
                soilder.TargetSmall = new Vector3(msgSTar.msgData.posX, msgSTar.msgData.posY, msgSTar.msgData.posZ);
                soilder.MoveToSmallTarget();
            }
        }

        /// <summary>
        /// 调用A向某地
        /// </summary>
        /// <param name="msgT"></param>
        private void AtkToPos(MsgT<PosDTO> msgT)
        {
            SoilderAgent soilderAgent = GetSoilderAgent(msgT.msgData.instanceId);
            if (soilderAgent != null)
            {
                soilderAgent._set_curTacticAtkPos(new bVector3(msgT.msgData.posX, msgT.msgData.posY, msgT.msgData.posZ));
            }
        }

        /// <summary>
        /// 调用对应的发呆
        /// </summary>
        /// <param name="msgInt"></param>
        private void Idle(MsgInt msgInt)
        {
            GameObject idleEntity = MgrCenter.EntityMgr.GetItem(msgInt.Int);
            if (idleEntity != null)
            {
                SoilderDriver soilderDriver = idleEntity.GetComponent<SoilderDriver>();
                soilderDriver.Idle();
            }
        }

        /// <summary>
        /// 调用对应的位置同步
        /// </summary>
        /// <param name="msgPosSync"></param>
        private void PosSync(MsgPosSync msgPosSync)
        {
            int intanceId = msgPosSync.instanceId;
            GameObject entity = MgrCenter.EntityMgr.GetItem(intanceId);
            if (entity != null)
            {
                PosSyncDriver posSyncDriver = entity.GetComponent<PosSyncDriver>();
                if (posSyncDriver != null)
                {
                    posSyncDriver.PosSync(msgPosSync.pos, msgPosSync.dir, msgPosSync.timeStamp);
                }
            }
        }

        /// <summary>
        /// 得到士兵Agent
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        private SoilderAgent GetSoilderAgent(int instanceId)
        {
            GameObject go = MgrCenter.EntityMgr.GetItem(instanceId);
            if (go != null)
            {
                return go.GetComponent<SoilderAgent>();
            }
            return null;
        }

        #endregion

        protected override void SetMsgType()
        {
            this.msgType = MsgType.Entity;
        }
    }
}
