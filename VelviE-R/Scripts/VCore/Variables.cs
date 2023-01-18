using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace VelvieR
{
    public class Variables : ScriptableObject
    {
        [SerializeReference] public List<IVar> ivar = new List<IVar>();
        public List<BackgroundScheduler> scheduler = new List<BackgroundScheduler>();
        public int GlobalCounter = 1985;
        public string sceneGuid;

        void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}