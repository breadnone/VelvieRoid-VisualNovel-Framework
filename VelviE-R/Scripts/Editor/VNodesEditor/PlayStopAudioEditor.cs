using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using UnityEngine;
using System.Linq;
using System;

namespace VIEditor
{
    [CustomEditor(typeof(PlayStopAudio))]

    public class PlayStopAudioEditor : Editor
    {
        private VisualElement dummy;
        private VisualElement dummyTwo;
        private VisualElement dummyThree;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as PlayStopAudio;
            dummy = new VisualElement();
            dummyTwo = new VisualElement();
            dummyThree = new VisualElement();

            root.Add(DrawControl(t));
            root.Add(dummy);
            dummy.Add(DrawAu(t));
            dummy.Add(dummyTwo);
            dummy.Add(dummyThree);
            dummyTwo.Add(DrawAuClip(t));
            dummyThree.Add(DrawLoop(t));

            if (t.vaudioControl == VAudioControl.Play || t.vaudioControl == VAudioControl.Stop || t.vaudioControl == VAudioControl.Pause || t.vaudioControl == VAudioControl.Resume || t.vaudioControl == VAudioControl.PlayOneShot)
            {
                dummy.SetEnabled(true);

                if (t.vaudioControl == VAudioControl.PlayOneShot)
                {
                    dummyTwo.SetEnabled(true);
                }
                else
                {
                    dummyTwo.SetEnabled(false);
                }

                if (t.vaudioControl == VAudioControl.PlayOneShot || t.vaudioControl == VAudioControl.Play)
                {
                    dummyThree.SetEnabled(true);
                }
                else
                {
                    dummyThree.SetEnabled(false);
                }
            }
            else
            {
                dummy.SetEnabled(false);
                dummyTwo.SetEnabled(false);
                dummyThree.SetEnabled(false);
            }

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, t.OnVSummary);
            return root;
        }
        private VisualElement DrawAu(PlayStopAudio t)
        {
            var rootBox = VUITemplate.GetTemplate("AudioSource : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new ObjectField();
            objField.objectType = typeof(AudioSource);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.audioSource;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.audioSource = objField.value as AudioSource;
                });
            }

            return rootBox;
        }
        private VisualElement DrawAuClip(PlayStopAudio t)
        {
            var rootBox = VUITemplate.GetTemplate("AudioClip : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new ObjectField();
            objField.objectType = typeof(AudioClip);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.audioClip;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.audioClip = objField.value as AudioClip;
                });
            }

            return rootBox;
        }
        private VisualElement DrawControl(PlayStopAudio t)
        {
            var rootBox = VUITemplate.GetTemplate("Control : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.vaudioControl.ToString();

            if (!PortsUtils.PlayMode)
            {
                objField.choices = Enum.GetNames(typeof(VAudioControl)).ToList();
                objField.RegisterCallback<ChangeEvent<string>>((x) =>
                {
                    foreach (var nnm in Enum.GetValues(typeof(VAudioControl)))
                    {
                        var astype = (VAudioControl)nnm;

                        if (astype.ToString() == x.newValue)
                        {
                            if (astype == VAudioControl.Play || astype == VAudioControl.Stop || astype == VAudioControl.Pause || astype == VAudioControl.Resume || astype == VAudioControl.PlayOneShot)
                            {
                                dummy.SetEnabled(true);

                                if (t.vaudioControl == VAudioControl.PlayOneShot)
                                {
                                    dummyTwo.SetEnabled(true);
                                }
                                else
                                {
                                    dummyTwo.SetEnabled(false);
                                }
                            }
                            else
                            {
                                dummy.SetEnabled(false);
                                dummyTwo.SetEnabled(false);
                            }

                            t.vaudioControl = astype;

                            if (t.vaudioControl == VAudioControl.PlayOneShot || t.vaudioControl == VAudioControl.Play)
                            {
                                dummyThree.SetEnabled(true);
                            }
                            else
                            {
                                dummyThree.SetEnabled(false);
                            }

                            break;
                        }
                    }
                });
            }

            return rootBox;
        }
        private VisualElement DrawLoop(PlayStopAudio t)
        {
            var rootBox = VUITemplate.GetTemplate("Loop : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.loop;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.loop = objField.value;
                });
            }

            return rootBox;
        }
    }
}