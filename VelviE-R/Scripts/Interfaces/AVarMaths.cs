using UnityEngine;
using System;
using TMPro;

namespace VelvieR
{
    [System.Serializable]
    public enum MathBasic
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }
    public abstract class AVarMaths : VBlockCore
    {
        [SerializeReference] public IVar variable;
        [SerializeReference] public IVar localVariable;
        [SerializeField] public AnyTypes anyType = new AnyTypes();
        [SerializeField] public TMP_Text text;
        [SerializeField] public MathBasic mathBasics = MathBasic.Add;
        [SerializeField] public bool isLocal;

        public void StartMath()
        {
            if(variable != null)
            {
                if(localVariable != null)
                {
                    if(variable.GetVtype() == localVariable.GetVtype())
                    {
                        CompareVariableAndLocal();
                    }
                    else
                    {
                        throw new Exception("VError : Type mismatch! Two variables must be of the same type!");
                    }
                }
                else if(anyType.type != VTypes.None)
                {
                    CompareVariableAndAnyType();
                }
            }
        }

        public void CompareVariableAndLocal()
        {
            if(variable.GetVtype() == VTypes.Double)
            {
                if(mathBasics == MathBasic.Add)
                    variable.SetDouble(variable.GetDouble() + localVariable.GetDouble());
                else if(mathBasics == MathBasic.Subtract)
                    variable.SetDouble(variable.GetDouble() - localVariable.GetDouble());
                else if(mathBasics == MathBasic.Divide)
                    variable.SetDouble(variable.GetDouble() / localVariable.GetDouble());
                else if(mathBasics == MathBasic.Multiply)
                    variable.SetDouble(variable.GetDouble() * localVariable.GetDouble());

            }
            else if(variable.GetVtype() == VTypes.Float)
            {
                if(mathBasics == MathBasic.Add)
                    variable.SetFloat(variable.GetFloat() + localVariable.GetFloat());
                else if(mathBasics == MathBasic.Subtract)
                    variable.SetFloat(variable.GetFloat() - localVariable.GetFloat());
                else if(mathBasics == MathBasic.Divide)
                    variable.SetFloat(variable.GetFloat() / localVariable.GetFloat());
                else if(mathBasics == MathBasic.Multiply)
                    variable.SetFloat(variable.GetFloat() * localVariable.GetFloat());
            }
            else if(variable.GetVtype() == VTypes.Integer)
            {
                if(mathBasics == MathBasic.Add)
                    variable.SetInt(variable.GetInteger() + localVariable.GetInteger());
                else if(mathBasics == MathBasic.Subtract)
                    variable.SetInt(variable.GetInteger() - localVariable.GetInteger());
                else if(mathBasics == MathBasic.Divide)
                    variable.SetInt(variable.GetInteger() / localVariable.GetInteger());
                else if(mathBasics == MathBasic.Multiply)
                    variable.SetInt(variable.GetInteger() * localVariable.GetInteger());
            }
            else if(variable.GetVtype() == VTypes.Vector2)
            {
                if(mathBasics == MathBasic.Add)
                    variable.SetVector2(variable.GetVector2() + localVariable.GetVector2());
                else if(mathBasics == MathBasic.Subtract)
                    variable.SetVector2(variable.GetVector2() - localVariable.GetVector2());
                else if(mathBasics == MathBasic.Divide)
                    variable.SetVector2(variable.GetVector2() / localVariable.GetVector2());
                else if(mathBasics == MathBasic.Multiply)
                    variable.SetVector2(variable.GetVector2() * localVariable.GetVector2());
            }
            else if(variable.GetVtype() == VTypes.Vector3)
            {
                if(mathBasics == MathBasic.Add)
                    variable.SetVector3(variable.GetVector3() + localVariable.GetVector3());
                else if(mathBasics == MathBasic.Subtract)
                    variable.SetVector3(variable.GetVector3() - localVariable.GetVector3());
                else
                {
                    throw new Exception("VError : Vector3 does not support division or multiplication!.");
                }
            }
            else if(variable.GetVtype() == VTypes.Vector4)
            {
                if(mathBasics == MathBasic.Add)
                    variable.SetVector4(variable.GetVector4() + localVariable.GetVector4());
                else if(mathBasics == MathBasic.Subtract)
                    variable.SetVector4(variable.GetVector4() - localVariable.GetVector4());
                else
                {
                    throw new Exception("VError : Vector4 does not support division or multiplication!.");
                }
            }
        }
        public void CompareVariableAndAnyType()
        {
            if(variable.GetVtype() == VTypes.Double)
            {
                if(mathBasics == MathBasic.Add)
                    variable.SetDouble(variable.GetDouble() + anyType.doubleVal);
                else if(mathBasics == MathBasic.Subtract)
                    variable.SetDouble(variable.GetDouble() - anyType.doubleVal);
                else if(mathBasics == MathBasic.Divide)
                    variable.SetDouble(variable.GetDouble() / anyType.doubleVal);
                else if(mathBasics == MathBasic.Multiply)
                    variable.SetDouble(variable.GetDouble() * anyType.doubleVal);
            }
            else if(variable.GetVtype() == VTypes.Float)
            {
                if(mathBasics == MathBasic.Add)
                    variable.SetFloat(variable.GetFloat() + anyType.floatVal);
                else if(mathBasics == MathBasic.Subtract)
                    variable.SetFloat(variable.GetFloat() - anyType.floatVal);
                else if(mathBasics == MathBasic.Divide)
                    variable.SetFloat(variable.GetFloat() / anyType.floatVal);
                else if(mathBasics == MathBasic.Multiply)
                    variable.SetFloat(variable.GetFloat() * anyType.floatVal);
            }
            else if(variable.GetVtype() == VTypes.Integer)
            {
                if(mathBasics == MathBasic.Add)
                    variable.SetInt(variable.GetInteger() + anyType.intVal);
                else if(mathBasics == MathBasic.Subtract)
                    variable.SetInt(variable.GetInteger() - anyType.intVal);
                else if(mathBasics == MathBasic.Divide)
                    variable.SetInt(variable.GetInteger() / anyType.intVal);
                else if(mathBasics == MathBasic.Multiply)
                    variable.SetInt(variable.GetInteger() * anyType.intVal);
            }
            else if(variable.GetVtype() == VTypes.Vector2)
            {
                if(mathBasics == MathBasic.Add)
                    variable.SetVector2(variable.GetVector2() + anyType.vec2Val);
                else if(mathBasics == MathBasic.Subtract)
                    variable.SetVector2(variable.GetVector2() - anyType.vec2Val);
                else if(mathBasics == MathBasic.Divide)
                    variable.SetVector2(variable.GetVector2() / anyType.vec2Val);
                else if(mathBasics == MathBasic.Multiply)
                    variable.SetVector2(variable.GetVector2() * anyType.vec2Val);
            }
            else if(variable.GetVtype() == VTypes.Vector3)
            {
                if(mathBasics == MathBasic.Add)
                    variable.SetVector3(variable.GetVector3() + anyType.vec3Val);
                else if(mathBasics == MathBasic.Subtract)
                    variable.SetVector3(variable.GetVector3() - anyType.vec3Val);
                else
                {
                    throw new Exception("VError : Vector3 does not support division or multiplication!.");
                }
            }
            else if(variable.GetVtype() == VTypes.Vector4)
            {
                if(mathBasics == MathBasic.Add)
                    variable.SetVector4(variable.GetVector4() + anyType.vec4Val);
                else if(mathBasics == MathBasic.Subtract)
                    variable.SetVector4(variable.GetVector4() - anyType.vec4Val);
                else
                {
                    throw new Exception("VError : Vector4 does not support division or multiplication!.");
                }
            }
        }
    }
}