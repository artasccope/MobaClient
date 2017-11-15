using UnityEngine;

namespace GameFW.Core.Msg
{
    public class MsgHUD:MsgBase
    {
        public int Id
        {
            get; set;
        }

        public Vector3 Pos { get; set; }
        public string Name { get; set; }
        public float Percent { get; set; }

        public MsgHUD(ushort msgId, int id, Vector3 pos, string name, float percent)
        {
            this.msgId = msgId;
            this.Id = id;
            this.Pos = pos;
            this.Name = name;
            this.Percent = percent;
        }

        public MsgHUD() { }

        public void SetMsgHUD(ushort msgId, int id, Vector3 pos, string name, float percent)
        {
            this.msgId = msgId;
            this.Id = id;
            this.Pos = pos;
            this.Name = name;
            this.Percent = percent;
        }
    }
}
