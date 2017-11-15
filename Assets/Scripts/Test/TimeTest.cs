using System;
using System.Collections;
using UnityEngine;

namespace GameFW
{
    public class TimeTest:MonoBehaviour
    {

        private void Start()
        {
            StartCoroutine(drive());
        }

        protected IEnumerator drive()
        {
            while (true)
            {
                Debug.Log(DateTime.Now.Ticks);

                yield return new WaitForSeconds(1f);
            }
        }
    }
}
