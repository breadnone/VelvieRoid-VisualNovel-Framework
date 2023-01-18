
using UnityEngine;

namespace VelvieR
{
    public interface IVar
    {
        public string Name{get;set;}
        public int VarId{get;set;}
        public VTypes GetVtype();
        public int Vindex {get;set;}
        public bool IsPublic{get;set;}
        public string SceneGuid{get;set;}
    }

    public interface IGetValue<T>
    {
        public T GetValue();
        public void SetValue(T t);
    }
    public abstract class AnyAbstract
    {
        public VTypes type = VTypes.None;
        public int intVal;
        public string strVal;
        public double doubleVal;
        public float floatVal;
        public bool boolVal;
        public Vector2 vec2Val;
        public Vector3 vec3Val;
        public Vector4 vec4Val;
    }
}