using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
using VTasks;

namespace VelvieR
{
    [VTag("Flow/Label", "Label to jump to", VColor.White, "J")]
    public class JumpLabel : VBlockCore
    {
        [SerializeField] private string label = string.Empty;
        public string Label { get { return label; } set { label = value; } }
        private bool nextIsdialogue = false;
        private VTokenSource cts;
        void OnDisable()
        {
            if (cts != null && !cts.wasDisposed)
            {
                VTokenManager.CancelVToken(cts, true);
            }
        }
        public override async void OnVEnter()
        {
            nextIsdialogue = false;
            var nextIsVdialogue = vmanager.NextIsVDialogue(thisIndex);

            if (nextIsVdialogue.state && nextIsVdialogue.vdialogue != null)
            {
                cts = new VTokenSource();
                var pauseCtoke = cts.Token;
                VTokenManager.PoolVToken(cts);
                nextIsdialogue = true;

                try
                {
                    while (nextIsVdialogue.vdialogue.gameObject.LeanIsTweening())
                    {
                        if (pauseCtoke.IsCancellationRequested)
                        {
                            return;
                        }

                        await Task.Delay(1, cancellationToken: pauseCtoke);
                    }
                }
                catch (Exception e)
                {
                    if(e is MissingReferenceException || e is NullReferenceException)
                        return;
                    else
                        throw e;
                }

                VTokenManager.CancelVToken(cts, true);
                cts = null;
                OnVContinue();
            }
        }

        public override void OnVExit()
        {
            if (!nextIsdialogue)
            {
                OnVContinue();
            }
        }
        public override string OnVSummary()
        {
            string summary = string.Empty;
            
            if(String.IsNullOrEmpty(label))
                summary = "Label name can't be empty!";

            return summary;
        }
    }
}