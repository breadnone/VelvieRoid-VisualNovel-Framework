using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using VTasks;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace VelvieR
{
    [VTag("UIUX/SetText", "Set text on runtime on a TextMeshPro UI component on runtime.", VColor.Pink01, "St")]
    public class SetText : VBlockCore
    {
        [SerializeField, HideInInspector] public TMP_Text text;
        [SerializeField, HideInInspector] public string content;
        [SerializeField, HideInInspector] public  float pauseBetweenWords = 0.1f;
        [SerializeField, HideInInspector] public bool isTypewriter;
        [SerializeField, HideInInspector] public bool waitUntilFinished = false;
        private VTokenSource cts;
        public override void OnVEnter()
        {
            if(text != null && !String.IsNullOrEmpty(content))
            {
                VWrite();                
            }
        }
        void OnDisable()
        {
            if(cts != null)
            {
                VTokenManager.CancelVToken(cts, true);
            }
        }
        private async void VWrite()
        {
            cts = new VTokenSource();
            CancellationToken vts = cts.Token;
            VTokenManager.PoolVToken(cts);

            if (isTypewriter)
            {
                text.SetText(content);
                text.ForceMeshUpdate();
                int counta = 0;
                int visibleCount = 0;
                int len = content.Length;
                int charCount = text.textInfo.characterCount;

                while (true)
                {
                    try
                    {
                        charCount = text.textInfo.characterCount;
                        visibleCount = counta % (charCount + 1);
                        text.maxVisibleCharacters = visibleCount;

                        if (len == counta)
                        {
                            if (cts.IsCancellationRequested)
                            {
                                return;
                            }

                            if(waitUntilFinished)
                                OnVContinue();

                            return;
                        }

                        char currentChar = text.text[counta];

                        if (vts.IsCancellationRequested)
                        {
                            return;
                        }

                        await VTask.VWaitSeconds(pauseBetweenWords, cancellationToken: vts);                        
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
            else
            {
                text.SetText(content);
            }

            VTokenManager.CancelVToken(cts, true);
        }
        public override void OnVExit()
        {
            if(!waitUntilFinished)
                OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if(text == null)
            {
                summary += "TMP_Text component can't be empty!";
            }

            if(String.IsNullOrEmpty(content))
            {
                summary += "::Content can't be empty!";
            }

            return summary;
        }
    }
}