namespace GameFW.Core.Msg
{
    /// <summary>
    /// 消息基类，所有消息都应继承它
    /// </summary>
    public class MsgBase
    {
        protected ushort msgId;//消息的id
        public ushort MsgId { get { return msgId; } set {  msgId = value; } }

        public MsgBase() { }
        public MsgBase(ushort msgId) {
            this.msgId = msgId;
        }

        #region 消息类型
        /// <summary>
        /// 此消息的类型
        /// </summary>
        /// <param name="msgId">消息Id</param>
        /// <returns></returns>
        public MsgType GetMsgType() {
            return (MsgType)((msgId / Settings.MsgSpan) * Settings.MsgSpan);
        }

        /// <summary>
        /// 根据消息id获取消息类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MsgType GetMsgType(ushort id) {
            return (MsgType)((id / Settings.MsgSpan) * Settings.MsgSpan);
        }

#endregion
    }
}
