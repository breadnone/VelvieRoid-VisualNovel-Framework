using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEditor;
using VelvieR;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using System.Linq;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Reflection;
using UnityEditor.Events;

namespace VIEditor
{
    [CustomEditor(typeof(VCoreUtil))]
    public class VGraphEditor : Editor
    {
        private Box KeyboardBindParent;
        private Box GamepadBindParent;
        private List<Box> boxes = new List<Box>();
        private Box SelectedBox;
        private List<VInputBuffer> inputs = new List<VInputBuffer>();
        public override VisualElement CreateInspectorGUI()
        {
            GetAllVInputs();
            var vv = target as VCoreUtil;

            //Button section
            Box box = new Box();
            box.style.marginLeft = 10;
            ScrollView scroll = new ScrollView();
            box.Add(scroll);
            Button graphbtn = new Button();
            graphbtn.style.height = 40;
            graphbtn.text = "Open VGraph";
            graphbtn.clicked += () => CheckVGraphsEditorWindow(vv);
            box.Add(graphbtn);

            //VInput Section/////////////////////////
            scroll.Add(DrawKeyboardInput(vv));
            scroll.Add(DrawStringIo(vv));
            scroll.Add(DrawCase(vv));
            scroll.Add(DrawAddRemoveKeyboardBind(vv));
            scroll.Add(DrawKeyboardBind(vv));
            scroll.Add(DrawMouseInput(vv));
            scroll.Add(DrawGamepadInput(vv));
            scroll.Add(DrawAddRemoveGamepadBind(vv));
            scroll.Add(DrawGamepadBind(vv));
            scroll.Add(DrawDelay(vv));
            return box;
        }
        public Box DrawMouseInput(VCoreUtil t)
        {
            //Input Delay
            var vinputBox = new Box();
            vinputBox.style.flexDirection = FlexDirection.Row;

            var lblMouse = new Label();
            lblMouse.style.width = 150;
            lblMouse.text = "Mouse Trigger Type : ";

            var objMouse = new ToolbarMenu();
            objMouse.style.width = 170;

            for (int i = 0; i < inputs.Count; i++)
            {
                if (inputs[i].MouseTrigger == UnityEngine.InputSystem.LowLevel.MouseButton.Left)
                {
                    objMouse.text = "Left Button";
                }
                else if (inputs[i].MouseTrigger == UnityEngine.InputSystem.LowLevel.MouseButton.Right)
                {
                    objMouse.text = "Right Button";
                }
                else if (inputs[i].MouseTrigger == UnityEngine.InputSystem.LowLevel.MouseButton.Middle)
                {
                    objMouse.text = "Middle Button";
                }
            }

            foreach (var mbtn in inputs)
            {
                objMouse.menu.AppendAction("Left Button", (x) =>
                {
                    objMouse.text = "Left Button";
                    mbtn.MouseTrigger = UnityEngine.InputSystem.LowLevel.MouseButton.Left;
                });
                objMouse.menu.AppendAction("Right Button", (x) =>
                {
                    objMouse.text = "Right Button";
                    mbtn.MouseTrigger = UnityEngine.InputSystem.LowLevel.MouseButton.Right;
                });
                objMouse.menu.AppendAction("Middle Button", (x) =>
                {
                    objMouse.text = "Middle Button";
                    mbtn.MouseTrigger = UnityEngine.InputSystem.LowLevel.MouseButton.Middle;
                });
            }

            vinputBox.Add(lblMouse);
            vinputBox.Add(objMouse);
            return vinputBox;
        }
        public Box DrawKeyboardInput(VCoreUtil t)
        {
            //Input Delay
            var vinputBox = new Box();
            vinputBox.style.marginTop = 15;
            vinputBox.style.flexDirection = FlexDirection.Row;

            var lblKb = new Label();
            lblKb.style.width = 150;
            lblKb.text = "Keyboard Trigger Type : ";

            var objKb = new ToolbarMenu();
            objKb.style.width = 170;

            for (int i = 0; i < inputs.Count; i++)
            {
                if (inputs[i] != null)
                {
                    if (inputs[i].KeyboardTrigger != KeyCode.None)
                    {
                        objKb.text = inputs[i].KeyboardTrigger.ToString();
                        break;
                    }
                    else
                    {
                        objKb.text = "None";
                        break;
                    }
                }
            }

            foreach (var key in Enum.GetValues(typeof(KeyCode)))
            {
                var st = (KeyCode)key;

                objKb.menu.AppendAction(st.ToString(), (x) =>
                {
                    GetAllVInputs();

                    for (int i = 0; i < inputs.Count; i++)
                    {
                        if (inputs[i] != null)
                        {
                            objKb.text = st.ToString();
                            inputs[i].KeyboardTrigger = st;
                            EditorUtility.SetDirty(inputs[i].gameObject);
                        }
                    }
                });
            }

            vinputBox.Add(lblKb);
            vinputBox.Add(objKb);
            return vinputBox;
        }
        public Box DrawAddRemoveGamepadBind(VCoreUtil t)
        {
            //Input Delay
            var vinputBox = new Box();
            vinputBox.style.marginTop = 15;
            vinputBox.style.flexDirection = FlexDirection.Row;

            var lblKb = new Label();
            lblKb.style.width = 150;
            lblKb.text = "Add Gamepad Bind : ";

            var bindBox = new Box();
            bindBox.style.flexDirection = FlexDirection.Row;

            var objKbADD = new Button();
            objKbADD.text = "+";
            objKbADD.style.width = 70;

            var objKbDel = new Button();
            objKbDel.text = "-";
            objKbDel.style.width = 70;

            objKbDel.clicked += () =>
            {
                if (SelectedBox == null)
                    return;

                if (boxes.Contains(SelectedBox))
                {
                    SelectedBox.RemoveFromHierarchy();
                    boxes.Remove(SelectedBox);
                    RemoveVBind(SelectedBox.userData as VBindKey);
                }
            };

            objKbADD.clicked += () =>
            {
                var box = new Box();
                VBindKey vbind = new VBindKey();
                box.userData = vbind as VBindKey;
                box.name = "vbindParent";
                boxes.Add(box);
                if (!PortsUtils.PlayMode)
                {
                    box.RegisterCallback<MouseDownEvent>((x) =>
                    {
                        SelectedBox = box;
                        box.style.backgroundColor = Color.yellow;

                        for (int i = 0; i < boxes.Count; i++)
                        {
                            if (boxes[i] != box)
                            {
                                if (boxes[i] != null)
                                    boxes[i].style.backgroundColor = Color.grey;
                            }
                        }
                    });
                }
                box.style.backgroundColor = Color.grey;
                box.style.marginBottom = 10;
                box.style.width = 190;
                box.style.flexDirection = FlexDirection.Column;

                var lblTitle = new Label();
                lblTitle.style.marginLeft = 5;
                lblTitle.style.marginTop = 5;
                lblTitle.text = "GamePad Key";

                var objKb = new ToolbarMenu();
                objKb.style.marginLeft = 5;
                objKb.style.width = 170;
                objKb.text = vbind.keyboardBindKeyCode.ToString();

                foreach (var key in Enum.GetValues(typeof(GamepadButton)))
                {
                    var st = (GamepadButton)key;

                    objKb.menu.AppendAction(st.ToString(), (x) =>
                    {
                        objKb.text = st.ToString();
                        SelectedBox = box;
                        box.style.backgroundColor = Color.yellow;
                        vbind.gamepadBindKeyCode = st;

                        for (int i = 0; i < boxes.Count; i++)
                        {
                            if (boxes[i] != box)
                            {
                                if (boxes[i] != null)
                                    boxes[i].style.backgroundColor = Color.grey;
                            }
                        }
                    });
                }

                ///UnityEvent here
                var lblActionVGraph = new Label();
                lblActionVGraph.style.marginLeft = 5;
                lblActionVGraph.style.marginRight = 5;
                lblActionVGraph.style.marginTop = 10;
                lblActionVGraph.text = "Exec Event :";

                var dropDmethods = new ToolbarMenu();
                dropDmethods.style.marginLeft = 5;
                dropDmethods.style.marginRight = 5;
                dropDmethods.style.marginBottom = 10;

                var objAction = new ObjectField();
                objAction.style.marginLeft = 5;
                objAction.style.marginRight = 5;
                objAction.objectType = typeof(GameObject);
                if (!PortsUtils.PlayMode)
                {
                    objAction.RegisterValueChangedCallback((x) =>
                    {
                        vbind.obj = objAction.value as GameObject;
                        //VLocalKeyboardFunction(vbind, objAction, txtMethod);

                        if (objAction.value != null)
                        {
                            VKeyboardMethodsRepopulate(objAction, vbind, dropDmethods);
                        }
                    });
                }

                GetAllVInputs();

                foreach (var inp in inputs)
                {
                    if (inp == null)
                        continue;

                    if (!inp.inputWrapper.Contains(vbind))
                        inp.inputWrapper.Add(vbind);

                    EditorUtility.SetDirty(inp.gameObject);
                }
                ////////////////

                box.Add(lblTitle);
                box.Add(objKb);
                box.Add(lblActionVGraph);
                box.Add(objAction);
                box.Add(dropDmethods);
                GamepadBindParent.Add(box);
            };

            bindBox.Add(objKbADD);
            bindBox.Add(objKbDel);
            vinputBox.Add(lblKb);
            vinputBox.Add(bindBox);
            return vinputBox;
        }
        public Box DrawAddRemoveKeyboardBind(VCoreUtil t)
        {
            //Input Delay
            var vinputBox = new Box();
            vinputBox.style.marginTop = 15;
            vinputBox.style.flexDirection = FlexDirection.Row;

            var lblKb = new Label();
            lblKb.style.width = 150;
            lblKb.text = "Add Keyboard Bind : ";

            var bindBox = new Box();
            bindBox.style.flexDirection = FlexDirection.Row;

            var objKbADD = new Button();
            objKbADD.text = "+";
            objKbADD.style.width = 70;

            var objKbDel = new Button();
            objKbDel.text = "-";
            objKbDel.style.width = 70;

            objKbDel.clicked += () =>
            {
                if (SelectedBox == null)
                    return;

                if (boxes.Contains(SelectedBox))
                {
                    SelectedBox.RemoveFromHierarchy();
                    boxes.Remove(SelectedBox);
                    RemoveVBind(SelectedBox.userData as VBindKey);
                }
            };

            objKbADD.clicked += () =>
            {
                var box = new Box();
                VBindKey vbind = new VBindKey();
                box.userData = vbind as VBindKey;
                box.name = "vbindParent";
                boxes.Add(box);
                if (!PortsUtils.PlayMode)
                {
                    box.RegisterCallback<MouseDownEvent>((x) =>
                    {
                        SelectedBox = box;
                        box.style.backgroundColor = Color.yellow;

                        for (int i = 0; i < boxes.Count; i++)
                        {
                            if (boxes[i] != box)
                            {
                                if (boxes[i] != null)
                                    boxes[i].style.backgroundColor = Color.grey;
                            }
                        }
                    });
                }
                box.style.backgroundColor = Color.grey;
                box.style.marginBottom = 10;
                box.style.width = 190;
                box.style.flexDirection = FlexDirection.Column;

                var lblTitle = new Label();
                lblTitle.style.marginLeft = 5;
                lblTitle.style.marginTop = 5;
                lblTitle.text = "Keyboard Key";

                var objKb = new ToolbarMenu();
                objKb.style.marginLeft = 5;
                objKb.style.width = 170;
                objKb.text = vbind.keyboardBindKeyCode.ToString();

                foreach (var key in Enum.GetValues(typeof(KeyCode)))
                {
                    var st = (KeyCode)key;

                    objKb.menu.AppendAction(st.ToString(), (x) =>
                    {
                        objKb.text = st.ToString();
                        SelectedBox = box;
                        box.style.backgroundColor = Color.yellow;

                        vbind.keyboardBindKeyCode = st;

                        for (int i = 0; i < boxes.Count; i++)
                        {
                            if (boxes[i] != box)
                            {
                                if (boxes[i] != null)
                                    boxes[i].style.backgroundColor = Color.grey;
                            }
                        }
                    });
                }

                ///UnityEvent here
                var lblActionVGraph = new Label();
                lblActionVGraph.style.marginLeft = 5;
                lblActionVGraph.style.marginRight = 5;
                lblActionVGraph.style.marginTop = 10;
                lblActionVGraph.text = "Exec Event :";

                var dropDmethods = new ToolbarMenu();
                dropDmethods.style.marginLeft = 5;
                dropDmethods.style.marginRight = 5;
                dropDmethods.style.marginBottom = 10;

                var objAction = new ObjectField();
                objAction.style.marginLeft = 5;
                objAction.style.marginRight = 5;
                objAction.objectType = typeof(GameObject);
                if (!PortsUtils.PlayMode)
                {
                    objAction.RegisterValueChangedCallback((x) =>
                    {
                        vbind.obj = objAction.value as GameObject;
                        //VLocalKeyboardFunction(vbind, objAction, txtMethod);

                        if (objAction.value != null)
                        {
                            VKeyboardMethodsRepopulate(objAction, vbind, dropDmethods);
                        }
                    });
                }
                GetAllVInputs();

                foreach (var inp in inputs)
                {
                    if (inp == null)
                        continue;

                    if (!inp.inputWrapper.Contains(vbind))
                        inp.inputWrapper.Add(vbind);

                    EditorUtility.SetDirty(inp.gameObject);
                }
                ////////////////

                box.Add(lblTitle);
                box.Add(objKb);
                box.Add(lblActionVGraph);
                box.Add(objAction);
                box.Add(dropDmethods);
                KeyboardBindParent.Add(box);
            };

            bindBox.Add(objKbADD);
            bindBox.Add(objKbDel);
            vinputBox.Add(lblKb);
            vinputBox.Add(bindBox);
            return vinputBox;
        }
        public void VKeyboardMethodsRepopulate(ObjectField objAction, VBindKey vbind, ToolbarMenu tb)
        {
            var obj = objAction.value as GameObject;

            if (obj == null)
                return;

            var components = obj.GetComponents<Component>();

            tb.menu.AppendAction("<None>", (x) =>
            {
                tb.text = "<None>";
                vbind.methodName = string.Empty;
                vbind.isKeyboard = true;

                try
                {
                    UnityEditor.Events.UnityEventTools.RemovePersistentListener(vbind.action, 0);
                }
                catch (Exception)
                {
                    //skip
                }

                EditorUtility.SetDirty(vbind.obj);
            });

            foreach (var component in components)
            {
                if (component != null && component is Transform || component is RectTransform)
                    continue;

                var parentType = component.GetType();
                MethodInfo[] vmethods = parentType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                foreach (var vmethod in vmethods)
                {
                    if (vmethod != null && !String.IsNullOrEmpty(vmethod.Name))
                    {
                        var mName = vmethod.Name;

                        tb.menu.AppendAction(vmethod.Name, (x) =>
                        {
                            if (String.IsNullOrEmpty(vbind.guid))
                                vbind.guid = Guid.NewGuid().ToString() + UnityEngine.Random.Range(0, int.MaxValue);

                            vbind.methodName = mName;
                            vbind.isKeyboard = true;
                            tb.text = vmethod.Name;

                            try
                            {
                                UnityEditor.Events.UnityEventTools.RemovePersistentListener(vbind.action, 0);
                            }
                            catch (Exception)
                            {
                                //skip
                            }

                            UnityAction act = Delegate.CreateDelegate(typeof(UnityAction), component, vmethod) as UnityAction;
                            UnityEditor.Events.UnityEventTools.AddPersistentListener(vbind.action, act);
                            EditorUtility.SetDirty(vbind.obj);
                        });
                    }
                }
            }
        }
        public void DrawVBinds()
        {
            foreach (var ipt in inputs)
            {
                if (ipt == null)
                    continue;

                for (int j = 0; j < ipt.inputWrapper.Count; j++)
                {
                    var otp = ipt.inputWrapper[j];
                    var box = new Box();
                    box.name = "vbindParent";
                    box.userData = otp;
                    boxes.Add(box);
                    if (!PortsUtils.PlayMode)
                    {
                        box.RegisterCallback<MouseDownEvent>((x) =>
                        {
                            SelectedBox = box;
                            box.style.backgroundColor = Color.yellow;

                            for (int i = 0; i < boxes.Count; i++)
                            {
                                if (boxes[i] != box)
                                {
                                    if (boxes[i] != null)
                                        boxes[i].style.backgroundColor = Color.grey;
                                }
                            }
                        });
                    }
                    box.style.backgroundColor = Color.grey;
                    box.style.marginBottom = 10;
                    box.style.width = 190;
                    box.style.flexDirection = FlexDirection.Column;

                    var lblTitle = new Label();
                    lblTitle.style.marginLeft = 5;
                    lblTitle.style.marginTop = 5;
                    lblTitle.text = "Keyboard Key";

                    var objKb = new ToolbarMenu();
                    objKb.style.marginLeft = 5;
                    objKb.style.width = 170;

                    if (inputs.Count > 0)
                        objKb.text = otp.keyboardBindKeyCode.ToString();
                    else
                        objKb.text = "None";

                    foreach (var key in Enum.GetValues(typeof(KeyCode)))
                    {
                        var st = (KeyCode)key;

                        objKb.menu.AppendAction(st.ToString(), (x) =>
                        {
                            objKb.text = st.ToString();
                            objKb.userData = (KeyCode)st;

                            otp.keyboardBindKeyCode = st;

                            SelectedBox = box;
                            box.style.backgroundColor = Color.yellow;

                            for (int i = 0; i < boxes.Count; i++)
                            {
                                if (boxes[i] != box)
                                {
                                    if (boxes[i] != null)
                                        boxes[i].style.backgroundColor = Color.grey;
                                }
                            }
                        });
                    }

                    if (otp == null)
                        continue;

                    ///UnityEvent here
                    var lblActionVGraph = new Label();
                    lblActionVGraph.style.marginLeft = 5;
                    lblActionVGraph.style.marginRight = 5;
                    lblActionVGraph.style.marginTop = 10;
                    lblActionVGraph.text = "Exec Event : ";

                    var dropDmethods = new ToolbarMenu();
                    dropDmethods.style.marginLeft = 5;
                    dropDmethods.style.marginRight = 5;
                    dropDmethods.style.marginBottom = 10;

                    if (String.IsNullOrEmpty(otp.methodName))
                        dropDmethods.text = "<None>";
                    else
                        dropDmethods.text = otp.methodName;

                    var objAction = new ObjectField();
                    objAction.style.marginLeft = 5;
                    objAction.style.marginRight = 5;
                    objAction.objectType = typeof(GameObject);
                    objAction.value = otp.obj;
                    if (!PortsUtils.PlayMode)
                    {
                        objAction.RegisterValueChangedCallback((x) =>
                        {
                            otp.obj = objAction.value as GameObject;
                            VKeyboardMethodsRepopulate(objAction, otp, dropDmethods);
                        });
                    }
                    VKeyboardMethodsRepopulate(objAction, otp, dropDmethods);

                    box.Add(lblTitle);
                    box.Add(objKb);
                    box.Add(lblActionVGraph);
                    box.Add(objAction);
                    box.Add(dropDmethods);

                    KeyboardBindParent.Add(box);
                }

                break;
            }
        }
        public void DrawVBindsGamepad()
        {
            foreach (var ipt in inputs)
            {
                if (ipt == null)
                    continue;

                for (int j = 0; j < ipt.inputWrapper.Count; j++)
                {
                    var otp = ipt.inputWrapper[j];
                    var box = new Box();
                    box.name = "vbindParent";
                    box.userData = otp;
                    boxes.Add(box);
                    if (!PortsUtils.PlayMode)
                    {
                        box.RegisterCallback<MouseDownEvent>((x) =>
                        {
                            SelectedBox = box;
                            box.style.backgroundColor = Color.yellow;

                            for (int i = 0; i < boxes.Count; i++)
                            {
                                if (boxes[i] != box)
                                {
                                    if (boxes[i] != null)
                                        boxes[i].style.backgroundColor = Color.grey;
                                }
                            }
                        });
                    }
                    box.style.backgroundColor = Color.grey;
                    box.style.marginBottom = 10;
                    box.style.width = 190;
                    box.style.flexDirection = FlexDirection.Column;

                    var lblTitle = new Label();
                    lblTitle.style.marginLeft = 5;
                    lblTitle.style.marginTop = 5;
                    lblTitle.text = "Gamepad Key";

                    var objKb = new ToolbarMenu();
                    objKb.style.marginLeft = 5;
                    objKb.style.width = 170;

                    if (inputs.Count > 0)
                        objKb.text = otp.keyboardBindKeyCode.ToString();
                    else
                        objKb.text = "None";

                    foreach (var key in Enum.GetValues(typeof(KeyCode)))
                    {
                        var st = (KeyCode)key;

                        objKb.menu.AppendAction(st.ToString(), (x) =>
                        {
                            objKb.text = st.ToString();
                            objKb.userData = (KeyCode)st;

                            otp.keyboardBindKeyCode = st;

                            SelectedBox = box;
                            box.style.backgroundColor = Color.yellow;

                            for (int i = 0; i < boxes.Count; i++)
                            {
                                if (boxes[i] != box)
                                {
                                    if (boxes[i] != null)
                                        boxes[i].style.backgroundColor = Color.grey;
                                }
                            }
                        });
                    }

                    if (otp == null)
                        continue;

                    ///UnityEvent here
                    var lblActionVGraph = new Label();
                    lblActionVGraph.style.marginLeft = 5;
                    lblActionVGraph.style.marginRight = 5;
                    lblActionVGraph.style.marginTop = 10;
                    lblActionVGraph.text = "Exec Event : ";

                    var dropDmethods = new ToolbarMenu();
                    dropDmethods.style.marginLeft = 5;
                    dropDmethods.style.marginRight = 5;
                    dropDmethods.style.marginBottom = 10;

                    if (String.IsNullOrEmpty(otp.methodName))
                        dropDmethods.text = "<None>";
                    else
                        dropDmethods.text = otp.methodName;

                    var objAction = new ObjectField();
                    objAction.style.marginLeft = 5;
                    objAction.style.marginRight = 5;
                    objAction.objectType = typeof(GameObject);
                    objAction.value = otp.obj;
                    if (!PortsUtils.PlayMode)
                    {
                        objAction.RegisterValueChangedCallback((x) =>
                        {
                            otp.obj = objAction.value as GameObject;
                            VKeyboardMethodsRepopulate(objAction, otp, dropDmethods);
                        });
                    }
                    VKeyboardMethodsRepopulate(objAction, otp, dropDmethods);

                    box.Add(lblTitle);
                    box.Add(objKb);
                    box.Add(lblActionVGraph);
                    box.Add(objAction);
                    box.Add(dropDmethods);

                    GamepadBindParent.Add(box);
                }

                break;
            }
        }
        public Box DrawKeyboardBind(VCoreUtil t)
        {
            //Input Delay
            var vinputBox = new Box();
            vinputBox.style.marginLeft = 15;
            vinputBox.style.marginBottom = 20;
            vinputBox.style.flexDirection = FlexDirection.Row;

            var lblKb = new Label();
            lblKb.style.width = 140;
            lblKb.text = "Bind Key : ";

            var objkbbox = new Box();

            if (KeyboardBindParent == null)
                KeyboardBindParent = objkbbox;

            DrawVBinds();

            objkbbox.style.flexDirection = FlexDirection.Column;
            objkbbox.style.width = 155;

            vinputBox.Add(lblKb);
            vinputBox.Add(objkbbox);
            return vinputBox;
        }
        public Box DrawGamepadBind(VCoreUtil t)
        {
            //Input Delay
            var vinputBox = new Box();
            vinputBox.style.marginLeft = 15;
            vinputBox.style.marginBottom = 20;
            vinputBox.style.flexDirection = FlexDirection.Row;

            var lblKb = new Label();
            lblKb.style.width = 140;
            lblKb.text = "Bind Key : ";

            var objkbbox = new Box();

            if (GamepadBindParent == null)
                GamepadBindParent = objkbbox;

            DrawVBindsGamepad();

            objkbbox.style.flexDirection = FlexDirection.Column;
            objkbbox.style.width = 155;

            vinputBox.Add(lblKb);
            vinputBox.Add(objkbbox);
            return vinputBox;
        }

        public Box DrawGamepadInput(VCoreUtil t)
        {
            //Input Delay
            var vinputBox = new Box();
            vinputBox.style.flexDirection = FlexDirection.Row;

            var lblgp = new Label();
            lblgp.style.width = 150;
            lblgp.text = "Gamepad Trigger Type : ";

            var objgp = new ToolbarMenu();
            objgp.style.width = 150;

            if (inputs.Count > 0)
                objgp.text = inputs[0].GamepadTrigger.ToString();

            foreach (var key in Enum.GetValues(typeof(GamepadButton)))
            {
                var st = (GamepadButton)key;
                var str = st.ToString();

                objgp.menu.AppendAction(str, (x) =>
                {
                    objgp.text = str;
                    for (int i = 0; i < inputs.Count; i++)
                    {
                        inputs[i].GamepadTrigger = st;
                    }
                });
            }

            vinputBox.Add(lblgp);
            vinputBox.Add(objgp);
            return vinputBox;
        }
        public Box DrawDelay(VCoreUtil t)
        {
            //Input Delay
            var vinputBox = new Box();
            vinputBox.style.flexDirection = FlexDirection.Row;

            var lblDelay = new Label();
            lblDelay.style.width = 150;
            lblDelay.text = "Input Delay : ";

            var objDelay = new FloatField();
            objDelay.style.width = 170;

            foreach (var ipt in inputs)
            {
                objDelay.value = ipt.Delay;
                break;
            }
            if (!PortsUtils.PlayMode)
            {
                objDelay.RegisterValueChangedCallback((x) =>
                {
                    foreach (var ipt in inputs)
                    {
                        ipt.Delay = objDelay.value;
                    }
                });
            }

            vinputBox.Add(lblDelay);
            vinputBox.Add(objDelay);
            return vinputBox;
        }
        public Box DrawStringIo(VCoreUtil t)
        {
            //Input Delay
            var vinputBox = new Box();
            vinputBox.style.flexDirection = FlexDirection.Row;

            var lblDelay = new Label();
            lblDelay.style.width = 150;
            lblDelay.text = "Keyboard String Output : ";

            var objtog = new Toggle();
            objtog.style.width = 170;
            objtog.value = GetSingleInput().EnableKeyboardIo;
            if (!PortsUtils.PlayMode)
            {
                objtog.RegisterValueChangedCallback((x) =>
                {
                    if (inputs.Count > 0)
                    {
                        inputs[0].EnableKeyboardIo = objtog.value;
                    }
                });
            }
            vinputBox.Add(lblDelay);
            vinputBox.Add(objtog);
            return vinputBox;
        }

        public Box DrawCase(VCoreUtil t)
        {
            //Input Delay
            var vinputBox = new Box();
            vinputBox.style.flexDirection = FlexDirection.Row;

            var lblDelay = new Label();
            lblDelay.style.width = 150;
            lblDelay.text = "String is LowerCase : ";

            var objtog = new Toggle();
            objtog.style.width = 170;
            objtog.value = GetSingleInput().StringOutputToLower;

            foreach (var i in inputs)
            {
                if (i != null)
                {
                    i.StringOutputToLower = objtog.value;
                }
            }

            vinputBox.Add(lblDelay);
            vinputBox.Add(objtog);
            return vinputBox;
        }
        public void RemoveVBind(VBindKey vbdata)
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                if (inputs[i].inputWrapper.Contains(vbdata))
                {
                    inputs[i].inputWrapper.Remove(vbdata);
                }
            }
        }

        public void GetAllVInputs()
        {
            if (inputs.Count == 0)
            {                
                var all = Resources.FindObjectsOfTypeAll<VInputBuffer>();
                inputs = all.ToList();
            }
        }
        public VInputBuffer GetSingleInput()
        {
            var all = Resources.FindObjectsOfTypeAll<VInputBuffer>();

            if (all.Length > 0)
                return inputs[0];

            return null;
        }
        public void CheckVGraphsEditorWindow(VCoreUtil t)
        {
            if (!EditorWindow.HasOpenInstances<VGraphs>())
            {
                VGraphs vg = EditorWindow.GetWindow<VGraphs>(typeof(HostGuiWindow));
            }
            else
            {
                EditorWindow.GetWindow<VGraphs>().Focus();
            }
        }
    }
}