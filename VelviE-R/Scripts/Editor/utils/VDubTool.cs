using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using VTasks;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Linq;

namespace VIEditor
{
    public class VDubTool : AssetPostprocessor
    {
        public static Label WriterHeader { get; set; }
        public static VisualElement PropertySplitPaneBottom { get; set; }
        private static VisualElement ProgressBar { get; set; }
        public static TextField EditorTextArea { get; set; }
        public static bool IsPlaying { get; set; }
        public static SayWord ActiveSayWord { get; set; }
        private static Label LableEstimation { get; set; }
        public static ListView DubItemList { get; set; }
        public static VisualElement SearchResultContainer { get; set; }
        public static VisualElement DrawWriterTitle()
        {
            var lbl = new Label { text = "MockupScreen" };
            lbl.name = "writerTitle";
            lbl.style.backgroundColor = Color.black;
            lbl.style.unityTextAlign = TextAnchor.MiddleCenter;
            lbl.style.height = new StyleLength(new Length(10, LengthUnit.Percent));
            lbl.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var dropD = new DropdownField();
            VEditorFunc.SetUIDynamicSize(dropD, 94, false);
            VEditorFunc.SetUIDynamicSize(dropD, 20, true);
            dropD.value = "Normal";
            dropD.choices = new List<string> { "Slow", "Normal" };

            if (EditorPrefs.HasKey("v-writer-slowMode"))
            {
                if (EditorPrefs.GetBool("v-writer-slowMode"))
                    dropD.value = "Slow";
            }

            dropD.RegisterValueChangedCallback(x =>
            {
                if (x.newValue == "Normal")
                {
                    EditorPrefs.SetBool("v-writer-slowMode", false);
                }
                else
                {
                    EditorPrefs.SetBool("v-writer-slowMode", true);
                }
            });

            dropD.style.alignSelf = Align.FlexEnd;
            lbl.Add(dropD);
            return lbl;
        }
        public static VisualElement DrawWriterToolbar()
        {
            var vis = new VisualElement();
            vis.style.alignItems = Align.Center;
            vis.style.backgroundColor = Color.green;
            VEditorFunc.SetUIDynamicSize(vis, 100, true);
            VEditorFunc.SetUIDynamicSize(vis, 20, false);
            vis.style.flexDirection = FlexDirection.Row;

            var btnPlay = new Button();
            btnPlay.style.color = Color.black;
            btnPlay.style.backgroundColor = Color.green;
            VEditorFunc.SetUIDynamicSize(btnPlay, 20, true);
            VEditorFunc.SetUIDynamicSize(btnPlay, 80, false);
            btnPlay.text = "Play";

            var btnRec = new Button();
            btnRec.style.color = Color.black;
            btnRec.style.backgroundColor = Color.green;
            VEditorFunc.SetUIDynamicSize(btnRec, 20, true);
            VEditorFunc.SetUIDynamicSize(btnRec, 80, false);
            btnRec.text = "REC";
            RecBtn = btnRec;

            var lblEst = new Label();
            LableEstimation = lblEst;
            lblEst.text = "TotalTime : ";
            lblEst.SetEnabled(false);
            lblEst.style.color = Color.black;
            lblEst.style.backgroundColor = Color.green;
            VEditorFunc.SetUIDynamicSize(lblEst, 30, true);
            VEditorFunc.SetUIDynamicSize(lblEst, 80, false);

            if (!PortsUtils.PlayMode)
            {
                btnPlay.clicked += () =>
                {
                    IsPlaying = !IsPlaying;

                    if (IsPlaying)
                    {
                        if (writer_cts != null)
                        {
                            if (writer_cts != null && !writer_cts.wasDisposed)
                            {
                                writer_cts.Cancel();
                            }
                        }

                        MockupIsWriting = false;
                        btnPlay.text = "STOP!";
                        btnPlay.style.backgroundColor = Color.blue;
                        btnPlay.style.color = Color.white;
                        StartTyping(EditorTextArea, EditorTextArea.value, ActiveSayWord.VDialogue.TextSpeed, ActiveSayWord.VDialogue.PauseBetweenWords, btnPlay);
                    }
                    else
                    {
                        btnPlay.style.backgroundColor = Color.green;
                        btnPlay.style.color = Color.black;
                        btnPlay.text = "Play";
                        VEditorFunc.SetUIDynamicSize(ProgressBar, 100, true);

                        if (writer_cts != null && !writer_cts.wasDisposed)
                        {
                            writer_cts.Cancel();
                        }

                        StopAllClips();
                    }
                };

                btnRec.clicked += () =>
                {
                    if (IsPlaying)
                        return;

                    if (!IsRecording)
                    {
                        btnRec.style.backgroundColor = Color.red;
                        Record(FileNameTxtField.value);
                    }
                    else
                    {
                        btnRec.style.backgroundColor = Color.green;
                        StopRecording();
                    }
                };
            }

            vis.Add(btnPlay);
            vis.Add(btnRec);
            vis.Add(lblEst);
            return vis;
        }
        public static VisualElement DrawWriterPropertyTab()
        {
            var lbl = new Label { text = "PropertyTab" };
            lbl.style.color = Color.black;
            lbl.name = "writerPropertyTab";
            lbl.style.backgroundColor = Color.cyan;
            lbl.style.unityTextAlign = TextAnchor.MiddleCenter;
            lbl.style.height = new StyleLength(new Length(10, LengthUnit.Percent));
            lbl.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            WriterHeader = lbl;
            return lbl;
        }
        private static List<(SayWord say, string id)> sayWords = new List<(SayWord, string)>();
        private static List<VelvieBlockComponent> allSayWords = new List<VelvieBlockComponent>();
        public static VisualElement MockupElement()
        {
            if (PortsUtils.activeVGraphAssets != null)
            {
                var getVcores = VEditorFunc.EditorGetVCoreUtils();
                var activecore = Array.Find(getVcores, x => x.vcoreid == PortsUtils.activeVGraphAssets.govcoreid);

                if (activecore == null)
                    return null;

                var lis = activecore.vBlockCores;
                var allNodes = new List<VelvieBlockComponent>();
                allSayWords = new List<VelvieBlockComponent>();

                for (int i = 0; i < lis.Count; i++)
                {
                    if (lis[i] == null)
                        continue;

                    allNodes.AddRange(lis[i].vblocks);
                }

                if (allNodes == null || allNodes.Count == 0)
                    return null;

                for (int i = 0; i < allNodes.Count; i++)
                {
                    if (allNodes[i] == null || allNodes[i].attachedComponent == null)
                        continue;

                    if (allNodes[i].attachedComponent.component is SayWord say)
                    {
                        allSayWords.Add(allNodes[i]);
                        sayWords.Add((say, allNodes[i].guid));
                    }
                }

                if (allSayWords == null || allSayWords.Count == 0)
                    return null;

                const int defHeight = 30;
                const int defWidth = 230;
                const int itemHeight = 35;

                Func<VBlockLabel> makeItem = () =>
                {
                    var vb = new VBlockLabel("DummyTitle", VColor.Yellow02, defHeight, defWidth, 0, true);
                    return vb;
                };

                Action<VisualElement, int> bindItem = (e, i) =>
                {
                    var vb = e as VBlockLabel;
                    vb.IsWriterMmode = true;

                    vb.SetRegisters();
                    vb.style.marginLeft = allSayWords[i].attachedComponent.leftMargin;
                    vb.style.opacity = allSayWords[i].attachedComponent.oppacity;

                    var asvctype = allSayWords[i].attachedComponent.component as VBlockCore;
                    vb.VBlockId = allSayWords[i].guid;

                    if (asvctype != null && !String.IsNullOrEmpty(asvctype.OnVSummary()))
                        vb.VBlockContent.text = allSayWords[i].name + "\n" + "<i><size=8>ERROR!</size></i>";
                    else
                        vb.VBlockContent.text = allSayWords[i].name;

                    VColorAttr.GetColor(vb.VBlockContent, allSayWords[i].vcolor);
                    var ints = i + 1;
                    vb.VBlockLineNumber.text = ints.ToString();

                    var getSay = sayWords.Find(x => x.id == allSayWords[i].guid);
                    vb.WriterDelegate = null;
                    vb.MockupIndex = i;

                    if (getSay.say != null)
                    {
                        var act = new Action(() =>
                        {
                            EditorTextArea.value = getSay.say.Words;
                            ActiveSayWord = getSay.say;
                            var valss = i + 1;
                            WriterHeader.text = "PropertyTab : " + valss;
                            PropertySplitPaneBottom.Clear();
                            PropertySplitPaneBottom.Add(DrawDialogProperties());
                        });

                        vb.WriterDelegate = act;
                    }

                    vb.vtoggle.RemoveFromHierarchy();
                };

                var listV = new ListView(allSayWords, itemHeight, makeItem, bindItem);
                DubItemList = listV;
                listV.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;
                listV.showBorder = true;
                listV.selectionType = SelectionType.Multiple;
                listV.reorderMode = ListViewReorderMode.Simple;
                listV.reorderable = false;
                listV.horizontalScrollingEnabled = false;
                listV.style.alignContent = Align.Center;
                listV.style.flexGrow = new StyleFloat(1);

                VEditorFunc.SetUIDynamicSize(listV, 100, true);
                VEditorFunc.SetUIDynamicSize(listV, 100, false);

                listV.focusable = true;
                listV.pickingMode = PickingMode.Position;
                listV.style.marginLeft = 5;
                return listV;
            }

            return null;
        }
        public static VisualElement SplitContainerRightPane()
        {
            var vis = new VisualElement();
            vis.style.flexDirection = FlexDirection.Column;
            VEditorFunc.SetUIDynamicSize(vis, 100, true);
            VEditorFunc.SetUIDynamicSize(vis, 100, false);

            var visOne = new VisualElement();
            visOne.style.flexDirection = FlexDirection.Column;
            visOne.style.backgroundColor = Color.black;
            visOne.name = "top";

            var visTwo = new VisualElement();
            visTwo.name = "bottom";

            PropertySplitPaneBottom = visTwo;
            VEditorFunc.SetUIDynamicSize(visOne, 50, false);
            VEditorFunc.SetUIDynamicSize(visTwo, 50, false);
            VEditorFunc.SetUIDynamicSize(visOne, 100, true);
            VEditorFunc.SetUIDynamicSize(visTwo, 100, true);

            vis.Add(visOne);
            vis.Add(visTwo);

            visOne.Add(DrawWriterPropertyTab());
            visOne.Add(DrawTextArea());
            return vis;
        }
        private static VisualElement DrawTextArea()
        {
            var vis = new VisualElement();
            vis.style.flexDirection = FlexDirection.Column;
            VEditorFunc.SetUIDynamicSize(vis, 100, true);
            VEditorFunc.SetUIDynamicSize(vis, 90, false);

            var lbl = new Label();
            lbl.style.marginLeft = 15;
            lbl.text = "WORDS";
            VEditorFunc.SetUIDynamicSize(lbl, 100, true);
            VEditorFunc.SetUIDynamicSize(lbl, 10, false);

            var objField = new TextField();
            objField.style.marginBottom = 15;
            objField.style.marginLeft = 15;
            objField.style.alignSelf = Align.FlexStart;
            EditorTextArea = objField;
            VEditorFunc.SetUIDynamicSize(objField, 90, true);
            VEditorFunc.SetUIDynamicSize(objField, 55, false);

            objField.style.flexDirection = FlexDirection.Column;
            objField.style.overflow = Overflow.Visible;
            objField.style.whiteSpace = WhiteSpace.Normal;
            objField.style.unityOverflowClipBox = OverflowClipBox.ContentBox;
            objField.multiline = true;

            vis.Add(lbl);
            vis.Add(objField);
            vis.Add(PlayProgressBar());
            vis.Add(DrawWriterToolbar());

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterCallback<FocusOutEvent>(x =>
                {
                    if (ActiveSayWord != null)
                    {
                        ActiveSayWord.Words = objField.value;
                    }
                });
            }
            return vis;
        }
        public static VisualElement DrawSearchField()
        {
            var vis = new VisualElement();
            VEditorFunc.SetUIDynamicSize(vis, 98, isWidth: true);

            var toolbarSearchField = new ToolbarPopupSearchField();
            VEditorFunc.SetUIDynamicSize(toolbarSearchField, 100, isWidth: true);
            VEditorFunc.SetUIDynamicSize(toolbarSearchField, 20, isWidth: false);
            
            var sresult = new VisualElement();
            VEditorFunc.SetUIDynamicSize(sresult, 100, isWidth: true);
            VEditorFunc.SetUIDynamicSize(sresult, 100, isWidth: false);

            toolbarSearchField.RegisterCallback<KeyDownEvent>(x => OnSearchTextChanged(x, toolbarSearchField.value, sresult));

            vis.Add(toolbarSearchField);
            vis.Add(sresult);
            SearchResultContainer = sresult;
            return vis;
        }
        public static VisualElement PlayProgressBar()
        {
            var vis = new VisualElement();
            ProgressBar = vis;
            vis.style.backgroundColor = Color.blue;
            VEditorFunc.SetUIDynamicSize(vis, 100, true);
            VEditorFunc.SetUIDynamicSize(vis, 5, false);
            return vis;
        }

        public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
        {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod("PlayPreviewClip", BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
                null
            );

            method.Invoke(
                null,
                new object[] { clip, startSample, loop }
            );
        }

        public static void StopAllClips()
        {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod("StopAllPreviewClips", BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[] { },
                null
            );

            method.Invoke(
                null,
                new object[] { }
            );
        }

        public static bool MockupIsWriting { get; set; }
        public static VTokenSource writer_cts;
        public static async void StartTyping(TextField tmp, string content, float rpm, float pauseBetweenWords, Button btnState)
        {
            if (String.IsNullOrEmpty(content) || MockupIsWriting)
                return;

            StopAllClips();
            string tmpval = tmp.value;
            MockupIsWriting = true;
            writer_cts = new VTokenSource();

            int counta = 0;
            int len = content.Length;
            tmp.value = string.Empty;

            var stp = new Stopwatch();
            stp.Start();

            if (ActiveSayWord != null && ActiveSayWord.CharacterSound != null)
            {
                PlayClip(ActiveSayWord.CharacterSound);
            }
            
            while (counta != len - 1 && MockupIsWriting)
            {
                try
                {
                    char currentChar = content[counta];

                    if (writer_cts.IsCancellationRequested && writer_cts != null)
                    {
                        MockupIsWriting = false;
                        tmp.value = tmpval;
                        return;
                    }

                    if (!Char.IsWhiteSpace(currentChar) && currentChar != ',')
                    {
                        await Task.Delay(TimeSpan.FromSeconds(rpm), writer_cts.Token);
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromSeconds(pauseBetweenWords), writer_cts.Token);
                    }

                    var tmpv = tmp.value + currentChar;
                    tmp.value = tmpv;
                    counta++;
                    var tmpProg = PercentCalc(counta, len) + 1;
                    VEditorFunc.SetUIDynamicSize(ProgressBar, tmpProg, true);
                    LableEstimation.text = "TotalTime : " + stp.Elapsed.ToString("mm\\:ss\\.ff");
                }
                catch (Exception)
                {
                    MockupIsWriting = false;
                }
            }

            btnState.style.backgroundColor = Color.green;
            btnState.style.color = Color.black;
            btnState.text = "Play";
            VEditorFunc.SetUIDynamicSize(ProgressBar, 100, true);
            tmp.value = tmpval;

            if (writer_cts != null && !writer_cts.wasDisposed)
            {
                writer_cts.Cancel();
            }

            MockupIsWriting = false;
            return;
        }
        public static int PercentCalc(int inputVal, int inputMaxPercent)
        {
            return (int)Math.Round((double)(100 * inputVal) / inputMaxPercent);
        }
        private static bool voiceIsPlaying = false;
        private static ObjectField RecordSlot;
        private static Button RecBtn;
        private static TextField FileNameTxtField;
        public static VisualElement DrawDialogProperties()
        {
            if (ActiveSayWord == null)
                return null;

            var vis = new VisualElement();
            vis.style.flexDirection = FlexDirection.Column;
            VEditorFunc.SetUIDynamicSize(vis, 100, true);
            VEditorFunc.SetUIDynamicSize(vis, 100, false);

            var visVoice = new VisualElement();
            VEditorFunc.SetUIDynamicSize(visVoice, 100, true);
            visVoice.style.marginTop = 10;
            visVoice.style.marginLeft = 20;
            visVoice.style.flexDirection = FlexDirection.Row;

            var visFileName = new VisualElement();
            VEditorFunc.SetUIDynamicSize(visFileName, 100, true);
            visFileName.style.marginTop = 10;
            visFileName.style.marginLeft = 20;
            visFileName.style.flexDirection = FlexDirection.Row;

            var lblRecName = new Label();
            lblRecName.style.color = Color.black;
            lblRecName.text = "FileName:";
            VEditorFunc.SetUIDynamicSize(lblRecName, 20, true);

            var fileName = new TextField();
            VEditorFunc.SetUIDynamicSize(fileName, 60, true);
            fileName.value = "NONAME-00";
            visFileName.Add(lblRecName);
            visFileName.Add(fileName);
            FileNameTxtField = fileName;

            var visDur = new VisualElement();
            VEditorFunc.SetUIDynamicSize(visDur, 100, true);
            visDur.style.marginTop = 10;
            visDur.style.marginLeft = 20;
            visDur.style.flexDirection = FlexDirection.Row;

            var lblEstRec = new Label();
            lblEstRec.style.color = Color.black;
            lblEstRec.text = "RecTime:";
            VEditorFunc.SetUIDynamicSize(lblEstRec, 20, true);

            var lblWarn = new Label();
            lblWarn.style.color = Color.black;
            lblWarn.text = "*Record time must be set!";
            VEditorFunc.SetUIDynamicSize(lblWarn, 20, true);

            var fldDur = new IntegerField();
            VEditorFunc.SetUIDynamicSize(fldDur, 10, true);
            visDur.Add(lblEstRec);
            visDur.Add(fldDur);
            visDur.Add(lblWarn);

            fldDur.value = 5;

            if (ActiveSayWord != null)
                fldDur.value = ActiveSayWord.recDurationForEditor;

            fldDur.RegisterValueChangedCallback(x =>
            {
                if (!IsRecording)
                {
                    ActiveSayWord.recDurationForEditor = x.newValue;
                }
            });

            var visVdial = new VisualElement();
            visVdial.SetEnabled(false);
            VEditorFunc.SetUIDynamicSize(visVdial, 100, true);
            visVdial.style.marginTop = 10;
            visVdial.style.marginLeft = 20;
            visVdial.style.flexDirection = FlexDirection.Row;

            var lblVoice = new Label();
            VEditorFunc.SetUIDynamicSize(lblVoice, 20, true);
            lblVoice.text = "Voice : ";

            var obj = new ObjectField();
            VEditorFunc.SetUIDynamicSize(obj, 60, true);
            obj.objectType = typeof(AudioClip);
            visVoice.Add(lblVoice);
            visVoice.Add(obj);
            RecordSlot = obj;

            obj.value = ActiveSayWord.CharacterSound;

            obj.RegisterValueChangedCallback(x =>
            {
                ActiveSayWord.CharacterSound = x.newValue as AudioClip;
            });
 
            var lblDial = new Label();
            VEditorFunc.SetUIDynamicSize(lblDial, 20, true);
            lblDial.text = "VDialog : ";

            var objDial = new ObjectField();
            VEditorFunc.SetUIDynamicSize(objDial, 60, true);
            objDial.objectType = typeof(VelvieDialogue);
            visVdial.Add(lblDial);
            visVdial.Add(objDial);
            objDial.value = ActiveSayWord.VDialogue;

            objDial.RegisterValueChangedCallback(x =>
            {
                ActiveSayWord.VDialogue = x.newValue as VelvieDialogue;
            });

            vis.Add(DrawMicDevices());
            vis.Add(visFileName);
            vis.Add(visDur);
            vis.Add(visVoice);
            vis.Add(visVdial);
            return vis;
        }
        private static VisualElement DrawMicDevices()
        {
            var vis = new VisualElement();
            VEditorFunc.SetUIDynamicSize(vis, 100, true);
            vis.style.flexDirection = FlexDirection.Row;
            var lblMic = new Label();
            lblMic.style.marginTop = 10;
            lblMic.style.marginLeft = 20;
            lblMic.style.color = Color.black;
            lblMic.text = "MicDevices : ";
            VEditorFunc.SetUIDynamicSize(lblMic, 20, true);
            var dropMic = new DropdownField();
            dropMic.style.marginTop = 10;
            VEditorFunc.SetUIDynamicSize(dropMic, 60, true);
            dropMic.choices = Microphone.devices.ToList();

            bool keyExist = EditorPrefs.HasKey("DefaultMic");

            if(keyExist && Microphone.devices != null && Array.Exists(Microphone.devices, x => x == EditorPrefs.GetString("DefaultMic")))
            {
                ActiveMic = Array.Find(Microphone.devices, x => x == EditorPrefs.GetString("DefaultMic"));
                dropMic.value = ActiveMic;
            }

            if(Microphone.devices != null && !keyExist)
            {
                dropMic.value = Microphone.devices[0];
                EditorPrefs.SetString("DefaultMic", Microphone.devices[0]);
                ActiveMic = Microphone.devices[0];
            }

            dropMic.RegisterValueChangedCallback(x =>
            {
                ActiveMic = x.newValue;
                EditorPrefs.SetString("DefaultMic", x.newValue);
            });

            vis.Add(lblMic);
            vis.Add(dropMic);
            return vis;
        }
        private static void OnSearchTextChanged(KeyDownEvent evt, string searchVal, VisualElement container)
        {
            if(allSayWords == null || allSayWords.Count == 0 || evt.keyCode != KeyCode.Return)
                return;
            
            SearchResultContainer.Clear();

            for(int i = 0; i < allSayWords.Count; i++)
            {
                var t = allSayWords[i].attachedComponent.component as SayWord;
                
                if(!String.IsNullOrEmpty(t.Words) && t.Words.Contains(searchVal))
                {
                    var lbl = new Label();
                    lbl.style.backgroundColor = Color.grey;
                    lbl.style.marginTop = 5;
                    lbl.style.marginBottom = 5;
                    lbl.style.marginLeft = 5;
                    lbl.pickingMode = PickingMode.Position;
                    lbl.style.color = Color.black;

                    if(t.Words.Length >= 25)
                    {
                        lbl.text = t.Words[0..25] + "...";
                    }
                    else
                    {
                        lbl.text = t.Words[0..t.Words.Length] + "...";
                    }
                    
                    VEditorFunc.SetUIDynamicSize(lbl, 100, true);

                    lbl.RegisterCallback<MouseDownEvent>(x =>
                    {
                        SearchResultContainer.Clear();                        

                        int val = i;
                        DubItemList.ScrollToItem(val);
                        DubItemList.SetSelection(val);

                        EditorTextArea.value = t.Words;
                        ActiveSayWord = t;
                        WriterHeader.text = "PropertyTab : ";
                        PropertySplitPaneBottom.Clear();
                        PropertySplitPaneBottom.Add(DrawDialogProperties());
                    });

                    SearchResultContainer.Add(lbl);
                }
            }
        }
        public static void StopVDubtool()
        {
            if (writer_cts != null && !writer_cts.wasDisposed)
            {
                writer_cts.Cancel();
            }

            MockupIsWriting = false;

            if (voiceIsPlaying)
            {
                StopAllClips();
                voiceIsPlaying = false;
            }

            StopRecording();
        }
        private static bool IsRecording = false;
        private static CancellationTokenSource WaitSource;
        private static string ActiveMic;
        private static async void Record(string fileName)
        {
            if (IsRecording)
                return;
                
            WaitSource = new CancellationTokenSource();
            IsRecording = true;
            var mics = Microphone.devices;
            AudioClip auClip = Microphone.Start(ActiveMic, false, ActiveSayWord.recDurationForEditor, 44100);

            while (Microphone.IsRecording(ActiveMic))
            {
                await Task.Yield();

                if (!IsRecording)
                    return;
            }

            AudioClip tmpClip = null;
            var func = SavWav.Save(fileName, auClip);
            int retryCount = 0;

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(2), WaitSource.Token);
                AssetDatabase.Refresh();
                await Task.Delay(TimeSpan.FromSeconds(1), WaitSource.Token);

                while(true)
                {
                    if(WaitSource.IsCancellationRequested)
                        break;

                    await Task.Delay(TimeSpan.FromSeconds(1), WaitSource.Token);
                    retryCount++;

                    if(retryCount == 2)
                        AssetDatabase.Refresh();
                    
                    if(retryCount == 3)
                        break;
                }

                while (tmpClip == null)
                {
                    if (WaitSource.IsCancellationRequested)
                        break;

                    if (retryCount == 5)
                        break;

                    await Task.Delay(TimeSpan.FromSeconds(1), WaitSource.Token);
                    tmpClip = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/VelviE-R/Resources/characterRecording/" + func.getFile, typeof(AudioClip));
                    
                    if(tmpClip != null)
                        break;

                    retryCount++;
                }
            }
            catch (Exception e)
            {
                if (e is OperationCanceledException)
                {
                    RecordSlot.value = tmpClip as AudioClip;
                    WaitSource.Cancel();
                    IsRecording = false;
                    RecBtn.style.backgroundColor = Color.green;
                }
            }

            RecBtn.style.backgroundColor = Color.green;
            RecordSlot.value = tmpClip as AudioClip;
            WaitSource.Cancel();
            IsRecording = false;
        }
        public static void StopRecording()
        {
            if (IsRecording)
            {
                if (Microphone.IsRecording(ActiveMic))
                    Microphone.End(ActiveMic);

                if (WaitSource != null)
                    WaitSource.Cancel();

                if (RecBtn != null)
                {
                    RecBtn.style.backgroundColor = Color.green;
                    IsRecording = false;
                }
            }
        }
    }
}