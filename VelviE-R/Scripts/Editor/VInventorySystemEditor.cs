using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using TMPro;

namespace VIEditor
{
    [CustomEditor(typeof(VInventory))]
    public class VInventorySystemEditor : Editor
    {
        private VisualElement root;
        private VisualElement dummy;
        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();

            var container = new VisualElement();
            container.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            container.style.flexDirection = FlexDirection.Row;

            var t = target as VInventory;
            root.Add(container);
            container.Add(DrawButton(t));
            

            var resetBtn = new Button();
            resetBtn.style.height = 40;
            resetBtn.text = "Reset Inventory!";
            resetBtn.clicked += ()=>
            {
                dummy.Add(DrawResetConfirm(t));
            };

            dummy = new VisualElement();
            container.Add(resetBtn);
            root.Add(dummy);

            return root;
        }
        public Box DrawCreateCategory(VInventory t)
        {
            Box box = new Box();
            box.style.flexDirection = FlexDirection.Column;

            var subroot = new Label();
            subroot.style.flexDirection = FlexDirection.Row;
            box.Add(subroot);

            var lbl = new Label();
            lbl.style.flexDirection = FlexDirection.Column;
            lbl.style.width = 110;
            lbl.text = "<b>Add category :</b> ";

            Func<VisualElement> makeItem = () =>
            {
                var vis = new VisualElement();
                vis.style.flexDirection = FlexDirection.Column;

                var catVisOne = new Box();
                catVisOne.style.flexDirection = FlexDirection.Row;
                var lblCatName = new Label();
                lblCatName.text = "Name : ";
                lblCatName.style.width = 100;
                catVisOne.Add(lblCatName);
                var txtCatName = new TextField();
                txtCatName.style.width = 200;
                txtCatName.name = "catName";
                catVisOne.Add(txtCatName);

                var catVisTwo = new Box();
                catVisTwo.style.flexDirection = FlexDirection.Row;
                var lblDescName = new Label();
                lblDescName.text = "Description : ";
                lblDescName.style.width = 100;
                catVisTwo.Add(lblDescName);
                var catDesc = new TextField();
                catDesc.style.width = 200;
                catDesc.style.height = 50;
                catDesc.name = "catDescription";
                catDesc.style.flexDirection = FlexDirection.Column;
                catDesc.style.overflow = Overflow.Visible;
                catDesc.style.whiteSpace = WhiteSpace.Normal;
                catDesc.style.unityOverflowClipBox = OverflowClipBox.ContentBox;
                catDesc.multiline = true;
                catVisTwo.Add(catDesc);

                var catVisThree = new Box();
                catVisThree.style.flexDirection = FlexDirection.Row;
                var lblSpriteName = new Label();
                lblSpriteName.text = "Sprite : ";
                lblSpriteName.style.width = 100;
                catVisThree.Add(lblSpriteName);
                var spriteField = new ObjectField();
                spriteField.objectType = typeof(Sprite);
                spriteField.allowSceneObjects = true;
                spriteField.style.width = 200;
                catVisThree.Add(spriteField);

                vis.userData = (txtCatName, catDesc, spriteField);

                vis.Add(catVisOne);
                vis.Add(catVisTwo);
                vis.Add(catVisThree);
                return vis;
            };

            Action<VisualElement, int> bindItem = (e, i) =>
            {
                var asdata = ((TextField, TextField, ObjectField))e.userData;
                asdata.Item1.value = t.category[i].name;
                asdata.Item2.value = t.category[i].description;
                asdata.Item3.value = t.category[i].sprite;

                var idx = i;

                asdata.Item1.RegisterValueChangedCallback((x) =>
                {
                    t.category[idx].name = x.newValue;

                    if(t.category[idx].tmp == null)
                    {
                        t.UpdateCategoryName(t.category[idx].categoryObject, x.newValue);
                    }
                    else
                    {
                        t.category[idx].tmp.SetText(x.newValue);
                        t.category[idx].tmp.ForceMeshUpdate();
                    }
                });

                asdata.Item2.RegisterValueChangedCallback((x) =>
                {
                    t.category[idx].description = x.newValue;
                });

                asdata.Item3.RegisterValueChangedCallback((x) =>
                {
                    t.category[idx].sprite = x.newValue as Sprite;
                    t.UpdateCategorySprite(t.category[idx].categoryObject, x.newValue as Sprite);
                });

            };

            const int itemHeight = 100;
            var addCat = new ListView(t.category, itemHeight, makeItem, bindItem);
            addCat.showBorder = true;
            addCat.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;

            var visBtn = new VisualElement();
            visBtn.style.flexDirection = FlexDirection.Row;

            var lblContainer = new VisualElement();
            lblContainer.style.marginTop = 20;
            lblContainer.style.flexDirection = FlexDirection.Row;
            lblContainer.style.width = 200;

            var btnAdd = new Button();
            btnAdd.style.width = 38;
            btnAdd.text = "<b>+</b>";
            var btnRem = new Button();
            btnRem.style.width = 38;
            btnRem.text = "<b>-</b>";
            lblContainer.Add(btnAdd);
            lblContainer.Add(btnRem);
            visBtn.Add(lblContainer);

            btnAdd.clicked += () =>
            {
                t.CreateCategory("Unnamed", "Empty-description");
                RePoolCategory(t);
                addCat.Rebuild();
            };

            btnRem.clicked += () =>
            {
                if (addCat.selectedItem == null)
                {
                    if (t.category.Count > 0)
                    {
                        DestroyImmediate(t.category[t.category.Count - 1].categoryObject);
                        t.category.RemoveAt(t.category.Count - 1);
                    }
                }
                else
                {
                    DestroyImmediate(t.category[addCat.selectedIndex].categoryObject);
                    t.category.RemoveAt(addCat.selectedIndex);
                }
                RePoolCategory(t);
                addCat.Rebuild();
            };

            subroot.Add(lbl);
            subroot.Add(addCat);
            lbl.Add(visBtn);
            return box;
        }
        public (TextField txt, IntegerField val) CreateSlots(VInventory t, VisualElement vis)
        {
            var slot = new Box();
            slot.name = "slotSlot";
            slot.style.flexDirection = FlexDirection.Row;

            var slotLbl = new TextField();
            slotLbl.value = "<SlotName>";
            slotLbl.tooltip = "Item slot name";
            slotLbl.style.width = 160;
            slot.Add(slotLbl);

            var slotField = new IntegerField();
            slotField.value = 0;
            slotField.tooltip = "Initial value for item slot";
            slotField.style.width = 40;
            slot.Add(slotField);
            vis.Add(slot);
            return (slotLbl, slotField);
        }
        private ListView listV;
        private DateTime date;
        
        public Box DrawCreateItem(VInventory t)
        {
            Box box = new Box();
            box.style.flexDirection = FlexDirection.Column;

            var subroot = new Label();
            subroot.style.width = 700;
            subroot.style.flexDirection = FlexDirection.Row;
            box.Add(subroot);

            var lbl = new Label();
            lbl.style.flexDirection = FlexDirection.Column;
            lbl.style.width = 110;
            lbl.text = "<b>Add item :</b> ";

            Func<VisualElement> makeItem = () =>
            {
                var vis = new VisualElement();
                vis.style.marginBottom = 20;
                vis.style.flexDirection = FlexDirection.Column;

                var catVisOne = new Box();
                catVisOne.style.flexDirection = FlexDirection.Row;
                var lblCatName = new Label();
                lblCatName.style.backgroundColor = Color.gray;
                lblCatName.text = "<b>Name</b> : ";
                lblCatName.style.width = 100;
                catVisOne.Add(lblCatName);
                var txtCatName = new TextField();
                txtCatName.style.width = 200;
                txtCatName.name = "catName";
                catVisOne.Add(txtCatName);

                var catVisTwo = new Box();
                catVisTwo.style.flexDirection = FlexDirection.Row;
                var lblDescName = new Label();
                lblDescName.text = "Description : ";
                lblDescName.style.width = 100;
                catVisTwo.Add(lblDescName);
                var catDesc = new TextField();
                catDesc.style.width = 200;
                catDesc.style.height = 50;
                catDesc.name = "catDescription";
                catDesc.style.flexDirection = FlexDirection.Column;
                catDesc.style.overflow = Overflow.Visible;
                catDesc.style.whiteSpace = WhiteSpace.Normal;
                catDesc.style.unityOverflowClipBox = OverflowClipBox.ContentBox;
                catDesc.multiline = true;
                catVisTwo.Add(catDesc);

                var catVisThree = new Box();
                catVisThree.style.flexDirection = FlexDirection.Row;
                var lblSpriteName = new Label();
                lblSpriteName.text = "Sprite : ";
                lblSpriteName.style.width = 100;
                catVisThree.Add(lblSpriteName);
                var spriteField = new ObjectField();
                spriteField.objectType = typeof(Sprite);
                spriteField.allowSceneObjects = true;
                spriteField.style.width = 200;
                catVisThree.Add(spriteField);

                var catVisFour = new Box();
                catVisFour.style.flexDirection = FlexDirection.Row;
                var lblCatsName = new Label();
                lblCatsName.text = "Category : ";
                lblCatsName.style.width = 100;
                catVisFour.Add(lblCatsName);
                var catsField = new DropdownField();
                categoryFieldListview = catsField;
                catsField.style.width = 200;
                catVisFour.Add(catsField);

                List<string> list = new List<string>();
                t.category.ForEach(x => list.Add(x.name));
                catsField.choices = list;

                var dummyBox = new Box();
                dummyBox.style.flexDirection = FlexDirection.Row;
                var dummylbl = new Label();
                dummylbl.style.width = 100;
                var dummyObject = new VisualElement();
                dummyObject.style.width = 200;
                dummyBox.Add(dummylbl);
                dummyBox.Add(dummyObject);

                var justBtn = new VisualElement();
                justBtn.style.backgroundColor = Color.green;
                justBtn.style.flexDirection = FlexDirection.Row;

                var btnSlotContainer = new VisualElement();
                btnSlotContainer.style.backgroundColor = Color.grey;
                btnSlotContainer.style.flexDirection = FlexDirection.Row;

                var lblBtns = new Label();
                lblBtns.text = "Create slot : ";
                justBtn.Add(lblBtns);
                var btnAddSlot = new Button();
                btnAddSlot.style.width = 40;
                btnAddSlot.text = "<b>+</b>";
                justBtn.Add(btnAddSlot);

                var btnRemSlot = new Button();
                btnRemSlot.style.width = 40;
                btnRemSlot.text = "<b>-</b>";
                justBtn.Add(btnRemSlot);
                dummyObject.Add(justBtn);
                dummyObject.Add(btnSlotContainer);

                vis.userData = (txtCatName, catDesc, spriteField, catsField, btnAddSlot, btnRemSlot, dummyObject);

                vis.Add(catVisOne);
                vis.Add(catVisTwo);
                vis.Add(catVisThree);
                vis.Add(catVisFour);
                vis.Add(dummyBox);
                return vis;
            };

            Action<VisualElement, int> bindItem = (e, i) =>
            {
                var idx = i;
                var asdata = ((TextField, TextField, ObjectField, DropdownField, Button, Button, VisualElement))e.userData;
                asdata.Item1.value = t.inventory[idx].name;
                asdata.Item2.value = t.inventory[idx].description;
                asdata.Item3.value = t.inventory[idx].sprite;

                if(t.inventory[idx] != null && t.inventory[idx].category != null && !String.IsNullOrEmpty(t.inventory[idx].category.name))
                    asdata.Item4.value = t.inventory[idx].category.name;
                else
                    asdata.Item4.value = "<None!>";
                

                if(t.inventory[i].slots.Count > 0)
                {
                    foreach(var itts in t.inventory[i].slots)
                    {
                        if(itts != null)
                        {
                            var slot = CreateSlots(t, asdata.Item7);
                            slot.txt.value = itts.name;
                            slot.val.value = itts.value;

                            slot.txt.RegisterValueChangedCallback((x)=>
                            {
                                itts.name = x.newValue;
                            });

                            slot.val.RegisterValueChangedCallback((x)=>
                            {
                                itts.value = x.newValue;
                            });
                        }
                    }
                }

                asdata.Item1.RegisterValueChangedCallback((x) =>
                {
                    t.inventory[idx].name = x.newValue;
                    t.UpdateCategoryName(t.inventory[idx].inventoryObject, x.newValue);
                });

                asdata.Item2.RegisterValueChangedCallback((x) =>
                {
                    t.inventory[idx].description = x.newValue;
                });

                asdata.Item3.RegisterValueChangedCallback((x) =>
                {
                    t.inventory[idx].sprite = x.newValue as Sprite;
                    t.UpdateCategorySprite(t.inventory[idx].inventoryObject, x.newValue as Sprite);
                });

                asdata.Item4.RegisterCallback<ChangeEvent<string>>((xx) =>
                {
                    if (t.inventory[idx] != null && t.inventory[idx].category != null)
                    {
                        t.inventory[idx].category = t.category.Find(x => x.name == xx.newValue);
                    }
                    else
                    {
                        t.inventory[idx].category = null;
                        asdata.Item4.value = "<None!>";
                    }

                    RePoolCategory(t);
                });

                asdata.Item5.clicked += () =>
                {
                    if(!wasClicked)
                    {
                        wasClicked = true;
                        
                        asdata.Item5.schedule.Execute(()=>
                        {
                            Debug.Log("SDS");
                            t.inventory[idx].slots.Add(new AVStat<int>.AVSlot());
                            var slot = CreateSlots(t, asdata.Item7);
                            slot.txt.value = t.inventory[idx].slots[t.inventory[idx].slots.Count - 1].name;
                            slot.val.value = t.inventory[idx].slots[t.inventory[idx].slots.Count - 1].value;

                            slot.txt.RegisterValueChangedCallback((x)=>
                            {
                                t.inventory[idx].slots[t.inventory[idx].slots.Count - 1].name = x.newValue;
                            });

                            slot.val.RegisterValueChangedCallback((x)=>
                            {
                                t.inventory[idx].slots[t.inventory[idx].slots.Count - 1].value = x.newValue;
                            });
                            
                            wasClicked = false;
                        }).ExecuteLater(1);
                    }
                };

                asdata.Item6.clicked += () =>
                {
                    if (asdata.Item7.childCount > 0)
                    {
                        var parents = asdata.Item7.Children().ToList();
                        if (parents[parents.Count - 1].name == "slotSlot")
                        {
                            if(t.inventory[idx].slots.Count > 0)
                            {
                                parents[parents.Count - 1].RemoveFromHierarchy();
                                t.inventory[idx].slots.RemoveAt(t.inventory[idx].slots.Count - 1);
                            }
                        }
                    }
                };
            };

            var addCat = new ListView(t.inventory, makeItem: makeItem, bindItem: bindItem);
            listV = addCat;
            addCat.showBorder = true;
            addCat.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

            var visBtn = new VisualElement();
            visBtn.style.flexDirection = FlexDirection.Row;

            var lblContainer = new VisualElement();
            lblContainer.style.marginTop = 20;
            lblContainer.style.flexDirection = FlexDirection.Row;
            lblContainer.style.width = 200;

            var btnAdd = new Button();
            btnAdd.style.width = 38;
            btnAdd.text = "<b>+</b>";
            var btnRem = new Button();
            btnRem.style.width = 38;
            btnRem.text = "<b>-</b>";
            lblContainer.Add(btnAdd);
            lblContainer.Add(btnRem);
            visBtn.Add(lblContainer);

            btnAdd.clicked += () =>
            {
                t.CreateItem("Unnamed", "Empty-description", t.activeCategory);
                addCat.Rebuild();
            };

            btnRem.clicked += () =>
            {
                if (addCat.selectedItem == null)
                {
                    if (t.inventory.Count > 0)
                    {
                        DestroyImmediate(t.inventory[t.inventory.Count - 1].inventoryObject);
                        t.inventory.RemoveAt(t.inventory.Count - 1);
                    }
                }
                else
                {
                    DestroyImmediate(t.inventory[addCat.selectedIndex].inventoryObject);
                    t.inventory.RemoveAt(addCat.selectedIndex);
                }

                addCat.Rebuild();
            };

            subroot.Add(lbl);
            subroot.Add(addCat);
            lbl.Add(visBtn);
            box.Add(DrawDefault(t));
            box.Add(ItemWhenZeroed(t));
            return box;
        }
        private bool wasClicked = false;
        private DropdownField categoryFieldListview;
        private DropdownField categoryFieldDefault;
        private VisualElement DrawDefault(VInventory t)
        {            
            var vis = new VisualElement();
            vis.style.flexDirection = FlexDirection.Row;

            var lbl = new Label();
            lbl.style.flexDirection = FlexDirection.Column;
            lbl.style.width = 110;
            lbl.text = "<b>Default category :</b> ";

            var dropd = new DropdownField();
            categoryFieldDefault = dropd;
            dropd.style.width = 300;

            if(t.setDefault == null)
            {
                if(t.category.Count > 0)
                {
                    dropd.value = t.category[0].name;
                    t.setDefault = t.category[0];
                }
                else
                {
                    dropd.value = "<None>";
                }
            }
            else
            {
                if(t.category.Exists(x => x.name == t.setDefault.name))
                {
                    dropd.value = t.setDefault.name;
                }
                else
                {
                    dropd.value = "<None>";
                }
            }

            var lis = new List<string>();
            if(t.category.Count > 0)
            {
                foreach(var cat in t.category)
                {
                    if(cat == null || String.IsNullOrEmpty(cat.name))
                        continue;
                    
                    lis.Add(cat.name);
                }
            }

            dropd.choices = lis;
            dropd.choices.Add("<None>");

            dropd.RegisterCallback<ChangeEvent<string>>((x)=>
            {
                if(x.newValue == "<None>")
                {
                    if(t.category.Count > 0)
                    {
                        dropd.value = t.category[0].name;
                        t.setDefault = t.category[0];
                    }
                    else
                    {
                        t.setDefault = null;
                    }
                }
                else
                {
                    t.setDefault = t.category.Find(xx => xx.name == x.newValue);
                }

                RePoolCategory(t);
            });

            vis.Add(lbl);
            vis.Add(dropd);
            return vis;
        }
        private void RePoolCategory(VInventory t)
        {
            categoryFieldDefault.choices.Clear();

            var lis = new List<string>();

            if(t.category.Count > 0)
            {
                foreach(var cat in t.category)
                {
                    if(cat == null || String.IsNullOrEmpty(cat.name))
                        continue;
                    
                    lis.Add(cat.name);
                }
            }

            lis.Add("<None>");

            categoryFieldDefault.choices = lis;
        }
        private VisualElement ItemWhenZeroed(VInventory t)
        {
            var vis = new VisualElement();
            vis.style.flexDirection = FlexDirection.Row;

            var lbl = new Label();
            lbl.style.flexDirection = FlexDirection.Column;
            lbl.style.width = 110;
            lbl.text = "<b>Item when zero :</b>\nChecks based on\n1st item slot.";

            var dropd = new DropdownField();
            dropd.style.width = 300;
            dropd.choices = Enum.GetNames(typeof(InventoryWhenZero)).ToList();
            dropd.value = t.itemWhenZero.ToString();
            dropd.RegisterCallback<ChangeEvent<string>>((x)=>
            {
                foreach(var item in Enum.GetValues(typeof(InventoryWhenZero)))
                {
                    var asenum = (InventoryWhenZero)item;

                    if(asenum.ToString() == x.newValue)
                    {
                        t.itemWhenZero = asenum;
                        break;
                    }
                }
            });

            vis.Add(lbl);
            vis.Add(dropd);
            return vis;
        }
        public VisualElement DrawButton(VInventory t)
        {
            var vis = new VisualElement();

            var btn = new Button();
            btn.text = "Open Inventory Manager";
            btn.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            btn.style.height = 40;
            vis.Add(btn);

            btn.clicked += ()=>
            {
                if (!EditorWindow.HasOpenInstances<VInventoryManagerWindow>())
                {
                    PortsUtils.ActiveInventory = t;
                    VInventoryManagerWindow vg = EditorWindow.GetWindow<VInventoryManagerWindow>();
                    //vg.ShowModal();
                }
                else
                {
                    EditorWindow.GetWindow<VInventoryManagerWindow>().Close();
                }
            };

            return vis;
        }
        public void ResetInventory(VInventory t)
        {
            if(t.inventory.Count > 0)
            {
                for (int i = t.inventory.Count; i --> 0; )
                {
                    if(t.inventory[i].inventoryObject != null)
                    {
                        DestroyImmediate(t.inventory[i].inventoryObject);
                    }
                }

                t.inventory = new List<VInventoryClass>();
            }
            if(t.category.Count > 0)
            {
                for (int i = t.category.Count; i --> 0; )
                {
                    if(t.category[i].categoryObject != null)
                    {
                        DestroyImmediate(t.category[i].categoryObject);
                    }
                }

                t.category = new List<VInventoryCategory>();
            }

            t.activeCategory = null;
            t.setDefault = null;
            t.inventoryIsMixable = false;
            t.itemSortType = SortType.None;

            Repaint();

            if(EditorWindow.HasOpenInstances<VInventoryManagerWindow>())
            {
                var getWindow = EditorWindow.GetWindow<VInventoryManagerWindow>();
                getWindow.RefreshInventoryWindow();
                getWindow.Repaint();
            }
        }
        private VisualElement DrawResetConfirm(VInventory t)
        {
            if(dummy.childCount > 0)
            {
                foreach(var child in dummy.Children().ToList())
                {
                    if(child != null)
                    child.RemoveFromHierarchy();
                }
            }
            
            var vis = new VisualElement();
            vis.style.flexDirection = FlexDirection.Row;

            var yes = new Button();
            yes.text = "OK!";
            yes.style.width = new StyleLength(new Length(50, LengthUnit.Percent));
            yes.style.height = 40;

            var no = new Button();
            no.text = "Cancel!";
            no.style.width = new StyleLength(new Length(50, LengthUnit.Percent));
            no.style.height = 40;
            vis.Add(yes);
            vis.Add(no);

            yes.clicked += ()=>
            {
                ResetInventory(t);

                if(dummy.childCount > 0)
                {
                    foreach(var child in dummy.Children().ToList())
                    {
                        if(child != null)
                        child.RemoveFromHierarchy();
                    }
                }
            };

            no.clicked += ()=>
            {
                if(dummy.childCount > 0)
                {
                    foreach(var child in dummy.Children().ToList())
                    {
                        if(child != null)
                        child.RemoveFromHierarchy();
                    }
                }
            };

            try
            {
                return vis;
            }
            finally
            {
                dummy.Add(DrawWarning());
            }
        }

        private VisualElement DrawWarning()
        {
            var vis = new VisualElement();
            var lbl = new Label();
            lbl.style.unityTextAlign = TextAnchor.MiddleCenter;
            lbl.text = "RESET INVENTORY!?\nAll inventory data will be cleared!";
            lbl.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            lbl.style.flexDirection = FlexDirection.Column;
            lbl.style.overflow = Overflow.Visible;
            lbl.style.whiteSpace = WhiteSpace.Normal;
            lbl.style.unityOverflowClipBox = OverflowClipBox.ContentBox;
            lbl.style.height = 100;

            vis.Add(lbl);
            return vis;
        }
    }
}