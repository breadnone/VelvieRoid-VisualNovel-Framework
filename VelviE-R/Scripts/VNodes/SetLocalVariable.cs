using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VelvieR
{

    [VTag("Variables/SetLocalVariable", "Sets value of a variable.", VColor.Yellow03, "Lv")]
    public class SetLocalVariable : VBlockCore
    {
        [SerializeReference, HideInInspector] private IVar variable;
        [SerializeField, HideInInspector] public AnyTypes val = new AnyTypes();
        public IVar Variable {get{return variable;} set{variable = value;}}

        public override void OnVEnter()
        {
            variable.SetValFromAnyType(val);
        }

        public override void OnVExit()
        {
            OnVContinue();
        }
    }
}