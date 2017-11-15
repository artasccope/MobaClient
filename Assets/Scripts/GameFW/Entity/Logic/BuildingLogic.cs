using GameFW.Core.Msg;

namespace GameFW.Entity.Logic
{
    /// <summary>
    /// 建筑逻辑模块
    /// </summary>
    public class BuildingLogic:EntityLogic
    {
        public override void Regist()
        {
            base.Regist();
            ushort[] newMsgIds = new ushort[2] {
                (ushort)BuildingFightEvent.DefenseTowerIdle,
                (ushort)BuildingFightEvent.DefenseTowerAtkCreq,
            };
            AddNewMsgIds(newMsgIds);
            this.RegistSelf();
        }

        public override void ProcessEvent(MsgBase msg)
        {
            switch (msg.MsgId)
            {
                case (ushort)BuildingFightEvent.DefenseTowerIdle:

                    break;
                case (ushort)BuildingFightEvent.DefenseTowerAtkCreq:


                    break;
            }
        }
    }
}
