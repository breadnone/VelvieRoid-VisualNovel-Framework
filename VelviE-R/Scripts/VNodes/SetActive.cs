using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VelvieR
{
    [VTag("GameObject/SetActive", "Sets active state of a gameObject in the scene.", VColor.Green, "Sa")]
    public class SetActive : VBlockCore
    {
        [SerializeField] public List<GameObject> targetObject = new List<GameObject>();
        [SerializeField] public bool activeState;
        public override void OnVEnter()
        {
            if(targetObject.Count > 0)
            {
                for(int i = 0; i < targetObject.Count; i++)
                {
                    if(targetObject[i] != null)
                    {
                        targetObject[i].SetActive(activeState);
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

            if(targetObject.Count == 0)
            {
                summary += "Terget object can't be empty!";
            }

            return summary;
        }
    }
}