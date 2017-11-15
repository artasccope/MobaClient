using Protocol.DTO;

namespace GameFW.Core.Msg
{
    public class MsgUserDTO:MsgBase
    {
        public UserDTO userDTO;

        public MsgUserDTO(ushort msgId, UserDTO userDTO) {
            this.msgId = msgId;
            this.userDTO = userDTO;
        }

        public MsgUserDTO() { }

        public void SetMsgUserDTO(ushort msgId, UserDTO userDTO) {
            this.msgId = msgId;
            this.userDTO = userDTO;
        }
    }
}
