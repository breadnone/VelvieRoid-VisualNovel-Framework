using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VelvieR
{
    [VTag("Events/RBodyConstraint", "Adds constraint to a RigidBody.", VColor.Magenta, "Rc")]
    public class RBodyConstraint : VBlockCore
    {
        [SerializeField, HideInInspector] public Rigidbody rbody3d;
        [SerializeField, HideInInspector] public Rigidbody2D rbody2d;
        [SerializeField, HideInInspector] public bool is2D = false;
        [SerializeField, HideInInspector] public bool isKinematic = false;
        [SerializeField, HideInInspector] public bool detectCollision = true;
        [SerializeField, HideInInspector] public RigidbodyConstraints rbodyConstraint3d = RigidbodyConstraints.None;
        [SerializeField, HideInInspector] public RigidbodyConstraints2D rbodyConstraint2d = RigidbodyConstraints2D.None;
        [SerializeField, HideInInspector] public RigidbodyConstraints rbodyROTConstraint3d = RigidbodyConstraints.None;
        [SerializeField, HideInInspector] public RigidbodyConstraints2D rbodyROTConstraint2d = RigidbodyConstraints2D.None;

        public override void OnVEnter()
        {
            if(is2D)
            {
                rbody2d.constraints = rbodyConstraint2d | rbodyROTConstraint2d;
                rbody2d.isKinematic = isKinematic;
            }
            else
            {
                rbody3d.constraints = rbodyConstraint3d | rbodyROTConstraint3d;
                rbody3d.isKinematic = isKinematic;
                rbody3d.detectCollisions = detectCollision;
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