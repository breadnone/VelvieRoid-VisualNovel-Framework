using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using VelvieR;
using System.Linq;

namespace VIEditor
{
    [CustomEditor(typeof(Find))]
    public class FindEditor : Editor
    {
        private VisualElement dummyOne;
        private VisualElement dummyVec;
        private VisualElement dummyAnim;
        private VisualElement dummyAngle;
        private VisualElement dummyTarget;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;

            var t = target as Find;
            dummyOne = new VisualElement();
            dummyAngle = new VisualElement();
            dummyTarget = new VisualElement();
            dummyVec = new VisualElement();
            dummyAnim = new VisualElement();
            root.Add(DrawToggleTag(t));

            if (!t.findViaTag)
            {
                dummyOne.Add(DrawName(t));
            }
            else
            {
                dummyOne.Add(DrawTag(t));
            }

            root.Add(dummyOne);
            root.Add(DrawGoProp(t));
            root.Add(dummyVec);
            root.Add(dummyAngle);
            dummyVec.Add(DrawVec(t));
            dummyAngle.Add(DrawAngle(t));
            dummyTarget.Add(DrawTarget(t));
            root.Add(dummyTarget);
            root.Add(DrawToggleAnim(t));
            dummyAnim.Add(DrawDuration(t));
            root.Add(dummyAnim);

            if (t.goProp == GameObjectProperty.SetPosition || t.goProp == GameObjectProperty.SetScale || t.goProp == GameObjectProperty.SetRotation)
            {
                dummyVec.SetEnabled(true);
                dummyTarget.SetEnabled(true);
            }
            else
            {
                dummyVec.SetEnabled(false);
                dummyTarget.SetEnabled(false);
            }

            if (t.goProp == GameObjectProperty.SetRotation)
            {
                dummyAngle.SetEnabled(true);
            }
            else
            {
                dummyAngle.SetEnabled(false);
            }
            if (t.animate)
            {
                dummyAnim.SetEnabled(true);
            }
            else
            {
                dummyAnim.SetEnabled(false);
            }

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        private VisualElement DrawToggleTag(Find t)
        {
            var rootBox = VUITemplate.GetTemplate("Find tag : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            objField.value = t.findViaTag;
            field.Add(objField);

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    foreach (var child in dummyOne.Children().ToList())
                    {
                        child.RemoveFromHierarchy();
                    }

                    t.findViaTag = x.newValue;

                    if (x.newValue)
                    {
                        dummyOne.Add(DrawTag(t));
                    }
                    else
                    {
                        dummyOne.Add(DrawName(t));
                    }
                });
            }

            return rootBox;
        }
        private VisualElement DrawToggleAnim(Find t)
        {
            var rootBox = VUITemplate.GetTemplate("Animate : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            objField.value = t.animate;
            field.Add(objField);

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.animate = x.newValue;

                    if (x.newValue)
                    {
                        dummyAnim.SetEnabled(true);
                    }
                    else
                    {
                        dummyAnim.SetEnabled(false);
                    }
                });
            }

            return rootBox;
        }
        private VisualElement DrawName(Find t)
        {
            var rootBox = VUITemplate.GetTemplate("Name : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new TextField();
            objField.style.width = field.style.width;
            objField.value = t.objectName;
            field.Add(objField);

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.objectName = x.newValue;
                });
            }

            return rootBox;
        }
        private VisualElement DrawDuration(Find t)
        {
            var rootBox = VUITemplate.GetTemplate("Duration : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new FloatField();
            objField.style.width = field.style.width;
            objField.value = t.duration;
            field.Add(objField);

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.duration = x.newValue;
                });
            }

            return rootBox;
        }
        private VisualElement DrawAngle(Find t)
        {
            var rootBox = VUITemplate.GetTemplate("Angle : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new FloatField();
            objField.style.width = field.style.width;
            objField.value = t.angle;
            field.Add(objField);

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.angle = x.newValue;
                });
            }

            return rootBox;
        }
        private VisualElement DrawTag(Find t)
        {
            var rootBox = VUITemplate.GetTemplate("Tag : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new TextField();
            objField.style.width = field.style.width;
            objField.value = t.objectName;
            field.Add(objField);

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.objectName = x.newValue;
                });
            }

            return rootBox;
        }
        private VisualElement DrawGoProp(Find t)
        {
            var rootBox = VUITemplate.GetTemplate("Operation : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new DropdownField();
            objField.style.width = field.style.width;
            objField.value = t.goProp.ToString();
            field.Add(objField);

            if (!PortsUtils.PlayMode)
            {
                objField.choices = Enum.GetNames(typeof(GameObjectProperty)).ToList();
                objField.RegisterCallback<ChangeEvent<string>>((x) =>
                {
                    foreach (var asEnum in Enum.GetValues(typeof(GameObjectProperty)))
                    {
                        var asetype = (GameObjectProperty)asEnum;

                        if (x.newValue == asetype.ToString())
                        {
                            t.goProp = asetype;
                        }

                        if (t.goProp == GameObjectProperty.SetPosition || t.goProp == GameObjectProperty.SetScale || t.goProp == GameObjectProperty.SetRotation)
                        {
                            dummyVec.SetEnabled(true);
                            dummyTarget.SetEnabled(true);
                        }
                        else
                        {
                            dummyVec.SetEnabled(false);
                            dummyTarget.SetEnabled(false);
                            t.target = null;
                        }

                        if (t.goProp == GameObjectProperty.SetRotation)
                        {
                            dummyAngle.SetEnabled(true);
                        }
                        else
                        {
                            dummyAngle.SetEnabled(false);
                        }
                    }
                });
            }

            return rootBox;
        }
        private VisualElement DrawVec(Find t)
        {
            var rootBox = VUITemplate.GetTemplate("Set to : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Vector3Field();
            objField.style.width = field.style.width;
            objField.value = t.vec;
            field.Add(objField);

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.vec = x.newValue;
                });
            }

            return rootBox;
        }
        private VisualElement DrawTarget(Find t)
        {
            var rootBox = VUITemplate.GetTemplate("Target : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(Transform);
            objField.style.width = field.style.width;
            objField.value = t.target;
            field.Add(objField);

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.target = x.newValue as Transform;
                });
            }

            return rootBox;
        }
    }
}