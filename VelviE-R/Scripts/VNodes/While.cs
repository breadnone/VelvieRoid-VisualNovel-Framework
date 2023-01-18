using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Threading;
using VTasks;

namespace VelvieR
{
    [VTag("Flow/While", "Conditional loop statement. Must be ended/coupled with End", VColor.Yellow01, "Wl")]
    public class While : VBlockCore
    {
        [SerializeReference, HideInInspector] private IVar variable;
        [SerializeReference, HideInInspector] private IVar localVariable;
        [SerializeField, HideInInspector] public AnyTypes anyType = new AnyTypes();
        [SerializeField, HideInInspector] private EnumCondition eCondition = EnumCondition.None;
        [SerializeField, HideInInspector] public bool isLocal = false;
        public EnumCondition ECondition { get { return eCondition; } set { eCondition = value; } }
        public IVar Variable { get { return variable; } set { variable = value; } }
        public IVar LocalVariable { get { return localVariable; } set { localVariable = value; } }
        private int? endIndex;
        private ICondition icon = null;
        private VTokenSource cts;
        void OnDisable()
        {
            if (cts != null)
            {
                VTokenManager.CancelVToken(cts);
            }
        }
        void Awake()
        {
            icon = new ICondition();
        }
        public override async void OnVEnter()
        {
            endIndex = null;

            for (int i = 0; i < vmanager.currentRunning.Count; i++)
            {
                var idx = vmanager.currentRunning[i];

                if (idx.attachedComponent.isEndIf &&
                idx.attachedComponent.leftMargin == vmanager.currentRunning[thisIndex].attachedComponent.leftMargin)
                {
                    endIndex = i;
                    break;
                }
            }

            if (endIndex.HasValue)
            {
                await WhileProcessing(thisIndex, endIndex.Value - 1);
            }
        }
        public override void OnVExit()
        {
            if(!endIndex.HasValue)
                OnVContinue();
        }

        private bool CheckFlag()
        {
            if (isLocal)
            {
                return icon.VStartCompare(variable, localVariable, eCondition);
            }
            else
            {
                return icon.VStartCompare(variable, null, eCondition, anyType);
            }
        }

        private async ValueTask WhileProcessing(int start, int end)
        {
            cts = new VTokenSource();
            CancellationToken vts = cts.Token;
            VTokenManager.PoolVToken(cts);

            try
            {
                if (isLocal)
                {
                    if(icon.VStartCompare(variable, localVariable, eCondition))
                    {
                        Flagging(start, end);
                        OnVContinue();

                        while (icon.VStartCompare(variable, localVariable, eCondition))
                        {
                            if (cts.IsCancellationRequested)
                            {
                                vmanager.ExecuteNext(endIndex.Value, true);
                                return;
                            }

                            await Task.Delay(1);
                        }
                    }
                }
                else
                {
                    if(icon.VStartCompare(variable, null, eCondition, anyType))
                    {
                        Flagging(start, end);
                        OnVContinue();
                        
                        while (icon.VStartCompare(variable, null, eCondition, anyType))
                        {
                            if (cts.IsCancellationRequested)
                            {
                                vmanager.ExecuteNext(endIndex.Value, true);
                                return;
                            }

                            await Task.Delay(1);
                        }
                    }
                }                
            }
            catch (OperationCanceledException)
            {
                return;
            }

            VTokenManager.CancelVToken(cts);
        }

        private void Flagging(int startIdx, int endIdx)
        {
            var vcore = (VBlockCore)vmanager.currentRunning[endIdx].attachedComponent.component;
            vcore.SetFlagIndex(true, startIdx, CheckFlag);
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if (variable == null)
            {
                summary += "Variable can't be null.";
            }

            if (isLocal && localVariable == null)
            {
                summary += "| Local variable is null.";
            }

            if (eCondition == EnumCondition.None)
            {
                summary += "| Condition must be selected.";
            }

            return summary;
        }
    }
}