
namespace GameFW.Core.Msg
{

    public class MsgKV<K, V> : MsgBase
    {
        public K msgKey;
        public V msgValue;

        public MsgKV() { }

        public MsgKV(ushort msgId, K key, V value)
        {
            this.msgId = msgId;
            this.msgKey = key;
            this.msgValue = value;
        }

        public void SetMsgKV(ushort msgId, K key, V value)
        {
            this.msgId = msgId;
            this.msgKey = key;
            this.msgValue = value;
        }
    }
}
