
using UnityEngine;
using UnityEngine.Audio;

namespace VelvieR
{
    [VTag("Multimedia/ChangeOutput", "Sets output of an audioSource to another MixerGroup.", VColor.Pink01, "Vl")]
    public class ChangeOutput : VBlockCore
    {
        [SerializeField, HideInInspector] public AudioSource audioSource;
        [SerializeField, HideInInspector] public AudioMixerGroup mixerGroup;
        [SerializeField, HideInInspector] public bool play;
        public override void OnVEnter()
        {
            audioSource.outputAudioMixerGroup = mixerGroup;
        }

        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if(audioSource == null || mixerGroup == null)
            {
                summary += "AudioSource and MixerGroup can't be empty!";
            }

            return summary;
        }
    }
}