using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Test
{
    //委托 -> 匿名方法 -> lambda表达式
    public class LambdaGCTest:MonoBehaviour
    {
        public delegate int Cal(int x, int y);



        private void Update()
        {
            Cal cal = (x, y) => x + y;

            Debug.Log(cal(1, 1));

            //=>左边为参数，右边为函数体
            Func<int, int, bool> gwl = (p, j) =>
            {
                if (p + j == 10)
                    return true;

                return false;
            };
        }
    }
}
