using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using VIEditor;
#endif

namespace VelvieR
{
    public class VCharacterUtil : VStageComponent
    {
        [Tooltip("VCharacterV class")]
        [SerializeField] public VCharacterV character;
        [Tooltip("Tied to scriptableObject's id")]
        [SerializeField] public string characterId = string.Empty;
        [SerializeField] public string gameobjectname = string.Empty;
        [SerializeField] public List<AnimThumbnailProps> animatableThumbnail = new List<AnimThumbnailProps>();

        void OnValidate()
        {
            if (gameobjectname != gameObject.name)
            {
                gameobjectname = gameObject.name;
            }
            if (String.IsNullOrEmpty(characterId))
            {
                characterId = Guid.NewGuid().ToString() + (int)UnityEngine.Random.Range(1985, int.MaxValue);
            }
        }

        void Awake()
        {
            if (character != null && character.charaPortrait.Count > 0)
            {
                foreach (var portrait in character.charaPortrait)
                {
                    if (portrait == null || portrait.portraitSprite == null)
                        continue;

                    if (character.is2D)
                    {
                        var go = new GameObject();
                        go.name = character.name + "-" + portrait.portraitSprite.name;

/*
                        go.AddComponent<UIDocument>();
                        var uidoc = go.GetComponent<UIDocument>();
                        uidoc.panelSettings = Resources.Load<PanelSettings>("VProps/VCharacterPanelSettings");
                        portrait.portraitContainer = GenerateImageContainer(portrait.portraitSprite);
                        uidoc.rootVisualElement.Add(portrait.portraitContainer);
                        character.uidoc = uidoc;
*/
                        
                        go.transform.SetParent(transform, false);
                        go.AddComponent<Image>();

                        var img = go.GetComponent<Image>();
                        img.preserveAspect = true;
                        img.sprite = portrait.portraitSprite;

                        if(character.isClickable)
                        {
                            img.raycastTarget = true;
                            go.AddComponent<EventTrigger>();
                            var evttrig = go.GetComponent<EventTrigger>();

                            EventTrigger.Entry onClick = new EventTrigger.Entry();
                            onClick.eventID = EventTriggerType.PointerClick;
                            onClick.callback.AddListener((eventdata) => { Debug.Log("TESTING CLICKABLE"); });
                            evttrig.triggers.Add(onClick);
                        }
                        else
                        {
                            img.raycastTarget = false;
                        }

                        portrait.portraitRect = go.GetComponent<RectTransform>();
                        portrait.portraitObj = go;
                        go.SetActive(false);
                    }
                }
                character.vcharacterUtil = this;
                VCharacterManager.AllCharacters.Add(character);
            }
        }
        void Start()
        {

        }
        //UITK Image container
        private UnityEngine.UIElements.Image GenerateImageContainer(Sprite spr)
        {
            var img = new UnityEngine.UIElements.Image();
            img.sprite = spr;
            return img;
        }
    }
}