using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace VelvieR
{
    [VTag("UIUX/SetCursor", "Sets default cursor to custom cursor icon.\n\nSprite texture mode, must be set as Cursor from import settings.", VColor.Pink01, "St")]
    public class SetCursor : VBlockCore
    {
        [SerializeField, HideInInspector] public Texture2D texture;
        [SerializeField, HideInInspector] public Vector2 hotspot;
        [SerializeField, HideInInspector] public CursorMode cursorMode = CursorMode.Auto;
        [SerializeField, HideInInspector] public bool singleMode = true;
        [SerializeField, HideInInspector] public bool reset = false;
        [SerializeField, HideInInspector] public List<GameObject> gameObjects;
        private bool isOnHover = false;
        public void ResetMouseCursor()
        {
            Cursor.SetCursor(null, Vector2.zero, cursorMode);
            isOnHover = false;
        }

        void Update()
        {
            if(isOnHover)
            {
                if(IsMouseOverUi)
                {
                    Cursor.SetCursor(texture, hotspot, cursorMode);
                }
                else
                {
                    ResetMouseCursor();
                }
            }
        }
        public override void OnVEnter()
        {
            if(reset)
            {
                ResetMouseCursor();
            }
            else
            {
                if(texture != null && singleMode)
                {
                    Cursor.SetCursor(texture, hotspot, cursorMode);
                }
                else if(texture != null && !singleMode)
                {
                    isOnHover = true;
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

            if(!reset)
            {
                if(texture == null)
                {
                    summary += "Sprite must be assigned!";
                }
            }

            return summary;
        }
        public bool IsMouseOverUi
        {
            get
            {
                // [Only works well while there is not PhysicsRaycaster on the Camera)
                //EventSystem eventSystem = EventSystem.current;
                //return (eventSystem != null && eventSystem.IsPointerOverGameObject());
     
                // [Works with PhysicsRaycaster on the Camera. Requires New Input System. Assumes mouse.)
                if (EventSystem.current == null)
                {
                    return false;
                }
                RaycastResult lastRaycastResult = ((InputSystemUIInputModule)EventSystem.current.currentInputModule).GetLastRaycastResult(Mouse.current.deviceId);
                const int uiLayer = 5;

                return lastRaycastResult.gameObject != null && lastRaycastResult.gameObject.layer == uiLayer;
            }
        }
    }
}