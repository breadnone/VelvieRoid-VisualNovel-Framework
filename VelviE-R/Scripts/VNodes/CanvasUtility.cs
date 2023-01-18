using UnityEngine;
using System;
using UnityEngine.UI;

namespace VelvieR
{
    [System.Serializable]
    public enum Canvasutilityv
    {
        SetResolution,
        SetRenderMode,
        SetSortOder,
        None
    }
    [VTag("UIUX/CanvasUtility", "Set of runtime Canvas utility.", VColor.Grey, "Cu")]
    public class CanvasUtility : VBlockCore
    {
        [SerializeField, HideInInspector] public Canvas canvas;
        [SerializeField, HideInInspector] public Canvasutilityv canvasUtil = Canvasutilityv.None;
        [SerializeField, HideInInspector] public Vector2 resolution;
        [SerializeField, HideInInspector] public int order = 0;
        [SerializeField, HideInInspector] public RenderMode renderMode = RenderMode.ScreenSpaceOverlay;
        [SerializeField, HideInInspector] public Camera camx;

        public override void OnVEnter()
        {
            if (canvas != null)
            {
                if (canvasUtil == Canvasutilityv.SetResolution)
                {
                    if (canvas.renderMode == RenderMode.WorldSpace)
                    {
                        throw new Exception("VError: Canvas is in WorldSpace mode! Only ScreenSpaceCamera and Overlay modes can be adjusted!");
                    }

                    var scaler = canvas.gameObject.GetComponent<CanvasScaler>();

                    if(scaler == null)
                    {
                        throw new Exception("VError: CanvasScaler can't be found!");
                    }

                    scaler.referenceResolution = resolution;
                }
                else if (canvasUtil == Canvasutilityv.SetRenderMode)
                {
                    canvas.renderMode = renderMode;

                    if (renderMode == RenderMode.ScreenSpaceCamera)
                    {
                        if (camx == null)
                        {
                            throw new Exception("VError: ScreenSpaceCamera canvas must assign active/main camera!");
                        }
                        else
                        {
                            canvas.worldCamera = camx;
                        }
                    }
                }
                else if (canvasUtil == Canvasutilityv.SetSortOder)
                {
                    canvas.sortingOrder = order;
                }

                Canvas.ForceUpdateCanvases();
            }
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if (canvas == null)
            {
                summary += "Canvas object can't be empty!";
            }

            return summary;
        }
    }
}