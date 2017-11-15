using UnityEngine;

namespace GameFW.UI.Fight.HUD
{
    /// <summary>
    /// HUD信息
    /// </summary>
    public class HUDInfo
    {
        public string name;//名字
        public int id;//对应id
        public float percent;//生命值比例
        public Vector3 pos;//位置

        public HUDInfo() { }

        public HUDInfo(string name, int id, float percent, Vector3 pos) {
            this.name = name;
            this.id = id;
            this.percent = percent;
            this.pos = pos;
        }
    }
}
