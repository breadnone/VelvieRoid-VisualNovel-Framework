using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;

namespace VIEditor
{
    [CustomEditor(typeof(WaitSeconds))]
    public class WaitSecondsEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var t = target as WaitSeconds;

            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;

            var box = new Box();
            box.style.flexDirection = FlexDirection.Row;

            var lblSeconds = new Label();
            lblSeconds.style.marginLeft = 10;
            lblSeconds.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lblSeconds.text = "Seconds : ";

            var visCon = new VisualElement();
            visCon.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var objSeconds = new FloatField();
            visCon.Add(objSeconds);

            objSeconds.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            objSeconds.value = t.Seconds;

            if (!PortsUtils.PlayMode)
            {
                objSeconds.RegisterValueChangedCallback((x) =>
                {
                    t.Seconds = objSeconds.value;
                });
            }

            box.Add(lblSeconds);
            box.Add(visCon);
            root.Add(box);

            var boxTwo = new Box();
            boxTwo.style.flexDirection = FlexDirection.Row;

            var lblScale = new Label();
            lblScale.style.marginLeft = 10;
            lblScale.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lblScale.text = "Unscaled time : ";

            var objScale = new Toggle();
            objScale.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            objScale.value = t.UnscaledTime;

            if (!PortsUtils.PlayMode)
            {
                objScale.RegisterValueChangedCallback((x) =>
                {
                    t.UnscaledTime = objScale.value;
                });
            }

            boxTwo.Add(lblScale);
            boxTwo.Add(objScale);
            root.Add(boxTwo);

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
    }
}