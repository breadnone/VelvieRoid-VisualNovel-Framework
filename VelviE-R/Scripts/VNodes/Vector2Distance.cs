using UnityEngine;

namespace VelvieR
{
    [VTag("Maths/Vector2Distance", "Compare distance between two Vector2s.\n\nThe boolean type variable will contain the results.", VColor.Green01, "Vd")]
    public class Vector2Distance : VBlockCore
    {
        [SerializeField] public Transform target;
        [SerializeField] public Transform targetPosition;
        [SerializeField] public IVar variable;
        public override void OnVEnter()
        {
            variable.SetBool(Vector2.Distance(target.position, targetPosition.position) < 0.001f);            
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if(target == null || targetPosition == null)
            {
                summary += "Target and targetPosition can't be empty!";
            }

            return summary;
        }
    }
}