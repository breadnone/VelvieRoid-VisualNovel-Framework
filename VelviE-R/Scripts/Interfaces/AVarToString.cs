using UnityEngine;
using TMPro;

namespace VelvieR
{
    public abstract class AVarToString : VBlockCore
    {
        [SerializeReference] public IVar variable;
        [SerializeField] public TMP_Text text;
        [SerializeField] public string appendText;
        [SerializeField] public PrefixSuffix prefixType = PrefixSuffix.AppendAsPrefix;
        public virtual void StringAppender(string valA, string valB)
        {
            text.SetText(valA + valB);
        }
        public virtual void SetText(string valA)
        {
            text.SetText(valA);
        }
    }
}