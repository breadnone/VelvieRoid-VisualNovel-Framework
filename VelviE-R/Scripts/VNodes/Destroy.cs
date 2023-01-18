using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VelvieR
{
    [VTag("GameObject/Destroy", "Destroys gameObject in the scene via.", VColor.Green, "Ro")]
    public class Destroy : VBlockCore
    {
        [SerializeField, HideInInspector] public GameObject targetObject;
        [SerializeField, HideInInspector] public float destroyAfter;

        public override void OnVEnter()
        {
            if (targetObject != null)
            {
                Destroy(targetObject, destroyAfter);
            }
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if (targetObject == null)
            {
                summary += "Target object can't be empty!";
            }

            return summary;
        }
    }
}