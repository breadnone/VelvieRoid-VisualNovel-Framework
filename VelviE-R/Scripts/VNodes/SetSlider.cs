
using UnityEngine;
using UnityEngine.UI;
namespace VelvieR
{
    [VTag("UIUX/SetSlider", "Sets slider value.", VColor.Pink01, "Si")]
    public class SetSlider : VBlockCore
    {
        [SerializeField, HideInInspector] public Slider slider;
        [SerializeField, HideInInspector] public float value = 1f;
        [SerializeField, HideInInspector] public bool lerp = false;
        [SerializeField, HideInInspector] public float duration = 1f;
        [SerializeField, HideInInspector] public bool waitUntilFinished = false;
        public override void OnVEnter()
        {
            if(!lerp)
            {
                slider.value = value;
            }
            else
            {
                LeanTween.value(slider.gameObject, slider.value, value, duration).setOnUpdate((float val)=>
                {
                    slider.value = val;
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

            if(slider == null)
            {
                summary += "Slider component can't be empty!";
            }

            return summary;
        }
    }
}