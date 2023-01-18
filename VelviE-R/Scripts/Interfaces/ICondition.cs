using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace VelvieR
{
    [System.Serializable]
    public enum EnumCondition
    {
        NotEqual,
        Equal,
        Contains,
        EqualCaseInsensitive,
        StartsWith,
        EndsWith,
        Length,
        DistanceEqual,
        DistanceNotEqual,
        NameEqual,
        NameNotEqual,
        InstanceIdEqual,
        InstanceIdNotEqual,
        PositionEqual,
        PositionNotEqual,
        ScaleEqual,
        ScaleNotEqual,
        True,
        False,
        BiggerThan,
        SmallerThan,
        BiggerThanEqual,
        SmallerThanEqual,
        LocalPositionEqual,
        LocalPositionNotEqual,
        VListContains,
        VListExist,
        None
    }
    public class ICondition
    {
        public virtual bool VStartCompare(IVar variable, IVar local, EnumCondition condition, AnyTypes anyType = null)
        {
            if(variable != null)
            {
                variable = variable.GetIVar();
            }

            if(local != null)
            {
                local = local.GetIVar();
            }

            if(variable != null && condition != EnumCondition.None)
            {
                if (variable.GetVtype() == VTypes.String)
                {
                    string val = null;

                    if(anyType != null)
                    {
                        val = anyType.strVal;
                    }
                    else
                    {
                        val = local.GetString();
                    }

                    return VCompareString(variable.GetString(), val, condition);
                }
                else if (variable.GetVtype() == VTypes.Float || variable.GetVtype() == VTypes.Double || variable.GetVtype() == VTypes.Integer)
                {
                    if(variable.GetVtype() == VTypes.Float)
                    {
                        float val = 0f;

                        if(anyType != null)
                        {
                            val = anyType.floatVal;
                        }
                        else
                        {
                            val = local.GetFloat();
                        }

                        return VCompareFloat(variable.GetFloat(), val, condition);
                    }
                    else if(variable.GetVtype() == VTypes.Double)
                    {
                        double val = 0;

                        if(anyType != null)
                        {
                            val = anyType.doubleVal;
                        }
                        else
                        {
                            val = local.GetDouble();
                        }

                        return VCompareDouble(variable.GetDouble(), val, condition);
                    }
                    else
                    {
                        int val = 0;

                        if(anyType != null)
                        {
                            val = anyType.intVal;
                        }
                        else
                        {
                            val = local.GetInteger();
                        }

                        return VCompareInteger(variable.GetInteger(), val, condition);
                    }
                }
                else if (variable.GetVtype() == VTypes.Boolean)
                {
                        bool val = false;

                        if(anyType != null)
                        {
                            val = anyType.boolVal;
                        }
                        else
                        {
                            val = local.GetBool();
                        }

                    return VCompareBool(variable.GetBool(), val, condition);
                }
                else if (variable.GetVtype() == VTypes.Vector2 || variable.GetVtype() == VTypes.Vector3 || variable.GetVtype() == VTypes.Vector4)
                {
                    if(variable.GetVtype() == VTypes.Vector2)
                    {
                        Vector2 val = Vector2.zero;

                        if(anyType != null)
                        {
                            val = anyType.vec2Val;
                        }
                        else
                        {
                            val = local.GetVector2();
                        }

                        return VCompareVectors(variable.GetVector2(), val, condition, vec2: true);
                    }
                    else if(variable.GetVtype() == VTypes.Vector3)
                    {
                        Vector3 val = Vector3.zero;

                        if(anyType != null)
                        {
                            val = anyType.vec3Val;
                        }
                        else
                        {
                            val = local.GetVector3();
                        }

                        return VCompareVectors(variable.GetVector3(), val, condition, vec3: true);
                    }
                    else
                    {
                        Vector4 val = Vector4.zero;

                        if(anyType != null)
                        {
                            val = anyType.vec4Val;
                        }
                        else
                        {
                            val = local.GetVector4();
                        }

                        return VCompareVectors(variable.GetVector4(), val, condition, vec4: true);
                    }
                }
            }

            return false;
        }
        public virtual bool VCompareBool(bool variable, bool local, EnumCondition condition)
        {
            return variable == local;
        }
        public virtual bool VCompareString(string variable, string local, EnumCondition condition)
        {
            if(condition == EnumCondition.Equal)
            {
                return variable == local;
            }
            else if(condition == EnumCondition.NotEqual)
            {
                return variable != local;
            }
            else if(condition == EnumCondition.EqualCaseInsensitive)
            {
                var varOne = variable.ToLower();
                var varTwo = local.ToLower();
                return varOne == varTwo;
            }
            else if(condition == EnumCondition.StartsWith)
            {
                var varOne = variable[0];
                var varTwo = local[0];
                return varOne == varTwo;
            }
            else if(condition == EnumCondition.EndsWith)
            {
                var varOne = variable[variable.Length - 1];
                var varTwo = local[local.Length - 1];
                return varOne == varTwo;
            }
            else if(condition == EnumCondition.Length)
            {
                return variable.Length == local.Length;
            }

            return false;
        }
        public virtual bool VCompareFloat(float variable, float local, EnumCondition condition)
        {
            if(condition == EnumCondition.Equal)
            {
                return Mathf.Approximately(variable, local);
            }
            else if(condition == EnumCondition.NotEqual)
            {
                return !Mathf.Approximately(variable, local);
            }
            else if(condition == EnumCondition.BiggerThan)
            {
                var tmpval = Mathf.Max(variable, local);

                if(Mathf.Approximately(tmpval, local))
                    return true;
                else
                    return false;
            }
            else if(condition == EnumCondition.SmallerThan)
            {
                var tmpval = Mathf.Min(variable, local);
                
                if(Mathf.Approximately(tmpval, local))
                    return true;
                else
                    return false;
            }
            else if(condition == EnumCondition.SmallerThanEqual)
            {
                var tmpval = Mathf.Min(variable, local);
                
                if(Mathf.Approximately(tmpval, variable) || Mathf.Approximately(variable, local))
                    return true;
                else
                    return false;
            }
            else if(condition == EnumCondition.BiggerThanEqual)
            {
                var tmpval = Mathf.Max(variable, local);

                if(Mathf.Approximately(tmpval, variable) || Mathf.Approximately(variable, local))
                    return true;
                else
                    return false;
            }

            return false;
        }
        public virtual bool VCompareInteger(int variable, int local, EnumCondition condition)
        {
            if(condition == EnumCondition.Equal)
            {
                return variable == local;
            }
            else if(condition == EnumCondition.NotEqual)
            {
                return variable != local;
            }
            else if(condition == EnumCondition.BiggerThan)
            {
                return variable > local;
            }
            else if(condition == EnumCondition.SmallerThan)
            {
                return variable < local;
            }
            else if(condition == EnumCondition.SmallerThanEqual)
            {
                return variable <= local;
            }
            else if(condition == EnumCondition.BiggerThanEqual)
            {
                return variable >= local;
            }

            return false;
        }
        public virtual bool VCompareDouble(double variable, double local, EnumCondition condition)
        {
            if(condition == EnumCondition.Equal)
            {
                var res = variable.CompareTo(local);

                if(res == 0)
                    return true;
            }
            else if(condition == EnumCondition.NotEqual)
            {
                var res = variable.CompareTo(local);

                if(res != 0)
                    return true;
            }
            else if(condition == EnumCondition.BiggerThan)
            {
                var res = variable.CompareTo(local);

                if(res != 0 && res != 1)
                    return true;
            }
            else if(condition == EnumCondition.SmallerThan)
            {
                var res = variable.CompareTo(local);

                if(res != 0 && res == 1)
                    return true;
            }
            else if(condition == EnumCondition.SmallerThanEqual)
            {
                var res = variable.CompareTo(local);

                if(res == 1 || res == 0)
                    return true;
            }
            else if(condition == EnumCondition.BiggerThanEqual)
            {
                var res = variable.CompareTo(local);

                if(res != 1 || res == 0)
                    return true;
            }

            return false;
        }
        public virtual bool VCompareTransform(Transform variable, Transform local, EnumCondition condition)
        {
            if(condition == EnumCondition.ScaleEqual)
            {
                return Vector3.Distance(variable.localScale, local.localScale) >= 0.0001;
            }
            else if(condition == EnumCondition.ScaleNotEqual)
            {
                var t = Vector3.Distance(variable.position, local.position) >= 0.0001;
                return !t;
            }
            else if(condition == EnumCondition.LocalPositionEqual)
            {
                var t = Vector3.Distance(variable.localPosition, local.localPosition) >= 0.0001;
                return t;
            }
            else if(condition == EnumCondition.LocalPositionNotEqual)
            {
                var t = Vector3.Distance(variable.localPosition, local.localPosition) >= 0.0001;
                return !t;
            }

            return false;
        }
        public virtual bool VCompareVectors(Vector4 variable, Vector4 local, EnumCondition condition, bool vec2 = false, bool vec3 = false, bool vec4 = false)
        {
            if(vec2)
            {
                if(condition == EnumCondition.Equal)
                {
                    var t = Vector2.Distance(new Vector2(variable.x, variable.y), new Vector2(local.x, local.y)) >= 0.0001;
                    return t;
                }
                else if(condition != EnumCondition.NotEqual)
                {
                    var t = Vector2.Distance(new Vector2(variable.x, variable.y), new Vector2(local.x, local.y)) >= 0.0001;
                    return !t;
                }
            }
            else if(vec3)
            {
                if(condition == EnumCondition.Equal)
                {
                    var t = Vector3.Distance(new Vector3(variable.x, variable.y, variable.z), new Vector3(local.x, local.y, local.z)) >= 0.0001;
                    return t;
                }
                else if(condition != EnumCondition.NotEqual)
                {
                    var t = Vector3.Distance(new Vector3(variable.x, variable.y, variable.z), new Vector3(local.x, local.y, local.z)) >= 0.0001;
                    return !t;
                }
            }
            else if(vec4)
            {
                if(condition == EnumCondition.Equal)
                {
                    var t = Vector4.Distance(variable, local) >= 0.0001;
                    return t;
                }
                else if(condition == EnumCondition.NotEqual)
                {
                    var t = Vector4.Distance(variable, local) >= 0.0001;
                    return !t;
                }
            }

            return false;
        }
    }
}