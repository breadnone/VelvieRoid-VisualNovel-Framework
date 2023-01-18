using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using VelvieR;
using VIEditor;

[System.Serializable]
public class VGraphsContainer : ScriptableObject
{
    public List<VPorts> vports = new List<VPorts>();
    public GraphStates graphState;
    public string vgraphGOname;
    public int govcoreid = 0;
    public string path;
    public int entityIndex = 0;
    public string sceneGuid;
}

