using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VelvieR
{
    [VTag("GameObject/LookAt", "Faces target gameObject.", VColor.Pink, "Of")]
    public class LookAt : VBlockCore
    {
        [SerializeField, HideInInspector] public Transform targetToFollow;
        [SerializeField, HideInInspector] public Transform objectFollowing;
        [SerializeField, HideInInspector] public RotateType vecType = RotateType.Vector3Left;
        [SerializeField, HideInInspector] public bool enable = true;
        private Vector3 vectorType;
        private bool isValid = false;

        void Update()
        {
            if (isValid)
            {
                objectFollowing.LookAt(targetToFollow, vectorType);
            }
        }
        public void StopLookingAt()
        {
            isValid = false;
            LeanTween.cancel(objectFollowing.gameObject);
        }
        public override void OnVEnter()
        {
            if (!enable && targetToFollow != null)
            {
                StopLookingAt();
            }
            else
            {
                if (targetToFollow != null && objectFollowing != null)
                {
                    if (vecType == RotateType.Vector3Left)
                    {
                        vectorType = Vector3.left;
                    }
                    else if (vecType == RotateType.Vector3Right)
                    {
                        vectorType = Vector3.right;
                    }
                    else if (vecType == RotateType.Vector3Forward)
                    {
                        vectorType = Vector3.forward;
                    }
                    else if (vecType == RotateType.Vector3Back)
                    {
                        vectorType = Vector3.back;
                    }
                    else if (vecType == RotateType.Vector3Up)
                    {
                        vectorType = Vector3.up;
                    }
                    else if (vecType == RotateType.Vector3Down)
                    {
                        vectorType = Vector3.down;
                    }
                    else
                    {
                        vectorType = Vector3.zero;
                    }

                    isValid = true;
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

            if (targetToFollow == null)
            {
                summary += "Target to look at can't be empty!";
            }
            if (objectFollowing == null)
            {
                summary += "| Object that's looking can't be empty!";
            }

            return summary;
        }
    }
}