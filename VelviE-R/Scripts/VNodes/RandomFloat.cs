using System.Collections.Generic;
using UnityEngine;

namespace VelvieR
{
    [VTag("Variables/RandomFloat", "Random float value.", VColor.Yellow01, "Ri")]
    public class RandomFloat : VBlockCore
    {
        [SerializeReference] public IVar variable;
        [SerializeField] public float min;
        [SerializeField] public float max;
        private List<(bool state, float value)> vals = new List<(bool state, float value)>();

        public override void OnVEnter()
        {
            variable.SetFloat(Random.Range(min, max));
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