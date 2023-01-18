using System.Collections.Generic;
using UnityEngine;

namespace VelvieR
{
    [VTag("Variables/RandomInt", "Random integer value.\n\nThe variable will contain the results.\n\nUse shuffle to get unique random out of min/max value, it will reset back once all min/max values are used.", VColor.Yellow01, "Ri")]
    public class RandomInt : VBlockCore
    {
        [SerializeReference] public IVar variable;
        [SerializeField] public int min;
        [SerializeField] public int max;
        [SerializeField] public bool shuffle = false;
        private List<(bool state, int value)> vals = new List<(bool state, int value)>();

        public override void OnVEnter()
        {
            if(!shuffle)
            {
                variable.SetInt((int)Random.Range(min, max));
            }
            else
            {
                void RePool()
                {
                    if(vals.Count == 0)
                    {
                        for(int i = min; i < max; i++)
                        {
                            vals.Add((false, i));
                        }
                    }
                }

                RePool();                
                var urands = vals.FindAll(x => !x.state);
                int? rands = (int)Random.Range(urands[0].value, urands[urands.Count - 1].value);
                bool executed = false;

                if(rands.HasValue)
                {
                    vals[rands.Value] = (true, vals[rands.Value].value);
                    variable.SetInt(rands.Value);
                    executed = true;
                }

                if(executed && (urands.Count == 1 || urands.Count == 0))
                {
                    vals = new List<(bool state, int value)>();
                    RePool();
                }
            }
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