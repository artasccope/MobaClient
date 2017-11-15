
namespace GameFW.Core.Msg
{
    public class MsgAtkRequest:MsgBase
    {
        public MsgAtkRequest(ushort msgId, int attackerId, int targetId)
        {
            this.msgId = msgId;
            this.attackerId = attackerId;
            this.targetId = targetId;
        }

        public MsgAtkRequest() { }

        public int attackerId;
        public int targetId;


        public void SetMsgAtkRequest(ushort msgId, int attackerId, int targetId)
        {
            this.msgId = msgId;
            this.attackerId = attackerId;
            this.targetId = targetId;
        }
    }
}
