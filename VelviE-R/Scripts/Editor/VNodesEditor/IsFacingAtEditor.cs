using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VelvieR;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace VIEditor
{
    [CustomEditor(typeof(IsFacingAt))]
    public class IsFacingAtEditor : Editor
    {
        private VisualElement dummySlot;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            dummySlot = new VisualElement();

            var t = target as IsFacingAt;
            root.Add(DrawThisObj(t));
            root.Add(DrawThatObj(t));
            root.Add(dummySlot);
            dummySlot.Add(DrawVars(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        private VisualElement DrawVars(IsFacingAt t)
        {

            var varTemplate = VUITemplate.VariableTemplate(type:VTypes.Boolean);

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
                    if(x.GetVtype() == VTypes.Boolean)
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

        public VisualElement DrawThisObj(IsFacingAt t)
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
        public VisualElement DrawThatObj(IsFacingAt t)
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