using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VelvieR;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using System.Linq;

namespace VIEditor
{
    [CustomEditor(typeof(MoveCharacter))]
    public class MoveCharacterEditor : Editor
    {
        private VisualElement slotOne;
        private VisualElement slotTwo;
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement myInspector = new Box();
            var t = target as MoveCharacter;
            Undo.RecordObject(t, "MoveCharacter undo object");

            myInspector.Add(DrawCharacter(t));
            myInspector.Add(DrawSelect(t));
            myInspector.Add(DrawCharacterSwap(t));
            myInspector.Add(GetStages(t));
            myInspector.Add(DrawIsTwoD(t));
            myInspector.Add(Draw2DStageFrom(t));
            myInspector.Add(Draw2DStageTo(t));
            myInspector.Add(DrawDuration(t));
            myInspector.Add(EasingType(t));
            myInspector.Add(DrawWaitUntilFinished(t));

            if (t.MoveType == CharacterMoveType.Swap)
            {
                swapChar.SetEnabled(true);
            }
            else
            {
                swapChar.SetEnabled(false);
            }

            if (t.MainStage == null)
            {
                toEl.SetEnabled(false);
                fromEl.SetEnabled(false);
            }
            else
            {
                toEl.SetEnabled(true);
                fromEl.SetEnabled(true);
            }

            myInspector.style.flexDirection = FlexDirection.Column;
            //Always add this at the end!
            VUITemplate.DrawSummary(myInspector, t, ()=> t.OnVSummary());
            return myInspector;
        }
        private VisualElement DrawWaitUntilFinished(MoveCharacter t)
        {
            var box = new VisualElement();
            box.style.marginTop = 10;
            box.style.flexDirection = FlexDirection.Row;

            var lbl = new Label();
            lbl.style.marginLeft = 5;
            lbl.style.width = 125;
            lbl.text = "Wait until finished :";

            var tbMenu = new Toggle();
            tbMenu.style.width = 190;
            tbMenu.value = t.WaitUntilFinished;

            tbMenu.RegisterValueChangedCallback((x) =>
            {
                t.WaitUntilFinished = tbMenu.value;
                PortsUtils.SetActiveAssetDirty();
            });

            box.Add(lbl);
            box.Add(tbMenu);
            return box;
        }
        private VisualElement DrawCharacter(MoveCharacter t)
        {
            var root = VUITemplate.CharacterTemplate();

            if (t.Character == null || String.IsNullOrEmpty(t.Character.name))
            {
                root.child.value = "<None>";
            }
            else
            {
                if(!String.IsNullOrEmpty(t.Character.name))
                root.child.value = t.Character.name;
                else
                root.child.value = "<None>";
            }

            root.child.RegisterCallback<ChangeEvent<string>>((x) =>
            {
                if (!PortsUtils.PlayMode)
                {
                    if (x.newValue == "<None>")
                    {
                        t.Character = null;
                        ShufflingMenu(t);
                        PortsUtils.SetActiveAssetDirty();
                    }
                    else
                    {
                        var chars = Resources.FindObjectsOfTypeAll<VCharacterUtil>();
                        t.Character = Array.Find(chars, xx => xx.character.name == x.newValue).character;
                        ShufflingMenu(t);
                        PortsUtils.SetActiveAssetDirty();
                    }
                }
            });
            return root.root;
        }
        private VisualElement swapChar;
        private VisualElement DrawCharacterSwap(MoveCharacter t)
        {
            var root = VUITemplate.CharacterTemplate("Swap to character : ");
            swapChar = root.root;

            if (t.CharacterToSwap == null)
            {
                root.child.value = "<None>";
            }
            else
            {
                if(!String.IsNullOrEmpty(t.CharacterToSwap.name))
                root.child.value = t.CharacterToSwap.name;
                else
                root.child.value = "<None>";
            }

            root.child.RegisterCallback<ChangeEvent<string>>((x) =>
            {
                if (!PortsUtils.PlayMode)
                {
                    if (x.newValue == "<None>" || String.IsNullOrEmpty(x.newValue))
                    {
                        t.CharacterToSwap = null;
                        PortsUtils.SetActiveAssetDirty();
                    }
                    else
                    {
                        var chars = Resources.FindObjectsOfTypeAll<VCharacterUtil>();
                        t.CharacterToSwap = Array.Find(chars, xx => xx.character.name == x.newValue).character;
                        PortsUtils.SetActiveAssetDirty();
                        ShufflingMenu(t);
                    }
                }
            });
            return root.root;
        }
        private VisualElement DrawSelect(MoveCharacter t)
        {
            var root = VUITemplate.GetTemplate("Move type :");
            var tbMenu = new DropdownField();
            tbMenu.style.width = 220;
            tbMenu.value = t.MoveType.ToString();
            tbMenu.choices = Enum.GetNames(typeof(CharacterMoveType)).ToList();
            root.Add(tbMenu);

            tbMenu.RegisterCallback<ChangeEvent<string>>((x) =>
            {
                if (!PortsUtils.PlayMode)
                {
                    foreach (var move in Enum.GetValues(typeof(CharacterMoveType)))
                    {
                        var astetype = (CharacterMoveType)move;

                        if (x.newValue == astetype.ToString())
                        {
                            t.MoveType = astetype;

                            if (astetype == CharacterMoveType.Swap)
                            {
                                swapChar.SetEnabled(true);
                                toEl.SetEnabled(false);
                                fromEl.SetEnabled(false);
                            }
                            else
                            {
                                swapChar.SetEnabled(false);

                                if(t.MainStage != null)
                                {
                                    toEl.SetEnabled(true);
                                    fromEl.SetEnabled(true);
                                }
                            }

                        }
                    }
                }
            });

            return root;
        }
        private ToolbarMenu tbm;
        private VisualElement fromEl;
        private VisualElement Draw2DStageFrom(MoveCharacter t)
        {
            var box = new VisualElement();
            fromEl = box;
            box.style.marginTop = 10;
            box.style.flexDirection = FlexDirection.Row;

            var lbl = new Label();
            lbl.style.marginLeft = 5;
            lbl.style.width = 125;
            lbl.text = "From :";

            var tbMenu = new ToolbarMenu();
            tbm = tbMenu;

            if (t.MainStage != null)
            {
                tbm.menu.MenuItems().Clear();

                if (t.FromStage != null)
                {
                    if(!String.IsNullOrEmpty(t.FromStage.name))
                    tbMenu.text = t.FromStage.name;
                    else
                    tbMenu.text = "<Previous>";
                }
                else
                {
                    tbMenu.text = "<Previous>";
                }

                var vstages = t.MainStage.TwoDStage;
                var vstages3 = t.MainStage.ThreeDStage;

                tbMenu.menu.AppendAction("<Previous>", (x) =>
                {
                    tbMenu.text = "<Previous>";
                    t.FromStage = null;
                    PortsUtils.SetActiveAssetDirty();
                });

                if (t.Character.is2D)
                {
                    foreach (var vstage in vstages)
                    {
                        tbMenu.menu.AppendAction(vstage.name, (x) =>
                        {
                            tbMenu.text = vstage.name;
                            t.FromStage = vstage;
                            PortsUtils.SetActiveAssetDirty();
                        });
                    }
                }
                else
                {
                    foreach (var vstage in vstages3)
                    {
                        tbMenu.menu.AppendAction(vstage.name, (x) =>
                        {
                            tbMenu.text = vstage.name;
                            t.FromStage = vstage;
                            PortsUtils.SetActiveAssetDirty();
                        });
                    }
                }
            }


            tbMenu.style.width = 190;

            if (t.MainStage != null)
            {
                tbm.SetEnabled(true);
            }
            else
            {
                tbMenu.text = "<Previous>";
                tbm.SetEnabled(false);
            }

            box.Add(lbl);
            box.Add(tbMenu);
            return box;
        }
        private VisualElement is2dvis;
        private VisualElement DrawIsTwoD(MoveCharacter t)
        {
            var box = new VisualElement();
            box.SetEnabled(false);
            box.style.marginTop = 10;
            box.style.flexDirection = FlexDirection.Row;

            var lbl = new Label();
            lbl.style.marginLeft = 5;
            lbl.style.width = 125;
            lbl.text = "Stage type : ";

            var tbMenu = new ToolbarMenu();
            is2dvis = tbMenu;
            tbMenu.style.width = 190;

            if (t.Is2D)
                tbMenu.text = "2D stage";
            else
                tbMenu.text = "3D stage";

            box.Add(lbl);
            box.Add(tbMenu);
            return box;
        }
        private ToolbarMenu tbmTo;
        private VisualElement toEl;
        private VisualElement Draw2DStageTo(MoveCharacter t)
        {
            var box = new VisualElement();
            toEl = box;
            box.style.marginTop = 10;
            box.style.flexDirection = FlexDirection.Row;

            var lbl = new Label();
            lbl.style.marginLeft = 5;
            lbl.style.width = 125;
            lbl.text = "To :";

            var tbMenu = new ToolbarMenu();
            tbmTo = tbMenu;
            tbMenu.style.width = 190;

            if (t.ToStage == null)
            {
                tbMenu.text = "<None>";
            }
            else
            {
                if(!String.IsNullOrEmpty(t.ToStage.name))
                tbMenu.text = t.ToStage.name;
                else
                tbMenu.text = "<None>";
            }

            if (t.MainStage != null)
            {
                var vstages = t.MainStage.TwoDStage;
                var vstages3 = t.MainStage.ThreeDStage;

                if (t.Is2D)
                {
                    foreach (var vstage in vstages)
                    {
                        tbMenu.menu.AppendAction(vstage.name, (x) =>
                        {
                            tbMenu.text = vstage.name;
                            t.ToStage = vstage;
                            PortsUtils.SetActiveAssetDirty();
                        });
                    }
                }
                else
                {
                    foreach (var vstage in vstages3)
                    {
                        tbMenu.menu.AppendAction(vstage.name, (x) =>
                        {
                            tbMenu.text = vstage.name;
                            t.ToStage = vstage;
                            PortsUtils.SetActiveAssetDirty();
                        });
                    }
                }
            }

            if (t.MainStage != null)
            {
                tbmTo.SetEnabled(true);
            }
            else
            {
                tbMenu.text = "<None>";
                t.ToStage = null;
                tbmTo.SetEnabled(false);
            }

            box.Add(lbl);
            box.Add(tbMenu);
            return box;
        }
        private VisualElement DrawDuration(MoveCharacter t)
        {
            var box = new VisualElement();
            box.style.marginTop = 10;
            box.style.flexDirection = FlexDirection.Row;

            var lbl = new Label();
            lbl.style.marginLeft = 5;
            lbl.style.width = 125;
            lbl.text = "Move duration : ";

            var tbMenu = new FloatField();
            tbMenu.style.width = 50;
            tbMenu.value = t.Duration;
            tbMenu.RegisterValueChangedCallback((x) =>
            {
                t.Duration = tbMenu.value;
                PortsUtils.SetActiveAssetDirty();
            });

            box.Add(lbl);
            box.Add(tbMenu);
            return box;
        }
        private void ShufflingMenu(MoveCharacter t)
        {
            if(t.MainStage != null && t.Character != null)
            {
                var stages = Resources.FindObjectsOfTypeAll<VStageUtil>();
                var vstages = stages[0].TwoDStage;
                var vstages3 = stages[0].ThreeDStage;

                tbm.menu.MenuItems().Clear();

                if (t.FromStage == null)
                {
                    tbm.text = "<Previous>";
                }
                else
                {
                    if(!String.IsNullOrEmpty(t.FromStage.name))
                    tbm.text = t.FromStage.name;
                    else
                    tbm.text = "<Previous>";
                }

                if (t.Character.is2D)
                {
                    foreach (var vstage in vstages)
                    {
                        tbm.menu.AppendAction(vstage.name, (x) =>
                        {
                            tbm.text = vstage.name;
                            t.FromStage = vstage;
                            PortsUtils.SetActiveAssetDirty();
                        });
                    }
                }
                else
                {
                    foreach (var vstage in vstages3)
                    {
                        tbm.menu.AppendAction(vstage.name, (x) =>
                        {
                            tbm.text = vstage.name;
                            t.FromStage = vstage;
                            PortsUtils.SetActiveAssetDirty();
                        });
                    }
                }

                tbmTo.menu.MenuItems().Clear();

                if (t.ToStage == null)
                {
                    tbmTo.text = "<None>";
                }
                else
                {
                    if(!String.IsNullOrEmpty(t.ToStage.name))
                    tbmTo.text = t.ToStage.name;
                    else
                    tbmTo.text = "<None>";
                }

                if (t.Character.is2D)
                {
                    foreach (var vstage in vstages)
                    {
                        tbmTo.menu.AppendAction(vstage.name, (x) =>
                        {
                            tbmTo.text = vstage.name;
                            t.ToStage = vstage;
                            PortsUtils.SetActiveAssetDirty();
                        });
                    }
                }
                else
                {
                    foreach (var vstage in vstages3)
                    {
                        tbmTo.menu.AppendAction(vstage.name, (x) =>
                        {
                            tbmTo.text = vstage.name;
                            t.ToStage = vstage;
                            PortsUtils.SetActiveAssetDirty();
                        });
                    }
                }
            }
            else
            {
                tbm.menu.MenuItems().Clear();
                tbm.text = "<None>";
                tbmTo.menu.MenuItems().Clear();
                tbmTo.text = "<None>";
            }
        }
        private VisualElement EasingType(MoveCharacter t)
        {
            var box = new VisualElement();
            box.style.marginTop = 10;
            box.style.flexDirection = FlexDirection.Row;

            var lbl = new Label();
            lbl.style.marginLeft = 5;
            lbl.style.width = 125;
            lbl.text = "Ease type :";

            var tbMenu = new ToolbarMenu();
            tbMenu.style.width = 190;
            tbMenu.text = t.EaseType.ToString();

            foreach (var mvtype in Enum.GetValues(typeof(LeanTweenType)))
            {
                var astype = (LeanTweenType)mvtype;
                tbMenu.menu.AppendAction(astype.ToString(), (x) =>
                {
                    tbMenu.text = astype.ToString();
                    t.EaseType = astype;
                    PortsUtils.SetActiveAssetDirty();
                });
            }

            box.Add(lbl);
            box.Add(tbMenu);
            return box;
        }
        private VisualElement GetStages(MoveCharacter t)
        {
            var root = VUITemplate.VStageTemplate("Stage : ");
            root.child.style.width = 220;

            if (t.MainStage != null)
            {
                root.child.value = t.MainStage.gameObject.name;
            }
            else
            {
                root.child.value = "<None>";

            }

            root.child.RegisterCallback<ChangeEvent<string>>((x) =>
            {
                if (String.IsNullOrEmpty(x.newValue) || x.newValue == "<None>")
                {
                    t.MainStage = null;
                    t.FromStage = null;
                    t.ToStage = null;
                    tbm.SetEnabled(false);
                    tbmTo.SetEnabled(false);
                    toEl.SetEnabled(false);
                    fromEl.SetEnabled(false);
                    ShufflingMenu(t);
                }
                else
                {
                    var stages = Resources.FindObjectsOfTypeAll<VStageUtil>();
                    t.MainStage = Array.Find(stages, xx => xx.vstageName == x.newValue);
                    tbm.SetEnabled(true);
                    tbmTo.SetEnabled(true);
                    toEl.SetEnabled(true);
                    fromEl.SetEnabled(true);
                    PortsUtils.SetActiveAssetDirty();
                    ShufflingMenu(t);
                }
            });

            return root.root;
        }
    }
}