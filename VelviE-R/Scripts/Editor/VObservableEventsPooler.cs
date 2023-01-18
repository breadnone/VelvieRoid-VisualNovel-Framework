using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEditor;
using VelvieR;

namespace VIEditor
{
    public class VObservableEventsPooler
    {
        public static ObservableCollection<ObservableCollection<VelvieDialogue>> VObserver = new ObservableCollection<ObservableCollection<VelvieDialogue>>();
        public static void VNotifyCollections()
        {
            VObserver.CollectionChanged += (x, y)=> {};
        }
    }
}