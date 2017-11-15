using GameFW.Entity.Driver;
using GameFW.Entity.Helper;
using GameFW.GameMgr;
using UnityEngine;

namespace GameFW.Entity
{
    /// <summary>
    /// 位置同步的驱动器
    /// </summary>
    public class PosSyncDriver : MonoBehaviour
    {
        #region 初始化

        private SoilderDriver soilderDriver;//可以移动的士兵driver

        private void Start()
        {
            soilderDriver = gameObject.GetComponent<SoilderDriver>();
        }

        #endregion

        #region 位置同步
        private Vector3 wantedPos;//期望的位置
        private bool isMoving;//是否正在移动
        private Quaternion wantedDir;//期望的方向
        private float arriveTime;//期望抵达的时间点
        private float syncCmdTime;//同步命令包的时间点
        private Vector3 syncCmdPos;//同步命令的位置
        private Quaternion syncCmdDir;//同步命令的方向
        /// <summary>
        /// 同步位置
        /// </summary>
        /// <param name="posSyncDTO"></param>
        public void PosSync(Vector3 pos, Vector3 dir, long syncSendTimeStamp)
        {
            this.isMoving = true;
            float lerpTime = GameRuntimeData.delayAndFloating.delay * 4;
            this.arriveTime = (syncSendTimeStamp + lerpTime) * 0.0000001f - GameRuntimeData.FloatingBwteenDatatimeAndUnityTime;
            //1.计算出期望的位置,方向
            wantedPos = MoveHelper.GetFuturePos(pos, dir, lerpTime, soilderDriver.Speed);
            wantedDir = Quaternion.Euler(dir);
            //3.记录现在的时间、位置、方向，将这个时间转化为以Time.time为准
            syncCmdTime = Time.time;
            syncCmdPos = transform.position;
            syncCmdDir = transform.rotation;

            if (soilderDriver != null)
                soilderDriver.Move();
        }

        /// <summary>
        /// 位置插值
        /// </summary>
        private void Update()
        {
            if (isMoving)
            {
                float percent = (Time.time - syncCmdTime) / (arriveTime - syncCmdTime);
                if (percent <= 1)
                {
                    transform.position = Vector3.Lerp(syncCmdPos, wantedPos, percent);
                    transform.rotation = Quaternion.Lerp(syncCmdDir, wantedDir, percent);
                }
                else
                {
                    isMoving = false;
                }
            }
        }
        #endregion
    }
}
