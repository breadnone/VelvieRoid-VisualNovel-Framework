using UnityEngine;
using System;

namespace VelvieR
{
    [System.Serializable]
    public enum ThumbnailEffects
    {
        Punch,
        RoundAndRound,
        BlinkAlpha,
        PlayAnimatableProps,
        None
    }
    [VTag("Flow/Say", "Displays character's dialogue.", VColor.Magenta, "S")]
    public class SayWord : VBlockCore
    {
        [SerializeField] private AnimThumbnailProps animatableThumbnailProp;
        [SerializeField] private float magnitude = 0.2f;
        [SerializeField] private int loopCount = 2;
        [SerializeField] private VCharacterV vcharacter;
        [SerializeField] private PortraitProps portrait;
        [SerializeField] private PortraitProps thumbnail;
        [SerializeField] private AudioClip characterSound;
        [SerializeField] private float characterSoundVolume = 0.5f;
        [SerializeField] private string words = string.Empty;
        [SerializeField] private float enderDelay = 3f;
        [SerializeField] private VelvieDialogue vdialogue;
        [SerializeField] private AudioClip playAudioClip;
        [SerializeField] private WaitForClick waitForClick = WaitForClick.Enable;
        [SerializeField] private bool loopCharacterAudio = false;
        [SerializeField] private int frameRate = 3;
        [SerializeField] private bool humanLikePause;
        [SerializeField] private string customSymbols = string.Empty;
        //Rounded number is a must here due to the api doesn't support floating point
        [SerializeField] public int recDurationForEditor = 5;
        public string CustomSymbols { get { return customSymbols; } set { customSymbols = value; } }
        public bool HumanLikePause { get { return humanLikePause; } set { humanLikePause = value; } }
        public int FrameRate { get { return frameRate; } set { frameRate = value; } }
        public AnimThumbnailProps AnimatableThumbnailProp { get { return animatableThumbnailProp; } set { animatableThumbnailProp = value; } }
        public float Magnitude { get { return magnitude; } set { magnitude = value; } }
        public PortraitProps Portrait { get { return portrait; } set { portrait = value; } }
        public PortraitProps Thumbnail { get { return thumbnail; } set { thumbnail = value; } }
        public virtual string Words { get { return words; } set { words = value; } }
        public virtual AudioClip PlayAudioClip { get { return playAudioClip; } set { playAudioClip = value; } }
        public virtual VCharacterV Character { get { return vcharacter; } set { vcharacter = value; } }
        public virtual WaitForClick WaitForClick { get { return waitForClick; } set { waitForClick = value; } }
        public virtual AudioClip CharacterSound { get { return characterSound; } set { characterSound = value; } }
        public virtual bool LoopCharacterAudio { get { return loopCharacterAudio; } set { loopCharacterAudio = value; } }
        public virtual float CharacterSoundVolume { get { return characterSoundVolume; } set { characterSoundVolume = value; } }
        public virtual VelvieDialogue VDialogue { get { return vdialogue; } set { vdialogue = value; } }
        public virtual float EnderDelay { get { return enderDelay; } set { enderDelay = value; } }
        [SerializeField, HideInInspector] private ThumbnailEffects effectThm = ThumbnailEffects.None;
        public ThumbnailEffects EffectThm { get { return effectThm; } set { effectThm = value; } }
        public int LoopCount { get { return loopCount; } set { loopCount = value; } }

        public override void OnVEnter()
        {
            if (vdialogue == null)
            {
                if(VBlockManager.DefaultDialog == null)
                {
                    throw new Exception("VError: VDialogue has not been assigned or none cannot be found in the scene.");
                }
                else
                {
                    vdialogue = VBlockManager.DefaultDialog;
                }
            }

            VBlockManager.DefaultDialog = vdialogue;
            vdialogue.thisIndex = thisIndex;

            if (waitForClick == WaitForClick.Disable)
            {
                vdialogue.EnderDelay = enderDelay;
            }

            if (characterSound != null)
            {
                vdialogue.audioSource.audioSource.loop = loopCharacterAudio;
                vdialogue.audioSource.audioSource.clip = characterSound;
                vdialogue.audioSource.audioSource.Play();
            }

            if (playAudioClip != null)
            {
                vdialogue.audioSourceCommon.audioSource.clip = playAudioClip;
                vdialogue.audioSourceCommon.audioSource.Play();
            }

            if (vcharacter != null)
            {
                //TODO move thumbnail system into VelvieDialogue class!
                bool noThumbnail = false;

                if (effectThm != ThumbnailEffects.PlayAnimatableProps)
                {
                    if (thumbnail != null && thumbnail.portraitSprite != null)
                    {
                        noThumbnail = true;
                        vdialogue.SetPortraitThumbnail(vcharacter, vdialogue.thmRect, thumbnail.portraitSprite, vdialogue.ThumbnailDuration, true, true, false, this);
                    }
                }
                else if (effectThm != ThumbnailEffects.None)
                {
                    if (animatableThumbnailProp != null && !String.IsNullOrEmpty(animatableThumbnailProp.name))
                    {
                        noThumbnail = true;
                        vdialogue.SetPortraitThumbnail(vcharacter, vdialogue.thmRect, null, vdialogue.ThumbnailDuration, true, false, true, this);
                    }
                }

                if (!noThumbnail)
                {
                    vdialogue.thmRect.gameObject.SetActive(false);
                }

                if (portrait != null && portrait.portraitSprite != null)
                {
                    VCharacterManager.ShowVPortrait(VStageManager.GetActiveStage(true), vdialogue.SpritesDuration, vcharacter, portrait, false, null);
                }
            }

            vdialogue.StartWriting(() => OnVContinue(), words, waitForClick, vmanager, vcharacter);
        }

        public override string OnVSummary()
        {
            var summary = string.Empty;

            if (vdialogue == null)
            {
                return summary = "VDialogue must be set!";
            }

            return summary;
        }
    }
}