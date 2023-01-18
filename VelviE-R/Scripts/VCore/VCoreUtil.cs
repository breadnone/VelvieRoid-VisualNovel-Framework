using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

#if UNITY_EDITOR
using VIEditor;
using UnityEditor;
#endif

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Events;

namespace VelvieR
{
    public class VCoreUtil : MonoBehaviour
    {
        //All velvieblocks/nodes attached to the gameObject in the hierarchy go here!
        [SerializeField, HideInInspector] public bool HideAllComponents = true;
        [SerializeField, HideInInspector] public List<VBlockComponent> vBlockCores = new List<VBlockComponent>();
        [SerializeField, HideInInspector] public List<VelvieDialogue> vdialogues = new List<VelvieDialogue>();
        [SerializeField, HideInInspector] public VBlockManager vblockmanager;
        [SerializeField, HideInInspector] public VInputBuffer VIo;
        [SerializeField, HideInInspector] public List<VStageClass> vstage = new List<VStageClass>();

        void Awake()
        {
            /* TESTING ONLY!
            if(vvariable.vstring != null && vvariable.vstring.Count > 0)
            {
                foreach(var e in vvariable.vstring)
                {
                    Debug.Log("Name : " + e.name + "Value : " + e.value);
                }
            }
            
            int counter = 0;
            foreach(var r in vvariable.vrlist)
            {
                Debug.Log(r.value);
                r.value.Add("Testing");
                Debug.Log( r.value[0] + " :: " + counter++);
            }
            */
        }

        #if UNITY_EDITOR
        void OnValidate()
        {
            if (vcoreid == 0)
            {
                System.Random rand = new System.Random();
                vcoreid = this.GetInstanceID() + rand.Next();
                vcorename = this.name;

                if (!VBlockManager.vcoreutils.Contains(this))
                    VBlockManager.vcoreutils.Add(this);
            }
        }
        public void HideAllVCoreComponents(bool state)
        {
            var components = GetComponents(typeof(Component));

            foreach (var coms in components)
            {
                if (coms is not VCoreUtil)
                {
                    if (state && coms.hideFlags == HideFlags.None)
                    {
                        coms.hideFlags = HideFlags.HideInInspector;
                    }
                    else if (!state && coms.hideFlags == HideFlags.HideInInspector)
                    {
                        coms.hideFlags = HideFlags.None;
                    }
                }
            }
        }
        #endif
        void Start()
        {
            var blockManager = new VBlockManager();
            vblockmanager = blockManager;

            List<VelvieBlockComponent> vblockCache = new List<VelvieBlockComponent>();
            ReIterate();

            void ReIterate()
            {
                foreach (var vblock in vBlockCores)
                {
                    if (vblock.vblocks.Count == 0)
                        continue;

                    foreach (var blocks in vblock.vblocks)
                    {
                        if (!blocks.isInGameStartedBlock)
                            continue;

                        vblockCache = vblock.vblocks;
                        return;
                    }
                }
            }

            if (vblockCache.Count > 0)
                blockManager.StartExecutingVBlock(vblockCache);
        }
        public void OnCall(string vnodeId)
        {
            if (vblockmanager == null)
            {
                var blockManager = new VBlockManager();
                vblockmanager = blockManager;
            }

            List<VelvieBlockComponent> vblockCache = new List<VelvieBlockComponent>();
            VelvieBlockComponent vcom = null;
            ReIterate();

            void ReIterate()
            {
                foreach (var vblock in vBlockCores)
                {
                    if (vblock.vblocks.Count == 0)
                        continue;

                    for (int i = 0; i < vblock.vblocks.Count; i++)
                    {
                        var block = vblock.vblocks[i];

                        if (block.vnodeId != vnodeId)
                            continue;

                        vblockCache = new List<VelvieBlockComponent>(vblock.vblocks);
                        vcom = block;
                        return;
                    }
                }
            }

            if (vblockCache.Count > 0 && vcom != null)
            {
                vblockmanager.StartExecutingVBlock(vblockCache, true, vcom);
            }
        }

        public SayWord ThisIndexIsSayDialog(string vnodeId)
        {
            foreach (var vblock in vBlockCores)
            {
                if (vblock.vblocks.Count == 0)
                    continue;

                for (int i = 0; i < vblock.vblocks.Count; i++)
                {
                    var block = vblock.vblocks[i];

                    if (block.vnodeId != vnodeId)
                        continue;

                    if (block.attachedComponent.component is SayWord vdial)
                    {
                        return vdial;
                    }
                }
            }

            return null;
        }

        [SerializeField, HideInInspector] public int vcoreid = 0;
        [SerializeField, HideInInspector] public string vcorename = string.Empty;
        #if UNITY_EDITOR
        public void ResetVCore()
        {
            vcorename = string.Empty;
            vcoreid = 0;
            OnValidate();

        }
        #endif
    }
}