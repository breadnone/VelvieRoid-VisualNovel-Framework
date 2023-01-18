using UnityEngine;

namespace VelvieR
{
    [VTag("GameObject/DistanceBetween", "Get the distance between two gameObjects. Returns float.", VColor.Yellow, "Db")]
    public class DistanceBetween : VBlockCore
    {
        [SerializeField] public Transform thisTarget;
        [SerializeField] public Transform thatTarget;
        [SerializeReference] public IVar variable;
        [SerializeField] public bool isVector3 = true;

        public override void OnVEnter()
        {
            if(isVector3)
                variable.SetFloat(Vector3.Distance(thisTarget.position, thatTarget.position));
            else
                variable.SetFloat(Vector2.Distance(thisTarget.position, thatTarget.position));
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