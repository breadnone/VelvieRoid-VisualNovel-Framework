using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VelvieR;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;

namespace VIEditor
{
    [System.Serializable]
    public class GraphStates
    {
        public bool InspectorBtnStates = false;
        public bool miniMapActiveState = false;
        public Color entityBackgroundColor;
        public StyleColor entityDefaultBackgroundColor;
        public string graphStateId;
        public List<Group> inGroup = new List<Group>();
    }

    [System.Serializable]
    public class VPortsInstance
    {
        public VNodeProperty vnodeProperty;
        public List<string> connectedTo = new List<string>();
        public string vParentNodeId;
        public string vportInstanceGuid;
    }
    [System.Serializable]
    public class VNodeProperty
    {
        public string nodeName;
        public string nodeTitle;
        public Rect nodePosition;
        public StyleColor nodeColor; 
        public string nodeId;
        public EnableState isGameStarted = EnableState.None;
    }
    [System.Serializable]
    public class VPorts
    {
        public string guid;
        public VPortsInstance vport;
    }
}