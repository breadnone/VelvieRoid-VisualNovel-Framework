using VelvieR;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VIEditor;

[CustomEditor(typeof(HideVDialogue))]
public class HideVDialogueEditor : Editor
{
    private ToolbarMenu vdialogues;
    private HideVDialogue hidevdialogue;
    private VEditorNotify activeBind;
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement myInspector = new VisualElement();
        myInspector.style.flexDirection = FlexDirection.Column;
        var t = target as HideVDialogue;
        hidevdialogue = t;

        var binds = new VEditorNotify
        {
            editor = this,
            act = RefreshThis
        };

        activeBind = binds;
        PortsUtils.RefreshBinds.Add(binds);
        myInspector.Add(DrawVDialogue(t));
        myInspector.Add(DrawSummary(t));

        //Always add this at the end!
        VUITemplate.DrawSummary(myInspector, t, ()=> t.OnVSummary());

        return myInspector;
    }
    public Box DrawVDialogue(HideVDialogue t)
    {
        var bosVdialogue = new Box();
        bosVdialogue.style.height = 50;
        bosVdialogue.style.marginTop = 5;
        bosVdialogue.style.marginBottom = 5;
        bosVdialogue.style.flexDirection = FlexDirection.Row;

        Label vdialogueLbl = new Label();
        vdialogueLbl.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
        vdialogueLbl.style.height = 15;
        vdialogueLbl.style.marginLeft = 5;
        vdialogueLbl.style.marginTop = 5;
        vdialogueLbl.text = "VDialogue : ";
        bosVdialogue.Add(vdialogueLbl);

        var vdialogue = new ToolbarMenu { text = "None" };
        vdialogue.style.height = 15;
        vdialogue.style.marginLeft = 4;
        vdialogue.style.marginTop = 5;
        vdialogues = vdialogue;

        if (t.Vdialogue != null)
        {
            vdialogue.text = t.Vdialogue.gameObject.name;
        }

        vdialogue.menu.AppendAction("None", (x) =>
        {
            vdialogue.text = "None";
            t.Vdialogue = null;
        });

        AppendMenus();

        void AppendMenus()
        {
            var vdialogueCom = RePoolVDialogues();

            if (vdialogueCom.Length == 0)
                return;

            foreach (var vd in vdialogueCom)
            {
                vdialogue.menu.AppendAction(vd.gameObject.name, (x) =>
                {
                    vdialogue.text = vd.gameObject.name;
                    t.Vdialogue = vd;
                });
            }
        }

        vdialogue.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
        bosVdialogue.Add(vdialogue);
        return bosVdialogue;
    }
    public Box DrawSummary(HideVDialogue t)
    {
        var boxSum = new Box();
        boxSum.style.height = 70;
        boxSum.style.marginTop = 5;
        boxSum.style.marginBottom = 5;
        boxSum.style.flexDirection = FlexDirection.Row;

        Label vsumLbl = new Label();
        vsumLbl.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
        vsumLbl.style.height = 20;
        vsumLbl.style.marginLeft = 5;
        vsumLbl.style.marginTop = 5;
        vsumLbl.text = "<b>NOTE : </b>";
        boxSum.Add(vsumLbl);

        var vsum = new Label { text = "Hides active VDialoguePanel in the scene. Hiding active VDialoguePanel will prematurely cancel all running operations in the VDialoguePanel." };
        vsum.style.height = 20;
        vsum.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
        vsum.style.marginLeft = 4;
        vsum.style.marginTop = 5;
        vsum.style.flexDirection = FlexDirection.Column;
        vsum.style.whiteSpace = WhiteSpace.Normal;
        vsum.style.unityOverflowClipBox = OverflowClipBox.ContentBox;

        boxSum.Add(vsum);
        return boxSum;
    }

    private VelvieDialogue[] RePoolVDialogues()
    {
        return VEditorFunc.EditorGetVDialogues();
    }
    public void RefreshThis()
    {
        var t = vdialogues.menu.MenuItems();

        for (int i = 0; i < t.Count; i++)
        {
            vdialogues.menu.RemoveItemAt(i);
        }

        var vdialogueCom = RePoolVDialogues();

        if (vdialogueCom.Length == 0)
            return;

        vdialogues.menu.AppendAction("<None>", (x) =>
        {
            vdialogues.text = "<None>";
            hidevdialogue.Vdialogue = null;
        });

        foreach (var vd in vdialogueCom)
        {
            vdialogues.menu.AppendAction(vd.gameObject.name, (x) =>
            {
                vdialogues.text = vd.gameObject.name;
                hidevdialogue.Vdialogue = vd;
            });
        }

        if(hidevdialogue.Vdialogue != null)
            vdialogues.text = hidevdialogue.Vdialogue.gameObject.name;
        else
            vdialogues.text = "None";

        vdialogues.MarkDirtyRepaint();
    }
}
