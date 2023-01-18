using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VelvieR
{
    [VTag("Camera/ZoomInOut", "Zoom in/out a camera.", VColor.Green01, "")]
    public class ZoomInOutCamera : VBlockCore
    {
        [SerializeField, HideInInspector] public Camera cam;
        [SerializeField, HideInInspector] public float to;
        [SerializeField, HideInInspector] public bool animate = false;
        [SerializeField, HideInInspector] public float duration = 1f;
        [SerializeField, HideInInspector] public bool waitUntilFinished = false;

        public override void OnVEnter()
        {
            if(cam != null)
            {
                if(cam.orthographic)
                {
                    if(animate)
                    {
                        LeanTween.value(cam.gameObject, SetCamValue, cam.orthographicSize, to, duration).setOnComplete(()=>
                        {
                            if(waitUntilFinished)
                            {
                                OnVContinue();
                            }
                        });
                    }
                    else
                    {
                        cam.orthographicSize = to;
                    }
                }
                else
                {
                    if(animate)
                    {
                        LeanTween.value(cam.gameObject, SetCamValue, cam.fieldOfView, to, duration).setOnComplete(()=>
                        {
                            if(waitUntilFinished)
                            {
                                OnVContinue();
                            }
                        });
                    }
                    else
                    {
                        cam.fieldOfView = to;
                    }
                }
            }
        }
        private void SetCamValue(float val, float ratio)
        {
            if(cam.orthographic)
            {
                cam.orthographicSize = val;
            }
            else
            {
                cam.fieldOfView = val;
            }
        }
        public override void OnVExit()
        {
            if(animate)
            {
                if(!waitUntilFinished)
                    OnVContinue();
            }
            else
            {
                OnVContinue();
            }
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if(cam == null)
            {
                summary += "Camera can't be empty!";
            }

            return summary;
        }
    }
}