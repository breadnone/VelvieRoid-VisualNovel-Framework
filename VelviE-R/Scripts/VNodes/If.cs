
using UnityEngine;


namespace VelvieR
{
    [VTag("Flow/If", "Conditional statement. Must be ended/coupled with End", VColor.Yellow01)]
    public class If : VBlockCore
    {
        [SerializeReference, HideInInspector] private IVar variable;
        [SerializeReference, HideInInspector] private IVar localVariable;
        [SerializeField, HideInInspector] private string localOrValue = string.Empty;
        [SerializeField, HideInInspector] private EnumCondition eCondition = EnumCondition.None;
        
        public EnumCondition ECondition { get { return eCondition; } set { eCondition = value; } }
        public IVar Variable { get { return variable; } set { variable = value; } }
        public IVar LocalVariable { get { return localVariable; } set { localVariable = value; } }
        public string LocalorValue { get { return localOrValue; } set { localOrValue = value; } }
        private bool result = false;
        private int? endIndex;
        private ICondition icon = null;
        void Awake()
        {
            icon = new ICondition();
        }

        public override void OnVEnter()
        {
            endIndex = null;
            result = false;
            result = icon.VStartCompare(variable, localVariable, eCondition);

            for(int i = 0; i < vmanager.currentRunning.Count; i++)
            {
                var idx = vmanager.currentRunning[i];

                if(idx.attachedComponent.isEndIf && 
                idx.attachedComponent.leftMargin == vmanager.currentRunning[thisIndex].attachedComponent.leftMargin)
                {
                    endIndex = i;
                    break;
                }
            }
        }
        public override void OnVExit()
        {
            if (result)
            {
                OnVContinue();
            }
            else
            {
                if(endIndex.HasValue)
                    vmanager.ExecuteNext(endIndex.Value, true);
            }
        }
        public override string OnVSummary()
        {
            string summary = string.Empty;

            if (Variable == null)
            {
                summary += "Variable can't be empty! :: ";
            }

            if (ECondition == EnumCondition.None)
            {
                summary += "Condition can't be empty!";
            }

            return summary;
        }
    }
}