using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using VTasks;

namespace VelvieR
{
    [System.Serializable]
    public enum MenuAnimType
    {
        SlideDown,
        SlideUp,
        SlideLeft,
        SlideRight,
        ZoomIn,
        ZoomOut,
        Alpha,
        RotateZoomIn,
        RotateZoomOut,
        None
    }
    public class VMenuOption : MenuOption
    {
        [SerializeField] private MenuAnimType animType = MenuAnimType.Alpha;
        [SerializeField] private TMP_Text txtComponent;
        [SerializeField] private GameObject parentPanel;
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private LeanTweenType ease = LeanTweenType.easeInOutQuad;
        [SerializeField] private List<Button> buttons = new List<Button>();        
        [SerializeField, HideInInspector] private string vmenuName = string.Empty;
        [SerializeField, HideInInspector] private string vmenuId = string.Empty;
        [SerializeField, HideInInspector] private RectTransform menuRect;
        public MenuAnimType AnimType{get{return animType;} set{animType = value;}}
        public string VmenuName { get { return vmenuName; } }
        public string VmenuId { get { return vmenuId; } }
        public TMP_Text TxtComponent { get { return txtComponent; } set { txtComponent = value; } }
        public List<Button> Buttons { get { return buttons; } set { buttons = value; } }
        public RectTransform MenuRect { get { return menuRect; } set { menuRect = value; } }
        public float Duration { get { return duration; } set { duration = value; } }
        private Vector2 defRectPos;
        private Vector3 defParentPanelPosition;
        private CanvasGroup canvyg;
        private VTokenSource cts;
        void OnDisable()
        {
            if (cts != null)
            {
                VTokenManager.CancelVToken(cts, true);
                cts = null;
            }
        }
        void OnDestroy()
        {
            if (cts != null)
            {
                VTokenManager.CancelVToken(cts, true);
                cts = null;
            }
        }
        void OnValidate()
        {
            if (parentPanel == null || txtComponent == null)
            {
                GetComponentMenu();
            }
            if (String.IsNullOrEmpty(vmenuName) || vmenuName != gameObject.name)
            {
                vmenuName = gameObject.name;
            }
            if (String.IsNullOrEmpty(vmenuId))
            {
                vmenuId = Guid.NewGuid().ToString() + (int)UnityEngine.Random.Range(1954, int.MaxValue);
            }
            if (menuRect == null)
            {
                menuRect = GetComponent<RectTransform>();
            }
            if (canvyg == null)
            {
                canvyg = GetComponent<CanvasGroup>();
            }
        }
        void Awake()
        {
            VMenuOption.InsertMenuOptions(this);
            defRectPos = menuRect.anchoredPosition;
            defParentPanelPosition = parentPanel.transform.position;

            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i] != null)
                {
                    buttons[i].gameObject.SetActive(false);
                }
            }

            gameObject.SetActive(false);
        }
        public void SetMenuAnim(MenuAnimType animType, bool setOnComplete = false, Action act = null)
        {
            if(animType != MenuAnimType.Alpha)
            {
                canvyg.alpha = 1f;
            }

            if (animType == MenuAnimType.SlideDown)
            {
                var defPos = parentPanel.transform.position;
                LeanMove(parentPanel, duration, new Vector3(defPos.x, defPos.y * 2f, defPos.z), setOnComplete, act);
            }
            else if (animType == MenuAnimType.Alpha)
            {
                float from = 0f;
                float to = 1f;

                if (gameObject.activeInHierarchy)
                {
                    to = 0f;
                    from = 1f;
                }
                else
                {
                    gameObject.SetActive(true);
                }

                VExtension.VTweenAlphaCanvas(canvyg, from, to, duration, setOnComplete, act, ease);
            }
            else if (animType == MenuAnimType.SlideUp)
            {
                var defPos = parentPanel.transform.position;
                LeanMove(parentPanel, duration, new Vector3(defPos.x, defPos.y / 2f, defPos.z), setOnComplete, act);
            }
            else if (animType == MenuAnimType.ZoomIn)
            {
                LeanScale(parentPanel, duration, true, setOnComplete, act);
            }
            else if (animType == MenuAnimType.ZoomOut)
            {
                LeanScale(parentPanel, duration, false, setOnComplete, act);
            }
            else if(animType == MenuAnimType.SlideLeft)
            {
                var defPos = parentPanel.transform.position;
                LeanMove(parentPanel, duration, new Vector3(defPos.x / 2f, defPos.y, defPos.z), setOnComplete, act);
            }
            else if(animType == MenuAnimType.SlideRight)
            {
                var defPos = parentPanel.transform.position;
                LeanMove(parentPanel, duration, new Vector3(defPos.x * 2f, defPos.y, defPos.z), setOnComplete, act);
            }
            else if(animType == MenuAnimType.RotateZoomIn)
            {
                parentPanel.transform.localScale = Vector3.zero;
                parentPanel.transform.eulerAngles = new Vector3(0f, 0f, 100f);

                LeanScale(parentPanel, duration, false, setOnComplete, act);
                LeanTween.value(parentPanel, 100f, 0f, duration).setOnUpdate((float val)=>
                {
                    parentPanel.transform.eulerAngles = new Vector3(0f, 0f, val);
                }).setEase(ease).setOnComplete(()=>
                {
                    parentPanel.transform.eulerAngles = Vector3.zero;
                });
            }
            else if(animType == MenuAnimType.RotateZoomOut)
            {
                parentPanel.transform.eulerAngles = new Vector3(0f, 0f, 100f);

                LeanScale(parentPanel, duration, true, setOnComplete, act);
                LeanTween.value(parentPanel, 100f, 0f, duration).setOnUpdate((float val)=>
                {
                    parentPanel.transform.eulerAngles = new Vector3(0f, 0f, val);
                }).setEase(ease).setOnComplete(()=>
                {
                    parentPanel.transform.eulerAngles = Vector3.zero;
                });
            }
            else
            {
                if (!gameObject.activeInHierarchy)
                {
                    gameObject.SetActive(true);
                }
            }
        }
        private void LeanMove(GameObject obj, float duration, Vector3 from, bool setOnComplete = false, Action act = null)
        {
            VExtension.CheckTweensToCancel(obj);

            obj.transform.position = from;

            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }

            LeanTween.move(obj, defParentPanelPosition, duration).setOnComplete(() =>
            {
                obj.transform.position = defParentPanelPosition;

                if (setOnComplete && act != null)
                {
                    act.Invoke();
                }

            }).setEase(ease);
        }
        private void LeanScale(GameObject obj, float duration, bool zoomIn, bool setOnComplete = false, Action act = null)
        {
            VExtension.CheckTweensToCancel(parentPanel);

            if (zoomIn)
            {
                obj.transform.localScale = obj.transform.localScale * 2f;
            }
            else
            {
                obj.transform.localScale = obj.transform.localScale / 2f;
            }

            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }

            LeanTween.scale(obj, Vector3.one, duration).setEase(ease).setOnComplete(() =>
            {
                obj.transform.localScale = Vector3.one;

                if (setOnComplete && act != null)
                {
                    act.Invoke();
                }
            });
        }
        private void GetComponentMenu()
        {
            var comps = GetComponentsInChildren<Transform>();

            buttons = new List<Button>();

            foreach (var child in comps)
            {
                if (child == null)
                    continue;

                if (child.name.Contains("Button"))
                {
                    buttons.Add(child.gameObject.GetComponent<Button>());
                }
                else if (child.name.Equals("optionContent"))
                {
                    txtComponent = child.gameObject.GetComponent<TMP_Text>();
                }
                else if (child.name.Equals("Panel"))
                {
                    parentPanel = child.gameObject;
                }
            }

            canvyg = GetComponent<CanvasGroup>();
        }
        public void PrepMenu(VMenuOption menu, List<VMenuPools> vmenuPools, string mainText, Action next)
        {
            Canvas.ForceUpdateCanvases();
            var getButtons = buttons.FindAll(x => x != null && !x.gameObject.activeInHierarchy);
            List<Button> btns = new List<Button>();

            if (!String.IsNullOrEmpty(mainText))
            {
                if (!txtComponent.gameObject.transform.parent.gameObject.activeInHierarchy)
                {
                    txtComponent.gameObject.transform.parent.gameObject.SetActive(true);
                }

                txtComponent.SetText(mainText);
            }
            else
            {
                txtComponent.gameObject.transform.parent.gameObject.SetActive(false);
            }

            for (int i = 0; i < buttons.Count; i++)
            {
                if (vmenuPools[i] == null || vmenuPools[i].vgraph == null || String.IsNullOrEmpty(vmenuPools[i].vnode) || vmenuPools[i].counter != 0)
                {
                    continue;
                }

                int idx = i;

                btns.Add(buttons[i]);
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].GetComponentInChildren<TMP_Text>().SetText(vmenuPools[i].menuText);
                buttons[i].gameObject.SetActive(true);
                //Debug.Log("index = " + i + " NodeName " + vmenuPools[i].vnode + " id = " + vmenuPools[i].vnodeId);

                buttons[i].onClick.AddListener(() =>
                {
                    MenuOption.AddRemoveToActiveMenu(menu, false);
                    GetVBlockLabel(vmenuPools[idx].vgraph, vmenuPools[idx].vnode, !String.IsNullOrEmpty(vmenuPools[idx].jumpToLabel), vmenuPools[idx].jumpToLabel, btns, next);
                    if(vmenuPools[idx].exclude)
                    {
                        vmenuPools[idx].counter++;
                    }
                });

                if (i == vmenuPools.Count - 1)
                {
                    break;
                }
            }

            if(btns.Count == 0)
            {
                throw new Exception("Menu is not valid! Check for missing properties, make sure required fields are not left empty.");
            }

            MenuOption.AddRemoveToActiveMenu(menu, true);
            SetMenuAnim(animType);
        }
        private async void GetVBlockLabel(VCoreUtil vgraph, string vnodeName, bool jumpToLbl, string labelName, List<Button> btns, Action next = null)
        {
            if (vgraph == null || String.IsNullOrEmpty(vnodeName))
            {
                return;
            }

            string id = string.Empty;
            List<VelvieBlockComponent> vlist = new List<VelvieBlockComponent>();
            VelvieBlockComponent vb = null;

            foreach (var vblock in vgraph.vBlockCores)
            {
                if (vblock.vblocks.Count == 0)
                    continue;

                // Need to always check this ahead due to how the dialog window behaves
                //TODO: CHECK THIS ON EDITOR CODES! MUCH FASTER AND BETTER!
                foreach (var blocks in vblock.vblocks)
                {
                    if (blocks != null && blocks.vnodeName == vnodeName && blocks.enable)
                    {
                        if (!jumpToLbl)
                        {
                            var currentIndexIsDialog = vgraph.ThisIndexIsSayDialog(blocks.vnodeId);

                            if (currentIndexIsDialog != null)
                            {
                                cts = new VTokenSource();
                                var pauseCtoke = cts.Token;
                                VTokenManager.PoolVToken(cts);

                                try
                                {
                                    while (currentIndexIsDialog.VDialogue.gameObject.LeanIsTweening())
                                    {
                                        if (pauseCtoke.IsCancellationRequested)
                                        {
                                            break;
                                        }
                                        await Task.Delay(1, cancellationToken: pauseCtoke);
                                    }
                                }
                                catch (Exception e)
                                {
                                    if (e is MissingReferenceException || e is NullReferenceException)
                                        return;
                                    else
                                        throw e;
                                }

                                VTokenManager.CancelVToken(cts, true);
                                cts = null;
                            }

                            id = blocks.vnodeId;
                            break;
                        }
                        else
                        {
                            var vlabel = blocks.attachedComponent.component as JumpLabel;

                            if (vlabel != null && vlabel.Label == name && blocks.enable)
                            {
                                vlist = vblock.vblocks;
                                vb = blocks;
                                break;
                            }
                        }
                    }
                }

                if (!String.IsNullOrEmpty(id) || vb != null)
                {
                    break;
                }
            }

            if (!String.IsNullOrEmpty(id) && !jumpToLbl)
            {
                LeanTween.alphaCanvas(canvyg, 0f, duration).setOnComplete(() =>
                {
                    gameObject.SetActive(false);
                }).setEase(LeanTweenType.easeInOutQuad);

                vgraph.OnCall(id);
            }
            else
            {
                LeanTween.alphaCanvas(canvyg, 0f, duration).setOnComplete(() =>
                {
                    gameObject.SetActive(false);
                }).setEase(LeanTweenType.easeInOutQuad);

                vgraph.vblockmanager.StartExecutingVBlock(vlist, true, vb);
            }
        }
    }
}