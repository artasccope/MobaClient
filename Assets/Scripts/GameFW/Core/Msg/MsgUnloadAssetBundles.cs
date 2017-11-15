
namespace GameFW.Core.Msg
{
    public class MsgUnloadAssetBundles:MsgBase
    {
        public bool ifUnloadObjs;
        /// <summary>
        /// asset所在的bundle name
        /// </summary>
        public string[] bundleNames;


        public void SetMsgUnloadAssetBundles(ushort msgId, string[] bundleNames, bool ifUnloadObjs)
        {
            this.msgId = msgId;
            this.bundleNames = bundleNames;
            this.ifUnloadObjs = ifUnloadObjs;
        }
    }
}
