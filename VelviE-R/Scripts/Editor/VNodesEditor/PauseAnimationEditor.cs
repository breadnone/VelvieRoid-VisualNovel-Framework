using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using System.Linq;

namespace VIEditor
{
    [CustomEditor(typeof(PauseAnimation))]
    public class PauseAnimationEditor : Editor
    {
        private VisualElement dummy;
        private VisualElement dummyTwo;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;

            dummy = new VisualElement();
            dummyTwo = new VisualElement();
            var t = target as PauseAnimation;

            root.Add(DrawType(t));
            root.Add(dummyTwo);

            if(!t.isResume)
            dummyTwo.Add(DrawToggleAll(t));
            else
            dummyTwo.Add(DrawToggleAllResume(t));

            if(!t.pauseAll)
            DrawObject(t);

            root.Add(dummy);
            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }

        private void DrawObject(PauseAnimation t)
        {
            var rootBox = VUITemplate.GetTemplate("Object to pause : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new ObjectField();

            objField.objectType = typeof(GameObject);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.targetobject;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.targetobject = objField.value as GameObject;
            });

            dummy.Add(rootBox);
        }
        private VisualElement DrawToggleAll(PauseAnimation t)
        {
            var rootBox = VUITemplate.GetTemplate("Pause all : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.pauseAll;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.pauseAll = x.newValue;

                if(x.newValue)
                {
                    if(dummy.childCount > 0)
                    {
                        foreach(var list in dummy.Children().ToList())
                        {
                            list.RemoveFromHierarchy();
                        }
                    }
                }
                else
                {
                    DrawObject(t);
                    t.targetobject = null;
                }
            });
            return rootBox;
        }
        private VisualElement DrawToggleAllResume(PauseAnimation t)
        {
            var rootBox = VUITemplate.GetTemplate("Resume all : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.pauseAll;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.pauseAll = x.newValue;

                if(x.newValue)
                {
                    if(dummy.childCount > 0)
                    {
                        foreach(var list in dummy.Children().ToList())
                        {
                            list.RemoveFromHierarchy();
                        }
                    }
                }
                else
                {
                    DrawObject(t);
                    t.targetobject = null;
                }
            });
            return rootBox;
        }
        private VisualElement DrawType(PauseAnimation t)
        {
            var rootBox = VUITemplate.GetTemplate("Operation : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.choices = new List<string>{"Pause", "Resume"};

            if(t.isResume)
            {
                objField.value = "Resume";
            }
            else
            {
                objField.value = "Pause";
            }

            objField.RegisterValueChangedCallback((x)=>
            {
                if(dummyTwo.childCount > 0)
                {
                    foreach(var i in dummyTwo.Children().ToList())
                    {
                        i.RemoveFromHierarchy();
                    }
                }
                if(x.newValue == "Pause")
                {
                    t.isResume = false;
                    dummyTwo.Add(DrawToggleAll(t));
                }
                else
                {
                    t.isResume = true;
                    dummyTwo.Add(DrawToggleAllResume(t));
                }
            });
            
            return rootBox;
        }
    }
}