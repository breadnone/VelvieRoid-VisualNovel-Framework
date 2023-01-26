using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using VIEditor;

[CustomEditor(typeof(JumpToLabel))]
public class JumpEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement myInspector = new VisualElement();
        myInspector.style.flexDirection = FlexDirection.Column;
        var t = target as JumpToLabel;

        //Jump
        var boxJump = new Box();
        boxJump.style.marginTop = 5;
        boxJump.style.marginBottom = 5;
        boxJump.style.flexDirection = FlexDirection.Row;

        Label strJump = new Label();
        strJump.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
        strJump.style.marginLeft = 5;
        strJump.text = "VGraph : ";
        boxJump.Add(strJump);

        var auJump = new ObjectField();
        auJump.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
        auJump.objectType = typeof(VCoreUtil);
        boxJump.Add(auJump);

        if (t.VGraph != null)
            auJump.value = t.VGraph;

        if (!PortsUtils.PlayMode)
        {
            auJump.RegisterValueChangedCallback((x) =>
            {
                if (auJump.value != null)
                    t.VGraph = auJump.value as VCoreUtil;
                else
                    t.VGraph = null;
            });
        }

        //Jump String
        var boxSStrJump = new Box();
        boxSStrJump.style.marginTop = 5;
        boxSStrJump.style.marginBottom = 5;
        boxSStrJump.style.flexDirection = FlexDirection.Row;

        Label strStrJump = new Label();
        strStrJump.style.width = 120;
        strStrJump.style.marginLeft = 5;
        strStrJump.text = "Label : ";
        boxSStrJump.Add(strStrJump);

        var auStrJump = new TextField();
        auStrJump.style.width = 210;
        boxSStrJump.Add(auStrJump);

        if (t.VGraph != null)
            auStrJump.value = t.JumpsToLabel;

        if (!PortsUtils.PlayMode)
        {
            auStrJump.RegisterValueChangedCallback((x) =>
            {
                if (auStrJump.value != null)
                    t.JumpsToLabel = auStrJump.value;
                else
                    t.JumpsToLabel = null;
            });
        }

        myInspector.Add(boxJump);
        myInspector.Add(boxSStrJump);
        //Always add this at the end!
        VUITemplate.DrawSummary(myInspector, t, () => t.OnVSummary());
        return myInspector;
    }
}
