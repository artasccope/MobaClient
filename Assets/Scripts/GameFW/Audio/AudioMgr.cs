using GameFW.Core.Base;
using UnityEngine;

namespace GameFW.Audio
{
    public class AudioMgr:Mgr<GameObject>
    {
        private AudioListener listener;

        public AudioListener Listener {
            get {
                return this.listener;
            }
            set {
                this.listener = value;
            }
        }

    }
}
