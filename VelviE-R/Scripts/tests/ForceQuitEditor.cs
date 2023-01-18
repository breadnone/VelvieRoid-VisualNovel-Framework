using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceQuitEditor : MonoBehaviour
{
    public void ForceQuit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
