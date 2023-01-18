using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace VelvieR
{
    public static class VColorAttr
    {
        public static void GetColor(VisualElement vis, VColor vcol)
        {
            switch (vcol)
            {
                case VColor.Magenta:
                    vis.style.backgroundColor = Color.magenta;
                    break;
                case VColor.Yellow:
                    vis.style.backgroundColor = Color.yellow;
                    break;
                case VColor.Yellow01:
                    vis.style.backgroundColor = new StyleColor(new Color(0.694f, 0.740f, 0.104f, 1f));
                    break;
                case VColor.Yellow02:
                    vis.style.backgroundColor = new StyleColor(new Color(0.952f, 0.990f, 0.416f, 1f));
                    break;
                case VColor.Yellow03:
                    vis.style.backgroundColor = new StyleColor(new Color(0.570f, 0.610f, 0.00610f, 1f));
                    break;
                case VColor.Blue:
                    vis.style.backgroundColor = Color.blue;
                    break;
                case VColor.Green:
                    vis.style.backgroundColor = Color.green;
                    break;
                case VColor.Green01:
                    vis.style.backgroundColor = new StyleColor(new Color(0.570f, 1.0f, 0.699f, 1f));
                    break;
                case VColor.Green02:
                    vis.style.backgroundColor = new StyleColor(new Color(0.141f, 0.440f, 0.231f, 1f));
                    break;
                case VColor.Black:
                    vis.style.backgroundColor = Color.black;
                    break;
                case VColor.Clear:
                    vis.style.backgroundColor = Color.clear;
                    break;
                case VColor.Grey:
                    vis.style.backgroundColor = Color.grey;
                    break;
                case VColor.Gray:
                    vis.style.backgroundColor = Color.gray;
                    break;
                case VColor.Red:
                    vis.style.backgroundColor = Color.red;
                    break;
                case VColor.White:
                    vis.style.backgroundColor = Color.white;
                    break;
                case VColor.Pink:
                    vis.style.backgroundColor = new StyleColor(new Color(0.770f, 0.177f, 0.760f, 1f));
                    break;
                case VColor.Pink01:
                    vis.style.backgroundColor = new StyleColor(new Color(1.0f, 0.640f, 0.994f, 1f));
                    break;
            }
        }
    }
} 