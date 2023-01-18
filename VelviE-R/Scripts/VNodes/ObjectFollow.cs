using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VelvieR
{
    public enum FollowTypes
    {
        FollowDamp,
        FollowSpring,
        FollowBounceOut,
        FollowLinear
    }
    [VTag("GameObject/ObjectFollow", "Follows target gameObject.", VColor.Blue, "Of")]
    public class ObjectFollow : VBlockCore
    {
        [SerializeField, HideInInspector] public Transform targetToFollow;
        [SerializeField, HideInInspector] public Transform objectFollowing;
        [SerializeField, HideInInspector] public FollowTypes followType = FollowTypes.FollowDamp;
        [SerializeField, HideInInspector] public LeanProp followProperty = LeanProp.position;
        [SerializeField, HideInInspector] public bool lookAtTarget = false;
        [SerializeField, HideInInspector] public Vector3 offset;
        [SerializeField, HideInInspector] public RotateType vecType = RotateType.None;
        [SerializeField, HideInInspector] public float smooth = 1f;
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
        public void StopFollowing()
        {
            isValid = false;
            LeanTween.cancel(objectFollowing.gameObject);
        }
        public override void OnVEnter()
        {
            if (!enable && targetToFollow != null)
            {
                StopFollowing();
            }
            
            if(enable)
            {
                if (targetToFollow != null && objectFollowing != null)
                {
                    if (lookAtTarget)
                    {
                        isValid = true;

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
                        else if(vecType == RotateType.Vector3Down)
                        {
                            vectorType = Vector3.down;
                        }
                        else
                        {
                            vectorType = Vector3.zero;
                        }
                    }

                    switch (followType)
                    {
                        case FollowTypes.FollowDamp:
                            LeanTween.followDamp(objectFollowing, targetToFollow, followProperty, smooth)
                            .setOffset(offset);
                            break;
                        case FollowTypes.FollowSpring:
                            LeanTween.followSpring(objectFollowing, targetToFollow, followProperty, smooth)
                            .setOffset(offset);
                            break;
                        case FollowTypes.FollowBounceOut:
                            LeanTween.followBounceOut(objectFollowing, targetToFollow, followProperty, smooth)
                            .setOffset(offset);
                            break;
                        case FollowTypes.FollowLinear:
                            LeanTween.followLinear(objectFollowing, targetToFollow, followProperty, smooth)
                            .setOffset(offset);
                            break;
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

            if (targetToFollow == null)
            {
                summary += "Target to follow can't be empty!";
            }
            if (objectFollowing == null)
            {
                summary += "| Object following can't be empty!";
            }

            return summary;
        }
    }
}