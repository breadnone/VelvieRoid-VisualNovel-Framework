using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using UnityEngine;

namespace VIEditor
{
    [CustomEditor(typeof(CameraShake))]
    public class CameraShakeEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;
            var t = target as CameraShake;

            root.Add(DrawCam(t));
            root.Add(DrawPower(t));
            root.Add(DrawMagnitude(t));
            root.Add(DrawShakeTime(t));
            return root;
        }
        private VisualElement DrawCam(CameraShake t)
        {
            var rootBox = VUITemplate.GetTemplate("Target to look : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new ObjectField();
            objField.objectType = typeof(Camera);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.cam as Camera;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.cam = objField.value as Camera;
                });
            }

            return rootBox;
        }
        private VisualElement DrawPower(CameraShake t)
        {
            var rootBox = VUITemplate.GetTemplate("Power : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.power;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.power = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawMagnitude(CameraShake t)
        {
            var rootBox = VUITemplate.GetTemplate("Magnitude : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.power;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.power = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawShakeTime(CameraShake t)
        {
            var rootBox = VUITemplate.GetTemplate("Shake time : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.shakeTime;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.shakeTime = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawDropOff(CameraShake t)
        {
            var rootBox = VUITemplate.GetTemplate("DropOff time : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.dropOffTime;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.dropOffTime = objField.value;
                });
            }

            return rootBox;
        }
    }
}