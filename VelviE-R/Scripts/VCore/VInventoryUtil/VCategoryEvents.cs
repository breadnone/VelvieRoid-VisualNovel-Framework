using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace VelvieR
{
    public class VCategoryEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField, HideInInspector] public VInventoryCategory vcategory;
        [SerializeField] public string subHeaderText = "Category : ";
        [SerializeField] public Color onEnterColor = Color.blue;
        [SerializeField] public Image imgContainer;
        [SerializeField] public TMP_Text text;
        [SerializeField] public VInventory vin;

        void OnValidate()
        {
            if(text == null)
                text = GetComponentInChildren<TMP_Text>();

            if(imgContainer == null)
                imgContainer = GetComponent<Image>();
        } 
        void OnEnable()
        {
            transform.localScale = Vector3.one;
            imgContainer.color = Color.white;
            
        }
        void OnDisable()
        {
            transform.localScale = Vector3.one;
            imgContainer.color = Color.white;
        }
        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            imgContainer.color = onEnterColor;
        }

        public void OnPointerExit(PointerEventData pointerEventData)
        {
            transform.localScale = Vector3.one;
            imgContainer.color = Color.white;
        }
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            VPropsManager.SetCategoryClassification(vcategory);
            vin.subHeaderText.SetText(vcategory.name);
        }
        public void SetCategoryTitleAndSprite(string str, Sprite sprite)
        {
            var vcat = VPropsManager.GetVcategory(vcategory);
            imgContainer.sprite = vcat.sprite;
            text.SetText(subHeaderText + vcat.name);
        }
    }
}