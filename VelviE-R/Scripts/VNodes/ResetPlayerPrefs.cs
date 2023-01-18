using UnityEngine;

namespace VelvieR
{
    [VTag("Variables/ResetPlayerPrefs", "Resets all PlayerPref variables.", VColor.Red, "Rp")]
    public class ResetPlayerPrefs : VBlockCore
    {
        public override void OnVEnter()
        {
            PlayerPrefs.DeleteAll();
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
    }
}