using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace VelvieR
{

    [VTag("Variables/DoubleToString", "Displays double variable as string to ui element.\n\nUse prefix and suffix to append custom texts.", VColor.Yellow03, "Ds")]
    public class DoubleToString : AVarToString
    {
        public override void OnVEnter()
        {
            if(variable != null)
            {
                if(text != null)
                {
                    if(!String.IsNullOrEmpty(appendText))
                    {
                        if(prefixType == PrefixSuffix.AppendAsPrefix)
                        {
                            StringAppender(appendText, variable.GetDouble().ToString());
                        }
                        else
                        {
                            StringAppender(variable.GetDouble().ToString(), appendText);
                        }
                    }
                    else
                    {
                        SetText(variable.GetDouble().ToString());
                    }
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

            if(text == null)
            {
                summary += "Text component can't be empty.";
            }

            if(variable == null)
            {
                summary += "| Variable can't be empty!";
            }

            return summary;
        }
    }
}