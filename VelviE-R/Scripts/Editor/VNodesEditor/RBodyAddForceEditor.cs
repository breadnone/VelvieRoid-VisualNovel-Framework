using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using UnityEngine;
using System;
using System.Linq;

namespace VIEditor
{
    [CustomEditor(typeof(RBodyAddForce))]
    public class RBodyAddForceEditor : Editor
    {
        private VisualElement dummySlot;
        private VisualElement dummy;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as RBodyAddForce;

            dummySlot = new VisualElement();
            dummy = new VisualElement();
            root.Add(DrawRbodyBool(t));
            root.Add(dummy);
            dummy.Add(DrawRbodSlot(t));
            root.Add(dummySlot);

            if (t.is2D)
            {
                dummySlot.Add(DrawRbodyTwoD(t));
                dummySlot.Add(DrawRbodyVecTwo(t));
            }
            else
            {
                dummySlot.Add(DrawRbodyThreeD(t));
                dummySlot.Add(DrawRbodyVecThree(t));
            }

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, t.OnVSummary);
            return root;
        }
        private VisualElement DrawRbodyTwoD(RBodyAddForce t)
        {
            var rootBox = VUITemplate.GetTemplate("RigidBody2D : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new ObjectField();
            objField.objectType = typeof(Rigidbody2D);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.rbody2d;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.rbody2d = x.newValue as Rigidbody2D;
                });
            }

            return rootBox;
        }
        private VisualElement DrawRbodyThreeD(RBodyAddForce t)
        {
            var rootBox = VUITemplate.GetTemplate("RigidBody : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new ObjectField();
            objField.objectType = typeof(Rigidbody);
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.rbody3d;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.rbody3d = objField.value as Rigidbody;
                });
            }

            return rootBox;
        }
        private VisualElement DrawRbodyVecTwo(RBodyAddForce t)
        {
            var rootBox = VUITemplate.GetTemplate("Value : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Vector2Field();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.value2d;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.value2d = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawRbodyVecThree(RBodyAddForce t)
        {
            var rootBox = VUITemplate.GetTemplate("Value : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Vector3Field();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.value3d;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.value3d = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawRbodyBool(RBodyAddForce t)
        {
            var rootBox = VUITemplate.GetTemplate("Is2D : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.is2D;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.is2D = objField.value;

                    if (dummySlot != null && dummySlot.childCount > 0)
                    {
                        foreach (var child in dummySlot.Children().ToList())
                        {
                            child.RemoveFromHierarchy();
                        }
                    }

                    if (dummy != null && dummy.childCount > 0)
                    {
                        foreach (var child in dummy.Children().ToList())
                        {
                            child.RemoveFromHierarchy();
                        }
                    }

                    dummy.Add(DrawRbodSlot(t));

                    if (t.is2D)
                    {
                        dummySlot.Add(DrawRbodyTwoD(t));
                        dummySlot.Add(DrawRbodyVecTwo(t));
                    }
                    else
                    {
                        dummySlot.Add(DrawRbodyThreeD(t));
                        dummySlot.Add(DrawRbodyVecThree(t));
                    }
                });
            }

            return rootBox;
        }
        private VisualElement DrawRbodSlot(RBodyAddForce t)
        {
            var rootBox = VUITemplate.GetTemplate("ForceMode : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);

            if (t.is2D)
            {
                objField.value = t.forcemode2d.ToString();
                objField.choices = Enum.GetNames(typeof(ForceMode2D)).ToList();
            }
            else
            {
                objField.value = t.forcemode3d.ToString();
                objField.choices = Enum.GetNames(typeof(ForceMode)).ToList();
            }

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    if (t.is2D)
                    {
                        foreach (var num in Enum.GetValues(typeof(ForceMode2D)))
                        {
                            var astype = (ForceMode2D)num;
                            if (astype.ToString() == x.newValue)
                            {
                                t.forcemode2d = astype;
                            }
                        }
                    }
                    else
                    {
                        foreach (var num in Enum.GetValues(typeof(ForceMode)))
                        {
                            var astype = (ForceMode)num;
                            if (astype.ToString() == x.newValue)
                            {
                                t.forcemode3d = astype;
                            }
                        }
                    }
                });
            }

            return rootBox;
        }
    }
}