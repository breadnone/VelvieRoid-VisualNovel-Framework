using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using System;
using System.Linq;
using UnityEngine;

namespace VIEditor
{
    [CustomEditor(typeof(AnimatorSetParam))]
    public class AnimatorSetParamEditor : Editor
    {
        private VisualElement dummy;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as AnimatorSetParam;
            root.Add(DrawAnimator(t));
            root.Add(DrawOptions(t));
            root.Add(DrawString(t));
            dummy = new VisualElement();
            dummy.Add(DrawFields(t, t.paramType));
            root.Add(dummy);

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, t.OnVSummary);
            return root;
        }
        private VisualElement DrawAnimator(AnimatorSetParam t)
        {
            var rootBox = VUITemplate.GetTemplate("Animator : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(Animator);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.animator;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.animator = objField.value as Animator;
            });

            return rootBox;
        }
        private void ClearDummy()
        {
            if (dummy != null && dummy.childCount > 0)
            {
                foreach (var child in dummy.Children().ToList())
                {
                    if (child == null)
                        continue;

                    child.RemoveFromHierarchy();
                }
            }
        }
        private VisualElement DrawString(AnimatorSetParam t)
        {
            var rootBox = VUITemplate.GetTemplate("Param : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new TextField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.paramStr;

            objField.RegisterCallback<ChangeEvent<string>>((x) =>
            {
                t.paramStr = x.newValue;
            });

            return rootBox;
        }
        private VisualElement DrawOptions(AnimatorSetParam t)
        {
            var rootBox = VUITemplate.GetTemplate("Param type : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.paramType.ToString();
            objField.choices = Enum.GetNames(typeof(AnimatorParamType)).ToList();

            objField.RegisterCallback<ChangeEvent<string>>((x) =>
            {
                foreach (var vparam in Enum.GetValues(typeof(AnimatorParamType)))
                {
                    var astype = (AnimatorParamType)vparam;
                    if (astype.ToString() == x.newValue)
                    {
                        t.paramType = astype;
                        ClearDummy();
                        dummy.Add(DrawFields(t, astype));
                        break;
                    }
                }
            });

            return rootBox;
        }
        private VisualElement DrawFields(AnimatorSetParam t, AnimatorParamType type)
        {
            var rootBox = VUITemplate.GetTemplate("Value : ");
            var field = VUITemplate.GetField(rootBox);

            if (type == AnimatorParamType.Int)
            {
                var objField = new IntegerField();
                objField.style.width = field.style.width;
                field.Add(objField);

                objField.value = t.intvalue;

                objField.RegisterValueChangedCallback((x) =>
                {
                    t.intvalue = objField.value;
                });
            }
            else if (type == AnimatorParamType.Float)
            {
                var objField = new FloatField();
                objField.style.width = field.style.width;
                field.Add(objField);

                objField.value = t.floatvalue;

                objField.RegisterValueChangedCallback((x) =>
                {
                    t.floatvalue = objField.value;
                });
            }
            else
            {
                var objField = new Toggle();
                objField.style.width = field.style.width;
                field.Add(objField);

                objField.value = t.boolvalue;

                objField.RegisterValueChangedCallback((x) =>
                {
                    t.boolvalue = objField.value;
                });
            }
            return rootBox;
        }
    }
}