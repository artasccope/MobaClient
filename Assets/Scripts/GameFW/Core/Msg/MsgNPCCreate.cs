using Protocol.DTO.Fight.Instance;

namespace GameFW.Core.Msg
{
    public class MsgNPCCreate : MsgBase
    {
        public MsgNPCCreate(ushort msgId, AbsFightInstance npcInstance, bool isHost)
        {
            this.msgId = msgId;
            this.npcInstance = npcInstance;
            this.isHost = isHost;
        }

        public MsgNPCCreate() { }

        public AbsFightInstance npcInstance;
        public bool isHost;

        public void SetNPCCreate(ushort msgId, AbsFightInstance npcInstance, bool isHost)
        {
            this.msgId = msgId;
            this.npcInstance = npcInstance;
            this.isHost = isHost;
        }
    }
}
