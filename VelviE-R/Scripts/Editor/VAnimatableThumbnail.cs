using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VIEditor
{
    public class VAnimatableThumbnail : EditorWindow
    {
        [MenuItem("VelviE-R/VAnimatable Thumbnail")]
        public static void OpenVGraphsWindow()
        {
            var window = GetWindow<VAnimatableThumbnail>(typeof(HostGuiWindow));
            window.titleContent = new GUIContent("VAnimatable Thumbnail");
            window.titleContent.tooltip = "VAnimatable Thumbnail Editor";
        }
        void OnLostFocus()
        {
            if (isPlaying)
            {
                isPlaying = false;

                if (cts != null)
                {
                    cts.Cancel();
                    cts.Dispose();
                    cts = new CancellationTokenSource();
                }
            }
        }
        private VisualElement root;
        private void OnEnable()
        {
            root = rootVisualElement;
            root.Add(SetToolbar());
        }
        private void OnDisable()
        {
            ResetPlayState();
        }
        private VisualElement dummyElementTwo;
        private VisualElement dummyElementThree;
        public VisualElement SetToolbar()
        {
            var rootel = new VisualElement();
            rootel.style.marginLeft = 30;
            rootel.style.marginTop = 20;
            rootel.style.width = 2000;
            rootel.style.flexDirection = FlexDirection.Row;

            var box = new VisualElement();
            box.style.width = 200;
            box.style.flexDirection = FlexDirection.Column;

            var tbMenu = new ToolbarMenu();
            tbMenu.style.backgroundColor = Color.yellow;
            tbMenu.style.flexDirection = FlexDirection.Column;
            dummyElementThree = new VisualElement();
            dummyElementThree.style.alignItems = Align.Center;

            tbMenu.style.width = 200;
            tbMenu.style.height = 40;
            tbMenu.text = "<SelectCharacter>";

            box.Add(tbMenu);
            box.Add(dummyElementThree);
            rootel.Add(box);
            dummyElement = new VisualElement();
            dummyElementTwo = new VisualElement();
            rootel.Add(dummyElement);
            rootel.Add(dummyElementTwo);

            tbMenu.menu.AppendAction("<SelectCharacter>", (x) =>
            {
                tbMenu.text = "<None>";
                DrawList(null, true);
            });

            var chars = VEditorFunc.EditorGetVCharacterUtils();

            foreach (var vchar in chars)
            {
                if(vchar == null)
                    continue;

                tbMenu.menu.AppendAction(vchar.character.name, (x) =>
                {
                    tbMenu.text = vchar.character.name;
                    DrawList(vchar, false);
                });
            }
            
            return rootel;
        }
        private VisualElement activeList;
        private VisualElement dummyElement;
        private List<Sprite> activeSpritesList;
        private void DrawList(VCharacterUtil t, bool state = false)
        {
            if (activeList != null)
            {
                activeList.RemoveFromHierarchy();
                activeList = null;
            }

            if (state)
            {
                return;
            }

            var vis = new VisualElement();
            vis.style.flexDirection = FlexDirection.Column;
            vis.style.width = 220;

            //add header
            var header = new Label();
            header.style.backgroundColor = Color.yellow;
            header.text = "Add Sprite";
            header.style.unityTextAlign = TextAnchor.MiddleCenter;
            header.style.height = 40;
            header.style.width = 220;
            vis.Add(header);

            activeList = vis;

            labels = new List<Label>();

            Func<Label> makeItem = () =>
            {
                var lbl = new Label();
                lbl.style.height = 30;
                SetLabelBorders(lbl);
                labels.Add(lbl);
                return lbl;
            };

            Action<VisualElement, int> bindItem = (e, i) =>
            {
                if(!PortsUtils.PlayMode)
                {
                    var aslbl = e as Label;
                    ResetPlayState();

                    if(t.animatableThumbnail[i] == null)
                    {
                        t.animatableThumbnail[i] = new AnimThumbnailProps();
                        t.animatableThumbnail[i].name = CheckNames("NoName", t.animatableThumbnail);
                        t.animatableThumbnail[i].description = "<Empty>";
                        EditorUtility.SetDirty(t.gameObject);
                    }
                    else
                    {
                        aslbl.text = t.animatableThumbnail[i].name;
                    }                                                       
                    
                    aslbl.userData = (int)i;
                    aslbl.text = t.animatableThumbnail[i].name;

                    aslbl.RegisterCallback<MouseDownEvent>((x) =>
                    {
                        ResetPlayState();
                        DrawAnimProps(t, t.animatableThumbnail[i].sprites, t.animatableThumbnail[i], aslbl);
                        activeSpritesList = t.animatableThumbnail[i].sprites;
                        RevalidateLabels(aslbl);

                        Sprite spr = null;
                        spr = t.animatableThumbnail[i].sprites.Find(x => x != null);

                        if (spr != null)
                        {
                            DrawImagePreview(spr);
                        }
                        else
                        {
                            DrawImagePreview(null);
                        }
                    });
                }
            };

            const int itemHeight = 30;
            var listView = new ListView(t.animatableThumbnail, itemHeight, makeItem, bindItem);
            listView.style.marginTop = 20;
            listView.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;
            listView.showAddRemoveFooter = true;
            listView.reorderable = true;
            listView.selectionType = SelectionType.Single;
            listView.reorderMode = ListViewReorderMode.Animated;
            listView.itemsSourceChanged += ()=>
            {
                EditorUtility.SetDirty(t.gameObject);
            };
            vis.Add(listView);
            dummyElement.Add(vis);
        }

        private string CheckNames(string strName, List<AnimThumbnailProps> list)
        {
            int counta = 0;

            Check();

            void Check()
            {
                if(list.Count != 0)
                {
                    foreach(var l in list)
                    {
                        if(l.name == strName)
                        {
                            if(counta == 0)
                            {
                                counta++;
                                strName += counta;
                                Check();
                                return;
                            }
                            else
                            {
                                char[] charsToTrim = {'1', '2', '3', '4', '5', '6', '7', '8', '9'};
                                var t = strName.TrimEnd(charsToTrim);
                                counta++;
                                strName = t += counta;
                                Check();
                                return;
                            }
                        }
                    }
                }
            }

            return strName;
        }
        private void SetLabelBorders(Label lbl)
        {
            lbl.style.borderBottomWidth = 2;
            lbl.style.borderTopWidth = 2;
            lbl.style.borderLeftWidth = 2;
            lbl.style.borderRightWidth = 2;
            lbl.style.borderBottomColor = Color.blue;
            lbl.style.borderTopColor = Color.blue;
            lbl.style.borderLeftColor = Color.blue;
            lbl.style.borderRightColor = Color.blue;
            lbl.style.backgroundColor = Color.green;
            lbl.style.unityTextAlign = TextAnchor.MiddleCenter;
        }
        private void ResetPlayState()
        {
            if (isPlaying)
            {
                isPlaying = false;

                if (cts != null)
                {
                    cts.Cancel();
                    cts.Dispose();
                    cts = new CancellationTokenSource();
                }
            }
        }
        private void RevalidateLabels(Label label)
        {
            foreach (var lbl in labels)
            {
                if (lbl != label)
                {
                    lbl.style.backgroundColor = Color.green;
                }
                else
                {
                    lbl.style.backgroundColor = Color.blue;
                }
            }
        }
        private Button playBtn;
        private void DrawImagePreview(Sprite spr)
        {
            if (dummyElementThree.childCount > 0)
            {
                foreach (var child in dummyElementThree.Children().ToList())
                {
                    child.RemoveFromHierarchy();
                }
            }

            var vis = new VisualElement();
            vis.style.flexDirection = FlexDirection.Column;
            vis.style.marginTop = 20;

            var img = new Image();
            img.scaleMode = ScaleMode.ScaleToFit;
            img.style.width = 160;
            img.style.height = 190;
            img.style.backgroundColor = Color.white;
            img.style.backgroundImage = null;
            img.sprite = spr;

            var box = new VisualElement();
            box.style.alignContent = Align.Center;
            box.style.flexDirection = FlexDirection.Row;

            var btnPlay = new Button();
            playBtn = btnPlay;
            btnPlay.style.backgroundColor = Color.green;

            btnPlay.clicked += () =>
            {
                if (activeSpritesList.Exists(x => x != null))
                {
                    PlayPreview(true, img);
                }
            };

            btnPlay.text = "Play";
            btnPlay.style.width = 60;
            btnPlay.style.height = 30;

            var btnStop = new Button(() =>
            {
                PlayPreview(false, img);
            });

            btnStop.text = "Stop";
            btnStop.style.width = 60;
            btnStop.style.height = 30;

            box.Add(btnPlay);
            box.Add(btnStop);
            vis.Add(img);
            vis.Add(box);

            img.MarkDirtyRepaint();
            dummyElementThree.Add(vis);
        }
        private AnimThumbnailProps prop;
        private List<Label> labels;
        private void DrawAnimProps(VCharacterUtil t, List<Sprite> sprites, AnimThumbnailProps props, Label lbl)
        {
            var vis = new VisualElement();
            vis.style.flexDirection = FlexDirection.Column;
            vis.style.width = 310;
            prop = props;

            var header = new Label();
            header.text = "Properties";
            header.style.unityTextAlign = TextAnchor.MiddleCenter;
            header.style.backgroundColor = Color.green;
            header.style.height = 40;
            header.style.width = 310;
            vis.Add(header);

            ///Name
            var box = new Box();
            box.style.marginTop = 20;
            box.style.marginLeft = 5;
            box.style.width = 300;
            box.style.flexDirection = FlexDirection.Row;

            var lblName = new Label();
            lblName.text = "Animation name : ";
            lblName.style.width = 100;

            var objField = new TextField();
            objField.style.width = 200;
            objField.value = props.name;

            objField.RegisterCallback<FocusOutEvent>((x) =>
            {
                props.name = CheckNames(objField.value, t.animatableThumbnail);
                lbl.text = props.name;
                EditorUtility.SetDirty(t.gameObject);
            });

            box.Add(lblName);
            box.Add(objField);
            ///////////////////////

            ///Description
            var boxDesc = new Box();
            boxDesc.style.marginLeft = 5;
            boxDesc.style.flexDirection = FlexDirection.Row;

            var lblDesc = new Label();
            lblDesc.text = "Description : ";
            lblDesc.style.width = 100;

            var objFieldDesc = new TextField();
            objFieldDesc.style.width = 200;
            objFieldDesc.value = props.description;
            boxDesc.Add(lblDesc);
            boxDesc.Add(objFieldDesc);

            var boxOpt = new Box();
            boxOpt.style.marginLeft = 5;
            boxOpt.style.flexDirection = FlexDirection.Row;

            var lblopt = new Label();
            lblopt.style.width = 100;
            lblopt.text = "Loop type : ";

            var tbOpt = new ToolbarMenu();
            tbOpt.style.width = 200;

            if (props.loopClamp)
            {
                tbOpt.text = "Clamp";
            }
            else
            {
                tbOpt.text = "PingPong";
            }

            tbOpt.menu.AppendAction("Clamp", (x) =>
            {
                tbOpt.text = "Clamp";
                props.loopClamp = true;
                EditorUtility.SetDirty(t.gameObject);
            });
            tbOpt.menu.AppendAction("PingPong", (x) =>
            {
                tbOpt.text = "PingPong";
                props.loopClamp = false;
                EditorUtility.SetDirty(t.gameObject);
            });

            boxOpt.Add(lblopt);
            boxOpt.Add(tbOpt);

            ///Delay
            var boxDelay = new Box();
            boxDelay.style.marginLeft = 5;
            boxDelay.style.width = 300;
            boxDelay.style.flexDirection = FlexDirection.Row;

            var lblNameDelay = new Label();
            lblNameDelay.text = "Frame delay : ";
            lblNameDelay.style.width = 100;

            var objFieldDelay = new FloatField();
            objFieldDelay.style.width = 200;
            objFieldDelay.value = props.delay;
            objFieldDelay.RegisterCallback<FocusOutEvent>((x) =>
            {
                props.delay = objFieldDelay.value;
                lbl.text = props.name;
                EditorUtility.SetDirty(t.gameObject);
            });

            boxDelay.Add(lblNameDelay);
            boxDelay.Add(objFieldDelay);
            ///////////////////////

            objFieldDesc.RegisterValueChangedCallback((x) =>
            {
                props.description = objFieldDesc.value;
                EditorUtility.SetDirty(t.gameObject);
            });
            ///////////////////////

            ///Draw ListView
            Func<ObjectField> makeItem = () =>
            {
                var lbl = new ObjectField();
                lbl.style.width = 300;
                lbl.objectType = typeof(Sprite);
                lbl.style.backgroundColor = Color.green;
                return lbl;
            };

            Action<VisualElement, int> bindItem = (e, i) =>
            {
                var aslbl = e as ObjectField;
                aslbl.value = sprites[i];

                aslbl.RegisterValueChangedCallback((x) =>
                {
                    if ((Sprite)aslbl.value != null)
                    {
                        sprites[i] = aslbl.value as Sprite;
                    }
                    else
                    {
                        sprites[i] = null;
                    }

                    if(t.gameObject != null)
                    EditorUtility.SetDirty(t.gameObject);
                });

                aslbl.RegisterCallback<MouseDownEvent>((x) =>
                {
                    if (aslbl.value != null)
                    {
                        DrawImagePreview(sprites[i]);
                    }
                    else
                    {
                        DrawImagePreview(null);
                    }
                });
            };

            const int itemHeight = 20;
            var listView = new ListView(sprites, itemHeight, makeItem, bindItem);
            listView.style.width = 310;
            listView.showAddRemoveFooter = true;
            listView.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;

            /////////

            vis.Add(box);
            vis.Add(boxDesc);
            vis.Add(boxOpt);
            vis.Add(boxDelay);
            vis.Add(listView);
            listView.Rebuild();

            if (dummyElementTwo != null && dummyElementTwo.childCount > 0)
            {
                foreach (var child in dummyElementTwo.Children().ToList())
                {
                    child.RemoveFromHierarchy();
                }
            }

            dummyElementTwo.Add(vis);
            //vis.Add(ChainProp(t, props));
        }

        private CancellationTokenSource cts = new CancellationTokenSource();
        private bool isPlaying = false;
        private async void PlayPreview(bool state, Image waitAnchor)
        {
            if (state && !isPlaying)
            {
                var vts = cts.Token;
                var yts = cts.Token;
                var pts = cts.Token;

                isPlaying = true;

                if (activeSpritesList.Count > 0)
                {
                    try
                    {
                        bool oneCycle = false;

                        while (isPlaying)
                        {
                            if (vts.IsCancellationRequested)
                            {
                                break;
                            }

                            if (!oneCycle)
                            {
                                if (!prop.loopClamp)
                                {
                                    oneCycle = true;
                                }

                                for (int i = 0; i < activeSpritesList.Count; i++)
                                {
                                    if (activeSpritesList[i] != null)
                                    {
                                        waitAnchor.sprite = activeSpritesList[i];
                                        waitAnchor.MarkDirtyRepaint();
                                        await Task.Delay(TimeSpan.FromSeconds(prop.delay), yts);
                                    }
                                }
                            }
                            else
                            {
                                oneCycle = false;

                                for (int i = activeSpritesList.Count; i-- > 0;)
                                {
                                    if (activeSpritesList[i] != null && i != activeSpritesList.Count - 1)
                                    {
                                        waitAnchor.sprite = activeSpritesList[i];
                                        waitAnchor.MarkDirtyRepaint();
                                        await Task.Delay(TimeSpan.FromSeconds(prop.delay), pts);
                                    }
                                }
                            }

                            await Task.Delay(1, vts);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        ResetPlayState();
                        //Do nothing!
                    }
                }
            }
            else
            {
                ResetPlayState();
            }
        }
    }
}