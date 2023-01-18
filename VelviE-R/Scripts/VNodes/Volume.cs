
using UnityEngine;

namespace VelvieR
{
    [VTag("Multimedia/Volume", "Sets volume of an audioSource.", VColor.Pink01, "Vl")]
    public class Volume : VBlockCore
    {
        [SerializeField, HideInInspector] public AudioSource audioSource;
        [SerializeField, HideInInspector] public float value = 1f;
        [SerializeField, HideInInspector] public bool lerp = false;
        [SerializeField, HideInInspector] public float duration = 1f;
        [SerializeField, HideInInspector] public bool waitUntilFinished = false;
        public override void OnVEnter()
        {
            if(!lerp)
            {
                audioSource.volume = value;
            }
            else
            {
                LeanTween.value(audioSource.gameObject, audioSource.volume, value, duration).setOnUpdate((float val)=>
                {
                    audioSource.volume = val;
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
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if(audioSource == null)
            {
                summary += "AudioSource component can't be empty!";
            }

            return summary;
        }
    }
}