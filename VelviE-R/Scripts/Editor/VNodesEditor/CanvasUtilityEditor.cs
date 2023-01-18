using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using VelvieR;
using System.Linq;

namespace VIEditor
{
    [CustomEditor(typeof(CanvasUtility))]
    public class CanvasUtilityEditor : Editor
    {
        private VisualElement dummy;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as CanvasUtility;

            dummy = new VisualElement();

            root.Add(DrawCanvas(t));
            root.Add(DrawEnumCanvas(t));
            root.Add(dummy);

            if (t.canvasUtil == Canvasutilityv.SetResolution)
            {
                if (dummy.childCount > 0)
                {
                    foreach (var child in dummy.Children().ToList())
                    {
                        child.RemoveFromHierarchy();
                    }
                }

                dummy.Add(DrawRectVector(t));
            }
            else if (t.canvasUtil == Canvasutilityv.SetSortOder)
            {
                if (dummy.childCount > 0)
                {
                    foreach (var child in dummy.Children().ToList())
                    {
                        child.RemoveFromHierarchy();
                    }
                }

                dummy.Add(DrawOrder(t));
            }
            else if (t.canvasUtil == Canvasutilityv.SetRenderMode)
            {
                if (dummy.childCount > 0)
                {
                    foreach (var child in dummy.Children().ToList())
                    {
                        child.RemoveFromHierarchy();
                    }
                }

                dummy.Add(DrawEnumRenderMode(t));
            }
            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, ()=> t.OnVSummary());
            return root;
        }

        private VisualElement DrawCanvas(CanvasUtility t)
        {
            var rootBox = VUITemplate.GetTemplate("Canvas : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(Canvas);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.canvas;
            objField.RegisterValueChangedCallback((x) =>
            {
                t.canvas = objField.value as Canvas;
            });

            return rootBox;
        }
        private VisualElement DrawEnumCanvas(CanvasUtility t)
        {
            var rootBox = VUITemplate.GetTemplate("Set canvas : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.canvasUtil.ToString();
            objField.choices = Enum.GetNames(typeof(Canvasutilityv)).ToList();
            objField.RegisterCallback<ChangeEvent<string>>((x) =>
            {
                foreach (var venum in Enum.GetValues(typeof(Canvasutilityv)))
                {
                    var asetype = (Canvasutilityv)venum;

                    if (x.newValue == asetype.ToString())
                    {
                        t.canvasUtil = asetype;
                    }

                    if (x.newValue == Canvasutilityv.SetResolution.ToString())
                    {
                        if (dummy.childCount > 0)
                        {
                            foreach (var child in dummy.Children().ToList())
                            {
                                child.RemoveFromHierarchy();
                            }
                        }

                        dummy.Add(DrawRectVector(t));
                    }
                    else if (x.newValue == Canvasutilityv.SetSortOder.ToString())
                    {
                        if (dummy.childCount > 0)
                        {
                            foreach (var child in dummy.Children().ToList())
                            {
                                child.RemoveFromHierarchy();
                            }
                        }

                        dummy.Add(DrawOrder(t));
                    }
                    else if(x.newValue == Canvasutilityv.SetRenderMode.ToString())
                    {
                        if (dummy.childCount > 0)
                        {
                            foreach (var child in dummy.Children().ToList())
                            {
                                child.RemoveFromHierarchy();
                            }
                        }

                        dummy.Add(DrawEnumRenderMode(t));
                    }
                    else
                    {
                        if (dummy.childCount > 0)
                        {
                            foreach (var child in dummy.Children().ToList())
                            {
                                child.RemoveFromHierarchy();
                            }
                        }
                    }
                }
            });

            return rootBox;
        }
        private VisualElement DrawRectVector(CanvasUtility t)
        {
            var rootBox = VUITemplate.GetTemplate("ScreenResolution : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Vector2Field();
            objField.style.width = field.style.width;
            field.Add(objField);

            if (t.resolution == Vector2.zero)
            {
                var screenRes = Handles.GetMainGameViewSize();
                t.resolution = screenRes;
                objField.value = screenRes;
            }
            else
            {
                objField.value = t.resolution;
            }

            objField.RegisterValueChangedCallback((x) =>
            {
                if (x.newValue != Vector2.zero)
                    t.resolution = x.newValue;
                else
                    t.resolution = Handles.GetMainGameViewSize();
            });

            return rootBox;
        }
        private VisualElement DrawOrder(CanvasUtility t)
        {
            var rootBox = VUITemplate.GetTemplate("Set order : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new IntegerField();
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.order;
            objField.RegisterValueChangedCallback((x) =>
            {
                t.order = x.newValue;
            });

            return rootBox;
        }

        private VisualElement DrawEnumRenderMode(CanvasUtility t)
        {
            var rootBox = VUITemplate.GetTemplate("Set canvas : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.renderMode.ToString();
            objField.choices = Enum.GetNames(typeof(RenderMode)).ToList();
            objField.RegisterCallback<ChangeEvent<string>>((x) =>
            {
                foreach (var venum in Enum.GetValues(typeof(RenderMode)))
                {
                    var asetype = (RenderMode)venum;

                    if (x.newValue == asetype.ToString())
                    {
                        t.renderMode = asetype;
                    }
                }
            });

            return rootBox;
        }
    }
}