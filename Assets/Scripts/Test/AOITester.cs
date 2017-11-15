using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GameFW;
using CommonTools;
using GameFW.Entity;

namespace Assets.Scripts.Test
{
    public class AOITester:MonoBehaviour
    {
        Vector3 vector = new Vector3(115f, 0, 115f);
        private void Awake()
        {
            BattleFieldAOI.Instance.Init(25f, 10, 10);
        }

        private void Update()
        {

        }
    }
}
