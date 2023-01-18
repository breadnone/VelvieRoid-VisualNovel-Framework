using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using UnityEditor.UIElements;
using VelvieR;

namespace VIEditor
{
    [CustomEditor(typeof(SetActive))]
    public class SetActiveEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as SetActive;

            root.Add(DrawObject(t));
            root.Add(DrawBool(t));
            
            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        public VisualElement DrawObject(SetActive t)
        {
            var rootBox = VUITemplate.GetTemplate("GameObject : ");
            var field = VUITemplate.GetField(rootBox);

            Func<ObjectField> makeItem = () => 
            {
                var t = new ObjectField();
                t.objectType = typeof(GameObject);
                t.allowSceneObjects = true;
                return t;
            };

            Action<VisualElement, int> bindItem = (e, i) => 
            {
                var astype = e as ObjectField;                
                astype.value = t.targetObject[i];

                astype.RegisterValueChangedCallback((x)=>
                {
                    if(!PortsUtils.PlayMode)
                    t.targetObject[i] = x.newValue as GameObject;
                });
            };

            var btnAdd = new Button();
            btnAdd.style.width = 30;
            btnAdd.style.height = 20;
            btnAdd.text = "+";

            var btnRem = new Button();
            btnRem.style.width = 30;
            btnRem.style.height = 20;
            btnRem.text = "-";

            const int itemHeight = 20;
            var objField = new ListView(t.targetObject, itemHeight, makeItem, bindItem);
            objField.reorderable = false;
            objField.selectionType = SelectionType.Single;
            objField.showBorder = true;
            objField.style.width = field.style.width;
            field.style.flexDirection = FlexDirection.Column;
            field.Add(objField);

            btnAdd.clicked += ()=>
            {
                t.targetObject.Add(null);
                objField.Rebuild();
            };
            btnRem.clicked += ()=>
            {
                if(objField.selectedItem != null)
                {
                    t.targetObject.RemoveAt(objField.selectedIndex);
                }
                else
                {
                    if(t.targetObject.Count > 0)
                    t.targetObject.RemoveAt(t.targetObject.Count - 1);
                }
                objField.Rebuild();
            };
            var visEl = new VisualElement();
            visEl.style.flexDirection = FlexDirection.Row;
            field.Add(visEl);

            visEl.Add(btnAdd);
            visEl.Add(btnRem);

            return rootBox;
        }

        public VisualElement DrawBool(SetActive t)
        {
            var rootBox = VUITemplate.GetTemplate("Enable : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.activeState;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.activeState = objField.value;
            });

            return rootBox;
        }
    }
}