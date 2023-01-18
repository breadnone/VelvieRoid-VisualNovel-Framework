using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VelvieR
{
    public class VDialogueManager : MonoBehaviour
    {
        public List<VelvieDialogue> vdialogue = new List<VelvieDialogue>();
        //execute this on the very first load
        public void PoolVelvieDialogue()
        {
            var vdialogues = Resources.FindObjectsOfTypeAll<VelvieDialogue>();
        }


    }
}