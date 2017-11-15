using System.Text;
using Object = UnityEngine.Object;

namespace GameFW.Core.Msg
{
    public class MsgAssetLoaded: MsgBase
    {
        /// <summary>
        /// asset所在的bundle name
        /// </summary>
        public string bundleName;
        /// <summary>
        /// asset的name
        /// </summary>
        public string assetName;

        public Object asset;

        public void SetMsgAssetLoaded(ushort msgId, string bundleName, string assetName, Object asset)
        {
            this.msgId = msgId;
            this.bundleName = bundleName;
            this.assetName = assetName;
            this.asset = asset;
        }

        public string GetAssetAndABName() {
            return new StringBuilder(bundleName).Append(" ").Append(assetName).ToString();
        }
    }
}
