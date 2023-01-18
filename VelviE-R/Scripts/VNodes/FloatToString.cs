using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace VelvieR
{

    [VTag("Variables/FloatToString", "Displays float variable as string to ui element.\n\nUse prefix and suffix to append custom texts.", VColor.Yellow03, "Fs")]
    public class FloatToString : AVarToString
    {
        public override void OnVEnter()
        {
            if (variable != null)
            {
                if (text != null)
                {
                    if (!String.IsNullOrEmpty(appendText))
                    {
                        if (prefixType == PrefixSuffix.AppendAsPrefix)
                        {
                            StringAppender(appendText, variable.GetFloat().ToString());
                        }
                        else
                        {
                            StringAppender(variable.GetFloat().ToString(), appendText);
                        }
                    }
                    else
                    {
                        SetText(variable.GetFloat().ToString());
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

            if (text == null)
            {
                summary += "Text component can't be empty.";
            }

            if (variable == null)
            {
                summary += "| Variable can't be empty!";
            }

            return summary;
        }
    }
}