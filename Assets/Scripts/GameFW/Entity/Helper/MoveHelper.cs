using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameFW.Entity.Helper
{
    /// <summary>
    /// 移动帮助类
    /// </summary>
    public class MoveHelper
    {
        /// <summary>
        /// 根据现在的位置，方向，将要流逝的时间长度，单位移动速度，推算出将来的位置
        /// </summary>
        /// <param name="posNow"></param>
        /// <param name="dir"></param>
        /// <param name="time"></param>
        /// <param name="Speed"></param>
        /// <returns></returns>
        public static Vector3 GetFuturePos(Vector3 posNow, Vector3 dir, float time, float Speed)
        {
            float dirX = (dir.x / 180f) * Mathf.PI;
            float dirY = (dir.y / 180f) * Mathf.PI;
            float dirZ = (dir.z / 180f) * Mathf.PI;
            float x = posNow.x + (Mathf.Sin(dirY) + Mathf.Cos(dirZ - Mathf.PI * 0.5f)) * Speed * time;
            float y = posNow.y + (Mathf.Cos(dirX) + Mathf.Sin(dirZ - Mathf.PI * 0.5f)) * Speed * time;
            float z = posNow.z + (Mathf.Sin(dirX) + Mathf.Cos(dirY)) * Speed * time;

            return new Vector3(x, y, z);
        }
    }
}
