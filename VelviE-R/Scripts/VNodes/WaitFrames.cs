using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VelvieR
{
    [VTag("Flow/WaitFrames", "Waits until certain frames", VColor.Green01, "Wf")]
    public class WaitFrames : VBlockCore
    {
        [SerializeField] private int frames = 1;
        public int Frames { get { return frames; } set { frames = value; } }
        public override void OnVEnter() { }
        private IEnumerator WaitCoroutine()
        {
            if(frames > 1)
            {
                for(int i = 0; i < frames; i++)
                {
                    yield return null;
                }
            }
            else
            {
                yield return null;
            }

            OnVContinue();
        }

        public override void OnVExit()
        {
            StartCoroutine(WaitCoroutine());
        }
    }
}