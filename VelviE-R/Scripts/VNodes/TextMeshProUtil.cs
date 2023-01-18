using UnityEngine;
using TMPro;

namespace VelvieR
{
    [System.Serializable]
    public enum TMpUtilv
    {
        SetFontSize,
        SetFont,
        SetColor,
        SetAlignment,
        SetBold,
        SetItalic,
        SetUnderline,
        SetStrikesThrough,
        SetAllUppercase,
        SetAllLowerCase,
        EnableDisableRaycast,
        RemoveFirstLetter,
        RemoveLastLetter,
        None
    }

    [VTag("UIUX/TextMeshProUtil", "Set of runtime utility for TextMeshPro.", VColor.Pink01, "Tm")]
    public class TextMeshProUtil : VBlockCore
    {
        [SerializeField, HideInInspector] public TMP_Text text;
        [SerializeField, HideInInspector] public float fontSize;
        [SerializeField, HideInInspector] public TMP_FontAsset font;
        [SerializeField, HideInInspector] public Color textColor;
        [SerializeField, HideInInspector] public TextAlignmentOptions align = TextAlignmentOptions.TopLeft;
        [SerializeField, HideInInspector] public TMpUtilv tmpUtil = TMpUtilv.None;
        [SerializeField, HideInInspector] public bool raycast = true;
        public override void OnVEnter()
        {
            if(text != null && tmpUtil != TMpUtilv.None)
            {
                if(tmpUtil == TMpUtilv.SetAlignment)
                {
                    text.alignment = align;
                }
                else if(tmpUtil == TMpUtilv.SetFontSize)
                {
                    text.fontSize = fontSize;
                }
                else if(tmpUtil == TMpUtilv.SetColor)
                {
                    text.color = textColor;
                }
                else if(tmpUtil == TMpUtilv.SetFont)
                {
                    if(font != null)
                    text.font = font;
                }
                else if(tmpUtil == TMpUtilv.EnableDisableRaycast)
                {
                    text.raycastTarget = raycast;
                }
                else if(tmpUtil == TMpUtilv.SetAllLowerCase)
                {
                    text.SetText(text.text.ToLowerInvariant());
                }
                else if(tmpUtil == TMpUtilv.SetAllUppercase)
                { 
                    text.SetText(text.text.ToUpperInvariant());
                }
                else if(tmpUtil == TMpUtilv.SetBold)
                {
                    text.fontStyle = FontStyles.Bold;
                }
                else if(tmpUtil == TMpUtilv.SetUnderline)
                {
                    text.fontStyle = FontStyles.Underline;
                }
                else if(tmpUtil == TMpUtilv.SetStrikesThrough)
                {
                    text.fontStyle = FontStyles.Strikethrough;
                }
                else if(tmpUtil == TMpUtilv.SetItalic)
                {
                    text.fontStyle = FontStyles.Italic;
                }
                else if(tmpUtil == TMpUtilv.RemoveFirstLetter)
                {
                    if(text.text.Length > 0)
                    text.SetText(text.text.Substring(1));
                }
                else if(tmpUtil == TMpUtilv.RemoveLastLetter)
                {
                    if(text.text.Length > 0)
                    text.SetText(text.text.Substring(0, text.text.Length - 1));
                }
            }
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if(text == null)
            {
                summary += "TMP_Text component can't be empty!";
            }

            return summary;
        }
    }
}