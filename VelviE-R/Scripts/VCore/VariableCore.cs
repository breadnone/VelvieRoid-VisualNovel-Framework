using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace VelvieR
{
    public static class Val
    {
        //NOTE: Mostly for editors
        public static void SetValue(this IVar var, object val)
        {
            //NOTE: Use this, only if System.object used.
            //Only for convenience. Otherwise explicit casting is prefered.

            if(var.GetVtype() == VTypes.Boolean)
            {
                ((VBoolean)var.GetIVar()).value = (bool)val;
            }
            else if(var.GetVtype() == VTypes.Integer)
            {
                ((VInteger)var.GetIVar()).value = (int)val;
            }
            else if(var.GetVtype() == VTypes.Float)
            {
                ((VFloat)var.GetIVar()).value = (float)val;
            }
            else if(var.GetVtype() == VTypes.String)
            {
                ((VString)var.GetIVar()).value = (string)val;
            }
            else if(var.GetVtype() == VTypes.Double)
            {
                ((VDouble)var.GetIVar()).value = (double)val;
            }
            else if(var.GetVtype() == VTypes.Vector2)
            {
                ((VVector2)var.GetIVar()). value = (Vector2)val;
            }
            else if(var.GetVtype() == VTypes.Vector3)
            {
                ((VVector3)var.GetIVar()).value = (Vector3)val;
            }
            else if(var.GetVtype() == VTypes.Vector4)
            {
                ((VVector4)var.GetIVar()).value = (Vector4)val;
            }
        }
        public static void SetValFromAnyType(this IVar val, AnyTypes anyType)
        {
            //NOTE: Use this, only if AnyTypes used. Mostly for editors!
            if(val.GetVtype() == VTypes.Boolean)
            {
                val.SetBool(anyType.boolVal);
            }
            else if(val.GetVtype() == VTypes.Integer)
            {
                val.SetInt(anyType.intVal);
            }
            else if(val.GetVtype() == VTypes.Float)
            {
                val.SetFloat(anyType.floatVal);
            }
            else if(val.GetVtype() == VTypes.String)
            {
                val.SetString(anyType.strVal);
            }
            else if(val.GetVtype() == VTypes.Double)
            {
                val.SetDouble(anyType.doubleVal);
            }
            else if(val.GetVtype() == VTypes.Vector2)
            {
                val.SetVector2(anyType.vec2Val);
            }
            else if(val.GetVtype() == VTypes.Vector3)
            {
                val.SetVector3(anyType.vec3Val);
            }
            else if(val.GetVtype() == VTypes.Vector4)
            {
                val.SetVector4(anyType.vec4Val);
            }
        }
        public static IVar GetIVar(this IVar var)
        {
            return VBlockManager.variables[var.VarId];
        }

        public static int GetInteger(this IVar var)
        {
            return ((IGetValue<int>)var).GetValue();
        }
        public static float GetFloat(this IVar var)
        {
            return ((IGetValue<float>)var).GetValue();
        }
        public static bool GetBool(this IVar var)
        {
            return ((IGetValue<bool>)var).GetValue();
        }
        public static string GetString(this IVar var)
        {
            return ((IGetValue<string>)var).GetValue();
        }
        public static double GetDouble(this IVar var)
        {
            return ((IGetValue<double>)var).GetValue();
        }
        public static Transform GetTransform(this IVar var)
        {
            return ((IGetValue<Transform>)var).GetValue();
        }
        public static GameObject GetGameObject(this IVar var)
        {
            return ((IGetValue<GameObject>)var).GetValue();
        }
        public static Vector2 GetVector2(this IVar var)
        {
            return ((IGetValue<Vector2>)var).GetValue();
        }
        public static Vector3 GetVector3(this IVar var)
        {
            return ((IGetValue<Vector3>)var).GetValue();
        }
        public static Vector4 GetVector4(this IVar var)
        {
            return ((IGetValue<Vector4>)var).GetValue();
        }
        public static void SetInt(this IVar var, int val)
        {
            ((IGetValue<int>)var).SetValue(val);
        }
        public static void SetBool(this IVar var, bool val)
        {
            ((IGetValue<bool>)var).SetValue(val);
        }
        public static void SetFloat(this IVar var, float val)
        {
            ((IGetValue<float>)var).SetValue(val);
        }
        public static void SetDouble(this IVar var, double val)
        {
            ((IGetValue<double>)var).SetValue(val);
        }
        public static void SetString(this IVar var, string val)
        {
            ((VString)var.GetIVar()).value = val;
        }
        public static void SetVector2(this IVar var, Vector2 val)
        {
            ((IGetValue<Vector2>)var).SetValue(val);
        }
        public static void SetVector3(this IVar var, Vector3 val)
        {
            ((IGetValue<Vector3>)var).SetValue(val);
        }
        public static void SetVector4(this IVar var, Vector4 val)
        {
            ((IGetValue<Vector4>)var).SetValue(val);
        }
    }
}