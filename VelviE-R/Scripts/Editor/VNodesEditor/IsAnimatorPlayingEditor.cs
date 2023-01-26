using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using UnityEngine;
using System.Collections.Generic;

namespace VIEditor
{
    [CustomEditor(typeof(IsAnimatorPlaying))]
    public class IsAnimatorPlayingEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as IsAnimatorPlaying;
            root.Add(DrawAnimator(t));
            root.Add(DrawVars(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        private Box DrawAnimator(IsAnimatorPlaying t)
        {
            var rootBox = VUITemplate.GetTemplate("Animator : ");
            var field = rootBox.userData as VisualElement;
            var objField = new ObjectField();
            
            objField.objectType = typeof(Animator);
            objField.allowSceneObjects = true;
            objField.style.width =  new StyleLength(new Length(100, LengthUnit.Percent));
            field.Add(objField);
            objField.value = t.animator;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.animator = objField.value as Animator;
                });
            }

            return rootBox;
        }
        private VisualElement DrawVars(IsAnimatorPlaying t)
        {
            var varTemplate = VUITemplate.VariableTemplate(type: VTypes.Boolean);

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
                    if (x.GetVtype() == VTypes.Boolean)
                        varlist.Add(x.Name);
                });

                varlist.Add("<None>");
                varTemplate.child.choices = varlist;
            }
            if (!PortsUtils.PlayMode && PortsUtils.variable.ivar.Count > 0)
            {
                varTemplate.child.RegisterCallback<ChangeEvent<string>>((evt) =>
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
                });
            }
            return varTemplate.root;
        }
    }
}