
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

namespace VelvieR
{
    public class VInventoryItemEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public static List<VInventoryItemEvent> vinventoryitems = new List<VInventoryItemEvent>();
        [SerializeField] public Image imgContainer;
        [SerializeField] public TMP_Text textContainer;
        [SerializeField] public float duration = 0.3f;
        [SerializeField] public LeanTweenType ease = LeanTweenType.easeOutBounce;
        [SerializeField, HideInInspector] public VInventoryClass vinventory;
        [SerializeField, HideInInspector] public Image thisImage;
        [SerializeField] public List<Button> buttons = new List<Button>();
        [SerializeField, HideInInspector] public int editorCounter = 10;
        [SerializeField, HideInInspector] public VInventory vin;
        [SerializeField, HideInInspector] public TMP_Text leftbannerText;
        [SerializeField] public string customLeftBannerText = "Select item!!!";
        void OnValidate()
        {
            if (thisImage == null)
            {
                thisImage = GetComponent<Image>();
            }

            if (buttons[0] == null || buttons.Count == 0)
            {
                var objTrans = imgContainer.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform;
                var childs = objTrans.GetComponentsInChildren<Button>();

                foreach (var child in childs)
                {
                    if (child == null)
                        continue;

                    if (!buttons.Contains(child))
                        buttons.Add(child);
                }
            }

        }
        void OnEnable()
        {
            vinventoryitems.Add(this);

            if (buttons.Count > 0)
            {
                SetButtonStates(false);
            }

            imgContainer.gameObject.transform.parent.gameObject.transform.parent.gameObject.SetActive(false);

            if (leftbannerText == null)
            {
                leftbannerText = imgContainer.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.Find("TxtHost").GetComponentInChildren<TMP_Text>();
                leftbannerText.SetText(customLeftBannerText);
                leftbannerText.gameObject.SetActive(true);
            }

            transform.localScale = Vector3.one;
        }
        void OnAwake()
        {
            VPropsManager.InsertVevent(this);
        }

        void OnDisable()
        {
            imgContainer.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.localScale = Vector3.one;
            SetButtonStates(false);
            imgContainer.gameObject.transform.parent.gameObject.transform.parent.gameObject.SetActive(false);
            leftbannerText.gameObject.SetActive(true);
        }
        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            gameObject.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }

        public void OnPointerExit(PointerEventData pointerEventData)
        {
            gameObject.transform.localScale = Vector3.one;
        }
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            if (vinventory != null)
            {
                leftbannerText.gameObject.SetActive(false);
                imgContainer.gameObject.transform.parent.gameObject.transform.parent.gameObject.SetActive(true);
                var go = imgContainer.gameObject.transform.parent.gameObject.transform.parent.gameObject;

                if (LeanTween.isTweening(go))
                {
                    LeanTween.cancel(go, true);
                }

                go.SetActive(true);
                go.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                SetButtonStates(true);

                LeanTween.scale(go, Vector3.one, duration).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
                {
                    go.transform.localScale = Vector3.one;  

                }).setEase(ease);

                for (int i = 0; i < vin.inventory.Count; i++)
                {
                    if (vin.inventory[i] == null)
                        continue;

                    if (vin.inventory[i].name == vinventory.name && vin.inventory[i].guid == vinventory.guid)
                    {
                        VPropsManager.lastClickedInventory = vin.inventory[i];
                        imgContainer.sprite = vin.inventory[i].sprite;
                        textContainer.SetText("<b>" + vin.inventory[i].name + "</b>" + "\n" + vin.inventory[i].description);
                        break;
                    }
                }               
            }
        }

        private void SetButtonStates(bool state)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i] == null)
                    continue;

                buttons[i].gameObject.SetActive(state);
            }
        }
    }
}