
namespace GameFW.Core.Msg
{
    public class MsgT<T> : MsgBase
    {
        public T msgData;

        public MsgT(){}

        public MsgT(ushort msgId, T data) {
            this.msgId = msgId;
            this.msgData = data;
        }

        public void SetMsgT(ushort msgId, T data)
        {
            this.msgId = msgId;
            this.msgData = data;
        }
    }

}
