
namespace GameFW.Core.Msg
{
    public class MsgInt:MsgBase
    {
        public int Int {
            get; set;
        }

        public MsgInt(ushort msgId, int i) {
            this.msgId = msgId;
            this.Int = i;
        }

        public MsgInt() { }

        public void SetMsgInt(ushort msgId, int i) {
            this.msgId = msgId;
            this.Int = i;
        }
    }
}
