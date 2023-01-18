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
    public class VInventoryManagerWindow : EditorWindow
    {
        public VInventory vinventory { get; set; }
        private ScrollView root;
        private VisualElement categoryRoot;
        private VisualElement itemRoot;
        private VisualElement propertyRoot;
        private DateTime date;
        private bool wasClicked = false;
        private DropdownField categoryFieldListview;
        private DropdownField categoryFieldDefault;
        private DropdownField categorySelector;
        private ListView categoryListv;
        private ListView itemListv;
        private List<DropdownField> itemCategoryDrop;
        void OnEnable()
        {
            vinventory = PortsUtils.ActiveInventory;
            SetToolbar();
        }
        private void SetToolbar()
        {
            if (vinventory == null)
                return;

            categoryRoot = new VisualElement();
            categoryRoot.style.marginLeft = 10;
            itemRoot = new VisualElement();
            itemRoot.style.marginLeft = 10;
            propertyRoot = new VisualElement();
            propertyRoot.style.marginLeft = 10;

            var splitView = new VisualElement();
            splitView.style.flexDirection = FlexDirection.Row;

            var vis = new ScrollView();
            vis.Add(splitView);
            vis.style.marginTop = 20;
            vis.style.marginLeft = 20;
            root = vis;
            rootVisualElement.style.width = Screen.currentResolution.width - 100;
            rootVisualElement.Add(vis);
            splitView.Add(categoryRoot);
            splitView.Add(itemRoot);
            splitView.Add(propertyRoot);

            DrawCategory();
            DrawItem(new List<VInventoryClass>(0));
            DrawDefault();
            ItemWhenZeroed();
            ItemIsMixable();
            ItemIsSortable();

            if (vinventory.activeCategory != null)
            {
                var lis = vinventory.inventory.FindAll(xx => xx.category.name == vinventory.activeCategory.name);
                DrawItem(lis);
            }
            else if (vinventory.activeCategory == null && vinventory.category.Count > 0)
            {
                var lis = vinventory.inventory.FindAll(xx => xx.category.name == vinventory.category[0].name);
                DrawItem(lis);
            }
            else
            {
                DrawItem(new List<VInventoryClass>(0));
                itemAddBtn.SetEnabled(false);
                itemRemBtn.SetEnabled(false);
            }
        }
        private ToolbarSearchField DrawSearchBar()
        {
            var searchBar = new ToolbarSearchField();
            searchBar.style.width = 150;
            searchBar.style.height = 30;
            searchBar.focusable = true;

            searchBar.RegisterCallback<KeyDownEvent>((x) =>
            {
                if (!PortsUtils.PlayMode)
                {
                    if (x.keyCode == KeyCode.Return)
                    {
                        if (!String.IsNullOrEmpty(searchBar.value))
                        {
                            Debug.Log("Return key pressed!");
                        }
                    }
                }
            });

            return searchBar;
        }
        private VisualElement DrawCategorySelector()
        {
            var catVisFour = new Box();
            catVisFour.name = "selector";
            catVisFour.style.backgroundColor = Color.magenta;
            catVisFour.style.marginBottom = 10;
            catVisFour.style.flexDirection = FlexDirection.Row;

            var lblCatsName = new Label();
            lblCatsName.style.color = Color.white;
            lblCatsName.text = "<b>Select Category : </b>";
            lblCatsName.style.width = 110;
            catVisFour.Add(lblCatsName);

            var catsField = new DropdownField();
            categorySelector = catsField;
            catsField.style.width = 160;
            catVisFour.Add(catsField);
            catVisFour.Add(DrawSearchBar());

            if (vinventory.activeCategory != null)
                catsField.value = vinventory.activeCategory.name;
            else
                catsField.value = "<Select Category>";

            if (vinventory.category.Count > 0)
            {
                List<string> list = new List<string>();
                vinventory.category.ForEach(x => list.Add(x.name));
                catsField.choices = list;
            }
            else
            {
                catsField.value = "<Create Category First!>";
            }

            catsField.RegisterCallback<ChangeEvent<string>>((x) =>
            {
                if (!String.IsNullOrEmpty(x.newValue))
                    vinventory.activeCategory = vinventory.category.Find(xx => xx.name == x.newValue);
                else
                    vinventory.activeCategory = null;

                RePoolCategories();

                if (vinventory.activeCategory != null)
                {
                    var lis = vinventory.inventory.FindAll(xx => xx.category.name == x.newValue);
                    DrawItem(lis);
                }
                else
                {
                    DrawItem(new List<VInventoryClass>(0));
                }
            });

            return catVisFour;
        }

        public void DrawCategory()
        {
            if (vinventory == null)
                return;

            Box box = new Box();
            box.style.flexDirection = FlexDirection.Column;

            var subroot = new Label();
            subroot.style.height = Screen.currentResolution.height - 355;
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
                lblCatName.style.backgroundColor = Color.magenta;
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

                vis.userData = (txtCatName, catDesc, spriteField, lblCatName);

                vis.Add(catVisOne);
                vis.Add(catVisTwo);
                vis.Add(catVisThree);
                return vis;
            };

            Action<VisualElement, int> bindItem = (e, i) =>
            {
                var asdata = ((TextField, TextField, ObjectField, Label))e.userData;
                asdata.Item4.text = "<b>" + i + ".</b>" + " :: Name : ";
                asdata.Item1.value = vinventory.category[i].name;
                asdata.Item2.value = vinventory.category[i].description;
                asdata.Item3.value = vinventory.category[i].sprite;

                asdata.Item1.RegisterCallback<FocusOutEvent>((x) =>
                {
                    var idx = i;
                    vinventory.category[idx].name = asdata.Item1.value;

                    if (vinventory.category[idx].tmp == null)
                    {
                        vinventory.UpdateCategoryName(vinventory.category[idx].categoryObject, asdata.Item1.value);
                        vinventory.category[idx].tmp = vinventory.category[idx].categoryObject.GetComponentInChildren<TMP_Text>();
                        vinventory.category[idx].tmp.ForceMeshUpdate();
                    }
                    else
                    {
                        vinventory.category[idx].tmp.SetText(asdata.Item1.value);
                        vinventory.category[idx].tmp.ForceMeshUpdate();
                    }

                    EditorUtility.SetDirty(vinventory.gameObject);
                });

                asdata.Item2.RegisterValueChangedCallback((x) =>
                {
                    var idx = i;
                    vinventory.category[idx].description = x.newValue;
                    EditorUtility.SetDirty(vinventory.gameObject);
                });

                asdata.Item3.RegisterValueChangedCallback((x) =>
                {
                    var idx = i;
                    vinventory.category[idx].sprite = x.newValue as Sprite;
                    vinventory.UpdateCategorySprite(vinventory.category[idx].categoryObject, x.newValue as Sprite);
                    EditorUtility.SetDirty(vinventory.gameObject);
                });

            };

            const int itemHeight = 100;
            var addCat = new ListView(vinventory.category, itemHeight, makeItem, bindItem);
            categoryListv = addCat;
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
                string getName = GetNonDuplicateName("Unnamed");
                vinventory.CreateCategory(getName, "Empty-description");
                addCat.Rebuild();

                itemAddBtn.SetEnabled(true);
                itemRemBtn.SetEnabled(true);

                RePoolCategories();
                EditorUtility.SetDirty(vinventory.gameObject);

            };

            btnRem.clicked += () =>
            {
                if (addCat.selectedItem == null)
                {
                    if (vinventory.category.Count > 0)
                    {
                        var strCatname = vinventory.category[vinventory.category.Count - 1].name;
                        DestroyImmediate(vinventory.category[vinventory.category.Count - 1].categoryObject);
                        vinventory.category.RemoveAt(vinventory.category.Count - 1);

                        var iobj = vinventory.inventory.FindAll(x => x.category.name == strCatname);

                        if (iobj != null && iobj.Count > 0)
                        {
                            foreach (var obj in iobj.ToList())
                            {
                                if (obj != null)
                                {
                                    DestroyImmediate(obj.inventoryObject);
                                    vinventory.inventory.Remove(obj);
                                }
                            }
                        }
                    }
                }
                else
                {
                    var strCatname = vinventory.category[addCat.selectedIndex].name;
                    DestroyImmediate(vinventory.category[addCat.selectedIndex].categoryObject);
                    vinventory.category.RemoveAt(addCat.selectedIndex);

                    var iobj = vinventory.inventory.FindAll(x => x.category.name == strCatname);

                    if (iobj != null && iobj.Count > 0)
                    {
                        foreach (var obj in iobj.ToList())
                        {
                            if (obj != null)
                            {
                                DestroyImmediate(obj.inventoryObject);
                                vinventory.inventory.Remove(obj);
                            }
                        }
                    }
                }

                if (vinventory.category.Count == 0)
                {
                    itemAddBtn.SetEnabled(false);
                    itemRemBtn.SetEnabled(false);
                    vinventory.setDefault = null;
                    vinventory.activeCategory = null;
                }

                RePoolCategories();
                addCat.Rebuild();
                itemListv.Rebuild();
                EditorUtility.SetDirty(vinventory.gameObject);
            };

            subroot.Add(lbl);
            subroot.Add(addCat);
            lbl.Add(visBtn);
            categoryRoot.Add(box);
        }

        private void RePoolCategories()
        {
            if (categoryFieldListview != null)
                categoryFieldListview.choices.Clear();
            if (categoryFieldDefault != null)
                categoryFieldDefault.choices.Clear();
            if (categorySelector != null)
                categorySelector.choices.Clear();

            if (itemCategoryDrop.Count > 0)
            {
                foreach (var cats in itemCategoryDrop.ToList())
                {
                    cats.choices.Clear();
                }
            }

            if (vinventory.category.Count > 0)
            {
                var lis = new List<string>();

                for (int i = 0; i < vinventory.category.Count; i++)
                {
                    if (vinventory.category[i] == null)
                        continue;

                    lis.Add(vinventory.category[i].name);
                }

                if (categoryFieldListview != null)
                    categoryFieldListview.choices = lis;
                if (categoryFieldDefault != null)
                    categoryFieldDefault.choices = lis;
                if (categorySelector != null)
                    categorySelector.choices = lis;

                if (itemCategoryDrop.Count > 0)
                {
                    foreach (var cats in itemCategoryDrop.ToList())
                    {
                        cats.choices = lis;
                    }
                }
            }
            else
            {
                if (categoryFieldListview != null)
                    categoryFieldListview.value = "<No categories!>";
                if (categoryFieldDefault != null)
                    categoryFieldDefault.value = "<No categories!>";
                if (categorySelector != null)
                    categorySelector.value = "<No categories!>";

                if (itemCategoryDrop.Count > 0)
                {
                    foreach (var cats in itemCategoryDrop.ToList())
                    {
                        cats.value = "<No categories!>";
                    }
                }
            }

            if (categorySelector != null && vinventory.activeCategory != null)
            {
                categorySelector.value = vinventory.activeCategory.name;
            }
            else if (categorySelector != null)
            {
                categorySelector.value = "<No categories!>";
            }
        }
        private void ClearItemRoot()
        {
            if (itemRoot != null && itemRoot.childCount > 0)
            {
                foreach (var child in itemRoot.Children().ToList())
                {
                    if (child != null)
                    {
                        child.RemoveFromHierarchy();
                    }
                }
            }
        }

        public void DrawItem(List<VInventoryClass> invi)
        {
            if (vinventory == null)
                return;

            ClearItemRoot();

            Box box = new Box();
            box.style.width = 450;
            box.style.flexDirection = FlexDirection.Column;
            box.Add(DrawCategorySelector());

            var subroot = new Label();
            subroot.style.height = Screen.currentResolution.height - 400;
            subroot.style.width = 440;
            subroot.style.flexDirection = FlexDirection.Row;
            box.Add(subroot);

            var lbl = new Label();
            lbl.style.flexDirection = FlexDirection.Column;
            lbl.style.width = 110;
            lbl.text = "<b>Add item :</b> ";

            itemCategoryDrop = new List<DropdownField>();

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
                catsField.style.width = 200;
                catVisFour.Add(catsField);

                var catVisBool = new Box();
                catVisBool.style.flexDirection = FlexDirection.Row;
                var lblCatsBoolName = new Label();
                lblCatsBoolName.text = "Set Active : ";
                lblCatsBoolName.style.width = 100;
                catVisBool.Add(lblCatsBoolName);
                var catsBoolField = new Toggle();
                catsBoolField.style.width = 200;
                catVisBool.Add(catsBoolField);

                List<string> list = new List<string>();
                vinventory.category.ForEach(x => list.Add(x.name));
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

                vis.userData = (txtCatName, catDesc, spriteField, catsField, btnAddSlot, btnRemSlot, dummyObject, lblCatName);

                vis.Add(catVisOne);
                vis.Add(catVisTwo);
                vis.Add(catVisThree);
                vis.Add(catVisFour);
                vis.Add(dummyBox);
                return vis;
            };

            Action<VisualElement, int> bindItem = (e, i) =>
            {
                var asdata = ((TextField, TextField, ObjectField, DropdownField, Button, Button, VisualElement, Label))e.userData;
                asdata.Item1.value = invi[i].name;
                asdata.Item2.value = invi[i].description;
                asdata.Item3.value = invi[i].sprite;
                asdata.Item8.text = "<b><color=white> " + i + ".</color></b>" + " Name : ";

                if (invi[i] != null && invi[i].category != null && !String.IsNullOrEmpty(invi[i].category.name))
                    asdata.Item4.value = invi[i].category.name;
                else
                    asdata.Item4.value = "<None!>";


                if (invi[i].slots.Count > 0)
                {
                    foreach (var itts in invi[i].slots)
                    {
                        if (itts != null)
                        {
                            var slot = CreateSlots(vinventory, asdata.Item7);
                            slot.txt.value = itts.name;
                            slot.val.value = itts.value;

                            slot.txt.RegisterValueChangedCallback((x) =>
                            {
                                itts.name = x.newValue;
                            });

                            slot.val.RegisterValueChangedCallback((x) =>
                            {
                                itts.value = x.newValue;
                            });
                        }
                    }
                }

                asdata.Item1.RegisterCallback<FocusOutEvent>((x) =>
                {
                    var idx = i;
                    invi[idx].name = asdata.Item1.value;

                    if (invi[idx].tmp == null)
                    {
                        vinventory.UpdateCategoryName(invi[idx].inventoryObject, asdata.Item1.value);
                        invi[idx].tmp = invi[idx].inventoryObject.GetComponentInChildren<TMP_Text>();
                        invi[idx].tmp.ForceMeshUpdate();
                    }
                    else
                    {
                        invi[idx].tmp.SetText(asdata.Item1.value);
                        invi[idx].tmp.ForceMeshUpdate();
                    }

                    EditorUtility.SetDirty(vinventory.gameObject);
                });

                asdata.Item2.RegisterValueChangedCallback((x) =>
                {
                    var idx = i;
                    invi[idx].description = x.newValue;
                    EditorUtility.SetDirty(vinventory.gameObject);
                });

                asdata.Item3.RegisterValueChangedCallback((x) =>
                {
                    var idx = i;
                    invi[idx].sprite = x.newValue as Sprite;
                    vinventory.UpdateCategorySprite(invi[idx].inventoryObject, x.newValue as Sprite);
                    EditorUtility.SetDirty(vinventory.gameObject);
                });

                itemCategoryDrop.Add(asdata.Item4);

                asdata.Item4.RegisterCallback<ChangeEvent<string>>((xx) =>
                {
                    var idx = i;

                    if (invi[idx] != null && invi[idx].category != null)
                    {
                        invi[idx].category = vinventory.category.Find(x => x.name == xx.newValue);
                    }
                    else
                    {
                        invi[idx].category = null;
                        asdata.Item4.value = "<None!>";
                    }

                    RePoolCategories();
                    EditorUtility.SetDirty(vinventory.gameObject);
                });

                asdata.Item5.clicked += () =>
                {
                    if (!wasClicked)
                    {
                        if (vinventory.category.Count > 0 && vinventory.activeCategory != null)
                        {
                            var idx = i;
                            wasClicked = true;

                            asdata.Item5.schedule.Execute(() =>
                            {
                                invi[idx].slots.Add(new AVStat<int>.AVSlot());
                                var slot = CreateSlots(vinventory, asdata.Item7);
                                slot.txt.value = invi[idx].slots[invi[idx].slots.Count - 1].name;
                                slot.val.value = invi[idx].slots[invi[idx].slots.Count - 1].value;

                                slot.txt.RegisterValueChangedCallback((x) =>
                                {
                                    invi[idx].slots[invi[idx].slots.Count - 1].name = x.newValue;
                                });

                                slot.val.RegisterValueChangedCallback((x) =>
                                {
                                    invi[idx].slots[invi[idx].slots.Count - 1].value = x.newValue;
                                });

                                wasClicked = false;
                                EditorUtility.SetDirty(vinventory.gameObject);
                            }).ExecuteLater(1);
                        }
                    }
                };

                asdata.Item6.clicked += () =>
                {
                    if (asdata.Item7.childCount > 0)
                    {
                        var idx = i;
                        var parents = asdata.Item7.Children().ToList();
                        if (parents[parents.Count - 1].name == "slotSlot")
                        {
                            if (invi[idx].slots.Count > 0)
                            {
                                parents[parents.Count - 1].RemoveFromHierarchy();
                                invi[idx].slots.RemoveAt(invi[idx].slots.Count - 1);
                            }
                        }

                        EditorUtility.SetDirty(vinventory.gameObject);
                    }
                };
            };

            var addCat = new ListView(invi, makeItem: makeItem, bindItem: bindItem);
            itemListv = addCat;
            addCat.showBorder = true;
            addCat.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

            var visBtn = new VisualElement();
            visBtn.style.flexDirection = FlexDirection.Row;

            var lblContainer = new VisualElement();
            lblContainer.style.marginTop = 20;
            lblContainer.style.flexDirection = FlexDirection.Row;
            lblContainer.style.width = 200;

            var btnAdd = new Button();
            itemAddBtn = btnAdd;
            btnAdd.style.width = 38;
            btnAdd.text = "<b>+</b>";
            var btnRem = new Button();
            itemRemBtn = btnRem;
            btnRem.style.width = 38;
            btnRem.text = "<b>-</b>";
            lblContainer.Add(btnAdd);
            lblContainer.Add(btnRem);
            visBtn.Add(lblContainer);

            btnAdd.clicked += () =>
            {
                if (vinventory.activeCategory != null)
                {
                    string getName = GetNonDuplicateName("Unnamed item", false);
                    vinventory.CreateItem(getName, "Empty-description", vinventory.activeCategory);
                    var lis = vinventory.inventory.FindAll(x => x.category == vinventory.activeCategory);
                    DrawItem(lis);
                    EditorUtility.SetDirty(vinventory.gameObject);
                }
            };

            btnRem.clicked += () =>
            {
                if (addCat.selectedItem == null)
                {
                    if (invi.Count > 0)
                    {
                        var lis = vinventory.inventory.FindAll(x => x.category == vinventory.activeCategory);

                        var lastItem = lis[lis.Count - 1];
                        invi.RemoveAt(lis.Count - 1);
                        var foundInvi = vinventory.inventory.Find(x => x.category == lastItem.category && x.name == lastItem.name);
                        DestroyImmediate(foundInvi.inventoryObject);
                        vinventory.inventory.Remove(foundInvi);
                    }
                }
                else
                {
                    var item = invi[addCat.selectedIndex];
                    var sorted = vinventory.inventory.Find(x => x.name == item.name && x.category == item.category);
                    invi.RemoveAt(addCat.selectedIndex);
                    DestroyImmediate(item.inventoryObject);
                    vinventory.inventory.Remove(sorted);
                }

                addCat.Rebuild();
            };

            subroot.Add(lbl);
            subroot.Add(addCat);
            lbl.Add(visBtn);
            itemRoot.Add(box);
        }
        private Button itemAddBtn;
        private Button itemRemBtn;

        private void ItemWhenZeroed()
        {
            var vis = new VisualElement();
            vis.style.marginTop = 10;
            vis.style.flexDirection = FlexDirection.Row;

            var lbl = new Label();
            lbl.style.flexDirection = FlexDirection.Column;
            lbl.style.width = 110;
            lbl.text = "<b>Item when zero :</b>\nChecks based on\n1st item slot.";

            var dropd = new DropdownField();
            dropd.style.width = 300;
            dropd.choices = Enum.GetNames(typeof(InventoryWhenZero)).ToList();
            dropd.value = vinventory.itemWhenZero.ToString();
            dropd.RegisterCallback<ChangeEvent<string>>((x) =>
            {
                foreach (var item in Enum.GetValues(typeof(InventoryWhenZero)))
                {
                    var asenum = (InventoryWhenZero)item;

                    if (asenum.ToString() == x.newValue)
                    {
                        vinventory.itemWhenZero = asenum;
                        break;
                    }
                }
            });

            vis.Add(lbl);
            vis.Add(dropd);
            propertyRoot.Add(vis);
        }
        private void ItemIsMixable()
        {
            var vis = new VisualElement();
            vis.style.marginTop = 10;
            vis.style.flexDirection = FlexDirection.Row;

            var lbl = new Label();
            lbl.style.flexDirection = FlexDirection.Column;
            lbl.style.width = 110;
            lbl.text = "<b>Mixable :</b> ";

            var dropd = new DropdownField();
            dropd.style.width = 300;
            dropd.choices = Enum.GetNames(typeof(InventoryWhenZero)).ToList();

            if (vinventory.inventoryIsMixable)
            {
                dropd.value = "Mixable";
            }
            else
            {
                dropd.value = "Non-Mixable";
            }

            dropd.choices = new List<string> { "Mixable", "Non-Mixable" };

            dropd.RegisterCallback<ChangeEvent<string>>((x) =>
            {
                if (x.newValue == "Mixable")
                {
                    vinventory.inventoryIsMixable = true;
                }
                else
                {
                    vinventory.inventoryIsMixable = false;
                }
            });

            vis.Add(lbl);
            vis.Add(dropd);
            propertyRoot.Add(vis);
        }
        private void ItemIsSortable()
        {
            var vis = new VisualElement();
            vis.style.marginTop = 10;
            vis.style.flexDirection = FlexDirection.Row;

            var lbl = new Label();
            lbl.style.flexDirection = FlexDirection.Column;
            lbl.style.width = 110;
            lbl.text = "<b>Sort item :</b> ";

            var dropd = new DropdownField();
            dropd.style.width = 300;
            dropd.choices = Enum.GetNames(typeof(SortType)).ToList();
            dropd.value = vinventory.itemSortType.ToString();

            dropd.RegisterCallback<ChangeEvent<string>>((x) =>
            {
                foreach (var ast in Enum.GetValues(typeof(SortType)))
                {
                    var astype = (SortType)ast;

                    if (x.newValue == astype.ToString())
                    {
                        vinventory.itemSortType = astype;
                    }
                }

                if (x.newValue == "ByName")
                {
                    vinventory.inventory.Sort((a, b) => a.name.CompareTo(b.name));
                }
                else if (x.newValue == "ByNameReverse")
                {
                    vinventory.inventory.Sort((a, b) => b.name.CompareTo(a.name));
                }

                EditorUtility.SetDirty(vinventory.gameObject);
            });

            vis.Add(lbl);
            vis.Add(dropd);
            propertyRoot.Add(vis);
        }
        private void DrawDefault()
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

            if (vinventory.setDefault == null)
            {
                if (vinventory.category.Count > 0)
                {
                    dropd.value = vinventory.category[0].name;
                    vinventory.setDefault = vinventory.category[0];
                }
                else
                {
                    dropd.value = "<None>";
                }
            }
            else
            {
                if (vinventory.category.Exists(x => x.name == vinventory.setDefault.name))
                {
                    dropd.value = vinventory.setDefault.name;
                }
                else
                {
                    dropd.value = "<None>";
                }
            }

            var lis = new List<string>();
            if (vinventory.category.Count > 0)
            {
                foreach (var cat in vinventory.category)
                {
                    if (cat == null || String.IsNullOrEmpty(cat.name))
                        continue;

                    lis.Add(cat.name);
                }
            }

            dropd.choices = lis;
            dropd.choices.Add("<None>");

            dropd.RegisterCallback<ChangeEvent<string>>((x) =>
            {
                if (x.newValue == "<None>")
                {
                    if (vinventory.category.Count > 0)
                    {
                        dropd.value = vinventory.category[0].name;
                        vinventory.setDefault = vinventory.category[0];
                    }
                    else
                    {
                        vinventory.setDefault = null;
                    }
                }
                else
                {
                    vinventory.setDefault = vinventory.category.Find(xx => xx.name == x.newValue);
                }

                RePoolCategories();
            });

            vis.Add(lbl);
            vis.Add(dropd);
            propertyRoot.Add(vis);
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
        public string GetNonDuplicateName(string vname, bool isCategory = true)
        {
            string s = string.Empty;
            string defValue = vname;
            int counta = 0;

            ReIterateNodesDescription();

            void ReIterateNodesDescription()
            {
                if (isCategory)
                {
                    foreach (var catNames in vinventory.category)
                    {
                        if (catNames == null)
                            continue;

                        if (catNames.name == defValue && counta == 0)
                        {
                            counta++;
                            ReIterateNodesDescription();
                            return;
                        }
                        else if (catNames.name == defValue + counta && counta > 0)
                        {
                            counta++;
                            ReIterateNodesDescription();
                            return;
                        }
                    }
                }
                else
                {
                    foreach (var inNames in vinventory.inventory)
                    {
                        if (inNames == null)
                            continue;

                        if (inNames.name == defValue && counta == 0)
                        {
                            counta++;
                            ReIterateNodesDescription();
                            return;
                        }
                        else if (inNames.name == defValue + counta && counta > 0)
                        {
                            counta++;
                            ReIterateNodesDescription();
                            return;
                        }
                    }
                }

                if (counta == 0)
                {
                    s = defValue;
                }
                else
                {
                    s = defValue + counta;
                }
            }
            return s;
        }
        public void RefreshInventoryWindow()
        {
            if(rootVisualElement.childCount > 0)
            {
                foreach(var child in rootVisualElement.Children().ToList())
                {
                    if(child != null)
                    {
                        child.RemoveFromHierarchy();
                    }
                }
            }

            OnEnable();
        }
    }
}