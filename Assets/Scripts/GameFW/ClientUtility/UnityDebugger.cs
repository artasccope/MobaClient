using System;
using GameFW.Utility;
using UnityEngine;

namespace GameFW.ClientUtility
{
    class UnityDebugger : Debuger
    {
        public override void Log(string message)
        {
            Debug.Log(message);
        }
    }
}
