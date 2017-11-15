
namespace GameFW.Core.Msg
{
    public class MsgString:MsgBase
    {
        protected string str;
        public MsgString(ushort msgId, string str) {
            this.msgId = msgId;
            this.str = str;
        }

        public MsgString() { }

        public string Str{
            get{
                return this.str;
            }
        }

        public void SetMsgString(ushort msgId, string str) {
            this.str = str;
            this.msgId = msgId;
        }
    }
}
