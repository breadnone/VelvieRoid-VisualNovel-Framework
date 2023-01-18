using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VelvieR
{
    public class VAudioManager
    {
        public static List<VAudioClass> audios = new List<VAudioClass>();
        public static Queue<VAudioClass> audioQueues = new Queue<VAudioClass>();
        public static void InsertVAudio(VAudioClass vauclass)
        {
            if (vauclass != null && !audios.Contains(vauclass))
            {
                audios.Add(vauclass);
            }
        }

        public static AudioSource GetAudioSource(string name)
        {
            return audios.Find(x => x.name == name).audioSource;
        }
        public static VAudioClass GetVAudioClass(string name)
        {
            return audios.Find(x => x.name == name);
        }

        public static void GlobalVolume(float vals)
        {
            for (int i = 0; i < audios.Count; i++)
            {
                audios[i].audioSource.volume = vals;
                audios[i].audioSource.volume = vals;
            }
        }
        public static void SetVolume(float vals, string name)
        {
            for (int i = 0; i < audios.Count; i++)
            {
                if (audios[i].name == name)
                {
                    audios[i].audioSource.volume = vals;
                    break;
                }
            }
        }
        public static void MuteAll(bool mute)
        {
            for (int i = 0; i < audios.Count; i++)
            {
                audios[i].audioSource.mute = mute;
            }
        }
        public static void Mute(bool mute, string name)
        {
            for (int i = 0; i < audios.Count; i++)
            {
                if (audios[i].name == name)
                {
                    audios[i].audioSource.mute = mute;
                    break;
                }
            }
        }
        public static void GlobalPitch(float vals)
        {
            for (int i = 0; i < audios.Count; i++)
            {
                audios[i].audioSource.pitch = vals;
                audios[i].pitchLevel = vals;
            }
        }
        public static void PlayStopAllAudios(bool play = true)
        {
            for (int i = 0; i < audios.Count; i++)
            {
                if (!play)
                {
                    if (audios[i].audioSource.isPlaying)
                        audios[i].audioSource.Stop();
                }
                else
                {
                    if (!audios[i].audioSource.isPlaying)
                        audios[i].audioSource.Play();
                }
            }
        }
        public static void PlayStopAudio(bool play, string name)
        {
            for (int i = 0; i < audios.Count; i++)
            {
                if (audios[i].name == name)
                {
                    if (!play)
                        audios[i].audioSource.Stop();
                    else
                        audios[i].audioSource.Play();
                    break;
                }
            }
        }
        public static void PauseAllAudio(bool pause, string name)
        {
            for (int i = 0; i < audios.Count; i++)
            {
                if (!pause)
                    audios[i].audioSource.Play();
                else
                    audios[i].audioSource.Pause();
            }
        }
        public static void PauseAudio(bool pause, string name)
        {
            for (int i = 0; i < audios.Count; i++)
            {
                if (audios[i].name == name)
                {
                    if (!pause)
                        audios[i].audioSource.Play();
                    else
                        audios[i].audioSource.Pause();

                    break;
                }
            }
        }
        public static void ResetSettingsToDefault()
        {
            for (int i = 0; i < audios.Count; i++)
            {
                audios[i].volume = 1f;
                audios[i].stereoPanLevel = 0f;
                audios[i].pitchLevel = 1f;

                audios[i].audioSource.volume = 1f;
                audios[i].audioSource.panStereo = 0f;
                audios[i].audioSource.pitch = 1f;
            }
        }
    }
}