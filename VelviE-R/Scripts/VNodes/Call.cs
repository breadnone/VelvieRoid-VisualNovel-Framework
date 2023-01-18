using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Threading;
using VTasks;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.Events;
#endif

namespace VelvieR
{
    public enum ContinueVBlockState
    {
        Continue,
        StopAndContinue
    }
    [VTag("Flow/Call", "Jump to this/other VNodes in the scene", VColor.Yellow, "CA")]
    public class Call : VBlockCore
    {
        [SerializeField, HideInInspector] private VCoreUtil vgraph;
        [SerializeField, HideInInspector] private string vnodeName = string.Empty;
        [SerializeField, HideInInspector] private ContinueVBlockState continueState = ContinueVBlockState.StopAndContinue;
        public string vnode { get { return vnodeName; } set { vnodeName = value; } }
        public VCoreUtil VGraph { get { return vgraph; } set { vgraph = value; } }
        public ContinueVBlockState ContinueState { get { return continueState; } set { continueState = value; } }
        [SerializeField, HideInInspector] private string triggererId = string.Empty;
        public string TriggerId{get{return triggererId;}set{triggererId = value;}}
        private VTokenSource cts;
        #if UNITY_EDITOR
        [SerializeField, HideInInspector] public UnityEvent RePoolNodes;
        [SerializeField, HideInInspector] public TriggererCalls triggerer = null;
        #endif
        public override void OnVEnter()
        {
            if (vgraph != null && !String.IsNullOrEmpty(vnodeName))
            {
                GetVNode(vnodeName);
            }
        }
        public async void GetVNode(string name)
        {
            foreach (var vblock in vgraph.vBlockCores)
            {
                if (vblock.vblocks.Count == 0)
                    continue;

                // Need to always check this ahead due to how the dialog window behaves
                //TODO: CHECK THIS ON EDITOR CODES!
                foreach (var blocks in vblock.vblocks)
                {
                    if (blocks != null && blocks.vnodeName == name && blocks.enable)
                    {
                        var currentIndexIsDialog = vgraph.ThisIndexIsSayDialog(blocks.vnodeId);

                        if (currentIndexIsDialog != null)
                        {
                            cts = new VTokenSource();
                            var pauseCtoke = cts.Token;
                            VTokenManager.PoolVToken(cts);

                            try
                            {
                                while (currentIndexIsDialog.VDialogue.gameObject.LeanIsTweening())
                                {
                                    if (pauseCtoke.IsCancellationRequested)
                                    {
                                        break;
                                    }
                                    await Task.Delay(1, cancellationToken: pauseCtoke);
                                }
                            }
                            catch (Exception e)
                            {
                                if (e is MissingReferenceException || e is NullReferenceException)
                                    return;
                                else
                                    throw e;
                            }

                            VTokenManager.CancelVToken(cts, true);
                            cts = null;
                        }

                        vgraph.OnCall(blocks.vnodeId);
                        return;
                    }
                }
            }
        }
        public override void OnVExit()
        {
            if(continueState == ContinueVBlockState.Continue)
            {
                OnVContinue();
            }
        }
        public override string OnVSummary()
        {
            string graphStr = string.Empty;
            string nodeStr = string.Empty;

            if(VGraph == null)
            {
                graphStr = "VGraph not assigned!|";
            }

            if(String.IsNullOrEmpty(vnode))
            {
                nodeStr = "VNode can't be empty!|";
            }

            return graphStr + nodeStr;
        }
    }
}