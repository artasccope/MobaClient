using Protocol.DTO;

namespace GameFW.Core.Msg
{
    public class MsgDelayAndFloating:MsgBase
    {
        public DelayAndFloating DelayFloating
        {
            get; set;
        }

        public MsgDelayAndFloating() { }

        public MsgDelayAndFloating(ushort msgId, DelayAndFloating df)
        {
            this.msgId = msgId;
            this.DelayFloating = df;
        }


        public void SetMsgDelayAndFloating(ushort msgId, DelayAndFloating delayAndFloating)
        {
            this.msgId = msgId;
            this.DelayFloating = delayAndFloating;
        }
    }
}
