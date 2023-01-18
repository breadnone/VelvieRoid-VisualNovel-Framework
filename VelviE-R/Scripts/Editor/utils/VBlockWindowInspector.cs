using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using PopupWindow = UnityEngine.UIElements.PopupWindow;
using System.Collections;
using System.Collections.Generic;
using System;
using VelvieR;
using System.Linq;

namespace VIEditor
{
    public class VBlockWindow : PopupWindow
    {
        private bool findIsActive = false;
        private VisualElement root;
        public VBlockWindow(VGraphs vgraph)
        {
            this.style.width = 390;
            this.style.position = Position.Relative;
            this.style.alignSelf = Align.FlexStart; //This basically just saying: Attach to Left - dynmamic size
            this.style.flexGrow = new StyleFloat(1);
            this.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            this.text = "VBlock";
            this.style.backgroundColor = new StyleColor(Color.gray);

            //Add Toolbar
            var tb = new Toolbar();
            tb.style.backgroundColor = Color.grey;
            tb.style.height = 40;

            //VBlock content
            var blockBox = new Box();
            blockBox.style.alignSelf = Align.Center;
            blockBox.style.flexGrow = new StyleFloat(1);
            blockBox.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            blockBox.style.width = new StyleLength(new Length(90, LengthUnit.Percent));
            blockBox.style.backgroundColor = new StyleColor(Color.grey);
            blockBox.style.borderLeftColor = Color.black;
            blockBox.style.borderRightColor = Color.black;
            blockBox.style.borderLeftWidth = 2;
            blockBox.style.borderRightWidth = 2;
            blockBox.name = "blockyBoxBox";

            var btnDel = new Button(()=> VBlockUtils.Delete());
            btnDel.style.alignSelf = Align.Center;
            btnDel.style.flexGrow = new StyleFloat(1);
            btnDel.style.height = new StyleLength(new Length(80, LengthUnit.Percent));
            btnDel.style.width = new StyleLength(new Length(4, LengthUnit.Percent));
            btnDel.style.fontSize = 10;
            btnDel.text = "DEL";

            tb.Add(btnDel);
            
            var btnNext = new Button(()=> VBlockUtils.MoveForwardIndex());
            btnNext.style.alignSelf = Align.Center;
            btnNext.style.flexGrow = new StyleFloat(1);
            btnNext.style.height = new StyleLength(new Length(80, LengthUnit.Percent));
            btnNext.style.width = new StyleLength(new Length(4, LengthUnit.Percent));
            btnNext.style.fontSize = 10;
            btnNext.text = "-->";

            tb.Add(btnNext);

            var btnPrev = new Button(()=> VBlockUtils.MoveBackwardIndex());
            btnPrev.style.alignSelf = Align.Center;
            btnPrev.style.flexGrow = new StyleFloat(1);
            btnPrev.style.height = new StyleLength(new Length(80, LengthUnit.Percent));
            btnPrev.style.width = new StyleLength(new Length(4, LengthUnit.Percent));
            btnPrev.style.fontSize = 10;
            btnPrev.text = "<--";

            tb.Add(btnPrev);

            Button btnFind = new Button();

            btnFind.clicked += ()=> 
            {
                if(!PortsUtils.PlayMode && !findIsActive && PortsUtils.activeVGraphAssets != null && PortsUtils.activeVObject != null)
                {
                    findIsActive = true;
                    PopUps();
                }
            };

            btnFind.style.alignSelf = Align.Center;
            btnFind.style.flexGrow = new StyleFloat(1);
            btnFind.style.height = new StyleLength(new Length(80, LengthUnit.Percent));
            btnFind.style.width = new StyleLength(new Length(4, LengthUnit.Percent));
            btnFind.style.fontSize = 10;
            btnFind.text = "FIND";

            tb.Add(btnFind);
            tb.MarkDirtyRepaint();

            this.Add(tb);
            this.Add(blockBox);

            PortsUtils.VGraph.ParentVBlockBox = blockBox;
            this.MarkDirtyRepaint();
        }
        private VisualElement fparent;
        private bool isSayWord = true;
        public void PopUps()
        {
            PopupWindow pop = new PopupWindow();

            fparent = new VisualElement();
            fparent.style.flexDirection = FlexDirection.Row;
            fparent.style.position = new StyleEnum<Position>(Position.Absolute);

            pop.text = "Find vblocks";
            pop.style.width = 200;
            pop.style.height = 200;

            var tb = new ToolbarMenu();
            tb.text = "SayWordContent";
            tb.style.alignSelf = Align.Center;
            tb.style.height = 30;
            tb.menu.AppendAction("Symbols", (x)=>
            {
                tb.text = "Symbols";
                isSayWord = false;
            });
            tb.menu.AppendAction("SayWordContent", (x)=>
            {
                isSayWord = true;
                tb.text = "SayWordContent";
            });

            var txt = new TextField();

            var btnClose = new Button(()=>
            {
                fparent.RemoveFromHierarchy();
                findIsActive = false;
            });

            btnClose.text = "<b>Close</b>";

            var btnFind = new Button(()=>
            {
                if(!PortsUtils.PlayMode)
                {
                    var saylist = PortsUtils.activeVObject.GetComponent<VCoreUtil>();

                    if(isSayWord)
                    {
                        if(saylist != null && saylist.vBlockCores.Count > 0 && txt.value.Length > 0)
                        {
                            List<(SayWord, int)> filteredResult = new List<(SayWord, int)>();

                            for (int i = 0; i < saylist.vBlockCores.Count; i++)
                            {
                                if (saylist.vBlockCores[i].vnodeId != PortsUtils.activeVNode.VNodeId)
                                    continue;

                                for(int j = 0; j < saylist.vBlockCores[i].vblocks.Count; j++)
                                {
                                    var fsay = saylist.vBlockCores[i].vblocks[j];
                                    if(fsay.attachedComponent.component is SayWord saywrd)
                                    {
                                        if(!String.IsNullOrEmpty(saywrd.Words) && saywrd.Words.Contains(txt.value, StringComparison.OrdinalIgnoreCase))
                                        {
                                            filteredResult.Add((saywrd, j));
                                        }
                                    }
                                }
                                
                                break;
                            }

                            if(filterPopup != null && fparent.Contains(filterPopup))
                            {
                                fparent.Remove(filterPopup);
                            }
                            
                            FilterPopUps(filteredResult);
                        }
                    }
                    else
                    {
                        //TODO: CUSTOM SYMBOLS 
                        //Add extra serializedfield in every vblocks for user to enter their own.
                        //OR OR OR! We could just pull it out of the custom attributes! Yeah!, this is the way to go I think!
                    }
                }
            });

            btnFind.text = "<b>Find</b>";

            pop.Add(tb);
            pop.Add(txt);
            pop.Add(btnFind);
            pop.Add(btnClose);
            
            fparent.Add(pop);
            this.Add(fparent);
        }
        private PopupWindow filterPopup;
        private ListView listV;
        public void FilterPopUps(List<(SayWord, int)> says)
        {
            PopupWindow pop = new PopupWindow();
            filterPopup = pop;
            pop.text = "Found vblocks";
            pop.style.width = 200;
            pop.style.height = 200;

            Func<VisualElement> makeItem = () => 
            {
                var lbl = new Label();
                return lbl;
            };

            Action<VisualElement, int> bindItem = (e, i) => 
            {
                string str = string.Empty;

                if(says[i].Item1.Words.Length >= 25)
                {
                    str = (says[i].Item2 + 1).ToString() + ". " + says[i].Item1.Words[0..25] + "...";
                }
                else if(says[i].Item1.Words.Length < 25)
                {
                    str = (says[i].Item2 + 1).ToString() + ". " + says[i].Item1.Words + "...";
                }

                var astype = e as Label;
                astype.text = str;

                if(!PortsUtils.PlayMode)
                {
                    astype.RegisterCallback<MouseDownEvent>((x)=>
                    {
                        PortsUtils.VGraph.listV.ClearSelection();
                        PortsUtils.VGraph.listV.SetSelection(says[i].Item2);
                        PortsUtils.VGraph.listV.ScrollToItem(says[i].Item2);      

                        VBlockUtils.activeListVIndex = PortsUtils.VGraph.listV.selectedIndex;
                        PortsUtils.VGraph?.ShowSelectedVblockSerializedFields();
                    });
                }
            };

            const int itemHeight = 20;

            listV = new ListView(says, itemHeight, makeItem, bindItem);
            listV.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;
            listV.selectionType = SelectionType.Multiple;
            listV.style.flexGrow = 1.0f;

            pop.Add(listV);
            var btnClose = new Button(()=>
            {
                pop.RemoveFromHierarchy();
                findIsActive = false;
            });

            btnClose.text = "<b>Close</b>";

            pop.Add(listV);
            pop.Add(btnClose);
            fparent.Add(pop);
        }
    }
}