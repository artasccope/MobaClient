using GameFW.Core.Base;
using Protocol;

namespace GameFW.NetClient.Mgr
{
    /// <summary>
    /// 网络模块管理类
    /// </summary>
    public class NetMgr: Mgr<SocketModel>
    {
        /// <summary>
        /// 启动 网络连接、网络消息接收中心
        /// </summary>
        public override void Start()
        {
            NetCONClient.Instance.Start();
            NetMsgCenter.Instance.Init();
        }

        /// <summary>
        /// 发送数据, bytes是未经长度编码的消息体
        /// </summary>
        /// <param name="bytes"></param>
        public void Send(byte[] bytes)
        {
            NetCONClient.Instance.Send(bytes);
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseConnect()
        {
            NetCONClient.Instance.CloseConnect();
        }

        /// <summary>
        /// 清空
        /// </summary>
        public override void ClearAll() {
            base.ClearAll();
            //TODO 
        }
    }
}
