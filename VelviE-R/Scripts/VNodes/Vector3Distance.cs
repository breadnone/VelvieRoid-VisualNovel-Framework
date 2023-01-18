using UnityEngine;

namespace VelvieR
{
    [VTag("Maths/Vector3Distance", "Compare distance between two Vector3s.\n\nThe boolean type variable will contain the results.", VColor.Green01, "Su")]
    public class Vector3Distance : VBlockCore
    {
        [SerializeField] public Transform target;
        [SerializeField] public Transform targetPosition;
        [SerializeField] public IVar variable;
        public override void OnVEnter()
        {
            variable.SetBool(Vector3.Distance(target.position, targetPosition.position) < 0.001f);            
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