using System;
using Protocol;
using Protocol.DTO.Fight;
using Protocol.DTO.Fight.Instance;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameFW.Core.Msg;
using GameFW.GameMgr;
using GameFW.Entity;
using GameFW.Core.Base;

namespace GameFW.NetClient.Fight
{
    /// <summary>
    /// 战斗消息处理
    /// </summary>
    public class FightHandler : NetBase, IHandler
    {

        #region 应用内消息注册与处理

        /// <summary>
        /// 注册消息
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Regist();
            ushort[] newMsgIds = new ushort[6] {
                (ushort)BuildingFightEvent.DefenseTowerAtkCreq,
                (ushort)BuildingFightEvent.DefenseTowerIdle,
                (ushort)MoveEvent.PosSyncClientRequest,
                (ushort)SoilderFightEvent.SoilderAtkClientRequest,
                (ushort)MoveEvent.MoveClientRequest,
                (ushort)SoilderFightEvent.SoilderIdleClientRequest,
            };
            AddNewMsgIds(newMsgIds);
            RegistSelf();
        }

        /// <summary>
        /// 应用内消息处理
        /// </summary>
        /// <param name="msg"></param>
        public override void ProcessEvent(MsgBase msg)
        {
            switch (msg.MsgId)
            {
                case (ushort)BuildingFightEvent.DefenseTowerAtkCreq:

                    MsgInt msgInt = msg as MsgInt;
                    Send(FightProtocol.DAMAGE_CREQ, msgInt.Int);

                    break;
                case (ushort)BuildingFightEvent.DefenseTowerIdle:

                    MsgInt msgInt2 = msg as MsgInt;
                    Send(FightProtocol.IDLE_CREQ, msgInt2.Int);

                    break;
                case (ushort)MoveEvent.PosSyncClientRequest:

                    MsgPosSync msgPosSync = msg as MsgPosSync;
                    Vector3 pos = msgPosSync.pos;
                    Vector3 dir = msgPosSync.dir;
                    Send(FightProtocol.POS_SYNC_CREQ, new PosSyncDTO(msgPosSync.instanceId, pos.x, pos.y, pos.z, dir.x, dir.y, dir.z, msgPosSync.timeStamp));

                    break;
                case (ushort)SoilderFightEvent.SoilderAtkClientRequest:

                    MsgAtkRequest msgAtkRequest = msg as MsgAtkRequest;
                    Send(FightProtocol.ATTACK_CREQ, new AttackDTO(msgAtkRequest.attackerId, msgAtkRequest.targetId, DateTime.Now.Ticks));

                    break;

                case (ushort)MoveEvent.MoveClientRequest:

                    MsgMoveRequest msgMoveRequest = msg as MsgMoveRequest;
                    Send(FightProtocol.MOVE_CREQ, new PathRequestDTO(msgMoveRequest.InstanceId, msgMoveRequest.SourcePos.x, msgMoveRequest.SourcePos.y, msgMoveRequest.SourcePos.z, msgMoveRequest.TargetPos.x, msgMoveRequest.TargetPos.y, msgMoveRequest.TargetPos.z));

                    break;
                case (ushort)SoilderFightEvent.SoilderIdleClientRequest:

                    MsgInt msgInt5 = msg as MsgInt;
                    Send(FightProtocol.IDLE_CREQ, msgInt5.Int);

                    break;
            }
        }

        #endregion

        #region 发送进入战场请求

        /// <summary>
        /// 当level加载完成时调用
        /// </summary>
        /// <param name="level"></param>
        private void OnLevelWasLoaded(int level)
        {
            if (SceneManager.GetActiveScene().name == "Fight")
            {
                Send(FightProtocol.ENTER_CREQ);
            }
        }

        #endregion

        #region 网络消息处理

        private FightRoomDTO fightRoomDTO = null;//缓存战斗房间信息

        /// <summary>
        /// 处理网络消息
        /// </summary>
        /// <param name="sm"></param>
        public override void OnMessageReceived(SocketModel sm)
        {
            switch (sm.command)
            {
                case FightProtocol.START_BRO:
                    fightRoomDTO = sm.GetMessage<FightRoomDTO>();
                    CreateBattelField(fightRoomDTO);
                    Debug.LogError("收到创建战场信息");
                    break;

                case FightProtocol.ATK_TO_POS_SCMD:
                    PosDTO atkToPosDTO = sm.GetMessage<PosDTO>();
                    MgrCenter.Instance.SendMsg(Msgs.GetMsgPos((ushort)SoilderFightEvent.SoilderAtkToPosServerCmd, atkToPosDTO));
                    break;
                case FightProtocol.MOVE_TARGET_SRES:
                    PosDTO moveToTargetDTO = sm.GetMessage<PosDTO>();
                    MgrCenter.Instance.SendMsg(Msgs.GetMsgPos((ushort)MoveEvent.MoveServerRes, moveToTargetDTO));
                    break;
                case FightProtocol.SKILL_UP_SRES:

                    break;

                case FightProtocol.POS_SYNC_BRO:
                    PosSyncDTO posSyncDTO = sm.GetMessage<PosSyncDTO>();
                    //得到的PosSync是服务器处理时认可的服务器时间
                    //这里将它转化成client认可的时间
                    //floating = client - server
                    //client = server + floating
                    long timeStampClient = GetTimeStampClient(posSyncDTO.timeStamp);
                    MgrCenter.Instance.SendMsg(Msgs.GetMsgPosSync((ushort)MoveEvent.PosSync, posSyncDTO.instanceId, new Vector3(posSyncDTO.x, posSyncDTO.y, posSyncDTO.z), new Vector3(posSyncDTO.dirX, posSyncDTO.dirY, posSyncDTO.dirZ), timeStampClient));
                    break;
                case FightProtocol.SKILL_BRO:

                    break;
                case FightProtocol.ATTACK_BRO:


                    break;
                case FightProtocol.DAMAGE_BRO:
                    Damage(sm.GetMessage<DamageDTO>());

                    break;
                case FightProtocol.IDLE_BRO:
                    int idleEntityId = sm.GetMessage<int>();
                    MgrCenter.Instance.SendMsg(Msgs.GetMsgInt((ushort)SoilderFightEvent.SoilderIdleServerAllow, idleEntityId));
                    break;
            }
        }

        /// <summary>
        /// 根据服务器传来的数值，显示伤害
        /// </summary>
        /// <param name="damageDTO"></param>
        private void Damage(DamageDTO damageDTO)
        {
            foreach (int[] damage in damageDTO.targetDamages) {
                SendMsg(Msgs.GetMsgIntArr((ushort)FightEvent.Damage, damage));
            }
        }

        #endregion

        #region 发送消息给战场逻辑

        /// <summary>
        /// 将server时间转换为client时间
        /// </summary>
        /// <param name="timeStampServer"></param>
        /// <returns></returns>
        private long GetTimeStampClient(long timeStampServer)
        {
            return timeStampServer + GameRuntimeData.delayAndFloating.floating;
        }

        /// <summary>
        /// 创建战场所需的entity和建筑entity
        /// </summary>
        /// <param name="fightRoomDTO"></param>
        private void CreateBattelField(FightRoomDTO fightRoomDTO)
        {
            foreach (BuildingInstance building in fightRoomDTO.buildingEntities)
            {
                SendMsg(Msgs.GetMsgBuildingCreate((ushort)EntityEnum.CreateBuilding, building, fightRoomDTO.isHost));
            }

            foreach (AbsFightInstance creature in fightRoomDTO.entities)
            {
                SendMsg(Msgs.GetMsgNPCCreate((ushort)EntityEnum.CreateEntity, creature, fightRoomDTO.isHost));
            }
        }

        /// <summary>
        /// 网络消息类型
        /// </summary>
        /// <returns></returns>
        public override byte GetType()
        {
            return Protocol.Protocol.TYPE_FIGHT;
        }

        #endregion
    }
}
