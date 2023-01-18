using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace VelvieR
{
    [System.Serializable]
    public enum VInventoryEventType
    {
        Add,
        Subtract
    }
    public class InventoryEvents : MonoBehaviour
    {
        [SerializeField] public string slotName;
        [SerializeField] public VInventoryEventType mode = VInventoryEventType.Add;
        [SerializeField] public int value = 1;
        [SerializeField, HideInInspector] public Button btn;
        [SerializeField, HideInInspector] public OnEnableDisable confirmEvent;
        void OnValidate()
        {
            if(btn == null)
            {
                btn = GetComponent<Button>();
            }
        }
        public void SetValue()
        {
            var vinv = VPropsManager.lastClickedInventory;

            if(String.IsNullOrEmpty(slotName))
            {
                slotName = vinv.slots[0].name;
            }

            if(mode == VInventoryEventType.Add)
            {
                VPropsManager.SetVInventoryFieldData(vinv, slotName, value);
            }
            else
            {
                VPropsManager.SetVInventoryFieldData(vinv, slotName, -value);
            }

            confirmEvent.EnableToggle();
        }
        public void OpenMenu()
        {
            confirmEvent.EnableToggle();
            confirmEvent.AddButtonAction(()=> SetValue());
        }
    }
}