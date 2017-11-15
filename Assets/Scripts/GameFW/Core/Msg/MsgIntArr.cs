
namespace GameFW.Core.Msg
{
    public class MsgIntArr:MsgBase
    {
        public int[] Int
        {
            get; set;
        }

        public MsgIntArr(ushort msgId, int[] arr)
        {
            this.msgId = msgId;
            this.Int = arr;
        }

        public MsgIntArr() { }

        public void SetMsgIntArr(ushort msgId, int[] arr)
        {
            this.msgId = msgId;
            this.Int = arr;
        }
    }
}
