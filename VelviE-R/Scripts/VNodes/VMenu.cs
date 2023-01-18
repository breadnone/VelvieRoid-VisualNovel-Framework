using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Threading;
using VTasks;

namespace VelvieR
{
    [System.Serializable]
    public class VMenuPools
    {
        public VCoreUtil vgraph;
        public string menuText;
        public string vnode;
        public string jumpToLabel;
        public string id;
        public string vnodeId;
        public bool exclude = false;
        public int counter = 0;
    }

    [VTag("Flow/Menu", "Selectable menus for branching in/out of vblocks.", VColor.White, "Mu")]
    public class VMenu : VBlockCore
    {
        [SerializeField] private VMenuOption menu;
        [SerializeField] private string mainText;
        [SerializeField] bool continueThisBlock = false;
        [SerializeField] private List<VMenuPools> vmenuPools = new List<VMenuPools>();
        [SerializeField] private bool random = false;
        public bool Randomize{get{return random;} set{random = value;}}
        public VMenuOption Vmenu {get{return menu;} set{menu = value;}}
        public List<VMenuPools> VmenuPools {get{return vmenuPools;} set{vmenuPools = value;}}
        public string MainText {get{return mainText;} set{mainText = value;}}
        public bool ContinueThisBlock {get{return continueThisBlock;} set{continueThisBlock = value;}}
        public override void OnVEnter()
        {
            if(vmenuPools != null)
            {
                if(menu == null && MenuOption.DefaultMenu == null)
                {
                    throw new Exception("VError : Menu can't be empty! If none in the scene, create a new one from VGraph menu.");
                }

                if(vmenuPools.FindAll(x => x.vgraph == null || String.IsNullOrEmpty(x.vnode)).Count == vmenuPools.Count)
                {
                    throw new Exception("VError : Required properties for the menu were left empty! VGraph and VNode slots can't be empty!");
                }

                VMenuOption currentMenu = null;

                if(menu == null)
                {
                    currentMenu = MenuOption.DefaultMenu;
                }
                else
                {
                    //Fallback to default, if any.
                    currentMenu = menu;
                    MenuOption.SetDefaultMenu(menu);
                }

                if(continueThisBlock)
                {
                    menu.PrepMenu(currentMenu, vmenuPools, mainText, ()=> OnVContinue());
                }
                else
                {
                    menu.PrepMenu(currentMenu, vmenuPools, mainText, null);
                }
            }
        }
    }
}