using UnityEngine;
using TMPro;

namespace VelvieR
{
    [VTag("Variables/RandomBool", "Random true or false.\n\nOutput text is optional.", VColor.Gray, "Rb")]
    public class RandomBool : VBlockCore
    {
        [SerializeReference] public IVar variable;
        [SerializeField] public TMP_Text text;
        public override void OnVEnter()
        {
            if(variable != null)
                variable.SetBool((int)Random.Range(0, 20) % 2 == 0);

            if(text != null)
                text.SetText(variable.GetBool().ToString());
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
    }
}