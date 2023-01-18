
using UnityEngine;

namespace VelvieR
{
    [VTag("Multimedia/PlayFromTime", "Starts playing the audio from specific point in time.", VColor.Yellow01, "Pt")]
    public class PlayFromTime : VBlockCore
    {
        [SerializeField, HideInInspector] public AudioSource audioSource;
        [SerializeField, HideInInspector] public float start;
        [SerializeField, HideInInspector] public float stop;
        [SerializeField, HideInInspector] public bool useLengthToStop;
        [SerializeField, HideInInspector] public bool loop = false;
        
        public override void OnVEnter()
        {
            audioSource.Stop();

            if(!useLengthToStop)
            {
                audioSource.clip = MakeSubclip(audioSource.clip, start, stop);
            }
            else
            {
                audioSource.clip = MakeSubclip(audioSource.clip, start, audioSource.clip.length);
            }

            audioSource.loop = loop;
            audioSource.Play();
        }
        private AudioClip MakeSubclip(AudioClip clip, float start, float stop)
        {
            /* Create a new audio clip */
            int frequency = clip.frequency;
            float timeLength = stop - start;
            int samplesLength = (int)(frequency * timeLength);
            AudioClip newClip = AudioClip.Create(clip.name + "-sub", samplesLength, 1, frequency, false);
            /* Create a temporary buffer for the samples */
            float[] data = new float[samplesLength];
            /* Get the data from the original clip */
            clip.GetData(data, (int)(frequency * start));
            /* Transfer the data to the new clip */
            newClip.SetData(data, 0);
            /* Return the sub clip */
            return newClip;
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
                summary += "AudioSource component can't be empty!";
            }

            if(audioSource != null && audioSource.clip == null)
            {
                summary += "| AudioClip can't be empty!";
            }
            return summary;
        }
    }
}