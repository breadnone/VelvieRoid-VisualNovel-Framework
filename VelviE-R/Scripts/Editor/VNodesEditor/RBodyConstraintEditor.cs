using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using UnityEngine;
using System;
using System.Linq;

namespace VIEditor
{
    [CustomEditor(typeof(RBodyConstraint))]
    public class RBodyConstraintEditor : Editor
    {
        private VisualElement dummySlot;
        private VisualElement dummy;
        private VisualElement dummyLast;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as RBodyConstraint;

            dummySlot = new VisualElement();
            dummy = new VisualElement();
            dummyLast = new VisualElement();

            root.Add(DrawRbodyBool(t));
            root.Add(dummy);
            dummy.Add(DrawRbodSlot(t));
            dummy.Add(DrawRbodSlotROT(t));
            root.Add(dummySlot);
            root.Add(dummyLast);

            if (t.is2D)
            {
                dummySlot.Add(DrawRbodyTwoD(t));
            }
            else
            {
                dummySlot.Add(DrawRbodyThreeD(t));
                dummyLast.Add(DrawBoolCollision(t));
            }

            root.Add(DrawBoolIsKinematic(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, t.OnVSummary);
            return root;
        }
        private VisualElement DrawRbodyTwoD(RBodyConstraint t)
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
        private VisualElement DrawRbodyThreeD(RBodyConstraint t)
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
        private VisualElement DrawBoolCollision(RBodyConstraint t)
        {
            var rootBox = VUITemplate.GetTemplate("DetectCollission : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.detectCollision;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.detectCollision = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawBoolIsKinematic(RBodyConstraint t)
        {
            var rootBox = VUITemplate.GetTemplate("IsKinematic : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.isKinematic;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.isKinematic = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawRbodyBool(RBodyConstraint t)
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

                    if (dummyLast != null && dummyLast.childCount > 0)
                    {
                        foreach (var child in dummyLast.Children().ToList())
                        {
                            child.RemoveFromHierarchy();
                        }
                    }

                    dummy.Add(DrawRbodSlot(t));
                    dummy.Add(DrawRbodSlotROT(t));

                    if (t.is2D)
                    {
                        dummySlot.Add(DrawRbodyTwoD(t));
                    }
                    else
                    {
                        dummySlot.Add(DrawRbodyThreeD(t));
                        dummyLast.Add(DrawBoolCollision(t));
                    }

                });
            }

            return rootBox;
        }
        private VisualElement DrawRbodSlot(RBodyConstraint t)
        {
            var rootBox = VUITemplate.GetTemplate("ConstraintPosition : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);

            if (t.is2D)
            {
                objField.value = t.rbodyConstraint2d.ToString();
                objField.choices = Enum.GetNames(typeof(RigidbodyConstraints2D)).Where(x => x.Contains("Position") || x.Equals("FreezePosition") || x.Equals("None")).ToList();
            }
            else
            {
                objField.value = t.rbodyConstraint3d.ToString();
                objField.choices = Enum.GetNames(typeof(RigidbodyConstraints)).Where(x => x.Contains("Position") || x.Equals("FreezePosition") || x.Equals("None")).ToList();
            }

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    if (t.is2D)
                    {
                        foreach (var num in Enum.GetValues(typeof(RigidbodyConstraints2D)))
                        {
                            var astype = (RigidbodyConstraints2D)num;

                            if (astype.ToString() == x.newValue)
                            {
                                t.rbodyConstraint2d = astype;
                            }
                        }
                    }
                    else
                    {
                        foreach (var num in Enum.GetValues(typeof(RigidbodyConstraints)))
                        {
                            var astype = (RigidbodyConstraints)num;

                            if (astype.ToString() == x.newValue)
                            {
                                t.rbodyConstraint3d = astype;
                            }
                        }
                    }
                });
            }

            return rootBox;
        }
        private VisualElement DrawRbodSlotROT(RBodyConstraint t)
        {
            var rootBox = VUITemplate.GetTemplate("ConstraintRotation : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);

            if (t.is2D)
            {
                objField.value = t.rbodyROTConstraint2d.ToString();
                objField.choices = Enum.GetNames(typeof(RigidbodyConstraints2D)).Where(x => x.Contains("Rotation") || x.Equals("FreezeRotation") || x.Equals("None")).ToList();
            }
            else
            {
                objField.value = t.rbodyROTConstraint3d.ToString();
                objField.choices = Enum.GetNames(typeof(RigidbodyConstraints)).Where(x => x.Contains("Rotation") || x.Equals("FreezeRotation") || x.Equals("None")).ToList();
            }

            objField.RegisterValueChangedCallback((x) =>
            {
                if (t.is2D)
                {
                    foreach (var num in Enum.GetValues(typeof(RigidbodyConstraints2D)))
                    {
                        var astype = (RigidbodyConstraints2D)num;
                        if (astype.ToString() == x.newValue)
                        {
                            t.rbodyROTConstraint2d = astype;
                        }
                    }
                }
                else
                {
                    foreach (var num in Enum.GetValues(typeof(RigidbodyConstraints)))
                    {
                        var astype = (RigidbodyConstraints)num;
                        if (astype.ToString() == x.newValue)
                        {
                            t.rbodyROTConstraint3d = astype;
                        }
                    }
                }
            });

            return rootBox;
        }
    }
}