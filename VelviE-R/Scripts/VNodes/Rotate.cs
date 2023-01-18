using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VelvieR
{
    public enum RotateType
    {
        Vector3Left,
        Vector3Right,
        Vector3Forward,
        Vector3Back,
        Vector3Up,
        Vector3Down,
        None
    }
    [VTag("Animation/Rotate", "Rotates gameObject via Vector3 or target transform.", VColor.Red, "Ro")]
    public class Rotate : LeanAbstract
    {
        [field: SerializeField, HideInInspector] public float angle { get; set; }
        [field: SerializeField, HideInInspector] public RotateType rotateType { get; set; } = RotateType.Vector3Left;
        [field: SerializeField, HideInInspector] public bool isLocal { get; set; }
        public override void OnVEnter()
        {
            if(targetobject != null)
            {
                if(enableOnStart)
                {
                    targetobject.SetActive(true);
                }

                if (target != null)
                {
                    to = target.transform.rotation.eulerAngles;
                }

                var act = new Action(()=>
                {
                    if(waitUntilFinished)
                    {
                        OnVContinue();
                    }

                    if(disableOnComplete)
                    {
                        targetobject.SetActive(false);
                    }

                    if(LeanManager.ActiveTweens.Contains(targetobject))
                        LeanManager.ActiveTweens.Remove(targetobject);
                });

                Vector3 rotVal = Vector3.zero;

                if(rotateType == RotateType.Vector3Left)
                {
                    rotVal = Vector3.left;
                }
                else if(rotateType == RotateType.Vector3Right)
                {
                    rotVal = Vector3.right;
                }
                else if(rotateType == RotateType.Vector3Forward)
                {
                    rotVal = Vector3.forward;
                }
                else if(rotateType == RotateType.Vector3Back)
                {
                    rotVal = Vector3.back;
                }
                else if(rotateType == RotateType.Vector3Up)
                {
                    rotVal = Vector3.up;
                }
                else if(rotateType == RotateType.Vector3Down)
                {
                    rotVal = Vector3.down;
                }

                if(!LeanManager.ActiveTweens.Contains(targetobject))
                LeanManager.ActiveTweens.Add(targetobject);
                VExtension.VTweenRotateAround(targetobject, rotVal, angle, duration, loopCount, true, true, act, loopType, ease);
            }
        }
        public override void OnVExit()
        {
            if(!waitUntilFinished)
                OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if(targetobject == null)
            {
                summary += "Object to rotate can't be empty!";
            }

            return summary;
        }
    }
}