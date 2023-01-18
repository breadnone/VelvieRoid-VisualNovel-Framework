using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using VTasks;

namespace VelvieR
{
    public class VDialogue : MonoBehaviour
    {
        public bool VSwitchContext { get; set; }
        public TMP_Text conTmp { get; set; }
        public TMP_Text writeIndicTmp { get; set; }
        public GameObject conIcon { get; set; }
        public int maxVisibleContinueDots { get; set; } = 3;
        public virtual VDialogueState DialogueState { get; set; }
        public virtual float EnderDelay { get; set; } = 4;
        public virtual float FadeSpeed { get; set; } = 20f;
        public virtual int RolloverCharacterSpread { get; set; } = 10;
        public VelvieDialogue vdialogue { get; set; }
        private TMP_Text activeTmp;
        private string activeWords;
        private DialogType vdialogType;
        private WaitForClick waitingForClick;
        public ContinueAnim continueSignal { get; set; }
        public VBlockManager vmanager { get; set; }
        public Action nextInvoke { get; set; }
        public int thisIndex { get; set; }
        public VTokenSource cts;
        public VTokenSource typeSource;
        public VTokenSource continueSource;
        public CancellationToken[] vtokes;
        public VAudioClass auSource;
        public VAudioClass auSourceCommon;
        public AudioClip enderAu { get; set; }
        public AudioClip typeAu { get; set; }
        public bool enableTypeAu { get; set; }
        public bool enableEnderAudio { get; set; }
        public bool writeIndic { get; set; }
        public string writeTxtIndicator { get; set; }
        public Sprite previousThumbnail { get; set; }

        public void CancelVDialogTokens()
        {
            if (vtokes != null)
            {
                if (!typeSource.wasDisposed)
                {
                    try
                    {
                        typeSource.Cancel();
                        typeSource.Dispose();
                    }
                    catch (Exception e)
                    {
                        if (e is OperationCanceledException) { }

                        throw e;
                    }

                    typeSource = null;
                }
                if (!continueSource.wasDisposed)
                {
                    try
                    {
                        continueSource.Cancel();
                        continueSource.Dispose();
                    }
                    catch (Exception e)
                    {
                        if (e is OperationCanceledException) { }

                        throw e;
                    }

                    continueSource = null;
                }

                VTokenManager.CancelVToken(cts, true);
                cts = null;
            }
        }
        public void SetVBlockManager(VBlockManager vblockman)
        {
            if (vblockman != null)
                vmanager = vblockman;
        }
        public ValueTask<bool> TypingTask { get; set; }
        public ValueTask<bool> ContinueTask { get; set; }
        public virtual async ValueTask ExecTyping(TMP_Text tmp, string text, WaitForClick waitClick, float pauseBetweenWords, float rpm, bool skip = false, DialogType dialogType = DialogType.TypeWriter)
        {
            if (!String.IsNullOrEmpty(text))
            {
                if (!skip)
                {
                    auSource.audioSource.clip = typeAu;
                    activeTmp = tmp;
                    activeWords = text;
                    vdialogType = dialogType;
                    waitingForClick = waitClick;
                    ContinueIsRunning = false;
                    DialogueState = VDialogueState.Writing;
                    cts = new VTokenSource();
                    continueSource = new VTokenSource();
                    typeSource = new VTokenSource();

                    var length = 2;

                    if (writeIndic)
                        length = 3;

                    var vtokes = new CancellationToken[length];

                    for (int i = 0; i < vtokes.Length; i++)
                    {
                        if (i == 0)
                        {
                            vtokes[i] = typeSource.Token;
                        }
                        else if (i == 1)
                        {
                            vtokes[i] = continueSource.Token;
                        }
                        else if (i == 2 && writeIndic)
                        {
                            vtokes[i] = typeSource.Token;
                        }

                        vtokes[i].ThrowIfCancellationRequested();
                    }

                    VTokenManager.PoolVToken(cts);
                    bool isCompleted = false;

                    if (!VSwitchContext)
                    {
                        if (writeIndic)
                            _ = WriteIndicator(vtokes[2]);

                        TypingTask = StartTyping(tmp, text, rpm, pauseBetweenWords, vtokes[0], dialogType);
                        isCompleted = await TypingTask;
                    }
                    else
                    {
                        await Task.Factory.StartNew(async () => await StartTyping(tmp, text, rpm, pauseBetweenWords, vtokes[0], dialogType), vtokes[0], TaskCreationOptions.LongRunning, VBlockManager.UnityContext).ContinueWith((x) => isCompleted);
                        VSwitchContext = false;
                    }

                    await Task.Yield();

                    if (continueSource.IsCancellationRequested)
                    {
                        return;
                    }

                    if (!ContinueIsRunning)
                    {
                        await Continue(vtokes[1]);
                    }
                }
            }
            else if (String.IsNullOrEmpty(text) || skip)
            {
                NextFlag();
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual async ValueTask<bool> StartTyping(TMP_Text tmp, string content, float rpm, float pauseBetweenWords, CancellationToken vts, DialogType dialogType = DialogType.TypeWriter)
        {
            bool val = false;

            if (dialogType == DialogType.TypeWriter)
            {
                tmp.SetText(content);
                tmp.ForceMeshUpdate();
                int counta = 0;
                int visibleCount = 0;
                int len = content.Length;
                int charCount = tmp.textInfo.characterCount;

                while (DialogueState == VDialogueState.Writing)
                {
                    try
                    {
                        charCount = tmp.textInfo.characterCount;
                        visibleCount = counta % (charCount + 1);
                        tmp.maxVisibleCharacters = visibleCount;

                        if (len == counta)
                        {
                            if (typeSource.IsCancellationRequested)
                            {
                                return false;
                            }

                            if (enableEnderAudio && val)
                            {
                                auSource.audioSource.Stop();
                                auSource.audioSource.clip = enderAu;
                                auSource?.audioSource.Play();
                            }

                            CancelThumbnailTweens();
                            return true;
                        }

                        char currentChar = tmp.text[counta];

                        if (enableTypeAu && !Char.IsWhiteSpace(currentChar))
                        {
                            if (vts.IsCancellationRequested)
                            {
                                return false;
                            }
                            if (!auSource.audioSource.gameObject.activeInHierarchy)
                            {
                                return false;
                            }
                            auSource.audioSource.PlayOneShot(typeAu);
                        }

                        if (vts.IsCancellationRequested)
                        {
                            return false;
                        }
                        if (!Char.IsWhiteSpace(currentChar) && currentChar != ',')
                        {
                            val = await VTask.VWaitSeconds(rpm, cancellationToken: vts);
                        }
                        else
                        {
                            val = await VTask.VWaitSeconds(pauseBetweenWords, cancellationToken: vts);
                        }

                        if (!val)
                        {
                            return false;
                        }

                        if (DialogueState == VDialogueState.Paused)
                        {
                            var pauseCtoke = cts.Token;

                            while (DialogueState == VDialogueState.Paused)
                            {
                                if (pauseCtoke.IsCancellationRequested)
                                {
                                    break;
                                }
                                await VTask.VYield(pauseCtoke);
                            }
                        }

                        counta++;
                    }
                    catch (Exception e)
                    {
                        if (e is NullReferenceException || e is MissingReferenceException)
                        {
                            break;
                        }

                        throw e;
                    }
                }
            }
            else if (dialogType == DialogType.GradientFade)
            {
                tmp.SetText(content);
                tmp.color = new Color
                (
                    tmp.color.r, tmp.color.g, tmp.color.b, 0
                );

                if (!tmp.gameObject.transform.parent.transform.parent.gameObject.activeInHierarchy)
                    tmp.gameObject.SetActive(true);

                tmp.ForceMeshUpdate();
                TMP_TextInfo textInfo = tmp.textInfo;
                Color32[] newVertexColors;
                int currentCharacter = 0;
                int startingCharacterRange = currentCharacter;
                bool wasCancelled = false;
                DialogueState = VDialogueState.Writing;

                while (DialogueState == VDialogueState.Writing)
                {
                    int characterCount = textInfo.characterCount;
                    byte fadeSteps = (byte)Mathf.Max(1, 255 / RolloverCharacterSpread);

                    for (int i = startingCharacterRange; i < currentCharacter + 1; i++)
                    {
                        if (!textInfo.characterInfo[i].isVisible)
                            continue;

                        int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                        newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                        int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                        byte alpha = (byte)Mathf.Clamp(newVertexColors[vertexIndex + 0].a + fadeSteps, 0, 255);
                        newVertexColors[vertexIndex + 0].a = alpha;
                        newVertexColors[vertexIndex + 1].a = alpha;
                        newVertexColors[vertexIndex + 2].a = alpha;
                        newVertexColors[vertexIndex + 3].a = alpha;

                        if (alpha == 255)
                        {
                            startingCharacterRange += 1;

                            if (startingCharacterRange == characterCount)
                            {
                                if (vts.IsCancellationRequested)
                                {
                                    return false;
                                }

                                if (enableEnderAudio)
                                {
                                    auSourceCommon.audioSource.clip = enderAu;
                                    auSourceCommon.audioSource.Play();
                                }

                                CancelThumbnailTweens();
                                return true;
                            }
                        }

                        if (DialogueState == VDialogueState.Paused)
                        {
                            var pauseCtoke = typeSource.Token;

                            while (DialogueState == VDialogueState.Paused)
                            {
                                if (pauseCtoke.IsCancellationRequested)
                                {
                                    break;
                                }

                                await VTask.VYield(pauseCtoke);
                            }
                        }
                    }

                    if (wasCancelled)
                        break;

                    tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                    if (currentCharacter + 1 < characterCount)
                        currentCharacter += 1;

                    if (vts.IsCancellationRequested)
                    {
                        return false;
                    }

                    val = await VTask.VWaitSeconds(0.25f - FadeSpeed * 0.01f, cancellationToken: vts);
                    if (!val) return false;

                }
            }
            else if (dialogType == DialogType.None)
            {
                tmp.SetText(content);
            }

            return true;
        }
        private void CancelThumbnailTweens()
        {
            if (LeanTween.isTweening(vdialogue.thmRect.gameObject))
            {
                LeanTween.cancel(vdialogue.thmRect.gameObject, true);

                if (previousThumbnail != null)
                    vdialogue.Thumbnail.sprite = previousThumbnail;
            }
        }
        private async ValueTask<bool> Continue(CancellationToken ctoke)
        {
            (bool withContinue, bool val) vars = new(false, false);

            vars.withContinue = continueSignal != ContinueAnim.None;

            if (vars.withContinue && !ContinueIsRunning)
            {
                if (!ctoke.IsCancellationRequested)
                {
                    if (waitingForClick == WaitForClick.Enable)
                    {
                        DialogueState = VDialogueState.WaitForClick;
                        vdialogue.isWriting = false;
                        vdialogue.isWaiting = true;

                        ContinueTask = StartContinueIndicator(true, ctoke, continueSignal);
                        await ContinueTask;
                    }
                    else
                    {
                        DialogueState = VDialogueState.StopWriting;
                        if (vars.withContinue && !ContinueIsRunning)
                        {
                            if (!ctoke.IsCancellationRequested)
                            {
                                ContinueTask = StartContinueIndicator(true, ctoke, continueSignal);
                                _ = ContinueTask;
                            }
                        }

                        if (ctoke.IsCancellationRequested)
                        {
                            return false;
                        }

                        vdialogue.isWaiting = false;
                        vdialogue.isWriting = false;
                        vars.val = await VTask.VWaitSeconds(EnderDelay, cancellationToken: ctoke);
                        NextFlag(vars.withContinue);
                        return vars.val;
                    }
                }
            }

            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AutoComplete()
        {
            //this is necessary! Don't remove this! Unity doesn't support TryReset yet
            if (!typeSource.wasDisposed)
            {
                typeSource.Cancel();
            }

            if (activeTmp != null && DialogueState == VDialogueState.Writing && DialogueState != VDialogueState.Paused)
            {
                if (vdialogType == DialogType.TypeWriter)
                {
                    activeTmp.maxVisibleCharacters = activeWords.Length;
                }
                else
                {
                    Color32[] newVertexColors;

                    for (int i = 0; i < activeTmp.textInfo.characterCount; i++)
                    {
                        int materialIndex = activeTmp.textInfo.characterInfo[i].materialReferenceIndex;
                        newVertexColors = activeTmp.textInfo.meshInfo[materialIndex].colors32;
                        int vertexIndex = activeTmp.textInfo.characterInfo[i].vertexIndex;
                        newVertexColors[vertexIndex + 0].a = 255;
                        newVertexColors[vertexIndex + 1].a = 255;
                        newVertexColors[vertexIndex + 2].a = 255;
                        newVertexColors[vertexIndex + 3].a = 255;
                        activeTmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                    }
                }

                //activeTmp.ForceMeshUpdate();

                if (waitingForClick == WaitForClick.Enable)
                {
                    if (DialogueState != VDialogueState.StopWriting)
                        DialogueState = VDialogueState.WaitForClick;
                }
                else
                {
                    DialogueState = VDialogueState.StopWriting;
                }

                activeTmp = null;
                activeWords = string.Empty;
            }

            CancelThumbnailTweens();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<string> VTokenizer(string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                string[] splitStart = str.Split('{');
                List<string> splitEnd = new List<string>();

                if (splitStart.Length > 1)
                {
                    if (splitStart.Length > 0)
                    {
                        for (int i = 0; i < splitStart.Length; i++)
                        {
                            if (splitStart[i].Contains("}"))
                            {
                                var tmp = splitStart[i].Split('}');
                                splitEnd.Add(tmp[0]);
                            }
                        }

                        return splitEnd;
                    }
                }
            }
            return null;
        }
        public void CheckCustomTags()
        {

        }
        public void NextFlag(bool cancelVDialog = false)
        {
            DialogueState = VDialogueState.StopWriting;
            var nextvdial = vmanager.NextIsVDialogue(thisIndex);
            CancelVDialogTokens();

            if (nextvdial.vdialogue != null)
            {
                if (!nextvdial.state)
                {
                    nextvdial.vdialogue.HideVDialogue(null, string.Empty, true, nextvdial.vdialogue.waitingForClick);
                }

                if (nextvdial.vdialogue.AbortVDialogueExecution && !cancelVDialog)
                {
                    vmanager.CancelVDialogue(nextvdial.vdialogue, thisIndex);
                    return;
                }
                nextvdial.vdialogue.TmpComponent.SetText(string.Empty);
            }

            nextInvoke?.Invoke();
        }
        public bool ContinueIsRunning { get; set; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask<bool> StartContinueIndicator(bool state, CancellationToken? vts, ContinueAnim con = ContinueAnim.None, GameObject obj = null)
        {
            if (vdialogue == null || vdialogue.gameObject == null)
                return false;

            if (state)
            {
                if (vdialogue.gameObject.activeInHierarchy)
                {
                    ContinueIsRunning = true;

                    if (conTmp != null && con == ContinueAnim.ThreeDots)
                    {
                        conTmp.maxVisibleCharacters = 0;

                        if (!conTmp.gameObject.activeInHierarchy)
                            conTmp.gameObject.SetActive(true);
                    }

                    if (conIcon != null && con == ContinueAnim.Icon)
                    {
                        conIcon.SetActive(false);
                    }

                    (bool val, int counter) vars = new(false, 0);

                    while ((DialogueState == VDialogueState.WaitForClick || DialogueState == VDialogueState.StopWriting) && con != ContinueAnim.None)
                    {
                        if (conTmp != null && con == ContinueAnim.ThreeDots)
                        {
                            vars.counter++;
                            conTmp.maxVisibleCharacters = vars.counter;

                            if (vars.counter == maxVisibleContinueDots)
                                vars.counter = 0;
                        }
                        else if (conIcon != null && con == ContinueAnim.Icon)
                        {
                            if (vars.counter == 0)
                            {
                                vars.counter++;
                                conIcon.SetActive(true);
                            }
                            else
                            {
                                conIcon.SetActive(false);
                            }
                        }

                        //await Task.Delay(TimeSpan.FromSeconds(1.0), cancellationToken: vts);
                        if (vts.Value.IsCancellationRequested)
                        {
                            break;
                        }

                        vars.val = await VTask.VWaitSeconds(0.6f, cancellationToken: vts.Value);

                        if (!vars.val)
                        {
                            break;
                        }
                    }

                    if (conTmp != null && con == ContinueAnim.ThreeDots)
                    {
                        conTmp.gameObject.SetActive(false);
                        conTmp.maxVisibleCharacters = 0;
                    }
                    else if (conIcon != null && con == ContinueAnim.Icon)
                    {
                        conIcon.SetActive(false);
                    }

                    ContinueIsRunning = false;
                    return true;
                }
            }
            else
            {
                await Task.Yield();

                if (obj != null)
                    obj.SetActive(false);

                if (conTmp != null)
                {
                    conTmp.maxVisibleCharacters = 0;
                    conTmp.gameObject.SetActive(false);
                }

                ContinueIsRunning = false;
            }
            return true;
        }

        private async ValueTask WriteIndicator(CancellationToken vts, int indicFontSize = 20)
        {
            if (writeIndicTmp != null && writeTxtIndicator.Length > 0)
            {
                var oriSize = writeIndicTmp.fontSize;
                var obj = writeIndicTmp.gameObject;
                writeIndicTmp.fontSize = indicFontSize;
                writeIndicTmp.SetText(writeTxtIndicator);
                writeIndicTmp.maxVisibleCharacters = 0;
                bool val = false;
                int len = writeTxtIndicator.Length;

                while (DialogueState == VDialogueState.Writing)
                {
                    if (vts.IsCancellationRequested)
                    {
                        break;
                    }

                    if (len != writeIndicTmp.maxVisibleCharacters)
                        writeIndicTmp.maxVisibleCharacters += 1;

                    val = await VTask.VWaitSeconds(0.1f, cancellationToken: vts);
                    if (!val)
                        break;

                    if (writeIndicTmp.maxVisibleCharacters == len)
                        writeIndicTmp.maxVisibleCharacters = 0;
                }

                writeIndicTmp.fontSize = oriSize;
                writeIndicTmp.maxVisibleCharacters = 0;
            }
        }
    }
}