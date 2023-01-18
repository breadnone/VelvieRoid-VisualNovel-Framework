using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
public class OnEnableDisable : MonoBehaviour
{
    [SerializeField, HideInInspector] public GameObject onEnableDisablebcg;
    [SerializeField] public float showDuration = 0.3f;
    [SerializeField, HideInInspector] public Button okButton;
    [SerializeField, HideInInspector] public Button cancelButton;
    void OnValidate()
    {
        if (onEnableDisablebcg == null)
        {
            var parent = transform.parent.gameObject;
            onEnableDisablebcg = parent.transform.Find("bcgOverlay(Hide this)").gameObject;
        }

        if (okButton == null || cancelButton == null)
        {
            foreach (var child in transform.parent.transform.GetComponentsInChildren<Transform>())
            {
                if (child.name == "Button-OK")
                {
                    okButton = child.gameObject.GetComponent<Button>();
                }
                if (child.name == "Button-Cancel")
                {
                    cancelButton = child.gameObject.GetComponent<Button>();
                }
            }
        }
    }
    void OnEnable()
    {

    }
    void Start()
    {
        if (cancelButton != null)
            cancelButton.onClick.AddListener(() => EnableToggle());

        onEnableDisablebcg.transform.localScale = Vector3.zero;
        gameObject.transform.localScale = Vector3.zero;
        SetActives(false);
    }
    public void EnableToggle()
    {
        if (LeanTween.isTweening(onEnableDisablebcg))
        {
            LeanTween.cancel(onEnableDisablebcg, true);
        }
        if (LeanTween.isTweening(gameObject))
        {
            LeanTween.cancel(gameObject, true);
        }

        if (!onEnableDisablebcg.activeInHierarchy && !gameObject.activeInHierarchy)
        {
            SetActives(true);
            LeanTween.scale(onEnableDisablebcg, Vector3.one, showDuration).setEase(LeanTweenType.easeOutQuad);
            LeanTween.scale(gameObject, Vector3.one, showDuration).setEase(LeanTweenType.easeOutQuad);
        }
        else
        {
            LeanTween.scale(onEnableDisablebcg, Vector3.zero, showDuration - 0.01f).setEase(LeanTweenType.easeInQuad);
            LeanTween.scale(gameObject, Vector3.zero, showDuration).setEase(LeanTweenType.easeInQuad).setOnComplete(() =>
            {
                onEnableDisablebcg.transform.localScale = Vector3.zero;
                gameObject.transform.localScale = Vector3.zero;
                SetActives(false);
            });
        }
    }
    private void SetActives(bool state)
    {
        onEnableDisablebcg.SetActive(state);
        gameObject.SetActive(state);
    }
    private UnityAction previousOkListener;
    public void AddButtonAction(UnityAction act)
    {
        if (act != null && okButton != null)
        {
            if (previousOkListener != null)
            {
                okButton.onClick.RemoveListener(previousOkListener);
            }

            okButton.onClick.AddListener(act);
            previousOkListener = act;
        }
    }
    void OnDisable()
    {
        onEnableDisablebcg.transform.localScale = Vector3.zero;
        gameObject.transform.localScale = Vector3.zero;
    }
}
