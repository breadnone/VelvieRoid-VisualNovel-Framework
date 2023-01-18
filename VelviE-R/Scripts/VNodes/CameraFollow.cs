using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VelvieR
{
    [VTag("Camera/CameraFollow", "Camera follows target gameObject in the scene.", VColor.Green02, "Cf")]
    public class CameraFollow : VBlockCore
    {
        [SerializeField, HideInInspector] public Camera cam;
        [SerializeField, HideInInspector] public Transform targetToFollow;
        [SerializeField, HideInInspector] public FollowTypes followType = FollowTypes.FollowDamp;
        [SerializeField, HideInInspector] public LeanProp followProperty = LeanProp.position;
        [SerializeField, HideInInspector] public bool lookAtTarget = true;
        [SerializeField, HideInInspector] public Vector3 offset;
        [SerializeField, HideInInspector] public float smooth = 1f;
        [SerializeField, HideInInspector] public bool enable = true;
        private bool isValid = false;

        void Update()
        {
            if (isValid)
            {
                cam.transform.LookAt(targetToFollow);
            }
        }
        public void StopFollowing()
        {
            isValid = false;
            LeanTween.cancel(cam.gameObject);
        }
        public override void OnVEnter()
        {
            if (!enable && cam != null)
            {
                StopFollowing();
            }

            if(enable)
            {
                if (targetToFollow != null && cam != null)
                {
                    switch (followType)
                    {
                        case FollowTypes.FollowDamp:
                            LeanTween.followDamp(cam.transform, targetToFollow, followProperty, smooth)
                            .setOffset(offset);
                            break;
                        case FollowTypes.FollowSpring:
                            LeanTween.followSpring(cam.transform, targetToFollow, followProperty, smooth)
                            .setOffset(offset);
                            break;
                        case FollowTypes.FollowBounceOut:
                            LeanTween.followBounceOut(cam.transform, targetToFollow, followProperty, smooth)
                            .setOffset(offset);
                            break;
                        case FollowTypes.FollowLinear:
                            LeanTween.followLinear(cam.transform, targetToFollow, followProperty, smooth)
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

            if(cam == null)
            {
                summary += "Camera can't be empty!";
            }

            if(targetToFollow == null)
            {
                summary += "| Target to follow can't be empty!";
            }
            return summary;
        }
    }
}