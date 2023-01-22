using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace VelvieR
{

    public class VelvieDialogue : VDialogue
    {
        [SerializeField] private AudioClip enderAudioClip;
        [SerializeField] private TMP_Text tmpComponent;
        [SerializeField] private float textSpeed = 1f;
        [SerializeField] private DialogType textEffectType = DialogType.TypeWriter;
        [SerializeField] private string writingIndicatorText = "writing...";
        [SerializeField] private float pauseBetweenWords = 0.1f;
        [SerializeField] private AudioClip typingSound;
        [SerializeField] private ContinueAnim continueIndicator = ContinueAnim.ThreeDots;
        [SerializeField] private GameObject iconSprite;
        [SerializeField] private ShowHideEffect showEffect = ShowHideEffect.FadeInOut;
        [SerializeField] private LeanTweenType showEasing = LeanTweenType.easeInOutQuad;
        [SerializeField] private float showEffectDuration = 0.3f;
        [SerializeField] private ClickOnDialogue clickTarget = ClickOnDialogue.ClickAnywhere;
        [SerializeField] private TMP_Text continueTmpComponent;
        [SerializeField] private bool enableWritingIndicator = true;
        [SerializeField] private bool enableTypingSound = true;
        [SerializeField] private bool enableEndAudio = true;
        [SerializeField] private Image thumbnail;
        [SerializeField] private float spritesDuration = 0.2f;
        [SerializeField] private float thumbnailDuration = 0.2f;
        [SerializeField] private VTextSpeed vtextSpeed = VTextSpeed.Hundred;
        public ClickOnDialogue ClickTarget { get { return clickTarget; } set { clickTarget = value; } }
        public float SpritesDuration { get { return spritesDuration; } set { spritesDuration = value; } }
        public float ThumbnailDuration { get { return thumbnailDuration; } set { thumbnailDuration = value; } }
        public Image Thumbnail { get { return thumbnail; } set { thumbnail = value; } }
        public string WritingIndicatorText { get { return writingIndicatorText; } set { writingIndicatorText = value; } }
        public bool EnableEnderAudio { get { return enableEndAudio; } set { enableEndAudio = value; } }
        public bool EnableTypingSound { get { return enableTypingSound; } set { enableTypingSound = value; } }
        public AudioClip EnderDelayAudio { get { return enderAudioClip; } set { enderAudioClip = value; } }
        public bool EnableWritingIndicator { get { return enableWritingIndicator; } set { enableWritingIndicator = value; } }
        public WaitForClick waitingForClick { get; set; } = WaitForClick.Enable;
        public float TextSpeed { get { return textSpeed; } set { textSpeed = value; } }
        public DialogType TextEffectType { get { return textEffectType; } set { textEffectType = value; } }
        public ContinueAnim ContinueIndicator { get { return continueIndicator; } set { continueIndicator = value; } }
        public AudioClip TypingSound { get { return typingSound; } set { typingSound = value; } }
        public TMP_Text TmpComponent { get { return tmpComponent; } set { tmpComponent = value; } }
        public float PauseBetweenWords { get { return pauseBetweenWords; } set { pauseBetweenWords = value; } }
        public GameObject IconSprite { get { return iconSprite; } set { iconSprite = value; } }
        public ShowHideEffect ShowEffect { get { return showEffect; } set { showEffect = value; } }
        public float ShowEffectDuration { get { return showEffectDuration; } set { showEffectDuration = value; } }
        public VTextSpeed TxtSpeed { get{return vtextSpeed;} set{vtextSpeed = value;} }
        public bool isWaiting { get; set; }
        public bool isWriting { get; set; }
        public GameObject parentObject { get; set; }
        [SerializeField] public TMP_Text characterName;
        [SerializeField] public CanvasGroup canvyG;
        [SerializeField] public GameObject parentpanel;
        [SerializeField] public TMP_Text continueThreeDots;
        [SerializeField] public Vector3 vdialogDefaultPosition;
        [SerializeField] public string velvieDialogueName;
        [SerializeField] public string velvieDialogueId;
        [SerializeField] public VAudioClass audioSource = null;
        [SerializeField] public VAudioClass audioSourceCommon = null;
        public bool AbortVDialogueExecution { get; set; } = false;
        private bool sessionState = false;
        public RectTransform thmRect { get; set; }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (String.IsNullOrEmpty(velvieDialogueName) || velvieDialogueName != gameObject.name)
            {
                velvieDialogueName = gameObject.name;
            }

            if (String.IsNullOrEmpty(velvieDialogueId))
            {
                velvieDialogueId = Guid.NewGuid().ToString() + UnityEngine.Random.Range(0, int.MaxValue);
            }

        }
        public void ReValidate()
        {
            OnValidate();
        }
#endif

        void Awake()
        {
            var AuComs = gameObject.GetComponents<AudioSource>();

            if (AuComs.Length == 0)
            {
                gameObject.AddComponent<AudioSource>();
                gameObject.AddComponent<AudioSource>();
            }
            else if (AuComs.Length == 1)
            {
                gameObject.AddComponent<AudioSource>();
            }

            AuComs = gameObject.GetComponents<AudioSource>();

            var auClass = new VAudioClass();
            audioSource = auClass;
            auClass.name = "typeAudio";
            auClass.audioSource = AuComs[0];
            auClass.audioSource.playOnAwake = false;
            auClass.volume = auClass.audioSource.volume;
            auClass.pitchLevel = auClass.audioSource.pitch;
            auClass.stereoPanLevel = auClass.audioSource.panStereo;
            audioSource.audioSource.hideFlags = HideFlags.HideInInspector;

            var auClassCommon = new VAudioClass();
            audioSourceCommon = auClassCommon;
            auClassCommon.name = "commonAudio";
            auClassCommon.audioSource = AuComs[1];
            auClassCommon.audioSource.playOnAwake = false;
            auClassCommon.volume = auClassCommon.audioSource.volume;
            auClassCommon.pitchLevel = auClassCommon.audioSource.pitch;
            auClassCommon.stereoPanLevel = auClassCommon.audioSource.panStereo;
            audioSourceCommon.audioSource.hideFlags = HideFlags.HideInInspector;

            PoolingBeforeSceneLoaded();
            vdialogue = this;

            if (audioSource != null && audioSource.audioSource != null)
            {
                if (!VAudioManager.audios.Contains(audioSource))
                    VAudioManager.audios.Add(audioSource);
            }

            auSource = audioSource;
            auSourceCommon = audioSourceCommon;
            typeAu = typingSound;
            enderAu = enderAudioClip;
            writeIndic = EnableWritingIndicator;
            writeTxtIndicator = writingIndicatorText;

            if (clickTarget == ClickOnDialogue.ClickOnVDialogPanel)
            {
                var parentObj = parentpanel.GetComponent<Image>();
                parentObj.raycastTarget = true;
                parentpanel.AddComponent<EventTrigger>();
                var evttrig = parentpanel.GetComponent<EventTrigger>();

                EventTrigger.Entry onClick = new EventTrigger.Entry();
                onClick.eventID = EventTriggerType.PointerClick;
                onClick.callback.AddListener((eventdata) => ExecNext());
                evttrig.triggers.Add(onClick);
            }
            else
            {
                parentpanel.GetComponent<Image>().raycastTarget = false;
            }
        }
        void Start()
        {
            enableTypeAu = enableTypingSound;
            enableEnderAudio = enableEndAudio;
        }
        void OnEnable()
        {
            if (parentObject == null)
                parentObject = this.gameObject;
        }

        void OnDisable()
        {
            if (!sessionState)
            {
                sessionState = true;
                return;
            }

            CancelVDialogTokens();
        }
        void OnDestroy()
        {
            if (canvyG != null)
            {
                VExtension.CheckTweensToCancel(canvyG.gameObject);
                VExtension.CheckTweensToCancel(parentpanel);
            }

            CancelVDialogTokens();
        }
        public virtual void SpeedText(VTextSpeed txtSpeed)
        {

            TxtSpeed = txtSpeed;

            switch (txtSpeed)
            {
                case VTextSpeed.Twenty:
                    textSpeed = 0.9f;
                    break;
                case VTextSpeed.Forty:
                    textSpeed = 0.8f;
                    break;
                case VTextSpeed.Sixty:
                    textSpeed = 0.7f;
                    break;
                case VTextSpeed.Eighty:
                    textSpeed = 0.6f;
                    break;
                case VTextSpeed.Hundred:
                    textSpeed = 0.5f;
                    break;
                case VTextSpeed.HundredTwenty:
                    textSpeed = 0.4f;
                    break;
                case VTextSpeed.HundredForty:
                    textSpeed = 0.3f;
                    break;
                case VTextSpeed.HundredSixty:
                    textSpeed = 0.2f;
                    break;
                case VTextSpeed.HundredEighty:
                    textSpeed = 0.1f;
                    break;
                case VTextSpeed.TwoHundred:
                    textSpeed = 0.08f;
                    break;
                case VTextSpeed.TwoHundredTwenty:
                    textSpeed = 0.06f;
                    break;
                case VTextSpeed.TwoHundredForty:
                    textSpeed = 0.04f;
                    break;
                case VTextSpeed.TwoHundredSixty:
                    textSpeed = 0.02f;
                    break;
            }
        }

        void PoolingBeforeSceneLoaded()
        {
            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);

            tmpComponent.text = string.Empty;

            if (canvyG == null)
                canvyG = GetComponent<CanvasGroup>();

            if (parentpanel == null)
            {
                var child = canvyG.gameObject.GetComponentsInChildren<RectTransform>();

                for (int i = 0; i < child.Length; i++)
                {
                    if (child[i].gameObject.name.Equals("Panel"))
                    {
                        parentpanel = child[i].gameObject;
                        vdialogDefaultPosition = parentpanel.transform.position;
                    }
                    else if (child[i].gameObject.name.Equals("name"))
                    {
                        characterName = child[i].gameObject.GetComponent<TMP_Text>();
                    }
                    else if (child[i].gameObject.name.Equals("continue"))
                    {
                        continueThreeDots = child[i].gameObject.GetComponent<TMP_Text>();
                        continueThreeDots.maxVisibleCharacters = 0;
                        conTmp = continueThreeDots;
                    }
                    else if (child[i].gameObject.name.Equals("writing"))
                    {
                        writeIndicTmp = child[i].gameObject.GetComponent<TMP_Text>();
                        writeIndicTmp.maxVisibleCharacters = 0;
                    }
                    else if (child[i].gameObject.name.Equals("thumbnail"))
                    {
                        thumbnail = child[i].gameObject.GetComponent<Image>();
                        thumbnail.gameObject.SetActive(false);
                        thmRect = thumbnail.GetComponent<RectTransform>();
                    }
                }
            }

            if (iconSprite != null)
            {
                conIcon = iconSprite;
            }

            gameObject.SetActive(false);
        }
        public VCharacterV previousSpeakingCharacter { get; set; }
        private (Vector2 position, Vector2 size) defaultThmProps = (Vector2.zero, Vector2.zero);
        public void SetPortraitThumbnail(VCharacterV vchara, RectTransform rect, Sprite sprite, float duration = 0.3f, bool setAspectRatio = true, bool setOnComplete = false, bool isAnimatable = false, SayWord sayWord = null)
        {
            if (defaultThmProps.position == Vector2.zero || defaultThmProps.position == Vector2.zero)
            {
                defaultThmProps.position = rect.anchoredPosition;
                defaultThmProps.size = rect.sizeDelta;
            }

            if (LeanTween.isTweening(rect) && sprite != null)
            {
                var getPortrait = VCharacterManager.GetVPortrait(vchara.charaId, sprite.name);

                if (getPortrait.vchara.charaPortrait.Count != 0 && getPortrait.vchara != null && getPortrait.portraitProp != null)
                {
                    (VCharacterV vchara, PortraitProps portraitProp, int? prevIndex) chara = (null, null, null);
                    chara = VCharacterManager.GetActiveCharacter(getPortrait.vchara);

                    if (chara.vchara != null && chara.portraitProp.portraitSprite.name != sprite.name)
                    {
                        LeanTween.cancel(rect.gameObject, true);
                    }
                }
            }

            rect.anchoredPosition = defaultThmProps.position;
            rect.sizeDelta = defaultThmProps.size;

            if (thumbnail != null)
            {
                thumbnail.gameObject.SetActive(true);

                if (!isAnimatable)
                {
                    thumbnail.sprite = sprite;
                }

                if (thumbnail.sprite != null)
                {
                    VExtension.SetImageAlpha(1f, null, Thumbnail);
                }

                thumbnail.preserveAspect = setAspectRatio;
            }

            if (sayWord != null)
            {
                Action act = vdialogue.ThumbnailEffectLoader(vchara, sayWord.EffectThm, sayWord.Magnitude, sayWord.LoopCount, sayWord.AnimatableThumbnailProp, sayWord.FrameRate, sayWord.HumanLikePause, sayWord.HumanLikePause);

                if (!isAnimatable)
                {
                    previousThumbnail = sprite;
                    LeanTween.alpha(rect, 1f, duration).setFrom(0f).setOnComplete(() =>
                    {
                        rect.anchoredPosition = defaultThmProps.position;
                        rect.sizeDelta = defaultThmProps.size;

                        if (setOnComplete && act != null)
                        {
                            act.Invoke();
                        }
                    });
                }
                else
                {
                    if (act != null)
                    {
                        act.Invoke();
                    }
                }
            }
        }

        public void HideVDialogue(Action next, string text, bool hide, WaitForClick waitForClick, bool waitUntilFinished = true, VCharacterV character = null)
        {
            if (LeanTween.isTweening(gameObject))
            {
                LeanTween.cancel(gameObject);
            }

            if (!gameObject.activeInHierarchy && !hide)
            {
                gameObject.SetActive(true);
            }

            if (showEffect == ShowHideEffect.FadeInOut)
            {
                float defVal = 0f;

                if (!hide)
                {
                    defVal = 1f;
                    canvyG.alpha = 0;
                }
                else
                {
                    canvyG.alpha = 1;
                }

                LeanTween.alphaCanvas(canvyG, defVal, showEffectDuration).setEase(showEasing).setOnComplete(() =>
                {
                    canvyG.alpha = defVal;

                    if (!hide && DialogueState != VDialogueState.Paused)
                        StartWriting(next, text, waitForClick, character: character);

                    if (hide)
                    {
                        gameObject.SetActive(false);
                    }
                });
            }
            else if (showEffect == ShowHideEffect.ZoomInOut)
            {
                Vector3 scale = Vector3.one;

                if (hide)
                {
                    scale = Vector3.zero;
                }
                else
                {
                    parentpanel.transform.localScale = Vector3.zero;
                }

                VdialogueAlpha(hide);

                LeanTween.scale(parentpanel, scale, showEffectDuration).setEase(showEasing).setOnComplete(() =>
                {
                    if (!hide && DialogueState != VDialogueState.Paused)
                        StartWriting(next, text, waitForClick, character: character);
                });
            }
            else if (showEffect == ShowHideEffect.SlideLeftRight || showEffect == ShowHideEffect.SlideRightLeft)
            {
                Vector3 offsetPos = Vector3.zero;
                Vector3 defaultPos = vdialogDefaultPosition;

                if (showEffect == ShowHideEffect.SlideLeftRight)
                {
                    if (!hide)
                    {
                        offsetPos = new Vector3(defaultPos.x / 2f, defaultPos.y, defaultPos.z);
                        parentpanel.transform.position = offsetPos;
                    }
                    else
                    {
                        defaultPos = new Vector3(defaultPos.x * 2f, defaultPos.y, defaultPos.z);
                    }
                }
                else
                {
                    float rightOffset = 0f;
                    rightOffset = defaultPos.x / 2;

                    if (!hide)
                    {
                        offsetPos = new Vector3(defaultPos.x + rightOffset, defaultPos.y, defaultPos.z);
                        parentpanel.transform.position = offsetPos;
                    }
                    else
                    {
                        defaultPos = new Vector3(defaultPos.x - rightOffset, defaultPos.y, defaultPos.z);
                    }
                }

                VdialogueAlpha(hide);

                LeanTween.moveX(parentpanel, defaultPos.x, showEffectDuration).setEase(showEasing).setOnComplete(() =>
                {
                    if (!hide && DialogueState != VDialogueState.Paused)
                        StartWriting(next, text, waitForClick, character: character);
                });
            }
            else if (showEffect == ShowHideEffect.SlideUp)
            {
                Vector3 offsetPos = Vector3.zero;
                Vector3 defaultPos = vdialogDefaultPosition;

                float upOffset = 0f;
                upOffset = defaultPos.y / 2;

                if (!hide)
                {
                    offsetPos = new Vector3(defaultPos.x, defaultPos.y - upOffset, defaultPos.z);
                    parentpanel.transform.position = offsetPos;
                }
                else
                {
                    defaultPos = new Vector3(defaultPos.x, defaultPos.y + upOffset, defaultPos.z);
                }

                VdialogueAlpha(hide);

                LeanTween.moveY(parentpanel, defaultPos.y, showEffectDuration).setEase(showEasing).setOnComplete(() =>
                {
                    if (!hide && DialogueState != VDialogueState.Paused)
                        StartWriting(next, text, waitForClick, character: character);
                });
            }

            if (hide)
            {
                if (VBlockManager.ActiveDialogue.Contains(this))
                    VBlockManager.ActiveDialogue.Remove(this);
            }
        }
        public void VdialogueAlpha(bool hide)
        {
            float alphaValue = 1f;

            if (hide)
            {
                canvyG.alpha = 1f;
                alphaValue = 0f;
            }
            else
            {
                canvyG.alpha = 0f;
            }

            LeanTween.alphaCanvas(canvyG, alphaValue, showEffectDuration - 0.03f).setOnComplete(() =>
            {
                canvyG.alpha = alphaValue;

                if (hide)
                {
                    gameObject.SetActive(false);
                }
            });
        }

        public virtual async void StartWriting(Action next, string text, WaitForClick waitForClick, VBlockManager vmanager = null, VCharacterV character = null)
        {
            VExtension.CheckTweensToCancel(gameObject);

            if (vmanager != null)
            {
                SetVBlockManager(vmanager);
            }

            if (!gameObject.activeInHierarchy)
            {
                HideVDialogue(next, text, false, waitForClick, character: character);
                return;
            }

            waitingForClick = waitForClick;
            bool isEmpty = String.IsNullOrEmpty(text);
            nextInvoke = null;
            nextInvoke = next;
            isWriting = true;
            continueSignal = continueIndicator;

            VStageUtil stage = VStageManager.GetActiveStage();

            //Necessary for when spawned the portrait for the very 1st time via SayWord.
            if (stage == null)
            {
                stage = VCharacterManager.FindVStage(null, null, true);
                VStageManager.SetActiveStage = stage;
            }

            //Dim state to speaking/non-speaking characters
            if (character != null)
            {
                VCharacterV vchara = VCharacterManager.GetVCharacter(character.charaId);
                characterName.SetText(character.name);

                if (stage.Dim)
                {
                    if (previousSpeakingCharacter != vchara)
                    {
                        previousSpeakingCharacter = vchara;
                        VCharacterManager.DimPortrait(vchara, true);
                    }
                }
                else
                {
                    var col = vchara.charaPortrait[vchara.activePortraitIndex].portraitObj.GetComponent<Image>();

                    if (vchara.dimmed == true && vchara.charaPortrait[character.activePortraitIndex] != null)
                    {
                        if (col.color != Color.white)
                        {
                            col.color = Color.white;
                        }

                        vchara.dimmed = false;
                    }
                }
            }
            else
            {
                if (stage.Dim)
                {
                    VCharacterManager.SetAllDimmed(true);
                }
            }

            if (!VBlockManager.ActiveDialogue.Contains(vdialogue))
            {
                VBlockManager.ActiveDialogue.Add(vdialogue);
            }

            VBlockManager.DefaultDialog = this;
            await ExecTyping(tmpComponent, text, waitForClick, pauseBetweenWords, textSpeed, isEmpty, textEffectType);

        }
        private float prevTick = 0f;
        public void ExecNext()
        {
            if (prevTick + VBlockManager.VInputDelays > Time.realtimeSinceStartup || gameObject.LeanIsTweening())
            {
                return;
            }

            prevTick = Time.realtimeSinceStartup;

            try
            {
                if (nextInvoke == null || waitingForClick == WaitForClick.Disable || DialogueState == VDialogueState.Paused)
                {
                    return;
                }

                if (DialogueState == VDialogueState.Writing)
                {
                    ForceComplete();
                }
                else if (DialogueState == VDialogueState.WaitForClick)
                {
                    if (waitingForClick == WaitForClick.Enable)
                        ContinueExecute();
                }
            }
            catch (Exception e)
            {
                //skip false-positives due to async in the Editor runtime
                if (e is NullReferenceException || e is MissingReferenceException)
                { return; }

                throw e;
            }

        }
        public async void ContinueExecute()
        {
            try
            {
                if (gameObject.LeanIsTweening())
                    return;

                isWaiting = false;

                if (VBlockManager.ActiveDialogue.Contains(this))
                    VBlockManager.ActiveDialogue.Remove(this);

                await StartContinueIndicator(false, null);
                NextFlag();
            }
            catch (MissingReferenceException)
            {
                return;
            }
        }
        //TODO: Make a command for this! UNTESTED btw! Expect to fail..
        public void AbortExecution(bool execNext = true)
        {
            ForceComplete();
            AbortVDialogueExecution = true;
            DialogueState = VDialogueState.StopWriting;
            isWaiting = false;

            CancelVDialogTokens();
            //TODO: this for skipping and autocomplete!
            if (execNext)
                NextFlag();
        }
        public void HideThisVDialogue(bool hide, Action act)
        {
            nextInvoke = null;
            float vals = 1f;

            ForceComplete();

            if (hide)
            {
                canvyG.alpha = 1f;
                vals = 0f;
            }
            else
            {
                canvyG.alpha = 0f;
            }

            VExtension.CheckTweensToCancel(gameObject);
            this.gameObject.transform.position = vdialogDefaultPosition;

            LeanTween.alphaCanvas(canvyG, vals, showEffectDuration).setOnComplete(() =>
            {
                canvyG.alpha = vals;
                gameObject.SetActive(false);

                if (hide)
                    act?.Invoke();
            });
        }

        private void ForceComplete()
        {
            isWriting = false;
            AutoComplete();
        }
        public Action ThumbnailEffectLoader(VCharacterV vcharacter, ThumbnailEffects effectThm, float magnitude, int loopCount = 0, AnimThumbnailProps animatableThumbnailProp = null, int frameRate = 12, bool humanLikePause = false, bool randomizeFramerate = false)
        {
            VExtension.CheckTweensToCancel(vdialogue.thmRect.gameObject);

            if (effectThm == ThumbnailEffects.Punch)
            {
                return () =>
                {
                    var vecs = vdialogue.thmRect.gameObject.transform.localScale + new Vector3(magnitude, magnitude, magnitude);
                    VExtension.VTweenScale(vdialogue.thmRect.gameObject, to: vecs, duration: vdialogue.ThumbnailDuration, loopCount: loopCount, loopType: LeanTweenType.punch);
                };
            }
            else if (effectThm == ThumbnailEffects.RoundAndRound)
            {
                return () =>
                {
                    VExtension.VTweenRotateAround(vdialogue.thmRect.gameObject, to: Vector3.forward, magnitude, duration: vdialogue.ThumbnailDuration, loopCount: loopCount, loopType: LeanTweenType.pingPong);
                };
            }
            else if (effectThm == ThumbnailEffects.BlinkAlpha)
            {
                return () =>
                {
                    VExtension.VTweenAlpha(vdialogue.thmRect, 1f, 0f, magnitude, loopCount, false, null, LeanTweenType.pingPong);
                };
            }
            else if (effectThm == ThumbnailEffects.PlayAnimatableProps && animatableThumbnailProp != null)
            {
                if (animatableThumbnailProp == null || vcharacter == null)
                    return null;

                var animatable = VCharacterManager.GetAnimProps(vcharacter, animatableThumbnailProp.name);

                if (animatable == null)
                    return null;

                return () =>
                {
                    LeanTweenType ease = LeanTweenType.pingPong;

                    if (animatable.loopClamp)
                    {
                        ease = LeanTweenType.clamp;
                    }

                    var animSprites = animatable.sprites.ToArray();
                    vdialogue.previousThumbnail = Array.Find(animSprites, x => x != null);
                    VExtension.VAnimateSprites(vdialogue.thmRect, animatable.sprites.ToArray(), frameRate, -1, loopType: ease, setonrepeat: humanLikePause, randomizeFramerate: humanLikePause);
                };
            }

            return null;
        }
    }
}