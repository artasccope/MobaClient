
using UnityEngine;

namespace GameFW.Core.Msg
{
    public class MsgEntityPos:MsgBase
    {
        public MsgEntityPos(ushort msgId, int entityId, Vector3 pos)
        {
            this.msgId = msgId;
            this.entityId = entityId;
            this.pos = pos;
        }

        public MsgEntityPos() { }

        public int entityId;
        public Vector3 pos;
        public void SetMsgEntityPos(ushort msgId, int entityId, Vector3 pos)
        {
            this.msgId = msgId;
            this.entityId = entityId;
            this.pos = pos;
        }
    }
}
