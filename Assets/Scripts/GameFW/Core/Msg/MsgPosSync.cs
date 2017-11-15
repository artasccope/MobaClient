using UnityEngine;

namespace GameFW.Core.Msg
{
    public class MsgPosSync:MsgBase
    {
        public MsgPosSync(ushort msgId, int instanceId, Vector3 pos, Vector3 dir, long timeStamp)
        {
            this.msgId = msgId;
            this.pos = pos;
            this.dir = dir;
            this.instanceId = instanceId;
            this.timeStamp = timeStamp;
        }

        public MsgPosSync() { }

        public Vector3 pos;
        public Vector3 dir;
        public int instanceId;
        public long timeStamp;


        public void SetMsgPosSync(ushort msgId, int instanceId, Vector3 pos, Vector3 dir, long timeStamp)
        {
            this.msgId = msgId;
            this.pos = pos;
            this.dir = dir;
            this.instanceId = instanceId;
            this.timeStamp = timeStamp;
        }
    }
}
