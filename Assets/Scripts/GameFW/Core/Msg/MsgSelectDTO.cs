using Protocol.DTO;

namespace GameFW.Core.Msg
{
    public class MsgSelectDTO : MsgBase
    {
        private SelectDTO selectDTO;

        public SelectDTO SelectData {
            get {
                return selectDTO;
            }
        }
        public void SetMsgSelectDTO(ushort msgId, SelectDTO selectDTO) {
            this.msgId = msgId;
            this.selectDTO = selectDTO;
        }
    }
}
