using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using System.Linq;

namespace VelvieR
{
    [System.Serializable]
    public enum InventoryWhenZero
    {
        DisableWhenZeroAmount,
        HideWhenZeroAmount,
        None
    }
    [System.Serializable]
    public enum SortType
    {
        ByName,
        ByNameReverse,
        None
    }
    
    public class VInventory : MonoBehaviour
    {
        [SerializeField] public List<VInventoryClass> inventory = new List<VInventoryClass>();
        [SerializeField] public List<VInventoryCategory> category = new List<VInventoryCategory>();
        [SerializeField] private float scrollMultiplier = 1000;
        [SerializeField] private Scrollbar mainScrollbar;
        [SerializeField] private RectTransform contentToScroll;
        [SerializeField] private GameObject contentTemplate;
        [SerializeField] private GameObject headerContentTemplate;
        [SerializeField] private CanvasGroup canvyg;
        [SerializeField] private float showDuration = 2f;
        [SerializeField, HideInInspector] public Image imgContainer;
        [SerializeField, HideInInspector] public VInventoryItemEvent vevent;
        [SerializeField, HideInInspector] public GameObject leftContainer;
        [SerializeField, HideInInspector] public GameObject subHeader;
        [SerializeField, HideInInspector] public TMP_Text subHeaderText;
        [SerializeField, HideInInspector] public VInventoryCategory setDefault;
        [SerializeField, HideInInspector] public List<Button> actionButtons;
        [SerializeField, HideInInspector] public bool disableWhenItemZero = false;
        [SerializeField, HideInInspector] public InventoryWhenZero itemWhenZero = InventoryWhenZero.None;
        [SerializeField, HideInInspector] public GameObject confirmBox;
        [SerializeField, HideInInspector] public TMP_Text confirmBoxText;
        [SerializeField, HideInInspector] public OnEnableDisable confirmEvent;
        [SerializeField, HideInInspector] public GameObject btnHost;
        [SerializeField, HideInInspector] public bool inventoryIsMixable = false;
        [SerializeField, HideInInspector] public VInventoryCategory activeCategory;
        [SerializeField, HideInInspector] public SortType itemSortType = SortType.None;
        void OnValidate()
        {
            if (mainScrollbar == null || contentToScroll == null || subHeader == null || subHeaderText == null
            || contentTemplate == null || headerContentTemplate == null || imgContainer == null || leftContainer == null || actionButtons == null)
            {
                foreach (Transform child in transform.GetComponentsInChildren<Transform>())
                {
                    if (child.name == "headerTemplate")
                    {
                        headerContentTemplate = child.gameObject;
                    }
                    else if (child.name == "imgTemplate")
                    {
                        contentTemplate = child.gameObject;
                        var getcomp = contentTemplate.GetComponent<VInventoryItemEvent>();
                        getcomp.vin = this; 
                    }
                    else if (child.name == "Scrollbar")
                    {
                        mainScrollbar = child.gameObject.GetComponent<Scrollbar>();
                    }
                    else if (child.name == "ContentScroll")
                    {
                        contentToScroll = child.gameObject.GetComponent<RectTransform>();
                    }
                    else if (child.name == "LeftContainer")
                    {
                        imgContainer = child.gameObject.GetComponent<Image>();
                    }
                    else if(child.name == "leftContentBanner")
                    {
                        leftContainer = child.gameObject;
                    }
                    else if(child.name == "SubHeader")
                    {
                        subHeader = child.gameObject;
                        subHeaderText = subHeader.GetComponentInChildren<TMP_Text>();
                    }
                    else if(child.name == "ConfirmBox")
                    {
                        confirmBox = child.gameObject;

                        foreach(var childTrans in confirmBox.transform.GetComponentsInChildren<Transform>())
                        {
                            if(childTrans.name == "Text (TMP)-title")
                            {
                                confirmBoxText = childTrans.gameObject.GetComponent<TMP_Text>();
                            }
                        }

                        confirmBoxText = confirmBox.GetComponentInChildren<TMP_Text>();
                    }
                    else if(child.name == "ButtonHost")
                    {
                        var btns = child.GetComponentsInChildren<Button>();
                        actionButtons = btns.ToList();
                    }
                    else if(child.name == "imgPanel(Hide this)")
                    {
                        confirmEvent = child.gameObject.GetComponent<OnEnableDisable>();
                    }
                }

                for(int i = 0; i < actionButtons.Count; i++)
                {
                    if(confirmEvent != null && actionButtons[i] != null)
                    actionButtons[i].gameObject.GetComponent<InventoryEvents>().confirmEvent = confirmEvent;
                }
            }

            if (canvyg == null)
            {
                canvyg = gameObject.GetComponent<CanvasGroup>();
            }

            if (vevent == null)
            {
                vevent = contentTemplate.GetComponent<VInventoryItemEvent>();
                vevent.imgContainer = imgContainer;
            }

            if(vevent.textContainer == null)
            {
                vevent.textContainer = leftContainer.GetComponentInChildren<TMP_Text>();
            }
            
        }
        void Start()
        {
            if(!VPropsManager.vInventoryClass.Contains(this))
                VPropsManager.vInventoryClass.Add(this);
            
            contentTemplate.SetActive(false);
            headerContentTemplate.SetActive(false);
            defYscroll = contentToScroll.anchoredPosition.y;
            mainScrollbar.onValueChanged.AddListener((x) => OnScrollValueChanged(x));
            gameObject.SetActive(false);

            if(VPropsManager.vinventory.Count == 0)
            {
                VPropsManager.vinventory = inventory;
            }

            if(VPropsManager.vinventorycategory.Count == 0)
            {
                VPropsManager.vinventorycategory = category;
            }

            if(VPropsManager.lastClickedCategory == null)
            {
                VPropsManager.SetCategoryClassification(setDefault);
            }
            else
            {
                VPropsManager.SetCategoryClassification(VPropsManager.lastClickedCategory);
            }
        }
        void OnDisable()
        {
            contentToScroll.anchoredPosition = new Vector3(contentToScroll.anchoredPosition.x, defYscroll);
            isActive = false;
            VPropsManager.lastClickedInventory = null;
        }

        private bool isActive = false;
        public void OpenCloseInventory()
        {
            isActive = !isActive;

            if (!isActive)
            {
                if (gameObject.LeanIsTweening())
                {
                    LeanTween.cancel(gameObject, true);
                }

                canvyg.alpha = 1f;

                LeanTween.alphaCanvas(canvyg, 0f, showDuration).setOnComplete(() =>
                {
                    gameObject.SetActive(false);
                    isActive = false;
                });
            }
            else
            {
                gameObject.SetActive(true);

                if (gameObject.LeanIsTweening())
                {
                    LeanTween.cancel(gameObject, true);
                }

                canvyg.alpha = 0f;
                LeanTween.alphaCanvas(canvyg, 1f, showDuration);
            }
        }
        private float defYscroll;
        private void OnScrollValueChanged(float val)
        {
            contentToScroll.anchoredPosition = new Vector3(contentToScroll.anchoredPosition.x, defYscroll + (val * scrollMultiplier));
        }
        public GameObject LastCreated { get; set; }
        public void CreateCategory(string cname, string cdescription)
        {
            GameObject go = Instantiate(headerContentTemplate, headerContentTemplate.transform.position, Quaternion.identity);
            go.transform.SetParent(headerContentTemplate.transform.parent.transform, false);

            var cat = new VInventoryCategory();
            cat.categoryObject = go;
            cat.tmp = go.GetComponentInChildren<TMP_Text>();
            cat.guid = (int)UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            cat.name = cname;
            cat.description = cdescription;

            category.Add(cat);

            LastCreated = go;
            go.SetActive(true);

            var vcat = go.GetComponent<VCategoryEvents>();
            vcat.vcategory = cat;
            vcat.vin = this;
        }
        public void CreateItem(string iname, string idescription, VInventoryCategory vcat)
        {
            if (String.IsNullOrEmpty(iname))
            {
                return;
            }

            GameObject go = Instantiate(contentTemplate, contentTemplate.transform.position, Quaternion.identity);
            go.transform.SetParent(contentToScroll.transform, false);

            var iv = new VInventoryClass();
            iv.inventoryObject = go;
            iv.tmp = go.GetComponentInChildren<TMP_Text>();
            iv.name = iname;
            iv.category = vcat;
            iv.description = idescription;
            iv.SetGuid();

            var comp = go.GetComponent<VInventoryItemEvent>();
            inventory.Add(iv);

            comp.vinventory = inventory[inventory.Count - 1];
            go.SetActive(true);
        }

        public void CreateItemSlot(string inventoryClassName, string slotName, string sdesc = "", int sinitialValue = default, System.Action onCreated = null)
        {
            if (String.IsNullOrEmpty(inventoryClassName) || String.IsNullOrEmpty(slotName))
            {
                return;
            }

            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] == null || inventory[i].name != inventoryClassName)
                    continue;

                inventory[i].CreateSlot(slotName, sdesc, sinitialValue);
                inventory[i].slots[inventory[i].slots.Count - 1].id = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

                if (onCreated != null)
                {
                    onCreated.Invoke();
                }

                break;
            }
        }
        public void RemoveItemSlot(string inventoryClassName, string slotName, Action onDeleted = null)
        {
            if (String.IsNullOrEmpty(slotName))
            {
                return;
            }

            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] == null || inventory[i].name != inventoryClassName)
                    continue;

                inventory[i].RemoveSlot(slotName);

                if (onDeleted != null)
                {
                    onDeleted.Invoke();
                }

                break;
            }
        }
        public void UpdateCategorySprite(GameObject obj, Sprite sprite)
        {
            if (obj == null)
                return;

            if (sprite != null)
            {
                var img = obj.GetComponent<Image>();
                img.preserveAspect = true;
                img.sprite = sprite;
            }
        }
        public void UpdateCategoryName(GameObject obj, string str)
        {
            if (obj == null)
                return;

            obj.GetComponentInChildren<TMP_Text>().SetText(str);
        }
        #if UNITY_EDITOR
        public void AddCatTest()
        {
            CreateCategory("Category", "Description");
        }
        public void AddItemTest()
        {
            CreateItem("Item", "testing", null);
        }
        #endif
    }

}