using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace VIEditor
{
    [CustomEditor(typeof(SetCursor))]
    public class SetCursorEditor : Editor
    {
        private VisualElement subs;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as SetCursor;

            subs = new VisualElement();

            root.Add(DrawReset(t));
            root.Add(DrawSingleMode(t));
            //root.Add(subs);
            root.Add(DrawSprite(t));
            root.Add(DrawVector(t));
            root.Add(DrawCursoMode(t));
            
            /*root.Add(subs);

            if(!t.singleMode)
            {
                subs.Add(DrawList(t));
            }
            */

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, t.OnVSummary);
            return root;
        }
        private void FlushChild()
        {
            if(subs == null)
                return;

            foreach(var child in subs.Children().ToList())
            {
                if(child != null)
                {
                    child.RemoveFromHierarchy();
                }
            }
        }
        private VisualElement DrawSprite(SetCursor t)
        {
            var rootBox = VUITemplate.GetTemplate("Texture : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(Texture2D);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.texture;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.texture = objField.value as Texture2D;
            });

            return rootBox;
        }
        private VisualElement DrawVector(SetCursor t)
        {
            var rootBox = VUITemplate.GetTemplate("Hotspot : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Vector2Field();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.hotspot;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.hotspot = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawCursoMode(SetCursor t)
        {
            var rootBox = VUITemplate.GetTemplate("Cursor mode : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.cursorMode.ToString();

            objField.choices = Enum.GetNames(typeof(CursorMode)).ToList();

            objField.RegisterCallback<ChangeEvent<string>>((x) =>
            {
                foreach(var e in Enum.GetValues(typeof(CursorMode)))
                {
                    var ast = (CursorMode)e;

                    if(x.newValue == ast.ToString())
                    {
                        t.cursorMode = ast;
                    }
                }
            });

            return rootBox;
        }
        private VisualElement DrawSingleMode(SetCursor t)
        {
            var rootBox = VUITemplate.GetTemplate("Mode : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);

            if(t.singleMode)
                objField.value = "SingleMode";
            else
                objField.value = "OnMouseOverUI";

            objField.choices = new List<string>{"SingleMode", "OnMouseOverUI"};
            objField.RegisterCallback<ChangeEvent<string>>((x) =>
            {
                foreach(var e in objField.choices)
                {
                    if(x.newValue == "SingleMode")
                    {
                        t.singleMode = true;
                        //FlushChild();
                    }
                    else
                    {
                        t.singleMode = false;
                        //FlushChild();
                        //subs.Add(DrawList(t));
                    }
                }
            });

            return rootBox;
        }
        private VisualElement DrawList(SetCursor t)
        {
            var rootBox = VUITemplate.GetTemplate("ChangeOnHoverObjects : ");
            var field = VUITemplate.GetField(rootBox);

            Func<ObjectField> makeItem = () => 
            {
                var obj = new ObjectField();
                obj.objectType = typeof(GameObject);
                return obj;
            };
            Action<VisualElement, int> bindItem = (e, i) => 
            {
                (e as ObjectField).value = t.gameObjects[i];
                var idx = i;
                (e as ObjectField).RegisterValueChangedCallback((x)=>
                {
                    t.gameObjects[idx] = (e as ObjectField).value as GameObject;
                });
            
            };
            
            const int itemHeight = 20;
            var objField = new ListView(t.gameObjects, itemHeight, makeItem, bindItem);
            objField.showAddRemoveFooter = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            return rootBox;
        }
        private VisualElement DrawReset(SetCursor t)
        {
            var rootBox = VUITemplate.GetTemplate("Enable : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);

            if(t.reset)
            {
                objField.value = "Reset";
            }
            else
            {
                objField.value = "Enable";
            }

            objField.choices = new List<string>{"Reset", "Enable"};
            objField.RegisterCallback<ChangeEvent<string>>((x) =>
            {
                foreach(var e in objField.choices)
                {
                    if(x.newValue == "Reset")
                    {
                        t.reset = true;
                    }
                    else
                    {
                        t.reset = false;
                    }
                }
            });

            return rootBox;
        }
    }
}