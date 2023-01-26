using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using System.Linq;

namespace VIEditor
{
    [CustomEditor(typeof(CancelAnimation))]
    public class CancelAnimationEditor : Editor
    {
        private VisualElement dummy;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;

            dummy = new VisualElement();
            var t = target as CancelAnimation;

            root.Add(DrawToggleAll(t));

            if (!t.cancelAll)
                DrawObject(t);

            root.Add(dummy);
            root.Add(DrawToggleonComplete(t));
            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }

        private void DrawObject(CancelAnimation t)
        {
            var rootBox = VUITemplate.GetTemplate("Object to cancel : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new ObjectField();
            objField.objectType = typeof(GameObject);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.targetobject;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.targetobject = objField.value as GameObject;
                });
            }

            dummy.Add(rootBox);
        }
        private VisualElement DrawToggleAll(CancelAnimation t)
        {
            var rootBox = VUITemplate.GetTemplate("Cancell all : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.cancelAll;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.cancelAll = x.newValue;

                    if (x.newValue)
                    {
                        if (dummy.childCount > 0)
                        {
                            foreach (var list in dummy.Children().ToList())
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
            }

            return rootBox;
        }
        private VisualElement DrawToggleonComplete(CancelAnimation t)
        {
            var rootBox = VUITemplate.GetTemplate("Execute onComplete : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.executeOnComplete;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.executeOnComplete = x.newValue;
                });
            }

            return rootBox;
        }
    }
}