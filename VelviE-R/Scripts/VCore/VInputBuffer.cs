using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Linq;
using VTasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VelvieR
{
    public static class CheckFloat
    {
        public static bool FloatIs(this float float1, float float2, float precision)
        {
            return (Mathf.Abs(float1 - float2) <= precision);
        }
    }

    public class VInputBuffer : MonoBehaviour
    {
        [SerializeField] private float cooldownDelayInSeconds = 0.05f;
        [SerializeField] private KeyCode keyboardTriggerKey = KeyCode.Space;
        [SerializeField] private GamepadButton gamepadTriggerKey = GamepadButton.A;
        [SerializeField] private MouseButton mouseTriggerKey = MouseButton.Left;
        [SerializeField] private bool enableKeyboardStringOutput;
        [SerializeField] private bool lowerCaseOutput = false;
        [SerializeField] private bool letterKeysOnly = false;
        [HideInInspector] public List<VBindKey> inputWrapper = new List<VBindKey>();
        public bool EnableKeyboardIo { get { return enableKeyboardStringOutput; } set { enableKeyboardStringOutput = value; } }
        public MouseButton MouseTrigger { get { return mouseTriggerKey; } set { mouseTriggerKey = value; } }
        public KeyCode KeyboardTrigger { get { return keyboardTriggerKey; } set { keyboardTriggerKey = value; } }
        public GamepadButton GamepadTrigger { get { return gamepadTriggerKey; } set { gamepadTriggerKey = value; } }
        public float Delay { get { return cooldownDelayInSeconds; } set { cooldownDelayInSeconds = value; } }
        private List<InputDevice> devices = new List<InputDevice>();
        private float prevTick = 0f;
        public Func<string> VKeyboardOutPutString { get; set; }
        public bool StringOutputToLower { get { return lowerCaseOutput; } set { lowerCaseOutput = value; } }
        public bool LetterKeysOnly { get { return letterKeysOnly; } set { letterKeysOnly = value; } }
        private Dictionary<string, UnityEvent> VKBkeys = new Dictionary<string, UnityEvent>();
        private Dictionary<string, UnityEvent> VMSkeys = new Dictionary<string, UnityEvent>();
        private Dictionary<string, UnityEvent> VGPkeys = new Dictionary<string, UnityEvent>();
        public void InitPooling()
        {
            if(inputWrapper.Count > 0)
            {
                foreach(var kb in inputWrapper)
                {
                    if(kb == null)
                        continue;

                    if(kb.isKeyboard)
                    {
                        var str = kb.keyboardBindKeyCode.ToString().Replace("KeyCode.", "");
                        VKBkeys.Add(str, kb.action);
                    }
                    else if(kb.isMouse)
                    {
                        var str = kb.mouseBindKeyCode.ToString();

                        if(str.Equals("Left", StringComparison.OrdinalIgnoreCase))
                            VMSkeys.Add(str, kb.action);
                        else if(str.Equals("Right", StringComparison.OrdinalIgnoreCase))
                            VMSkeys.Add(str, kb.action);
                        else if(str.Equals("Middle", StringComparison.OrdinalIgnoreCase))                        
                            VMSkeys.Add(str, kb.action);                        
                    }
                    else if(kb.isGamepad)
                    {
                        var str = kb.gamepadBindKeyCode.ToString().Replace("GamepadButton.", "");
                        VGPkeys.Add(str, kb.action);                        
                    }
                }
            }
            
            var inputDevices = InputSystem.devices;

            foreach (var input in inputDevices)
            {
                devices.Add(input);
            }
        }
        void OnDestroyed()
        {
            if (VBlockManager.ActiveInput == this)
            {
                VTokenManager.CancelAllVTokens();
            }
        }
        //Unmanaged territory, careful!
        //Refreain from implementing your own function unless you really had to. Use built-ins instead
        private void VEventInput(InputEventPtr eventPtr, InputDevice device)
        {
            if (prevTick + Delay > Time.realtimeSinceStartup)
            {
                return;
            }

            prevTick = Time.realtimeSinceStartup;

            if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
            {
                return;
            }
               
            for (int i = 0; i < devices.Count; i++)
            {
                if (devices[i] == device)
                {
                    if (device is Keyboard vkeyboard)
                    {
                        //TODO : eventPtr.GetAllBUttonPresses();
                        // and detect button hold states!
                        KeyboardAction(vkeyboard, eventPtr.GetAllButtonPresses());
                    }
                    else if (device is Mouse vmouse)
                    {
                        MouseAction(vmouse);
                    }
                    else if (device is Gamepad vpad)
                    {
                        //TODO : eventPtr.GetAllBUttonPresses();
                        // and detect button hold states!
                        GamepadAction(vpad, eventPtr.GetAllButtonPresses());
                    }
                }
            }
        }
        public void SubscribeVEvents(bool subscribe = true)
        {
            //this is useless!
            if (subscribe)
            {
                InputSystem.onEvent += (x, y) => VEventInput(x, y);
            }


            //TODO: This is much SAFER way! For future reference
            /*
            var t = InputSystem.onEvent.Where(e => e.HasButtonPress()).CallOnce(eventPtr =>
            {
                foreach (var button in eventPtr.GetAllButtonPresses())
                    Debug.Log($"Button {button} was pressed");
            });
            */
        }
        void OnEnable()
        {
            GameViewRes = Handles.GetMainGameViewSize();
        }
        void OnDisable()
        {
            SubscribeVEvents(false);
        }
        private Vector2 GameViewRes;
        public void VPoolInput(bool ignoreClick = false)
        {
            if (!ignoreClick)
            {
                if (VBlockManager.ActiveDialogue.Count > 0)
                {
                    for (int i = 0; i < VBlockManager.ActiveDialogue.Count; i++)
                    {
                        var vdial = VBlockManager.ActiveDialogue[i];

                        if(vdial == null)
                            continue;

                        if (vdial.vmanager.IsPaused)
                            continue;

                        if(vdial.ClickTarget == ClickOnDialogue.ClickAnywhere)
                        {
                            vdial.ExecNext();
                        }
                    }
                }
            }
        }
        #if UNITY_EDITOR
        private Camera mainCam;
        #endif

        private void MouseAction(Mouse vmouse)
        {
            #if UNITY_EDITOR
            //Prevents unwanted behavior when user stopping the game while the dialogue 
            //is still playing in the background
            if(mainCam == null)
            {
                mainCam = Camera.main;
            }

            if(mainCam != null)
            {
                var view = mainCam.ScreenToViewportPoint(Mouse.current.position.ReadValue());
                var outsideGameView = view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1;

                if(outsideGameView)
                {
                    return;
                }
            }
            #endif

            if (vmouse.leftButton.isPressed && mouseTriggerKey == MouseButton.Left)
            {
                VPoolInput();
            }
            else if (vmouse.rightButton.isPressed && mouseTriggerKey == MouseButton.Right)
            {
                VPoolInput();
            }
            else if (vmouse.middleButton.isPressed && mouseTriggerKey == MouseButton.Middle)
            {
                VPoolInput();
            }
        }
        //private IEnumerable<InputControl> prevKeyboardInputs = new InputControl[]{};
        private void KeyboardAction(Keyboard vkeyboard, IEnumerable<InputControl> ict, bool lastButtonPressedIsPriority = false)
        {
            if (ict == null)
                return;

            string key = string.Empty;

            foreach(var inputButton in ict)
            {
                if(inputButton == null)
                    continue;
                    
                if(!lastButtonPressedIsPriority)
                {
                    key = inputButton.displayName;
                    break;
                }
                else
                {
                    key = inputButton.displayName;
                }
            }

            if (EnableKeyboardIo)
            {
                if (StringOutputToLower)
                {
                    if (!letterKeysOnly)
                    {
                        VKeyboardOutPutString = () => key.ToLower();
                    }
                    else
                    {
                        if (key.Length == 1)
                        {
                            VKeyboardOutPutString = () => key.ToLower();
                        }
                        else
                        {
                            VKeyboardOutPutString = () => "";
                        }
                    }
                }
                else
                {
                    if (key.Length == 1)
                    {
                        VKeyboardOutPutString = () => key;
                    }
                    else
                    {
                        VKeyboardOutPutString = () => "";
                    }
                }
            }
            
            if (key.Equals(keyboardTriggerKey.ToString().Replace("KeyCode.", ""), StringComparison.OrdinalIgnoreCase))
            {
                VPoolInput();
            }

            if(VKBkeys.ContainsKey(key))
            {
                VKBkeys[key].Invoke();                
            }
        }
        private void GamepadAction(Gamepad pad, IEnumerable<InputControl> ict, bool lastButtonPressedIsPriority = false)
        {
            string key = string.Empty;

            if (key.Equals(gamepadTriggerKey.ToString().Replace("GamepadButton.", ""), StringComparison.OrdinalIgnoreCase))
            {
                VPoolInput();
            }
        }
    }
}