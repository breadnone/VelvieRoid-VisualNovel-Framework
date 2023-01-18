using UnityEngine;

namespace VelvieR
{
    [VTag("GameObject/IsFacingAt", "Checks gameObject if it's facing another target object. Accepts Vector3, returns boolean.", VColor.Yellow, "Fa")]
    public class IsFacingAt : VBlockCore
    {
        [SerializeField] public Transform thisTarget;
        [SerializeField] public Transform thatTarget;
        [SerializeReference] public IVar variable;
        public override void OnVEnter()
        {
            float dot = Vector3.Dot(thisTarget.forward, (thatTarget.position - thisTarget.position).normalized);

            if(dot > 0.7f) 
            { 
                variable.SetBool(true);
            }
            else
            {
                variable.SetBool(false);
            }
            
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if(thisTarget == null || thatTarget == null)
            {
                summary += "Both target objects can't be empty!";
            }

            return summary;
        }
    }
}