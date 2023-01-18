using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VelvieR
{
    [VTag("Variables/LoadPlayerPrefs", "Loads PlayerPref based on it's key.", VColor.Yellow03, "Lv")]
    public class LoadPlayerPrefs : VBlockCore
    {
        [SerializeField] public string key;
        [SerializeReference] public IVar localVariable;
        public override void OnVEnter()
        {
            if (!String.IsNullOrEmpty(key) && localVariable != null)
            {
                if (PlayerPrefs.HasKey(key))
                {
                    if (localVariable.GetVtype() == VTypes.Boolean)
                    {
                        localVariable.SetBool(UPref.GetPrefBoolean(key));
                    }
                    else if (localVariable.GetVtype() == VTypes.String)
                    {
                        localVariable.SetString(UPref.GetPrefString(key));
                    }
                    else if (localVariable.GetVtype() == VTypes.Double)
                    {
                        localVariable.SetDouble(UPref.GetPrefDoubble(key));
                    }
                    else if (localVariable.GetVtype() == VTypes.Integer)
                    {
                        localVariable.SetInt(UPref.GetPrefInt(key));
                    }
                    else if (localVariable.GetVtype() == VTypes.Vector2)
                    {
                        localVariable.SetVector2(UPref.GetPrefVector2(key));
                    }
                    else if (localVariable.GetVtype() == VTypes.Vector3)
                    {
                        localVariable.SetVector3(UPref.GetPrefVector3(key));
                    }
                    else if (localVariable.GetVtype() == VTypes.Vector4)
                    {
                        localVariable.SetVector4(UPref.GetPrefVector4(key));
                    }
                }
                else
                {
                    #if UNITY_EDITOR
                    Debug.LogWarning("Key is not exist!. Make sure the key name is correct or previously created.");
                    #endif
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

            if (String.IsNullOrEmpty(key))
            {
                summary += "Key can't be empty!.";
            }

            if (localVariable == null)
            {
                summary += "| Local variable can't be empty!";
            }

            return summary;
        }
    }
}