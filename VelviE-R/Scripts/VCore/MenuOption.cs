using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

namespace VelvieR
{
    public class MenuOption : MonoBehaviour
    {
        private static List<VMenuOption> AllMenus = new List<VMenuOption>();
        private static List<VMenuOption> ActiveMenus = new List<VMenuOption>();
        private static VMenuOption defaultMenu;
        public static VMenuOption DefaultMenu {get{return defaultMenu;}}

        public static void InsertMenuOptions(VMenuOption vmenu)
        {
            if(vmenu != null && !AllMenus.Contains(vmenu))
            {
                AllMenus.Add(vmenu);
            }
        }
        public static void SetDefaultMenu(VMenuOption vmenu)
        {
            defaultMenu = vmenu;
        }
        public static List<VMenuOption> GetActiveMenus()
        {
            return ActiveMenus;
        }
        public static List<VMenuOption> GetAllMenus()
        {
            return AllMenus;
        }
        public static VMenuOption FindMenu(string menuName = null, VMenuOption vmenu = null)
        {
            if(!String.IsNullOrEmpty(menuName))
            {
                return AllMenus.Find(x => x.VmenuName == menuName);
            }
            else if(vmenu != null)
            {
                return AllMenus.Find(x => x == vmenu);
            }

            return null;
        }
        public static void AddRemoveToActiveMenu(VMenuOption vmenu , bool insert = true)
        {
            if(vmenu == null)
                return;
                
            if(insert)
            {
                if(!ActiveMenus.Contains(vmenu))
                {
                    ActiveMenus.Add(vmenu);
                }
            }
            else
            {
                if(ActiveMenus.Contains(vmenu))
                {
                    ActiveMenus.Remove(vmenu);
                }
            }
        }
    }
}