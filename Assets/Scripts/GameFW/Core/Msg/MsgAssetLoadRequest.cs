
namespace GameFW.Core.Msg
{
    public class MsgAssetLoadRequest : MsgBase
    {
        /// <summary>
        /// asset所在的bundle name
        /// </summary>
        public string bundleName;
        /// <summary>
        /// asset的name
        /// </summary>
        public string assetName;

        public void SetMsgAssetLoadRequest(ushort msgId, string bundleName, string assetName)
        {
            this.msgId = msgId;
            this.bundleName = bundleName;
            this.assetName = assetName;
        }
    }
}
