
using System.Collections.Generic;
using System;

namespace VelvieR
{
    [System.Serializable]
    public enum VItemRarity
    {
        OnlyOne,
        ExtraRare,
        VeryRare,
        Rare,
        Uncommon,
        Common,
        VeryCommon
    }
    [System.Serializable]
    public abstract class AVStat<T>
    {
        [System.Serializable]
        public class AVSlot
        {
            public string name;
            public string description;
            public T value;
            public int id;
        }
        public string name;
        public string description;
        public string summary;
        public int used;
        public VItemRarity rarity = VItemRarity.Common;
        public bool isMixable = false;
        public List<AVSlot> slots = new List<AVSlot>();
        public string lastTimeUsed;
        public bool isActive = true;
        public int guid;
        public int priority = 0;
        public T GetSlotValue(string sname)
        {
            if(String.IsNullOrEmpty(sname))
            {
                throw new Exception("VError : Can't return default(T)! Make sure the slot exists!");
            }
                
            for (int i = 0; i < slots.Count; i++)
            {
                if(String.IsNullOrEmpty(slots[i].name))
                    continue;

                if (slots[i].name == sname)
                {
                    return slots[i].value;
                }
            }

            throw new Exception("VError : Can't return default(T)! Make sure the slot exists!");
        }
        public void SetSlotValue(string sname, T val)
        {
            if(String.IsNullOrEmpty(sname))
                return;

            for (int i = 0; i < slots.Count; i++)
            {
                if(String.IsNullOrEmpty(slots[i].name))
                    continue;

                if (slots[i].name == sname)
                {
                    slots[i].value = val;
                    break;
                }
            }
        }
        public void CreateSlot(string sname, string sdesc = "", T value = default(T))
        {
            if(String.IsNullOrEmpty(sname))
                return;

            var av = new AVSlot();
            av.name = sname;
            av.description = sdesc;
            av.value = value;
            av.id = (int)UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }
        public void RemoveSlot(string sname)
        {
            if(String.IsNullOrEmpty(sname))
                return;

            for(int i = 0; i < slots.Count; i++)
            {
                if(slots[i].name == sname)
                {
                    slots.RemoveAt(i);
                    break;
                }
            }
        }
        public void LogTimeUsed()
        {
            lastTimeUsed = DateTime.Now.ToString();
        }
        public virtual void ResetAllVal()
        {
            if (slots.Count > 0)
            {
                for (int i = 0; i < slots.Count; i++)
                {
                    slots[i].value = default(T);
                }
            }
        }
    }
}
