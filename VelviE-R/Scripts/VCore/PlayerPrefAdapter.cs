using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

namespace VelvieR
{
    public static class UPref
    {
        public static void SavePlayerPrefs(string key, AnyTypes any)
        {
            if (!String.IsNullOrEmpty(key))
            {
                var type = any.type;

                if (type == VTypes.Boolean)
                {
                    string b = "0";

                    if(any.boolVal)
                    {
                        b = "1" + " " + "||v||bool";
                    }

                    PlayerPrefs.SetString(key, b);
                }
                else if (type == VTypes.Float)
                {
                    PlayerPrefs.SetFloat(key, any.floatVal);
                }
                else if (type == VTypes.Integer)
                {
                    PlayerPrefs.SetInt(key, any.intVal);
                }
                else if(type == VTypes.String)
                {
                    PlayerPrefs.SetString(key, any.strVal);
                }
                else if(type == VTypes.Double)
                {
                    string db = string.Empty;
                    db = any.doubleVal + "||v||double";                    
                    PlayerPrefs.SetString(key, db);
                }
                else if(type == VTypes.Vector2)
                {
                    string db = string.Empty;
                    db = any.vec2Val.x + " " + any.vec2Val.y + "||v||vector2";
                    PlayerPrefs.SetString(key, db);
                }
                else if(type == VTypes.Vector3)
                {
                    string db = string.Empty;
                    db = any.vec3Val.x + " " + any.vec3Val.y + " " + any.vec3Val.z + "||v||vector3";
                    PlayerPrefs.SetString(key, db);
                }
                else if(type == VTypes.Vector4)
                {
                    string db = string.Empty;
                    db = any.vec4Val.x + " " + any.vec4Val.y + " " + any.vec4Val.z + " " + any.vec4Val.w + "||v||vector4";
                    PlayerPrefs.SetString(key, db);
                }
            }
        }

        public static string GetPrefString(string key)
        {
            return PlayerPrefs.GetString(key);
        }
        public static bool GetPrefBoolean(string key)
        {
            var str = PlayerPrefs.GetString(key);
            var split = str.Split("||v||");
            string result = string.Empty;

            if(split[split.Length - 1] == "bool")
            {
                if(split[0] == "1")
                {
                    return true;
                }
            }

            return false;
        }
        public static int GetPrefInt(string key)
        {
            return PlayerPrefs.GetInt(key);
        }
        public static float GetPrefFloat(string key)
        {
            return PlayerPrefs.GetFloat(key);
        }
        public static Vector2 GetPrefVector2(string key)
        {
            var str = PlayerPrefs.GetString(key);
            var split = str.Split("||v||");
            var finalSplit = str.Split(' ');

            if(split[split.Length - 1] == "vector2")
            {
                return new Vector2((float)System.Convert.ToSingle(finalSplit[0]), (float)System.Convert.ToSingle(finalSplit[1]));
            }

            return new Vector2();
        }
        public static Vector3 GetPrefVector3(string key)
        {
            var str = PlayerPrefs.GetString(key);
            var split = str.Split("||v||");
            var finalSplit = str.Split(' ');

            if(split[split.Length - 1] == "vector3")
            {
                return new Vector3((float)System.Convert.ToSingle(finalSplit[0]), (float)System.Convert.ToSingle(finalSplit[1]), (float)System.Convert.ToSingle(finalSplit[2]));
            }

            return new Vector3();
        }
        public static Vector4 GetPrefVector4(string key)
        {
            var str = PlayerPrefs.GetString(key);
            var split = str.Split("||v||");
            var finalSplit = str.Split(' ');

            if(split[split.Length - 1] == "vector4")
            {
                return new Vector4((float)System.Convert.ToSingle(finalSplit[0]), (float)System.Convert.ToSingle(finalSplit[1]), (float)System.Convert.ToSingle(finalSplit[2]), (float)System.Convert.ToSingle(finalSplit[3]));
            }

            return new Vector4();
        }
        public static double GetPrefDoubble(string key)
        {
            var str = PlayerPrefs.GetString(key);
            var split = str.Split("||v||");

            if(split[split.Length - 1] == "double")
            {
                return (double)System.Convert.ToSingle(split[0]);
            }

            return 0;
        }
    }
}