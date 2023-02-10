using System;
using UnityEngine;
using UnityEngine.UIElements;
using VelvieR;
using System.Linq;

namespace VIEditor
{
    public class VBlockLabel : VisualElement
    {
        public string VBlockId { get; set; }
        private bool SetManipulator = false;
        public Label VBlockContent { get; set; }
        public Label VBlockLineNumber { get; set; }
        public Action WriterDelegate {get;set;}
        public bool IsWriterMmode {get;set;}
        public int MockupIndex {get;set;}
        public bool VBlockToggle
        {
            get
            {
                if (vtoggle != null)
                    return vtoggle.value;
                else
                    return true;
            }
            set
            {
                if (vtoggle != null)
                    vtoggle.value = value;
            }
        }
        public Toggle vtoggle;
        public VBlockLabel(string titleCon, VColor col, int defHeight, int defWidth, int blockCounter, bool manipulator)
        {
            VBlockId = Guid.NewGuid().ToString();
            SetManipulator = manipulator;
            SetDefault(titleCon, col, defHeight, defWidth, blockCounter);
        }
        private string EnableString()
        {
            var t = string.Empty;

            if (vtoggle.value)
                t = "Disable";
            else
                t = "Enable";

            return t;
        }
        public void SetDefault(string titleCon, VColor col, int defHeight, int defWidth, int blockCounter)
        {
            this.pickingMode = PickingMode.Position;
            this.style.marginLeft = 0;
            this.style.marginTop = 3;
            this.style.height = new StyleLength(new Length(30, LengthUnit.Percent));
            this.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            this.style.flexDirection = FlexDirection.Row;
            this.name = "VBlock";

            //Right click contextual menu
            if (SetManipulator == true)
            {
                if(!PortsUtils.PlayMode)
                {
                    this.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
                    {
                        //TODO: Multiple items selections!
                        if (PortsUtils.VGraph.listV.selectedItem != null)
                        {
                            evt.menu.AppendAction("Delete", (x) => VBlockUtils.Delete());
                            evt.menu.AppendAction(EnableString(), (x) => EnableVBlock());
                        }                    
                    }));   
                }            
            }

            var no = new Label();
            no.style.flexWrap = Wrap.Wrap;
            no.style.unityTextAlign = TextAnchor.MiddleCenter;
            no.style.color = Color.white;
            no.style.backgroundColor = Color.red;
            no.style.alignSelf = Align.FlexStart;
            no.style.height = defHeight;
            no.style.width = new StyleLength(new Length(15, LengthUnit.Percent));
            no.style.borderLeftWidth = 1;
            no.style.borderBottomWidth = 1;
            no.style.borderTopWidth = 1;
            no.style.borderLeftColor = Color.black;
            no.style.borderBottomColor = Color.black;
            no.style.borderTopColor = Color.black;
            VBlockLineNumber = no;

            var indicator = new Label();
            indicator.style.flexWrap = Wrap.Wrap;
            indicator.style.unityTextAlign = TextAnchor.MiddleCenter;
            indicator.style.color = Color.black;
            indicator.text = "||";
            indicator.style.backgroundColor = Color.yellow;
            indicator.style.alignSelf = Align.FlexStart;
            indicator.style.height = defHeight;
            indicator.style.width = new StyleLength(new Length(15, LengthUnit.Percent));
            indicator.style.borderBottomWidth = 1;
            indicator.style.borderTopWidth = 1;
            indicator.style.borderBottomColor = Color.black;
            indicator.style.borderTopColor = Color.black;
            Indicator = indicator;

            var content = new Label();
            content.name = "VContent";
            content.style.flexWrap = Wrap.Wrap;
            content.style.unityTextAlign = TextAnchor.MiddleCenter;
            content.style.color = Color.black;

            VColorAttr.GetColor(content, col);

            content.style.alignSelf = Align.FlexStart;
            content.style.height = defHeight;
            content.style.width = new StyleLength(new Length(60, LengthUnit.Percent));

            //Content ouline
            content.style.borderBottomWidth = 1;
            content.style.borderTopWidth = 1;
            content.style.borderLeftWidth = 1;
            content.style.borderRightWidth = 1;
            content.style.borderBottomColor = Color.black;
            content.style.borderTopColor = Color.black;
            content.style.borderLeftColor = Color.black;
            content.style.borderRightColor = Color.black;
            VBlockContent = content;

            var toggle = new Toggle();

            if (vtoggle == null)
                toggle.value = VBlockToggle; //Default is toggled on
            else
                toggle.value = true;

            toggle.style.color = Color.white;
            toggle.style.alignSelf = Align.FlexStart;
            toggle.style.height = defHeight;
            toggle.style.width = new StyleLength(new Length(10, LengthUnit.Percent));
            toggle.SetEnabled(false);
            vtoggle = toggle;

            this.Add(no);
            this.Add(indicator);
            this.Add(content);
            this.Add(toggle);
        }
        public void SetRegisters()
        {
            if(!PortsUtils.PlayMode)
            {
                this.RegisterCallback<MouseDownEvent>(RegisterCallB);
            }
        }

        private Label Indicator;
        public void RegisterCallB(MouseDownEvent e)
        {
            if(!PortsUtils.PlayMode)
            {
                if(!IsWriterMmode)
                {
                    if(PortsUtils.VGraph.listV == null || PortsUtils.VGraph.listV.selectedItem == null)
                        return;

                    VBlockUtils.activeListVIndex = PortsUtils.VGraph.listV.selectedIndex;

                    if (e.button == 1)
                    {
                        PortsUtils.VGraph.listV.ClearSelection();
                        PortsUtils.VGraph.listV.SetSelection(VBlockUtils.activeListVIndex);
                    }

                    PortsUtils.VGraph?.ShowSelectedVblockSerializedFields();
                }
                else
                {
                    if(WriterDelegate != null && !VDubTool.MockupIsWriting)
                    {
                        WriterDelegate.Invoke();
                    }

                    if(VDubTool.MockupIsWriting)
                    {
                        VDubTool.DubItemList.selectedIndex = MockupIndex;
                        VDubTool.DubItemList.SetSelection(MockupIndex);
                        e.StopImmediatePropagation();
                    }
                }
            }

        }
        public void UnregisterCallB() { this.UnregisterCallback<MouseDownEvent>(RegisterCallB); }

        public void EnableVBlock()
        {
            if (PortsUtils.VGraph.listV.selectedItem == null && PortsUtils.activeVObject != null)
                return;

            var tmp = PortsUtils.VGraph.listV.selectedItems.ToList().Count;

            if (tmp == 1)
            {
                var selected = PortsUtils.VGraph.listV.selectedItem as VelvieBlockComponent;
                selected.enable = !vtoggle.value;

                VBlockToggle = selected.enable;
                vtoggle.value = VBlockToggle;
                this.VBlockToggle = vtoggle.value;
                SetIndicators(VBlockToggle);
                PortsUtils.SetActiveAssetDirty();
            }
            else
            {
                bool b = false;
                int i = 0;

                foreach (var items in PortsUtils.VGraph.listV.selectedItems)
                {
                    var astype = items as VelvieBlockComponent;

                    if (i == 0)
                    {
                        b = !astype.enable;
                        i++;
                    }

                    astype.enable = b;
                }

                PortsUtils.VGraph.listV.Rebuild();
                PortsUtils.VGraph.listV.ClearSelection();
            }
        }
        public void SetSymbols(bool? normal, string customSymbol = "")
        {
            if(String.IsNullOrEmpty(customSymbol))
            {
                if(normal.HasValue && normal.Value == true)
                {
                    Indicator.text = "[ ]";
                }
                else if(normal.HasValue && normal.Value == false)
                {
                    Indicator.text = "||";
                }
                else if(!normal.HasValue)
                {
                    Indicator.text = "[==]";
                }
            }
            else
            {
                Indicator.text = customSymbol;
            }
        }
        public void SetIndicators(bool state)
        {
            Indicator.SetEnabled(state);
            VBlockContent.SetEnabled(state);
            VBlockLineNumber.SetEnabled(state);
        }
    }
}