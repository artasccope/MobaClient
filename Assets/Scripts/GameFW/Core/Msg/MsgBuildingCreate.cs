using Protocol.DTO.Fight.Instance;


namespace GameFW.Core.Msg
{
    public class MsgBuildingCreate:MsgBase
    {
        public MsgBuildingCreate(ushort msgId, BuildingInstance buildingCreateDTO, bool isHost)
        {
            this.msgId = msgId;
            this.buildingCreateDTO = buildingCreateDTO;
            this.isHost = isHost;
        }

        public MsgBuildingCreate() { }

        public BuildingInstance buildingCreateDTO;
        public bool isHost;

        public void SetBuildingCreate(ushort msgId, BuildingInstance buildingCreateDTO, bool isHost)
        {
            this.msgId = msgId;
            this.buildingCreateDTO = buildingCreateDTO;
            this.isHost = isHost;
        }
    }
}
