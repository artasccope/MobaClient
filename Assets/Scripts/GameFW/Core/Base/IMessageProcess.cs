using GameFW.Core.Msg;

namespace GameFW.Core.Base
{
    /// <summary>
    /// 发送、处理信息的核心接口
    /// </summary>
    public interface IMessageProcess
    {
        /// <summary>
        /// 处理信息
        /// </summary>
        /// <param name="msg"></param>
        void ProcessEvent(MsgBase msg);
    }
}
