using Protocol.DTO;

namespace GameFW.GameMgr
{
    /// <summary>
    /// 游戏实时运行的一些数据
    /// </summary>
    public class GameRuntimeData
    {
        /// <summary>
        /// 以tick为单位,10000000tick = 1s
        /// </summary>
        public static DelayAndFloating delayAndFloating = new DelayAndFloating(1000000, 1000000);
        /// <summary>
        /// 队伍号
        /// </summary>
        public static int teamId = 1;
        /// <summary>
        /// 位置同步间隔时间(秒)
        /// </summary>
        public static float PosSyncSecond = 1f;
        /// <summary>
        /// 延迟(按秒记)
        /// </summary>
        public static float GetDelaySecond {
            get {
                return delayAndFloating.delay * 0.0000001f;
            }
        }
        /// <summary>
        /// DateTime和Unity的Time之间的差异
        /// </summary>
        public static float FloatingBwteenDatatimeAndUnityTime;
    }
}
