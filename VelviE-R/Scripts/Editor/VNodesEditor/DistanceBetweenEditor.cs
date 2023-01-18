using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VelvieR;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Linq;

namespace VIEditor
{
    [CustomEditor(typeof(DistanceBetween))]
    public class DistanceBetweenEditor : Editor
    {
        private VisualElement dummySlot;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            dummySlot = new VisualElement();

            var t = target as DistanceBetween;
            root.Add(DrawBool(t));
            root.Add(DrawThisObj(t));
            root.Add(DrawThatObj(t));
            root.Add(dummySlot);
            dummySlot.Add(DrawVars(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        public VisualElement DrawBool(DistanceBetween t)
        {
            var rootBox = VUITemplate.GetTemplate("Type : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);

            if(t.isVector3)
                objField.value = "Vector3";
            else
                objField.value = "Vector2";
            
            objField.choices = new List<string>{"Vector3", "Vector2"};

            objField.RegisterValueChangedCallback((x)=>
            {
                RemoveChild();
                t.variable = null;

                if(x.newValue == "Vector3")
                    t.isVector3 = true;
                else
                    t.isVector3 = false;

                dummySlot.Add(DrawVars(t));
            });

            return rootBox;
        }
        private VisualElement DrawVars(DistanceBetween t)
        {
            var vtype = VTypes.Vector3;

            if(!t.isVector3)
            {
                vtype = VTypes.Vector2;
            }

            var varTemplate = VUITemplate.VariableTemplate(type:vtype);

            if (t.variable == null)
            {
                varTemplate.child.value = "<None>";
            }
            else
            {
                if (PortsUtils.variable.ivar.Exists(x => x.Name == t.variable.Name))
                {
                    varTemplate.child.value = t.variable.Name;
                }
                else
                {
                    varTemplate.child.value = "<None>";
                }
            }

            if (PortsUtils.variable.ivar.Count > 0)
            {
                var varlist = new List<string>();

                PortsUtils.variable.ivar.ForEach((x) => 
                { 
                    if(x.GetVtype() == vtype)
                        varlist.Add(x.Name); 
                });

                varlist.Add("<None>");
                varTemplate.child.choices = varlist;
            }

            varTemplate.child.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                if (!PortsUtils.PlayMode && PortsUtils.variable.ivar.Count > 0)
                {
                    if (evt.newValue == "<None>")
                    {
                        t.variable = null;
                        PortsUtils.SetActiveAssetDirty();
                    }
                    else
                    {
                        t.variable = PortsUtils.variable.ivar.Find(x => x.Name == evt.newValue);
                        PortsUtils.SetActiveAssetDirty();
                    }
                }
            });

            return varTemplate.root;
        }
        private void RemoveChild()
        {
            if (dummySlot.childCount > 0)
            {
                foreach (var child in dummySlot.Children().ToList())
                {
                    child.RemoveFromHierarchy();
                }
            }
        }
        public VisualElement DrawThisObj(DistanceBetween t)
        {
            var rootBox = VUITemplate.GetTemplate("Main target : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(Transform);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.thisTarget;
            
            objField.RegisterValueChangedCallback((x)=>
            {
                t.thisTarget = objField.value as Transform;
            });

            return rootBox;
        }
        public VisualElement DrawThatObj(DistanceBetween t)
        {
            var rootBox = VUITemplate.GetTemplate("Target object : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(Transform);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.thatTarget;
            
            objField.RegisterValueChangedCallback((x)=>
            {
                t.thatTarget = objField.value as Transform;
            });

            return rootBox;
        }
    }
}