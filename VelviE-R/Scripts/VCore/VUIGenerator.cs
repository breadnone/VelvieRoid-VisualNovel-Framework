using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UIElements;
using System;
using UnityEngine.UI;
using TMPro;

public class VUIGenerator : MonoBehaviour
{
    private static Vector2 GameViewResolution;
    void Start()
    {
        GenerateVDialoguePanel();
    }    //Generate Canvas
    public static void GenerateVDialoguePanel(bool is2D = true, float ppu = 108f)
    {
#if UNITY_EDITOR
        GameViewResolution = UnityEditor.Handles.GetMainGameViewSize();
#endif

        GameObject myGO;
        GameObject contentText;
        Canvas canvy;
        CanvasScaler canvyScaler;
        CanvasGroup canvyG;
        RectTransform rectTransform;

        // Canvas
        myGO = new GameObject();
        myGO.name = "VDialog";
        myGO.AddComponent<Canvas>();

        canvy = myGO.GetComponent<Canvas>();
        canvy.renderMode = RenderMode.ScreenSpaceOverlay;
        myGO.AddComponent<CanvasScaler>();
        myGO.AddComponent<GraphicRaycaster>();
        myGO.AddComponent<CanvasGroup>();
        canvyG = myGO.GetComponent<CanvasGroup>();
        canvyScaler = myGO.GetComponent<CanvasScaler>();

        canvyG.alpha = 1f;
        canvyG.ignoreParentGroups = false;
        canvyG.blocksRaycasts = true;
        canvyG.interactable = true;
        canvyScaler.referenceResolution = GameViewResolution;

        //canvas properties
        canvy.referencePixelsPerUnit = ppu;
        canvy.renderMode = RenderMode.ScreenSpaceOverlay;
        canvyScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvyScaler.matchWidthOrHeight = 0f;

        //Create Panel
        GameObject panel = new GameObject();
        panel.name = "panel";
        panel.transform.SetParent(myGO.transform, false);
        UnityEngine.UI.Image img = null;
        panel.AddComponent<UnityEngine.UI.Image>();
        img = panel.GetComponent<UnityEngine.UI.Image>();

        //panel rects
        var panelRectTransform = panel.GetComponent<RectTransform>();
        panel.GetComponent<UnityEngine.UI.Image>().color = Color.blue;
        panelRectTransform.localPosition = new Vector3(0, panelRectTransform.position.y - (GameViewResolution.y / 2f) * 1.6f, 0);
        panelRectTransform.sizeDelta = new Vector2(GameViewResolution.x / 1.1f, (GameViewResolution.y / 2) / 1.55f);

        // Text
        contentText = new GameObject();
        contentText.name = "content";
        contentText.transform.SetParent(panel.transform, false);

        var text = contentText.AddComponent<TextMeshProUGUI>();
        text.fontSize = 33;
        text.text = "story goes here... story goes here... story goes here... story goes here... story goes here... story goes here... story goes here... story goes here... story goes here... story goes here... ";

        // Text position
        var rectPanel = panelRectTransform.sizeDelta;
        rectTransform = text.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0, 0, 0);
        rectTransform.sizeDelta = new Vector2(rectPanel.x / 1.1f, rectPanel.y / 1.8f);

        // Continue Text
        var continueText = new GameObject();
        continueText.name = "continue";
        continueText.transform.SetParent(contentText.transform, false);

        var textCon = continueText.AddComponent<TextMeshProUGUI>();
        textCon.fontSize = 55;
        textCon.text = "...";

        // Continue Text position
        var rectContinue = continueText.GetComponent<RectTransform>();
        rectContinue.sizeDelta = new Vector2(rectPanel.x / 9f, rectPanel.y / 8f);

        //set bottom right
        rectContinue.anchorMin = new Vector2(1, 0);
        rectContinue.anchorMax = new Vector2(1, 0);
        rectContinue.pivot = new Vector2(1, 0);
    }

    public static void LoadAssetFiles(Type type, string name)
    {

    }

    public static RectTransform Left(RectTransform rt, float x)
    {
        rt.offsetMin = new Vector2(x, rt.offsetMin.y);
        return rt;
    }

    public static RectTransform Right(RectTransform rt, float x)
    {
        rt.offsetMax = new Vector2(-x, rt.offsetMax.y);
        return rt;
    }

    public static RectTransform Bottom(RectTransform rt, float y)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, y);
        return rt;
    }

    public static RectTransform Top(RectTransform rt, float y)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -y);
        return rt;
    }
    public void VTEST()
    {
        Debug.Log("Testing");
    }

}
