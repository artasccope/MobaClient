using UnityEngine;
using GameFW;
using GameFW.Utility;
using GameFW.Entity;
using GameFW.Core.Base;

namespace Assets.Scripts.Test
{
    public class MgrLoaderTest:MonoBehaviour
    {
        private void Awake()
        {
            Tools.debuger = ClientDebuger.Instance;
            BattleFieldAOI.Instance.Init(25f, 0, 0);
            MgrCenter.Instance.ClearAll();
            MgrCenter.EntityMgr.Start();
        }
    }
}
