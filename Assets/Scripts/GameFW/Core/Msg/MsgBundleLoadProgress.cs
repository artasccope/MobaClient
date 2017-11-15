
namespace GameFW.Core.Msg
{
    public class MsgBundleLoadProgress:MsgBase
    {
        private string bundleName;
        private float progress;

        public void SetBundleLoadProgress(ushort msgId, string bundleName, float progress) {
            this.msgId = msgId;
            this.bundleName = bundleName;
            this.progress = progress;
        }
    }
}
