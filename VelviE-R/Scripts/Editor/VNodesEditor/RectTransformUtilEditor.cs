using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using System;
using System.Linq;

namespace VIEditor
{
    [CustomEditor(typeof(RectTransformUtil))]
    public class RectTransformUtilEditor : Editor
    {
        private VisualElement dummySlot;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;
            var t = target as RectTransformUtil;
            dummySlot = new VisualElement();

            root.Add(DrawRect(t));
            root.Add(DrawRectTypes(t));
            root.Add(dummySlot);

            ReDraw(t);

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        private void ReDraw(RectTransformUtil t)
        {
            if (t.rectUtil == RectUtilv.RectIsOverlap)
            {
                dummySlot.Add(DrawRectTarget(t));
            }
            else if (t.rectUtil == RectUtilv.FlipX || t.rectUtil == RectUtilv.FlixY)
            {
                dummySlot.Add(DrawRecDuration(t));
                dummySlot.Add(DrawRectAnime(t));
                dummySlot.Add(DrawRectClamp(t));
                dummySlot.Add(DrawRectDisable(t));
                dummySlot.Add(DrawRecLoopCount(t));
                dummySlot.Add(DrawRectWait(t));
            }
            else if (t.rectUtil == RectUtilv.WidthHeight)
            {
                dummySlot.Add(DrawRectVector(t));
            }
        }
        private VisualElement DrawRect(RectTransformUtil t)
        {
            var rootBox = VUITemplate.GetTemplate("RectTransform : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new ObjectField();
            objField.objectType = typeof(RectTransform);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.rect;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.rect = objField.value as RectTransform;
                });
            }

            return rootBox;
        }
        private VisualElement DrawRectTypes(RectTransformUtil t)
        {
            var rootBox = VUITemplate.GetTemplate("Utility : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.rectUtil.ToString();
            objField.choices = Enum.GetNames(typeof(RectUtilv)).ToList();

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterCallback<ChangeEvent<string>>((x) =>
                {
                    foreach (var asEnum in Enum.GetValues(typeof(RectUtilv)))
                    {
                        var asetype = (RectUtilv)asEnum;

                        if (asetype.ToString() == x.newValue)
                        {
                            t.rectUtil = asetype;

                            if (dummySlot.childCount > 0)
                            {
                                foreach (var child in dummySlot.Children().ToList())
                                {
                                    child.RemoveFromHierarchy();
                                }
                            }

                            ReDraw(t);
                            break;
                        }
                    }
                });
            }

            return rootBox;
        }
        private VisualElement DrawRectAnime(RectTransformUtil t)
        {
            var rootBox = VUITemplate.GetTemplate("Animation : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.animate;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.animate = x.newValue;
                });
            }

            return rootBox;
        }
        private VisualElement DrawRecLoopCount(RectTransformUtil t)
        {
            var rootBox = VUITemplate.GetTemplate("Loop count : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new IntegerField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.loopCount;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.loopCount = x.newValue;
                });
            }

            return rootBox;
        }
        private VisualElement DrawRectWait(RectTransformUtil t)
        {
            var rootBox = VUITemplate.GetTemplate("WaitUntilFinished : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.waitUntilFinished;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.waitUntilFinished = x.newValue;
                });
            }

            return rootBox;
        }
        private VisualElement DrawRectClamp(RectTransformUtil t)
        {
            var rootBox = VUITemplate.GetTemplate("Clamp : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.clamp;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.clamp = x.newValue;
                });
            }

            return rootBox;
        }
        private VisualElement DrawRectDisable(RectTransformUtil t)
        {
            var rootBox = VUITemplate.GetTemplate("Disable on complete : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.disableOnComplete;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.disableOnComplete = x.newValue;
                });
            }

            return rootBox;
        }
        private VisualElement DrawRectVector(RectTransformUtil t)
        {
            var rootBox = VUITemplate.GetTemplate("Scale : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Vector3Field();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.value;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.value = x.newValue;
                });
            }

            return rootBox;
        }
        private VisualElement DrawRecDuration(RectTransformUtil t)
        {
            var rootBox = VUITemplate.GetTemplate("Duration : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.duration;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.duration = x.newValue;
                });
            }

            return rootBox;
        }
        private VisualElement DrawRectTarget(RectTransformUtil t)
        {
            var rootBox = VUITemplate.GetTemplate("Target rect : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new ObjectField();
            objField.objectType = typeof(RectTransform);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.rectTarget;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.rectTarget = objField.value as RectTransform;
                });
            }

            return rootBox;
        }
    }
}