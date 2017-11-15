using UnityEngine;

namespace GameFW.Core.Msg
{
    public class MsgTransform:MsgBase
    {
        protected Transform trans;

        public Transform Trans {
            get {
                return trans;
            }
            set {
                trans = value;
            }
        }

        public MsgTransform() { }

        public MsgTransform(ushort msg, Transform trans) : base(msg){
            this.trans = trans;
        }

        internal void SetMsgTransform(ushort msgId, Transform transform)
        {
            this.msgId = msgId;
            this.trans = transform;
        }
    }
}
