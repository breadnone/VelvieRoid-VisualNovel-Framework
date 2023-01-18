using System;
using UnityEngine;

namespace VelvieR
{
    public enum VColor
    {
        Magenta,
        Yellow,
        Yellow01,
        Yellow02,
        Yellow03,
        Blue,
        Green,
        Green01,
        Green02,
        Black,
        Clear,
        Grey,
        Gray,
        Red,
        White,
        Pink,
        Pink01,
        None
    }
    public enum VDelay
    {
        OnEnter,
        OnExit,
        None
    }
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct, AllowMultiple = true)]
    public class VTagAttribute : Attribute
    {
        public string vheaderattr;
        public string vtooltip;
        public VColor vcolor = VColor.Grey;
        public readonly string componentId;
        public string symbol;
        public string name;
        public VTagAttribute(string header, string tooltip, VColor vcolor, string symb = "")
        {
            this.vheaderattr = header;
            this.vtooltip = tooltip;
            this.vcolor = vcolor;

            if(String.IsNullOrEmpty(symb))
                this.symbol = "||";
            else
                this.symbol = symb;
                
            this.componentId = Guid.NewGuid().ToString() + (int)UnityEngine.Random.Range(0, int.MaxValue);
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
    public class VTagDelay : Attribute
    {
        public VDelay delayType = VDelay.None;
        public float delayTime = 0;
        public VTagDelay(float time, VDelay delayType)
        {
            this.delayType = delayType;
            this.delayTime = time;
        }
    }
    public class VTagRpm : Attribute
    {
        public float delayTime = 0;
        public VTagRpm(float time)
        {
            this.delayTime = time;
        }
    }
}