using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace VelvieR
{
    [VTag("Flow/JumpToLabel", "Jumps to a Label", VColor.White)]
    public class JumpToLabel : VBlockCore
    {
        [SerializeField] private VCoreUtil vgraph;
        [SerializeField] private string labelName = string.Empty;
        public string JumpsToLabel{get{return labelName;} set{labelName = value;}}
        public VCoreUtil VGraph {get{return vgraph;} set{vgraph = value;}}
        private bool found = false;

        public override void OnVEnter()
        {
            if(!String.IsNullOrEmpty(labelName) && vgraph != null)
            {
                GetVBlockLabel(labelName);
            }            
        }

        public void GetVBlockLabel(string name)
        {
            foreach (var vblock in vgraph.vBlockCores)
            {
                if (vblock.vblocks.Count == 0)
                    continue;

                foreach (var blocks in vblock.vblocks)
                {
                    var vlabel = blocks.attachedComponent.component as JumpLabel;

                    if(vlabel != null && vlabel.Label == name && blocks.enable)
                    {
                        vgraph.vblockmanager.StartExecutingVBlock(vblock.vblocks, true, blocks);
                        found = true;
                        return;
                    }
                }
            }
        }

        public override void OnVExit()
        {
            if(!found)
            {
                OnVContinue();
            }
        }
    }
}