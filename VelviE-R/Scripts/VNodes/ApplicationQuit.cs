using UnityEngine;

namespace VelvieR
{
    [VTag("Events/ApplicationQuit", "Quit application.", VColor.Red, "Rp")]
    public class ApplicationQuit : VBlockCore
    {
        public override void OnVEnter()
        {
            #if UNITY_EDITOR
                // Application.Quit() does not work in the editor so
                // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}