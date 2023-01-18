
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VelvieR
{
    [System.Serializable]
    public enum VAudioControl
    {
        Play,
        Stop,
        Pause,
        Resume,
        PauseAll,
        ResumeAll,
        PlayOneShot,
        StopAll
    }
    [VTag("Multimedia/PlayStopAudio", "Audio control over an audioSource.", VColor.Pink01, "Vl")]
    public class PlayStopAudio : VBlockCore
    {
        [SerializeField, HideInInspector] public AudioSource audioSource;
        [SerializeField, HideInInspector] public AudioClip audioClip;
        [SerializeField, HideInInspector] public VAudioControl vaudioControl = VAudioControl.Play;
        [SerializeField, HideInInspector] private List<AudioSource> ausources;
        [SerializeField, HideInInspector] public bool loop;

        void Awake()
        {
            if(vaudioControl == VAudioControl.StopAll)
            {
                if(ausources == null)
                {
                    var aus = Resources.FindObjectsOfTypeAll<AudioSource>();
                    ausources = new List<AudioSource>();

                    if(aus.Length > 0)
                    {
                        for(int i = 0; i < aus.Length; i++)
                        {
                            if(aus[i] != null)
                                ausources.Add(aus[i]);
                        }
                    }
                }
            }
        }
        
        public override void OnVEnter()
        {
            if(vaudioControl == VAudioControl.Play)
            {
                audioSource.Play();
            }
            else if(vaudioControl == VAudioControl.Stop)
            {
                audioSource.Stop();
            }
            else if(vaudioControl == VAudioControl.Pause)
            {
                audioSource.Pause();
            }
            else if(vaudioControl == VAudioControl.Resume)
            {
                audioSource.UnPause();
            }
            else if(vaudioControl == VAudioControl.PlayOneShot)
            {
                audioSource.PlayOneShot(audioClip);
            }
            else if(vaudioControl == VAudioControl.PauseAll)
            {
                AudioListener.pause = true;
            }
            else if(vaudioControl == VAudioControl.ResumeAll)
            {
                AudioListener.pause = false;
            }
            else if(vaudioControl == VAudioControl.StopAll)
            {
                if(ausources != null && ausources.Count > 0)
                {
                    for(int i = 0; i < ausources.Count; i++)
                    {
                        if(ausources[i] != null)
                        ausources[i].Stop();
                    }
                }
            }
        }

        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if(audioSource == null)
            {
                if(vaudioControl == VAudioControl.Play || vaudioControl == VAudioControl.Stop || vaudioControl == VAudioControl.Resume || vaudioControl == VAudioControl.Pause || vaudioControl == VAudioControl.PlayOneShot)
                    summary += "AudioSource component can't be empty!";
            }

            return summary;
        }
    }
}