using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VelvieR
{
    [VTag("Flow/WaitSeconds", "Waits until certain time in seconds", VColor.Green02, "Wa")]
    public class WaitSeconds : VBlockCore
    {
        [SerializeField] private float seconds = 0f;
        [SerializeField] bool unscaledTime = false;
        public float Seconds { get { return seconds; } set { seconds = value; } }
        public bool UnscaledTime {get{return unscaledTime;} set{unscaledTime = value;}}
        public override void OnVEnter() { }
        private IEnumerator WaitCoroutine()
        {
            if (!unscaledTime)
            {
                yield return new WaitForSeconds(seconds);
            }
            else
            {
                yield return new WaitForSecondsRealtime(seconds);
            }

            OnVContinue();
        }

        public override void OnVExit()
        {
            StartCoroutine(WaitCoroutine());
        }
    }
}