using System.Collections.Generic;
using UnityEngine;

namespace VelvieR
{
    [VTag("Variables/RandomString", "Random string value.", VColor.Yellow01, "Ri")]
    public class RandomString : VBlockCore
    {
        [SerializeReference] public IVar variable;
        [SerializeField] public List<string> words = new List<string>();

        public override void OnVEnter()
        {
            variable.SetString(words[UnityEngine.Random.Range(0, words.Count)]);
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if(variable == null)
            {
                summary += "Variable can't be null!";
            }

            return summary;
        }
    }
}