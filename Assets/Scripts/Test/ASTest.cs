using GameFW.Core.Msg;
using UnityEngine;

namespace Assets.Scripts.Test
{
    public class ASTest:MonoBehaviour
    {
        MsgInt msgInt = new MsgInt();

        MsgBase msgBase = null;
        private void Start()
        {
            MsgBase msgBase = msgInt;
        }

        private void Update()
        {
            MsgInt msg = msgBase as MsgInt;
        }
    }
}
