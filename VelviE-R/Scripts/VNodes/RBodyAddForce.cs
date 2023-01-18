using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VelvieR
{
    [VTag("Events/RBodyAddForce", "Adds force to a RigidBody.", VColor.Magenta, "Ra")]
    public class RBodyAddForce : VBlockCore
    {
        [SerializeField, HideInInspector] public Rigidbody rbody3d;
        [SerializeField, HideInInspector] public Rigidbody2D rbody2d;
        [SerializeField, HideInInspector] public Vector2 value2d;
        [SerializeField, HideInInspector] public Vector3 value3d;
        [SerializeField, HideInInspector] public ForceMode forcemode3d = ForceMode.Force;
        [SerializeField, HideInInspector] public ForceMode2D forcemode2d = ForceMode2D.Force;
        [SerializeField, HideInInspector] public bool is2D = false;

        public override void OnVEnter()
        {
            if(is2D)
            {
                rbody2d.AddForce(value2d, forcemode2d);
            }
            else
            {
                rbody3d.AddForce(value3d, forcemode3d);
            }
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if (is2D && rbody2d == null)
            {
                summary += "Rigidbody can't be empty!";
            }
            else if(!is2D && rbody3d == null)
            {
                summary += "Rigidbody can't be empty";
            }

            return summary;
        }
    }
}