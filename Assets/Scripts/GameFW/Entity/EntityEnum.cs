using GameFW.Core;

namespace GameFW.Entity
{
    /// <summary>
    /// 实体创建消息
    /// </summary>
    public enum EntityEnum
    {
        CreateEntity = MsgType.Entity,
        CreateBuilding,

        MaxValue,
    }

    /// <summary>
    /// 建筑战斗消息
    /// </summary>
    public enum BuildingFightEvent
    {
        DefenseTowerAtkCreq = EntityEnum.MaxValue,
        DefenseTowerIdle,

        MaxValue,
    }

    /// <summary>
    /// 移动消息
    /// </summary>
    public enum MoveEvent {
        MoveClientRequest = BuildingFightEvent.MaxValue,
        PosSyncClientRequest,
        PosSync,
        MoveServerRes,

        MaxValue,
    }

    /// <summary>
    /// 战斗消息
    /// </summary>
    public enum FightEvent {
        Damage = MoveEvent.MaxValue,

        MaxValue,
    }

    /// <summary>
    /// 战士战斗消息
    /// </summary>
    public enum SoilderFightEvent {
        SoilderIdleClientRequest = FightEvent.MaxValue,
        SoilderIdleServerAllow,
        SoilderAtkClientRequest,
        SoilderAtkToPosServerCmd,

        MaxValue,
    }
}
