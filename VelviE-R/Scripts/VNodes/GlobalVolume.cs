
using UnityEngine;

namespace VelvieR
{
    [VTag("Multimedia/GlobalVolume", "Sets global volume.", VColor.Pink01, "Vl")]
    public class GlobalVolume : VBlockCore
    {
        [SerializeField, HideInInspector] public float value = 1f;
        [SerializeField, HideInInspector] public bool lerp = false;
        [SerializeField, HideInInspector] public float duration = 1f;
        [SerializeField, HideInInspector] public bool waitUntilFinished = false;
        private GameObject dummyObj;
        public override void OnVEnter()
        {
            if(!lerp)
            {
                AudioListener.volume = value;
            }
            else
            {
                LeanTween.value(dummyObj, AudioListener.volume, value, duration).setOnUpdate((float val)=>
                {
                    AudioListener.volume = val;
                }).setOnComplete(()=>
                {
                    if(waitUntilFinished)
                        OnVContinue();
                });
            }
        }

        public override void OnVExit()
        {
            if(!waitUntilFinished)
                OnVContinue();
        }
    }
}