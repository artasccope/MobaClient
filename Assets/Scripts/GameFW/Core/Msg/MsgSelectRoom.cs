using Protocol.DTO;

namespace GameFW.Core.Msg
{
    public class MsgSelectRoom:MsgBase
    {
        public SelectRoomDTO selectRoom;

        public MsgSelectRoom(ushort msgId, SelectRoomDTO selectRoom) {
            this.msgId = msgId;
            this.selectRoom = selectRoom;
        }

        public MsgSelectRoom() { }

        public void SetMsgSelectRoom(ushort msgId, SelectRoomDTO selectRoom) {
            this.msgId = msgId;
            this.selectRoom = selectRoom;
        }
    }
}
