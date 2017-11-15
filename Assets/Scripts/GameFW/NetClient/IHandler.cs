using Protocol;

namespace GameFW.NetClient
{
    /// <summary>
    /// 网络消息处理
    /// </summary>
    public interface IHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sm"></param>
        void OnMessageReceived(SocketModel sm);
    }
}
