using UnityEngine;
using System;
using TMPro;

namespace VelvieR
{
    [System.Serializable]
    public enum TimeCategory
    {
        TimeSincelLevelLoad,
        FrameCountSinceStarted,
        RealTimeSinceStartup,
        CurrentTime
    }
    [VTag("Events/GetTime", "Time and frame utility.\n\nOutput text is optional.", VColor.Gray, "Gc")]
    public class GetTime : VBlockCore
    {
        [SerializeReference] public IVar variable;
        [SerializeField] public TMP_Text text;
        [SerializeField] public TimeCategory timeCategory = TimeCategory.CurrentTime;
        public override void OnVEnter()
        {
            if(variable != null)
            {
                if(timeCategory == TimeCategory.TimeSincelLevelLoad)
                {
                    variable.SetString(Time.timeSinceLevelLoadAsDouble.ToString());
                }
                else if(timeCategory == TimeCategory.FrameCountSinceStarted)
                {
                    variable.SetString(Time.frameCount.ToString());
                }
                else if(timeCategory == TimeCategory.RealTimeSinceStartup)
                {
                    variable.SetString(Time.realtimeSinceStartupAsDouble.ToString());
                }
                else if(timeCategory == TimeCategory.CurrentTime)
                {
                    variable.SetString(DateTime.Now.ToString());
                }
            }

            if(text != null)
            {
                if(timeCategory == TimeCategory.TimeSincelLevelLoad)
                {
                    text.SetText(Time.timeSinceLevelLoadAsDouble.ToString());
                }
                else if(timeCategory == TimeCategory.FrameCountSinceStarted)
                {
                    text.SetText(Time.frameCount.ToString());
                }
                else if(timeCategory == TimeCategory.RealTimeSinceStartup)
                {
                    text.SetText(Time.realtimeSinceStartupAsDouble.ToString());
                }
                else if(timeCategory == TimeCategory.CurrentTime)
                {
                    text.SetText(DateTime.Now.ToString());
                }
            }
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
    }
}