
using UnityEngine;

namespace VelvieR
{
    [VTag("Multimedia/PanStereo", "Sets left/right stereo value of an audioSource.", VColor.Pink01, "Vl")]
    public class PanStereo : VBlockCore
    {
        [SerializeField, HideInInspector] public AudioSource audioSource;
        [SerializeField, HideInInspector] public float value = 0f;
        [SerializeField, HideInInspector] public float duration = 0.5f;
        [SerializeField, HideInInspector] public bool waitUntilFinished = false;
        public override void OnVEnter()
        {
            LeanTween.value(audioSource.gameObject, audioSource.panStereo, value, duration).setOnUpdate((float val) =>
            {
                audioSource.panStereo = val;
            }).setOnComplete(() =>
            {
                if (waitUntilFinished)
                    OnVContinue();
            });
        }

        public override void OnVExit()
        {
            if (!waitUntilFinished)
                OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if (audioSource == null)
            {
                summary += "AudioSource component can't be empty!";
            }

            return summary;
        }
    }
}