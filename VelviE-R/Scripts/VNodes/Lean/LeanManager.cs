using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VelvieR
{
    public class LeanManager : MonoBehaviour
    {
        public static List<GameObject> ActiveTweens = new List<GameObject>();

        public static void CancelAll(bool onComplete = false)
        {
            if(ActiveTweens.Count > 0)
            {
                for (int i = ActiveTweens.Count; i --> 0; )
                {
                    if(ActiveTweens[i] != null)
                    {
                        LeanTween.cancel(ActiveTweens[i], onComplete);
                        ActiveTweens.RemoveAt(i);
                    }
                }
            }
        }
        public static void Cancel(GameObject obj, bool onComplete = false)
        {
            if(ActiveTweens.Count > 0)
            {
                for (int i = ActiveTweens.Count; i --> 0; )
                {
                    if(ActiveTweens[i] != null && ActiveTweens[i] == obj)
                    {
                        LeanTween.cancel(ActiveTweens[i], onComplete);
                        ActiveTweens.RemoveAt(i);
                    }
                }
            }
        }
        public static void PauseAll()
        {
            if(ActiveTweens.Count > 0)
            {
                for(int i = 0; i < ActiveTweens.Count; i++)
                {
                    if(ActiveTweens[i] != null)
                    {
                        LeanTween.pause(ActiveTweens[i]);
                    }
                }
            }
        }
        public static void Pause(GameObject obj)
        {
            if(ActiveTweens.Count > 0)
            {
                for(int i = 0; i < ActiveTweens.Count; i++)
                {
                    if(ActiveTweens[i] != null && ActiveTweens[i] == obj)
                    {
                        LeanTween.pause(ActiveTweens[i]);
                    }
                }
            }
        }
        public static void ResumeAll()
        {
            if(ActiveTweens.Count > 0)
            {
                for(int i = 0; i < ActiveTweens.Count; i++)
                {
                    if(ActiveTweens[i] != null && ActiveTweens[i].LeanIsPaused())
                    {
                        LeanTween.resume(ActiveTweens[i]);
                    }
                }
            }
        }
        public static void Resume(GameObject obj)
        {
            if(ActiveTweens.Count > 0)
            {
                for(int i = 0; i < ActiveTweens.Count; i++)
                {
                    if(ActiveTweens[i] != null && ActiveTweens[i].LeanIsPaused() && ActiveTweens[i] == obj)
                    {
                        LeanTween.resume(ActiveTweens[i]);
                    }
                }
            }
        }
    }
}