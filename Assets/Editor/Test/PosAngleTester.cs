using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace Assets.Scripts.Test
{
    class PosAngleTester
    {
        [MenuItem("Test/test pos angle")]
        public static void TestShow() {
            Vector3 futurePos = GetFuturePos(Vector3.zero, new Vector3(45, 45, 45), 4, 1);

            Debug.DrawLine(Vector3.zero, futurePos);
        }

        private static Vector3 GetFuturePos(Vector3 posNow, Vector3 dir, float time, float Speed)
        {
            float dirX = (dir.x / 180f) * Mathf.PI;
            float dirY = (dir.y / 180f) * Mathf.PI;
            float dirZ = (dir.z / 180f) * Mathf.PI;
            float x = posNow.x + (Mathf.Sin(dirY) + Mathf.Cos(dirZ - Mathf.PI*0.5f)) * Speed * time;
            float y = posNow.y + (Mathf.Cos(dirX) + Mathf.Sin(dirZ - Mathf.PI*0.5f)) * Speed * time;
            float z = posNow.z + (Mathf.Sin(dirX) + Mathf.Cos(dirY)) * Speed * time;


            return new Vector3(x, y, z);
        }
    }
}
