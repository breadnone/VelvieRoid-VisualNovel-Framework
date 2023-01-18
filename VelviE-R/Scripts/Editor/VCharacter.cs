using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Experimental;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using System.Linq;
using PopupWindow = UnityEngine.UIElements.PopupWindow;
using System;
using VelvieR;
using UnityEngine.Events;

/// One of these days, one would refactor this and splite them into proper uss.. lol... not me for sure!

namespace VIEditor
{
    [System.Serializable]
    public class PlaceHolderRect
    {
        public VisualElement visualElement;
        public string name;
        public int id;
    }
    [System.Serializable]
    public enum CharacterStageMode
    {
        Character,
        Stage,
        Edit,
        Property,
        None
    }
    [System.Serializable]
    public class CustomField
    {
        public string name;
        public string description;
        public string guid;
    }
    public class VCharacter : EditorWindow
    {
        private float defaultWidth;
        private float defaultHeight;
        public ToolbarMenu characters { get; set; }
        public CharacterStageMode mode { get; set; }
        public bool lockState { get; set; }
        public VisualElement screenPanel { get; set; }
        public static VCharacter vchar;
        public List<PlaceHolderRect> placeholders = new List<PlaceHolderRect>();
        private List<DragManipulator> manipulators;
        private List<ResizableElement> resizeables;
        private List<Button> buttons = new List<Button>();
        public ScrollView mainContainer { get; set; }
        private Box threeDhee;
        private ScrollView rootScroll;
        private Image CharacterThmImage;
        [MenuItem("VelviE-R/VCharacter")]
        public static void OpenVGraphsWindow()
        {
            var window = GetWindow<VCharacter>(typeof(HostGuiWindow));
            vchar = window;
            window.titleContent = new GUIContent("VCharacter");
            window.titleContent.tooltip = "VCharacter Editor";
        }
        private Vector2 GameViewRes()
        {
            return Handles.GetMainGameViewSize();
        }
        private void OnEnable()
        {
            defaultWidth = (GameViewRes().x / 3f) / 3.5f;
            defaultHeight = (GameViewRes().y / 2f) / 2.5f;

            mode = CharacterStageMode.Character;
            SetToolbar(CharacterStageMode.Character);
        }
        private void OnDisable()
        {
            mode = CharacterStageMode.None;
            PortsUtils.activeVCharacter = null;
        }

        public void SetToolbar(CharacterStageMode mode)
        {
            if (mainContainer != null)
            {
                rootVisualElement.Remove(mainContainer);
            }

            var scrollV = new ScrollView();
            scrollV.horizontalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
            scrollV.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
            mainContainer = scrollV;

            rootVisualElement.Add(mainContainer);
            manipulators = new List<DragManipulator>();

            Toolbar toolbar = new Toolbar();
            toolbar.style.marginTop = 10;
            toolbar.style.alignSelf = Align.Center;
            toolbar.style.position = Position.Relative;
            toolbar.style.borderBottomColor = Color.grey;
            toolbar.style.borderBottomWidth = 3;
            toolbar.style.height = 40;
            mainContainer.Add(toolbar);

            Button btnCreateChracter = new Button();
            btnCreateChracter.style.backgroundColor = Color.magenta;
            btnCreateChracter.text = "Create Character";
            btnCreateChracter.style.height = 30;
            btnCreateChracter.style.width = 120;
            buttons.Add(btnCreateChracter);

            if (!toolbar.Contains(buttons[0]))
            {
                toolbar.Add(buttons[0]);
            }

            SetScreenPanel(mode);

            buttons[0].clicked += () =>
            {
                if (mode != CharacterStageMode.Character)
                {
                    mode = CharacterStageMode.Character;
                    buttons[0].RemoveFromHierarchy();
                    buttons[1].RemoveFromHierarchy();
                    rootVisualElement.Remove(mainContainer);

                    SetToolbar(CharacterStageMode.Character);
                    ActiveButton(buttons[0]);
                }
            };
        }
        protected VisualElement DrawClickable()
        {
            var box = new VisualElement();
            box.style.flexDirection = FlexDirection.Row;

            var lbl = new Label();
            lbl.style.width = 150;
            lbl.text = "VGraph : ";
            box.Add(lbl);

            var boxcontain = new VisualElement();
            box.Add(boxcontain);

            var obj = new ObjectField();
            obj.style.width = 250;
            obj.allowSceneObjects = true;
            obj.objectType = typeof(VCoreUtil);
            boxcontain.Add(obj);

            box.SetEnabled(false);
            return box;
        }
        protected VisualElement DrawCustomTags(VCharacterV vchar)
        {
            var box = new VisualElement();
            box.style.flexDirection = FlexDirection.Row;

            var lbl = new Label();
            lbl.style.width = 150;
            lbl.text = "Custom tag : ";
            box.Add(lbl);

            var obj = new TextField();
            obj.style.width = 250;
            box.Add(obj);

            if (!String.IsNullOrEmpty(vchar.customTag))
                obj.value = vchar.customTag;
            else
                obj.value = "[None]";
            return box;
        }
        private void ActiveButton(Button btn)
        {
            btn.style.backgroundColor = Color.grey;

            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i] != btn)
                {
                    buttons[i].style.backgroundColor = Color.magenta;
                }
            }
        }
        public void SetScreenPanel(CharacterStageMode mode = CharacterStageMode.Stage)
        {
            if (mode == CharacterStageMode.Character)
            {
                Box box = new Box();
                VisualElement placeHolder = new VisualElement();
                placeHolder.style.width = GameViewRes().x / 3;
                placeHolder.style.height = GameViewRes().y / 2;
                box.Add(placeHolder);

                //set bottom toolbar
                var boxTool = new Box();
                boxTool.style.height = 40;
                boxTool.style.marginTop = 10;
                boxTool.style.flexDirection = FlexDirection.Row;
                boxTool.style.alignSelf = Align.Center;
                boxTool.style.alignItems = Align.Center;

                var sizeTitle = new Label();
                sizeTitle.text = "Set thumbnail size : ";

                var sizeField = new Vector2Field();
                sizeField.value = new Vector2(defaultWidth, defaultHeight);
                sizeField.RegisterValueChangedCallback((x) =>
                {
                    Debug.Log("thumbnail size changed");
                });

                sizeField.style.height = 20;
                sizeField.style.width = 180;
                mainContainer.Add(boxTool);

                var btnAddChara = new Button();
                btnAddChara.style.height = 20;
                btnAddChara.style.width = 120;
                btnAddChara.text = "Add Character";

                var btnRemChara = new Button();
                btnRemChara.style.height = 20;
                btnRemChara.style.width = 120;
                btnRemChara.text = "Remove Character";

                var charaD = new ToolbarMenu();
                charaD.style.width = 200;
                characters = charaD;

                boxTool.Add(charaD);
                boxTool.Add(sizeTitle);
                boxTool.Add(sizeField);
                boxTool.Add(btnAddChara);
                boxTool.Add(btnRemChara);

                ///Summary
                placeHolder.style.width = GameViewRes().x / 2;
                placeHolder.style.height = GameViewRes().y / 2;
                placeHolder.style.flexDirection = FlexDirection.Row;

                box.userData = placeHolder as VisualElement;
                box.style.backgroundColor = Color.grey;
                box.style.marginTop = 15;
                box.style.alignSelf = Align.Center;
                box.name = "screenPanel";
                box.style.width = GameViewRes().x / 3;
                box.style.height = GameViewRes().y / 2;
                box.style.borderBottomWidth = 6;
                box.style.borderTopWidth = 6;
                box.style.borderLeftWidth = 6;
                box.style.borderRightWidth = 6;
                box.style.borderBottomColor = Color.black;
                box.style.borderTopColor = Color.black;
                box.style.borderLeftColor = Color.black;
                box.style.borderRightColor = Color.black;

                mainContainer.Add(box);
                screenPanel = box;

                //Character properties
                Box imgBox = new Box();
                imgBox.style.flexDirection = FlexDirection.Row;
                imgBox.style.marginTop = 10;
                imgBox.style.marginLeft = 10;
                imgBox.style.width = box.style.width;
                imgBox.style.backgroundColor = Color.clear;
                imgBox.name = "charaBox";

                var imgContainer = new Box();
                imgContainer.style.borderBottomWidth = 2;
                imgContainer.style.borderTopWidth = 2;
                imgContainer.style.borderLeftWidth = 2;
                imgContainer.style.borderRightWidth = 2;
                imgContainer.style.borderBottomColor = Color.blue;
                imgContainer.style.borderTopColor = Color.blue;
                imgContainer.style.borderLeftColor = Color.blue;
                imgContainer.style.borderRightColor = Color.blue;
                imgContainer.style.backgroundColor = Color.white;
                imgContainer.style.width = defaultWidth;
                imgContainer.style.height = defaultHeight;

                var thmImg = new Image();
                thmImg.scaleMode = ScaleMode.ScaleToFit;
                CharacterThmImage = thmImg;
                imgBox.Add(imgContainer);
                imgContainer.Add(thmImg);
                placeHolder.Add(imgBox);

                var descBox = new ScrollView();
                rootScroll = descBox;
                descBox.style.marginLeft = 10;
                descBox.style.flexDirection = FlexDirection.Column;
                imgBox.Add(descBox);

                RepoolComboboxes();

                btnAddChara.clicked += () =>
                {
                    CreateCharacter(charaD);
                };
            }
        }
        private Box activeBox;

        private void RepoolComboboxes(string cname = "")
        {
            var vinstance = PortsUtils.GetCharacters();

            if (vinstance != null && vinstance.Length > 0)
            {
                characters.menu.MenuItems().Clear();

                foreach (var character in vinstance)
                {
                    characters.menu.AppendAction(character.name, (x) =>
                    {
                        characters.text = character.name;
                        AddCharacter(character);
                        PreviewCharaImage(null);
                    });
                }

                characters.text = "<None>";
                PreviewCharaImage(null);
            }
            else
            {
                characters.text = "<None>";
            }

            if (!String.IsNullOrEmpty(cname))
            {
                characters.text = cname;
            }
        }
        public void AddCharacter(VCharacterV vchara)
        {
            if (activeBox != null)
            {
                rootScroll.Remove(activeBox);
            }

            activeBox = CharaProperties(vchara: vchara);
            rootScroll.Add(activeBox);
            RepoolComboboxes(vchara.name);

            foreach (var descs in activeBox.Children())
            {
                foreach (var childs in descs.Children())
                {
                    if (childs.name == "nameField")
                    {
                        var nm = (TextField)childs;
                        nm.value = vchara.name;
                        nm.RegisterValueChangedCallback((x) =>
                        {
                            if (!PortsUtils.PlayMode)
                            {
                                vchara.name = nm.value;
                            }
                        });
                    }
                    else if (childs.name == "descField")
                    {
                        var nm = (TextField)childs;
                        nm.value = vchara.description;
                        nm.RegisterValueChangedCallback((x) =>
                        {
                            if (!PortsUtils.PlayMode)
                            {
                                vchara.description = nm.value;
                            }
                        });
                    }
                    else if (childs.name == "profField")
                    {
                        var nm = (TextField)childs;
                        nm.value = vchara.profile;
                        nm.RegisterValueChangedCallback((x) =>
                        {
                            if (!PortsUtils.PlayMode)
                            {
                                vchara.profile = nm.value;
                            }
                        });
                    }
                }
            }

            EditorUtility.SetDirty(vchara.root);
        }
        public void PreviewCharaImage(Sprite sprites)
        {
            CharacterThmImage.style.backgroundColor = Color.clear;
            CharacterThmImage.style.backgroundImage = new StyleBackground(sprites);
        }
        public void RemoveCharacter(VCharacterV vchara)
        {
            if (vchara == null)
                return;

            if (PortsUtils.VCharaContainer.character.Contains(vchara))
            {
                PortsUtils.VCharaContainer.character.Remove(vchara);
            }
        }
        public Box CreatePlaceholder()
        {
            var box = new Box();
            var placeholderBox = new Box();
            placeholderBox.style.alignItems = Align.Center;
            placeholderBox.name = "placeHolderBox";
            placeholderBox.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            box.Add(placeholderBox);

            var nameLabel = new Label();
            nameLabel.name = "stageLabel";
            box.userData = nameLabel as Label;
            nameLabel.transform.position = box.transform.position;

            placeholderBox.Add(nameLabel);
            ResizableElement rs = new ResizableElement();
            rs.name = "resizeable";
            rs.SetEnabled(false);
            box.Add(rs);

            resizeables.Add(rs);

            box.style.borderBottomWidth = 2;
            box.style.borderTopWidth = 2;
            box.style.borderLeftWidth = 2;
            box.style.borderRightWidth = 2;
            box.style.borderBottomColor = Color.red;
            box.style.borderTopColor = Color.red;
            box.style.borderLeftColor = Color.red;
            box.style.borderRightColor = Color.red;

            DragManipulator manipulator = new(box);
            manipulators.Add(manipulator);

            box.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
            {
                if (PortsUtils.activeVCharacter != null && lockState)
                {
                    manipulator.enableClick = true;

                    evt.menu.AppendAction("Align all Y-axis to this", (x) =>
                    {
                        foreach (var t in placeholders)
                        {
                            var e = t.visualElement as Box;

                            if (e != box)
                            {
                                e.transform.position = new Vector3(e.transform.position.x, box.transform.position.y, e.transform.position.z);
                            }
                        }
                    });

                    evt.menu.AppendAction("Set all stage sizes to this", (x) =>
                    {
                        foreach (var t in placeholders)
                        {
                            var e = t.visualElement as Box;

                            if (e != box)
                            {
                                e.style.width = box.style.width;
                            }
                        }
                    });

                    evt.menu.AppendAction("Remove", (x) => { });
                }
                else if (PortsUtils.activeVCharacter != null && !lockState)
                {
                    manipulator.enableClick = false;
                }
            }));

            return box;
        }
        private ListView spList;
        private VisualElement is2delement;
        private VisualElement graph;
        private VisualElement selectedCustomField;
        private List<(VisualElement, int, string)> tagsIndex = new List<(VisualElement, int, string)>();
  
        public Box CharaProperties(bool isDefault = false, VCharacterV vchara = null)
        {
            var boxy = new Box();
            var name = new Box(); //Name
            name.style.flexDirection = FlexDirection.Row;
            var nmLbl = new Label("Name : ");
            nmLbl.style.width = 150;
            name.Add(nmLbl);

            var strName = new TextField();
            strName.name = "nameField";
            strName.value = "Unnamed";
            strName.style.width = 250;
            name.Add(strName);
            boxy.Add(name);

            var is2dt = new Box();
            is2dt.style.flexDirection = FlexDirection.Row;
            var is2dlbl = new Label();
            is2dlbl.style.width = 150;
            is2dt.Add(is2dlbl);

            var is2dtoggle = new ToolbarMenu();
            is2dtoggle.style.width = 250;
            is2dtoggle.text = "Character is 2D";
            is2dt.Add(is2dtoggle);
            boxy.Add(is2dt);

            is2dtoggle.menu.AppendAction("Character is 2D", (x) =>
            {
                threeDhee.SetEnabled(false);
                is2delement.SetEnabled(false);
                vchara.is2D = true;
                is2dtoggle.text = "Character is 2D";
            });

            is2dtoggle.menu.AppendAction("Character is 3D", (x) =>
            {
                threeDhee.SetEnabled(true);
                is2delement.SetEnabled(true);
                vchara.is2D = false;
                is2dtoggle.text = "Character is 3D";
            });

            var profile = new Box(); //Description profile
            profile.style.flexDirection = FlexDirection.Row;

            var profileLbl = new Label("Description : ");
            profileLbl.style.width = 150;
            profile.Add(profileLbl);

            var strProf = new TextField();
            strProf.name = "descField";
            strProf.multiline = true;
            strProf.style.width = 250;
            strProf.style.height = 50;
            profile.Add(strProf);
            boxy.Add(profile);

            var hobby = new Box(); //Hobby
            hobby.style.flexDirection = FlexDirection.Row;

            var hobbyLbl = new Label("Profile : ");
            hobbyLbl.style.width = 150;
            hobby.Add(hobbyLbl);

            var strhobby = new TextField();
            strhobby.name = "profField";
            strhobby.multiline = true;
            strhobby.style.width = 250;
            strhobby.style.height = 50;
            hobby.Add(strhobby);
            boxy.Add(hobby);

            ///CUSTOM FIELDS
            //boxy.Add(DrawCustomFields(vchara));
            /////END OF CUSTOM FIELDS

            var clickChara = new Box(); //clickable
            clickChara.style.flexDirection = FlexDirection.Row;

            var clickLbl = new Label("Character is clickable : ");
            clickLbl.style.width = 150;
            clickChara.Add(clickLbl);

            var strclick = new Toggle();
            strclick.value = false;
            strclick.style.width = 250;
            clickChara.Add(strclick);
            boxy.Add(clickChara);

            graph = DrawClickable();
            graph.SetEnabled(vchara.isClickable);
            boxy.Add(graph);

            var eventChara = new Box(); //clickable
            eventChara.style.flexDirection = FlexDirection.Row;

            var eventLbl = new Label("Execute VBlock OnClick : ");
            eventLbl.style.width = 150;
            eventChara.Add(eventLbl);

            var strclickevent = new TextField();
            strclickevent.value = "<VBlock name here>";
            strclickevent.style.width = 250;
            eventChara.Add(strclickevent);
            boxy.Add(eventChara);
            eventChara.SetEnabled(false);
            boxy.Add(DrawCustomTags(vchara));

            strclick.RegisterValueChangedCallback((x) =>
            {
                //TODO: state change events here
                if (!PortsUtils.PlayMode)
                {
                    eventChara.SetEnabled(strclick.value);
                    graph.SetEnabled(strclick.value);
                    vchara.isClickable = strclick.value;
                }
            });

            var thm = new Box(); //TUMBNAIL UPLOAD
            thm.style.flexDirection = FlexDirection.Row;

            var thmLbl = new Label("Portrait : ");
            thmLbl.style.width = 150;
            thm.Add(thmLbl);
            ///
            Func<ObjectField> makeSprite = () =>
            {
                var t = new ObjectField();
                //t.RegisterValueChangedCallback(RegisterSprite);
                t.objectType = typeof(Sprite);
                t.style.width = 220;
                return t;
            };

            Action<VisualElement, int> bindSprite = (e, i) =>
            {
                var easobj = e as ObjectField;
                easobj.objectType = typeof(Sprite);

                if (vchara.charaPortrait[i] != null && vchara.charaPortrait[i].portraitSprite != null)
                {
                    easobj.value = vchara.charaPortrait[i].portraitSprite as Sprite;
                }

                easobj.RegisterValueChangedCallback(x =>
                {
                    vchara.charaPortrait[i].portraitSprite = x.newValue as Sprite;
                    spList.SetSelection(i);

                    if (!PortsUtils.PlayMode && vchara.charaPortrait[i] != null)
                        PreviewCharaImage(vchara.charaPortrait[i].portraitSprite);
                    else
                        PreviewCharaImage(null);
                });

                easobj.RegisterCallback<MouseDownEvent>(x =>
                {
                    if (!PortsUtils.PlayMode && vchara.charaPortrait[i] != null)
                        PreviewCharaImage(vchara.charaPortrait[i].portraitSprite);
                    else
                        PreviewCharaImage(null);
                });

                vchara.editorIndex = i;
                easobj.userData = vchara as VCharacterV;
            };

            ////
            //Set default portrait shown in the editor
            if (CharacterThmImage != null)
            {
                var txttwod = new Texture2D(200, 200, TextureFormat.ARGB32, false);
                Color fillColor = Color.clear;
                Color[] fillPixels = new Color[txttwod.width * txttwod.height];

                for (int j = 0; j < fillPixels.Length; j++)
                {
                    fillPixels[j] = fillColor;
                }

                txttwod.SetPixels(fillPixels);
                txttwod.Apply();
                CharacterThmImage.image = txttwod;
            }

            var spriteList = new ListView();
            spList = spriteList;
            spriteList.itemsSource = vchara.charaPortrait;
            spriteList.name = "spriteList";
            spriteList.style.width = 220;
            spriteList.selectionType = SelectionType.Single;
            spriteList.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;
            spriteList.fixedItemHeight = 20;
            spriteList.bindItem = bindSprite;
            spriteList.makeItem = makeSprite;
            spriteList.showBorder = true;
            thm.Add(spriteList);
            boxy.Add(thm);

            /////////////ADD/REMOVE Sprite button
            var btns = new Box(); //Name
            btns.style.flexDirection = FlexDirection.Row;

            var dumLblspr = new VisualElement();
            dumLblspr.style.width = 150;
            btns.Add(dumLblspr);

            var dumLblTwospr = new VisualElement();
            dumLblTwospr.style.flexDirection = FlexDirection.Row;
            dumLblTwospr.style.width = 150;
            btns.Add(dumLblTwospr);

            var addBtn = new Button(() =>
            {
                if (!PortsUtils.PlayMode)
                {
                    vchara.charaPortrait.Add(new PortraitProps());
                    spriteList.Rebuild();
                    EditorUtility.SetDirty(vchara.root);
                }
            });

            addBtn.text = "<b>+</b>";
            addBtn.style.width = 75;
            dumLblTwospr.Add(addBtn);

            var remBtn = new Button(() =>
            {
                if (!PortsUtils.PlayMode && spriteList.selectedItem != null)
                {
                    vchara.charaPortrait.RemoveAt(spriteList.selectedIndex);
                    spriteList.Rebuild();
                    EditorUtility.SetDirty(vchara.root);
                }
                else if(!PortsUtils.PlayMode && spriteList.selectedItem == null)
                {
                    if(vchara.charaPortrait.Count > 0)
                    {
                        vchara.charaPortrait.RemoveAt(vchara.charaPortrait.Count - 1);
                        spriteList.Rebuild();
                        EditorUtility.SetDirty(vchara.root);
                    }
                }
            });

            remBtn.text = "<b>-</b>";
            remBtn.style.width = 75;
            dumLblTwospr.Add(remBtn);
            boxy.Add(btns);

            /////////////////
            var threeD = new Box(); //3D Object field
            threeD.SetEnabled(!vchara.is2D);
            threeDhee = threeD;
            threeD.style.flexDirection = FlexDirection.Row;

            var threeDLbl = new Label("3D Model : ");
            threeDLbl.style.width = 150;
            threeD.Add(threeDLbl);

            ///
            Func<ObjectField> makeGo = () =>
            {
                var t = new ObjectField();
                t.objectType = typeof(GameObject);
                t.style.width = 220;
                t.allowSceneObjects = true;
                return t;
            };

            Action<VisualElement, int> bindGo = (e, i) =>
            {
                var easobj = e as ObjectField;
                easobj.objectType = typeof(GameObject);

                if (vchara != null && vchara.charaObject3D[i] != null)
                {
                    easobj.value = vchara.charaObject3D[i] as GameObject;
                }

                easobj.RegisterValueChangedCallback(x =>
                {
                    vchara.charaObject3D[i] = easobj.value as GameObject;
                });
            };
            //////

            ListView threedList = new ListView();
            threedList.name = "3dList";
            threedList.itemsSource = vchara.charaObject3D;
            threedList.fixedItemHeight = 20;
            threedList.bindItem = bindGo;
            threedList.makeItem = makeGo;
            threedList.style.width = 220;
            threedList.selectionType = SelectionType.Single;
            threedList.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;
            threedList.showBorder = true;
            threeD.Add(threedList);
            boxy.Add(threeD);

            /////////////ADD/REMOVE GameObject button
            var btnGo = new Box(); //Name
            is2delement = btnGo;
            btnGo.SetEnabled(!vchara.is2D);

            if (vchara.is2D)
            {
                btnGo.SetEnabled(false);
            }

            btnGo.style.flexDirection = FlexDirection.Row;
            var dumLblgo = new VisualElement();
            dumLblgo.style.width = 150;
            btnGo.Add(dumLblgo);

            var dumLblTwogo = new VisualElement();
            dumLblTwogo.style.flexDirection = FlexDirection.Row;
            dumLblTwogo.style.width = 150;
            btnGo.Add(dumLblTwogo);

            var addBtnGo = new Button(() =>
            {
                if (!PortsUtils.PlayMode)
                {
                    vchara.charaObject3D.Add(null);
                    threedList.Rebuild();
                    EditorUtility.SetDirty(vchara.root);
                }
            });

            addBtnGo.text = "<b>+</b>";
            addBtnGo.style.width = 75;
            dumLblTwogo.Add(addBtnGo);

            var remBtnGo = new Button(() =>
            {
                if (!PortsUtils.PlayMode && threedList.selectedItem != null)
                {
                    vchara.charaObject3D.RemoveAt(threedList.selectedIndex);
                    threedList.Rebuild();
                    EditorUtility.SetDirty(vchara.root);
                }
                else if(!PortsUtils.PlayMode && spriteList.selectedItem == null)
                {
                    if(vchara.charaObject3D.Count > 0)
                    {
                        vchara.charaObject3D.RemoveAt(vchara.charaObject3D.Count - 1);
                        threedList.Rebuild();
                        EditorUtility.SetDirty(vchara.root);
                    }
                }
            });

            remBtnGo.text = "<b>-</b>";
            remBtnGo.style.width = 75;
            dumLblTwogo.Add(remBtnGo);
            boxy.Add(btnGo);
            /////////////////

            var lisChara = new Box(); //clickable
            lisChara.style.flexDirection = FlexDirection.Row;

            var lisLbl = new Label("Character's voices : ");
            lisLbl.style.width = 150;
            lisChara.Add(lisLbl);

            ///
            Func<ObjectField> makeAu = () =>
            {
                var t = new ObjectField();
                t.objectType = typeof(AudioClip);
                t.style.width = 220;
                return t;
            };

            Action<VisualElement, int> bindAu = (e, i) =>
            {
                var easobj = e as ObjectField;
                easobj.objectType = typeof(AudioClip);

                if (vchara != null && vchara.charaSound != null && vchara.charaSound[i] != null)
                {
                    easobj.value = (AudioClip)vchara.charaSound[i];
                }

                easobj.RegisterValueChangedCallback((x) =>
                {
                    vchara.charaSound[i] = easobj.value as AudioClip;
                });
            };
            ////

            var auList = new ListView();
            auList.name = "audioList";
            auList.itemsSource = vchara.charaSound;
            auList.fixedItemHeight = 20;
            auList.bindItem = bindAu;
            auList.makeItem = makeAu;
            auList.style.width = 220;
            auList.selectionType = SelectionType.Single;
            auList.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;
            auList.showBorder = true;
            lisChara.Add(auList);
            boxy.Add(lisChara);

            /////////////ADD/REMOVE GameObject button
            var btnAu = new Box(); //Name
            btnAu.style.flexDirection = FlexDirection.Row;

            var dumLblau = new VisualElement();
            dumLblau.style.width = 150;
            btnAu.Add(dumLblau);

            var dumLblTwoau = new VisualElement();
            dumLblTwoau.style.flexDirection = FlexDirection.Row;
            dumLblTwoau.style.width = 150;
            btnAu.Add(dumLblTwoau);

            var addBtnAu = new Button(() =>
            {
                if (!PortsUtils.PlayMode)
                {
                    vchara.charaSound.Add(null);
                    auList.Rebuild();
                    EditorUtility.SetDirty(vchara.root);
                }
            });

            addBtnAu.text = "<b>+</b>";
            addBtnAu.style.width = 75;
            dumLblTwoau.Add(addBtnAu);

            var remBtnAu = new Button(() =>
            {
                if (!PortsUtils.PlayMode && auList.selectedItem != null)
                {
                    vchara.charaSound.RemoveAt(auList.selectedIndex);
                    auList.Rebuild();
                    EditorUtility.SetDirty(vchara.root);
                }
                else if(!PortsUtils.PlayMode && auList.selectedItem == null)
                {
                    if(vchara.charaSound.Count > 0)
                    {
                        vchara.charaSound.RemoveAt(vchara.charaSound.Count - 1);
                        auList.Rebuild();
                        EditorUtility.SetDirty(vchara.root);
                    }
                }
            });

            remBtnAu.text = "<b>-</b>";
            remBtnAu.style.width = 75;
            dumLblTwoau.Add(remBtnAu);

            if (vchara.is2D)
            {
                threeDhee.SetEnabled(false);
                is2delement.SetEnabled(false);
            }
            else
            {
                threeDhee.SetEnabled(true);
                is2delement.SetEnabled(true);
            }

            boxy.Add(btnAu);
            /////////////////
            return boxy;
        }

        public void CreateCharacter(ToolbarMenu vis)
        {
            GameObject charaObj = null;
            int counta = 0;
            bool found = false;
            ReLoop();

            void ReLoop()
            {
                string tmpStrName = "VCharacter";
                var getAll = PortsUtils.GetCharacters();

                if (counta == 0)
                {
                    if (getAll != null && getAll.Length != 0)
                        found = Array.Exists(getAll, x => x.name == tmpStrName);
                }
                else
                {
                    if (getAll != null && getAll.Length != 0)
                        found = Array.Exists(getAll, x => x.name == tmpStrName + counta);

                    tmpStrName += counta;
                }

                if (found)
                {
                    counta++;
                    ReLoop();
                }
                else
                {
                    //TODO block duplicate names
                    charaObj = new GameObject();
                    charaObj.name = tmpStrName;
                    charaObj.AddComponent<VCharacterUtil>();
                    var vcharComponent = charaObj.GetComponent<VCharacterUtil>();

                    var vcom = charaObj.GetComponent<VCharacterUtil>();
                    var chars = new VCharacterV();

                    chars.charaId = Guid.NewGuid().ToString() + (int)UnityEngine.Random.Range(0, int.MaxValue);
                    chars.name = charaObj.name;
                    vis.text = chars.name;
                    chars.isVisible = true;
                    chars.fullName = string.Empty;

                    charaObj.AddComponent<RectTransform>();
                    var rects = charaObj.GetComponent<RectTransform>();
                    chars.rootRect = rects;
                    chars.root = charaObj;
                    vcharComponent.characterId = chars.charaId;
                    vcom.character = chars;
                    AddCharacter(chars);
                }
            }
        }
    }
}