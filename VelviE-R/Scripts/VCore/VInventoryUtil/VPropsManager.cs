using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VelvieR
{
    public static class VPropsManager
    {
        public static List<VInventoryClass> vinventory = new List<VInventoryClass>();
        public static List<VInventoryCategory> vinventorycategory = new List<VInventoryCategory>();
        public static List<VInventoryItemEvent> vevents;
        public static VInventoryCategory lastClickedCategory {get;set;}
        public static VInventoryClass lastClickedInventory{get;set;}
        public static List<VInventory> vInventoryClass{get;set;} = new List<VInventory>();

        public static void InsertInventoryClass(VInventoryClass vinvi)
        {
            if(vinventory == null)
            {
                vinventory = new List<VInventoryClass>();
            }

            if(!vinventory.Contains(vinvi))
            {
                vinventory.Add(vinvi);
            }
        }
        public static void InsertCategory(VInventoryCategory vcat)
        {
            if(vinventorycategory == null)
            {
                vinventorycategory = new List<VInventoryCategory>();
            }

            if(!vinventorycategory.Contains(vcat))
            {
                vinventorycategory.Add(vcat);
            }
        }
        public static void InsertVevent(VInventoryItemEvent vev)
        {
            if(vevents == null)
            {
                vevents = new List<VInventoryItemEvent>();
            }

            if(!vevents.Contains(vev))
            {
                vevents.Add(vev);
            }
        }
        public static int GetVInventoryFieldData(VInventoryClass vclass, string fieldName)
        {
            return VPropsManager.vinventory.Find(x => x.name == vclass.name && x.guid == vclass.guid).GetSlotValue(fieldName);
        }
        public static void SetVInventoryFieldData(VInventoryClass vclass, string fieldName, int val)
        {
            var inv = vinventory.Find(x => x.name == vclass.name && x.guid == vclass.guid);
            Debug.Log(inv);
            inv.SetSlotValue(fieldName, inv.GetSlotValue(fieldName) + val);
        }
        public static void SetCategoryClassification(VInventoryCategory vcat)
        {
            if(vcat != null)
            {
                for(int i = 0; i < vinventory.Count; i++)
                {
                    if(vinventory[i] != null)
                    {
                        if(vinventory[i].inventoryObject != null)
                        {
                            if(!String.IsNullOrEmpty(vinventory[i].name) && vinventory[i].category.name == vcat.name)
                            {
                                vinventory[i].inventoryObject.SetActive(true);
                            }
                            else
                            {
                                vinventory[i].inventoryObject.SetActive(false);
                            }
                        }
                        else
                        {
                            vinventory.RemoveAt(i);
                        }
                    }
                }

                lastClickedCategory = vcat;
            }
        }
        public static VInventoryCategory GetVcategory(VInventoryCategory vcat)
        {
            return vinventorycategory.Find(x => x.name == vcat.name && x.guid == vcat.guid);
        }
    }
}