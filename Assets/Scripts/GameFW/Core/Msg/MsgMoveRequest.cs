using UnityEngine;

namespace GameFW.Core.Msg
{
    public class MsgMoveRequest:MsgBase
    {
        public Vector3 SourcePos 
        {
            get; set;
        }

        public Vector3 TargetPos
        {
            get; set;
        }

        public int InstanceId { get; set; }

        public MsgMoveRequest(ushort msgId, int instanceId, Vector3 sourcePos, Vector3 targetPos)
        {
            this.msgId = msgId;
            this.SourcePos = sourcePos;
            this.TargetPos = targetPos;
            this.InstanceId = instanceId;
        }

        public MsgMoveRequest() { }

        public void SetMsgMoveRequest(ushort msgId, int instanceId, Vector3 sourcePos, Vector3 targetPos)
        {
            this.msgId = msgId;
            this.SourcePos = sourcePos;
            this.TargetPos = targetPos;
            this.InstanceId = instanceId;
        }
    }
}
