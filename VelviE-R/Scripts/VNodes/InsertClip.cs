
using UnityEngine;

namespace VelvieR
{
    [VTag("Multimedia/InsertClip", "Sets audio clip to audioSource.", VColor.Pink01, "Vl")]
    public class InsertClip : VBlockCore
    {
        [SerializeField, HideInInspector] public AudioSource audioSource;
        [SerializeField, HideInInspector] public AudioClip auclip;
        [SerializeField, HideInInspector] public bool play;

        public override void OnVEnter()
        {
            audioSource.Stop();
            audioSource.clip = auclip;

            if(play)
            {
                audioSource.Play();
            }
        }

        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if(audioSource == null || auclip == null)
            {
                summary += "AudioSource and AudioClip can't be empty!";
            }

            return summary;
        }
    }
}