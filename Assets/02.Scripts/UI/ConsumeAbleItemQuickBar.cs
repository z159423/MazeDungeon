using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using TMPro;

public class ConsumeAbleItemQuickBar : MonoBehaviour
{
    public PlayerStats playerStats;

    public float itemQuickChangeTime = 1f;
    [Space]

    public TextMeshProUGUI First_QuickBar_Key;
    public MeshFilter First_QuickBar_filter;
    public MeshRenderer First_QuickBar_meshRenderer;
    public Image First_ItemQuickChangePanel;
    public Image first_ShortcutKeyImage;
    [SerializeField] private Item First_QuickBarItem;
    [SerializeField] private float First_itemQuickChangeCurrentTime = 0;
    [SerializeField] public bool First_itemQuickChangeOn = false;

    [SerializeField] private Item First_one_QuickChangeBarItem;
    public MeshFilter First_one_QuickChange_filter;
    public MeshRenderer First_one_QuickChange_meshRenderer;

    [SerializeField] private Item First_two_QuickChangeBarItem;
    public MeshFilter First_two_QuickChange_filter;
    public MeshRenderer First_two_QuickChange_meshRenderer;

    public Slider FirstQuickBarSlider;

    [Space]

    public TextMeshProUGUI Second_QuickBar_Key;
    public MeshFilter Second_QuickBar_filter;
    public MeshRenderer Second_QuickBar_meshRenderer;
    public Image Second_ItemQuickChangePanel;
    public Image second_ShortcutKeyImage;
    [SerializeField] private Item Second_QuickBarItem;
    [SerializeField] private float Second_itemQuickChangeCurrentTime = 0;
    [SerializeField] public bool Second_itemQuickChangeOn = false;

    [SerializeField] private Item Second_one_QuickChangeBarItem;
    public MeshFilter Second_one_QuickChange_filter;
    public MeshRenderer Second_one_QuickChange_meshRenderer;

    [SerializeField] private Item Second_two_QuickChangeBarItem;
    public MeshFilter Second_two_QuickChange_filter;
    public MeshRenderer Second_two_QuickChange_meshRenderer;

    public Slider SecondQuickBarSlider;

    [SerializeField] private Queue<Item> InventoryConsumeItem = new Queue<Item>();

    public static ConsumeAbleItemQuickBar instance;

    private PlayerInputAction playerInputActions;

    public bool FirstSlotHold = false;
    public bool SecondSlotHold = false;

    public float scroll;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found!");
            return;
        }
        instance = this;

        InitialKeyBinds();

        KeyBindindManager.instance.OnUpdateKeyBindsCallBack += UpdateKeyBind;

        KeyBindindManager.instance.DisplayCurrentControllerShortcut(First_QuickBar_Key, first_ShortcutKeyImage, playerInputActions.UI.QuickUseFirstSlot);
        KeyBindindManager.instance.DisplayCurrentControllerShortcut(Second_QuickBar_Key, second_ShortcutKeyImage, playerInputActions.UI.QuickUseSecondSlot);
    }

    private void InitialKeyBinds()
    {
        playerInputActions = new PlayerInputAction();

        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            playerInputActions.LoadBindingOverridesFromJson(rebinds);

        playerInputActions.UI.QuickUseFirstSlot.performed += context =>
        {
            if (context.interaction is TapInteraction)
            {
                if (First_QuickBarItem)
                {
                    bool succese = First_QuickBarItem.Use();

                    if (succese)
                    {
                        print("퀵슬롯으로 소모품 사용");
                        Inventory.instance.RemoveItemObject(First_QuickBarItem);
                        Inventory.instance.Remove(First_QuickBarItem);

                        if (Inventory.instance.onInventoryChangedCallback != null)
                        {
                            Inventory.instance.onInventoryChangedCallback.Invoke();
                        }
                    }
                    else
                    {
                        print("소모품을 사용하지 못했습니다.");
                    }
                }
            }
        };

        playerInputActions.UI.QuickUseFirstHold.performed += UseFirstQuickHold;

        playerInputActions.UI.QuickUseFirstHold.canceled += context => {
            if (context.interaction is HoldInteraction)
            {
                First_ItemQuickChangePanel.gameObject.SetActive(false);
                First_itemQuickChangeOn = false;
            }
        };

        playerInputActions.UI.QuickUseSecondSlot.performed += context => {
            if (context.interaction is TapInteraction)
            {
                if (Second_QuickBarItem)
                {
                    bool succese = Second_QuickBarItem.Use();

                    if (succese)
                    {
                        print("퀵슬롯으로 소모품 사용");
                        Inventory.instance.RemoveItemObject(Second_QuickBarItem);
                        Inventory.instance.Remove(Second_QuickBarItem);

                        if (Inventory.instance.onInventoryChangedCallback != null)
                        {
                            Inventory.instance.onInventoryChangedCallback.Invoke();
                        }
                    }
                    else
                    {
                        print("Full HP라 소모품을 사용하지 못했습니다.");
                    }
                }
            }
        };

        playerInputActions.UI.QuickUseSecondHold.performed += context => {
            if (context.interaction is HoldInteraction)
            {
                if (!First_itemQuickChangeOn)
                {
                    Second_ItemQuickChangePanel.gameObject.SetActive(true);
                    Second_itemQuickChangeOn = true;

                    InventoryConsumeItem.Clear();

                    foreach (Item item in Inventory.instance.items)
                    {
                        if (item.itemtype == ItemType.consumable)
                        {
                            InventoryConsumeItem.Enqueue(item);
                        }
                    }

                    for (int i = 0; i < InventoryConsumeItem.Count; i++)
                    {
                        var item = InventoryConsumeItem.Dequeue();
                        InventoryConsumeItem.Enqueue(item);

                        if (item != First_QuickBarItem
                            && item != Second_QuickBarItem
                            && item != Second_two_QuickChangeBarItem)
                        {
                            Second_one_QuickChange_filter.mesh = item.skinedMesh.sharedMesh;
                            Second_one_QuickChange_meshRenderer.materials = item.skinedMesh.sharedMaterials;
                            Second_one_QuickChangeBarItem = item;

                            break;
                        }
                    }

                    for (int i = 0; i < InventoryConsumeItem.Count; i++)
                    {
                        var item = InventoryConsumeItem.Dequeue();
                        InventoryConsumeItem.Enqueue(item);

                        if (item != First_QuickBarItem
                            && item != Second_QuickBarItem
                            && item != Second_one_QuickChangeBarItem)
                        {
                            Second_two_QuickChange_filter.mesh = item.skinedMesh.sharedMesh;
                            Second_two_QuickChange_meshRenderer.materials = item.skinedMesh.sharedMaterials;
                            Second_two_QuickChangeBarItem = item;

                            break;
                        }
                    }
                }
            }
        };

        playerInputActions.UI.QuickUseSecondHold.canceled += context => {
            if (context.interaction is HoldInteraction)
            {
                Second_ItemQuickChangePanel.gameObject.SetActive(false);
                Second_itemQuickChangeOn = false;
            }
        };

        playerInputActions.Player.ZoomCamara.performed += ChangeQuickBar;

        playerInputActions.Enable();
    }

    private void UpdateKeyBind()
    {
        playerInputActions.Disable();

        //KeyBindindManager.instance.DisplayCurrentControllerShortcut(First_QuickBar_Key, first_ShortcutKeyImage, playerInputActions.UI.QuickUseFirstSlot);
        //KeyBindindManager.instance.DisplayCurrentControllerShortcut(Second_QuickBar_Key, second_ShortcutKeyImage, playerInputActions.UI.QuickUseSecondSlot);

        OnChangeControl();

        InitialKeyBinds();
    }

    // Start is called before the first frame update
    void Start()
    {
        Inventory.instance.onInventoryChangedCallback += OnInventoryChange;

        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerStats>();
        FirstQuickBarSlider.maxValue = playerStats.HpPotionCoolTime;
        SecondQuickBarSlider.maxValue = playerStats.HpPotionCoolTime;

    }

    

    private void Update()
    {
        //Vector2 scroll2 = playerInputActions.Player.Wheel1.ReadValue<Vector2>();

        //Debug.Log(Mouse.current.scroll.ReadValue().normalized);
        //Debug.Log(playerInputActions.Player.Wheel1.ReadValue<Vector2>().normalized);

        FirstQuickBarSlider.value = playerStats.CurrentHpPotionCoolTime;
        SecondQuickBarSlider.value = playerStats.CurrentHpPotionCoolTime;

        /*if (Input.GetButton("QuickUseFirstSlot"))
        {
            First_itemQuickChangeCurrentTime += Time.deltaTime;

            if (First_itemQuickChangeCurrentTime > itemQuickChangeTime && !First_itemQuickChangeOn)
            {
                
                First_ItemQuickChangePanel.gameObject.SetActive(true);
                First_itemQuickChangeOn = true;

                InventoryConsumeItem.Clear();

                foreach (Item item in Inventory.instance.items)
                {
                    if (item.itemtype == ItemType.consumable)
                    {
                        InventoryConsumeItem.Enqueue(item);
                    }
                }

                for(int i = 0; i < InventoryConsumeItem.Count; i++)
                {
                    var item = InventoryConsumeItem.Dequeue();
                    InventoryConsumeItem.Enqueue(item);

                    if (item != First_QuickBarItem
                        && item != Second_QuickBarItem
                        && item != First_two_QuickChangeBarItem)
                    {

                        First_one_QuickChange_filter.mesh = item.skinedMesh.sharedMesh;
                        First_one_QuickChange_meshRenderer.materials = item.skinedMesh.sharedMaterials;
                        First_one_QuickChangeBarItem = item;

                        break;
                    }
                }

                for (int i = 0; i < InventoryConsumeItem.Count; i++)
                {
                    var item = InventoryConsumeItem.Dequeue();
                    InventoryConsumeItem.Enqueue(item);

                    if (item != First_QuickBarItem
                        && item != Second_QuickBarItem
                        && item != First_one_QuickChangeBarItem)
                    {

                        First_two_QuickChange_filter.mesh = item.skinedMesh.sharedMesh;
                        First_two_QuickChange_meshRenderer.materials = item.skinedMesh.sharedMaterials;
                        First_two_QuickChangeBarItem = item;

                        break;
                    }
                }

            }

            if (First_itemQuickChangeCurrentTime > itemQuickChangeTime)
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                {
                    First_two_QuickChange_filter.mesh = First_one_QuickChange_filter.mesh;
                    First_two_QuickChange_meshRenderer.materials = First_one_QuickChange_meshRenderer.materials;
                    First_two_QuickChangeBarItem = First_one_QuickChangeBarItem;

                    First_one_QuickChange_filter.mesh = First_QuickBar_filter.mesh;
                    First_one_QuickChange_meshRenderer.materials = First_QuickBar_meshRenderer.materials;
                    First_one_QuickChangeBarItem = First_QuickBarItem;

                    Item newItem = null;

                    for (int i = 0; i < InventoryConsumeItem.Count; i++)
                    {
                        var item = InventoryConsumeItem.Dequeue();
                        InventoryConsumeItem.Enqueue(item);

                        if (item != First_QuickBarItem
                            && item != Second_QuickBarItem
                            && item != First_one_QuickChangeBarItem
                            && item != First_two_QuickChangeBarItem)
                        {
                            newItem = item;

                            break;
                        }
                    }

                    if(newItem)
                    {
                        First_QuickBar_filter.mesh = newItem.skinedMesh.sharedMesh;
                        First_QuickBar_meshRenderer.materials = newItem.skinedMesh.sharedMaterials;
                        First_QuickBarItem = newItem;
                    }
                    else
                    {
                        First_QuickBar_filter.mesh = null;
                        First_QuickBarItem = null;
                    }
                    
                    *//*for (int i = 0; i < InventoryConsumeItem.Count; i++)
                    {
                        var item = InventoryConsumeItem.Dequeue();
                        InventoryConsumeItem.Enqueue(item);

                        if (item != First_QuickBarItem
                            && item != Second_QuickBarItem
                            && item != First_one_QuickChangeBarItem)
                        {
                            First_QuickBar_filter.mesh = item.skinedMesh.sharedMesh;
                            First_QuickBar_meshRenderer.materials = item.skinedMesh.sharedMaterials;
                            First_QuickBarItem = item;

                            break;
                        }
                    }*//*
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                {

                    First_QuickBar_filter.mesh = First_one_QuickChange_filter.mesh;
                    First_QuickBar_meshRenderer.materials = First_one_QuickChange_meshRenderer.materials;
                    First_QuickBarItem = First_one_QuickChangeBarItem;

                    First_one_QuickChange_filter.mesh = First_two_QuickChange_filter.mesh;
                    First_one_QuickChange_meshRenderer.materials = First_two_QuickChange_meshRenderer.materials;
                    First_one_QuickChangeBarItem = First_two_QuickChangeBarItem;

                    Item newItem = null;

                    for (int i = 0; i < InventoryConsumeItem.Count; i++)
                    {
                        var item = InventoryConsumeItem.Dequeue();
                        InventoryConsumeItem.Enqueue(item);

                        if (item != First_QuickBarItem
                            && item != Second_QuickBarItem
                            && item != First_one_QuickChangeBarItem
                            && item != First_two_QuickChangeBarItem)
                        {
                            newItem = item;

                            break;
                        }
                    }

                    if (newItem)
                    {
                        First_two_QuickChange_filter.mesh = newItem.skinedMesh.sharedMesh;
                        First_two_QuickChange_meshRenderer.materials = newItem.skinedMesh.sharedMaterials;
                        First_two_QuickChangeBarItem = newItem;
                    }
                    else
                    {
                        First_two_QuickChange_filter.mesh = null;
                        First_two_QuickChangeBarItem = null;
                    }

                    *//*for (int i = 0; i < InventoryConsumeItem.Count; i++)
                    {
                        var item = InventoryConsumeItem.Dequeue();
                        InventoryConsumeItem.Enqueue(item);

                        if (item != First_QuickBarItem
                            && item != Second_QuickBarItem
                            && item != First_one_QuickChangeBarItem)
                        {
                            First_two_QuickChange_filter.mesh = item.skinedMesh.sharedMesh;
                            First_two_QuickChange_meshRenderer.materials = item.skinedMesh.sharedMaterials;
                            First_two_QuickChangeBarItem = item;

                            break;
                        }
                    }*//*
                }
            }
            

        }
        else if (Input.GetButtonUp("QuickUseFirstSlot"))
        {
            First_itemQuickChangeCurrentTime = 0;

            if (First_itemQuickChangeOn)
            {
                First_ItemQuickChangePanel.gameObject.SetActive(false);
                First_itemQuickChangeOn = false;

            }
            else if (First_QuickBarItem)
            {
                //CleaningInventorySlot(First_QuickBarItem);

                bool succese = First_QuickBarItem.Use();

                if (succese)
                {
                    print("퀵슬롯으로 소모품 사용");
                    Inventory.instance.RemoveItemObject(First_QuickBarItem);
                    Inventory.instance.Remove(First_QuickBarItem);

                    if (Inventory.instance.onInventoryChangedCallback != null)
                    {
                        Inventory.instance.onInventoryChangedCallback.Invoke();
                    }

                }
                else
                {
                    print("소모품을 사용하지 못했습니다.");
                }
            }
        }
        else if (Input.GetButton("QuickUseSecondSlot"))
        {
            Second_itemQuickChangeCurrentTime += Time.deltaTime;

            if (Second_itemQuickChangeCurrentTime > itemQuickChangeTime && !Second_itemQuickChangeOn)
            {
                Second_ItemQuickChangePanel.gameObject.SetActive(true);
                Second_itemQuickChangeOn = true;

                InventoryConsumeItem.Clear();

                foreach (Item item in Inventory.instance.items)
                {
                    if (item.itemtype == ItemType.consumable)
                    {
                        InventoryConsumeItem.Enqueue(item);
                    }
                }

                for (int i = 0; i < InventoryConsumeItem.Count; i++)
                {
                    var item = InventoryConsumeItem.Dequeue();
                    InventoryConsumeItem.Enqueue(item);

                    if (item != First_QuickBarItem
                        && item != Second_QuickBarItem
                        && item != Second_two_QuickChangeBarItem)
                    {
                        Second_one_QuickChange_filter.mesh = item.skinedMesh.sharedMesh;
                        Second_one_QuickChange_meshRenderer.materials = item.skinedMesh.sharedMaterials;
                        Second_one_QuickChangeBarItem = item;

                        break;
                    }
                }

                for (int i = 0; i < InventoryConsumeItem.Count; i++)
                {
                    var item = InventoryConsumeItem.Dequeue();
                    InventoryConsumeItem.Enqueue(item);

                    if (item != First_QuickBarItem
                        && item != Second_QuickBarItem
                        && item != Second_one_QuickChangeBarItem)
                    {
                        Second_two_QuickChange_filter.mesh = item.skinedMesh.sharedMesh;
                        Second_two_QuickChange_meshRenderer.materials = item.skinedMesh.sharedMaterials;
                        Second_two_QuickChangeBarItem = item;

                        break;
                    }
                }
            }

            if (Second_itemQuickChangeCurrentTime > itemQuickChangeTime)
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                {
                    Second_two_QuickChange_filter.mesh = Second_one_QuickChange_filter.mesh;
                    Second_two_QuickChange_meshRenderer.materials = Second_one_QuickChange_meshRenderer.materials;
                    Second_two_QuickChangeBarItem = Second_one_QuickChangeBarItem;

                    Second_one_QuickChange_filter.mesh = Second_QuickBar_filter.mesh;
                    Second_one_QuickChange_meshRenderer.materials = Second_QuickBar_meshRenderer.materials;
                    Second_one_QuickChangeBarItem = Second_QuickBarItem;

                    Item newItem = null;

                    for (int i = 0; i < InventoryConsumeItem.Count; i++)
                    {
                        var item = InventoryConsumeItem.Dequeue();
                        InventoryConsumeItem.Enqueue(item);

                        if (item != Second_QuickBarItem
                            && item != First_QuickBarItem
                            && item != Second_one_QuickChangeBarItem
                            && item != Second_two_QuickChangeBarItem
                            )
                        {
                            newItem = item;

                            break;
                        }
                    }

                    if (newItem)
                    {
                        Second_QuickBar_filter.mesh = newItem.skinedMesh.sharedMesh;
                        Second_QuickBar_meshRenderer.materials = newItem.skinedMesh.sharedMaterials;
                        Second_QuickBarItem = newItem;
                    }
                    else
                    {
                        Second_QuickBar_filter.mesh = null;
                        Second_QuickBarItem = null;
                    }
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                {

                    Second_QuickBar_filter.mesh = Second_one_QuickChange_filter.mesh;
                    Second_QuickBar_meshRenderer.materials = Second_one_QuickChange_meshRenderer.materials;
                    Second_QuickBarItem = Second_one_QuickChangeBarItem;

                    Second_one_QuickChange_filter.mesh = Second_two_QuickChange_filter.mesh;
                    Second_one_QuickChange_meshRenderer.materials = Second_two_QuickChange_meshRenderer.materials;
                    Second_one_QuickChangeBarItem = Second_two_QuickChangeBarItem;

                    Item newItem = null;

                    for (int i = 0; i < InventoryConsumeItem.Count; i++)
                    {
                        var item = InventoryConsumeItem.Dequeue();
                        InventoryConsumeItem.Enqueue(item);

                        if (item != Second_QuickBarItem
                            && item != First_QuickBarItem
                            && item != Second_one_QuickChangeBarItem
                            && item != Second_two_QuickChangeBarItem)
                        {
                            newItem = item;

                            break;
                        }
                    }

                    if (newItem)
                    {
                        Second_two_QuickChange_filter.mesh = newItem.skinedMesh.sharedMesh;
                        Second_two_QuickChange_meshRenderer.materials = newItem.skinedMesh.sharedMaterials;
                        Second_two_QuickChangeBarItem = newItem;
                    }
                    else
                    {
                        Second_two_QuickChange_filter.mesh = null;
                        Second_two_QuickChangeBarItem = null;
                    }
                }
            }
        }
        else if(Input.GetButtonUp("QuickUseSecondSlot"))
        {
            Second_itemQuickChangeCurrentTime = 0;

            if (Second_itemQuickChangeOn)
            {
                Second_ItemQuickChangePanel.gameObject.SetActive(false);
                Second_itemQuickChangeOn = false;
            }
            else if (Second_QuickBarItem)
            {
                //CleaningInventorySlot(Second_QuickBarItem);

                bool succese = Second_QuickBarItem.Use();

                if (succese)
                {
                    print("퀵슬롯으로 소모품 사용");
                    Inventory.instance.RemoveItemObject(Second_QuickBarItem);
                    Inventory.instance.Remove(Second_QuickBarItem);

                    if (Inventory.instance.onInventoryChangedCallback != null)
                    {
                        Inventory.instance.onInventoryChangedCallback.Invoke();
                    }
                }
                else
                {
                    print("Full HP라 소모품을 사용하지 못했습니다.");
                }
            }
                
        }*/
        
    }

    private void UseFirstQuickTap(InputAction.CallbackContext inputValue)
    {
        print("퀵슬롯으로 소모품 사용");
        if (inputValue.interaction is MultiTapInteraction)
        {
            if (First_QuickBarItem)
            {
                bool succese = First_QuickBarItem.Use();

                if (succese)
                {
                    print("퀵슬롯으로 소모품 사용");
                    Inventory.instance.RemoveItemObject(First_QuickBarItem);
                    Inventory.instance.Remove(First_QuickBarItem);

                    if (Inventory.instance.onInventoryChangedCallback != null)
                    {
                        Inventory.instance.onInventoryChangedCallback.Invoke();
                    }
                }
                else
                {
                    print("소모품을 사용하지 못했습니다.");
                }
            }

        }
    }

    private void UseFirstQuickHold(InputAction.CallbackContext inputValue)
    {
        if (inputValue.interaction is HoldInteraction)
        {
            if (!Second_itemQuickChangeOn)
            {

                First_ItemQuickChangePanel.gameObject.SetActive(true);
                First_itemQuickChangeOn = true;

                InventoryConsumeItem.Clear();

                foreach (Item item in Inventory.instance.items)
                {
                    if (item.itemtype == ItemType.consumable)
                    {
                        InventoryConsumeItem.Enqueue(item);
                    }
                }

                for (int i = 0; i < InventoryConsumeItem.Count; i++)
                {
                    var item = InventoryConsumeItem.Dequeue();
                    InventoryConsumeItem.Enqueue(item);

                    if (item != First_QuickBarItem
                        && item != Second_QuickBarItem
                        && item != First_two_QuickChangeBarItem)
                    {

                        First_one_QuickChange_filter.mesh = item.skinedMesh.sharedMesh;
                        First_one_QuickChange_meshRenderer.materials = item.skinedMesh.sharedMaterials;
                        First_one_QuickChangeBarItem = item;

                        break;
                    }
                }

                for (int i = 0; i < InventoryConsumeItem.Count; i++)
                {
                    var item = InventoryConsumeItem.Dequeue();
                    InventoryConsumeItem.Enqueue(item);

                    if (item != First_QuickBarItem
                        && item != Second_QuickBarItem
                        && item != First_one_QuickChangeBarItem)
                    {

                        First_two_QuickChange_filter.mesh = item.skinedMesh.sharedMesh;
                        First_two_QuickChange_meshRenderer.materials = item.skinedMesh.sharedMaterials;
                        First_two_QuickChangeBarItem = item;

                        break;
                    }
                }

            }
        }
    }

    private void ChangeQuickBar(InputAction.CallbackContext inputValue)
    {
        //Debug.Log(-inputValue.ReadValue<Vector2>().y);

        if (First_itemQuickChangeOn)
        {
            if (-inputValue.ReadValue<Vector2>().y > 0f)
            {
                First_two_QuickChange_filter.mesh = First_one_QuickChange_filter.mesh;
                First_two_QuickChange_meshRenderer.materials = First_one_QuickChange_meshRenderer.materials;
                First_two_QuickChangeBarItem = First_one_QuickChangeBarItem;

                First_one_QuickChange_filter.mesh = First_QuickBar_filter.mesh;
                First_one_QuickChange_meshRenderer.materials = First_QuickBar_meshRenderer.materials;
                First_one_QuickChangeBarItem = First_QuickBarItem;

                Item newItem = null;

                for (int i = 0; i < InventoryConsumeItem.Count; i++)
                {
                    var item = InventoryConsumeItem.Dequeue();
                    InventoryConsumeItem.Enqueue(item);

                    if (item != First_QuickBarItem
                        && item != Second_QuickBarItem
                        && item != First_one_QuickChangeBarItem
                        && item != First_two_QuickChangeBarItem)
                    {
                        newItem = item;

                        break;
                    }
                }

                if (newItem)
                {
                    First_QuickBar_filter.mesh = newItem.skinedMesh.sharedMesh;
                    First_QuickBar_meshRenderer.materials = newItem.skinedMesh.sharedMaterials;
                    First_QuickBarItem = newItem;
                }
                else
                {
                    First_QuickBar_filter.mesh = null;
                    First_QuickBarItem = null;
                }

                /*for (int i = 0; i < InventoryConsumeItem.Count; i++)
                {
                    var item = InventoryConsumeItem.Dequeue();
                    InventoryConsumeItem.Enqueue(item);

                    if (item != First_QuickBarItem
                        && item != Second_QuickBarItem
                        && item != First_one_QuickChangeBarItem)
                    {
                        First_QuickBar_filter.mesh = item.skinedMesh.sharedMesh;
                        First_QuickBar_meshRenderer.materials = item.skinedMesh.sharedMaterials;
                        First_QuickBarItem = item;

                        break;
                    }
                }*/

            }
            else if (-inputValue.ReadValue<Vector2>().y < 0f)
            {

                First_QuickBar_filter.mesh = First_one_QuickChange_filter.mesh;
                First_QuickBar_meshRenderer.materials = First_one_QuickChange_meshRenderer.materials;
                First_QuickBarItem = First_one_QuickChangeBarItem;

                First_one_QuickChange_filter.mesh = First_two_QuickChange_filter.mesh;
                First_one_QuickChange_meshRenderer.materials = First_two_QuickChange_meshRenderer.materials;
                First_one_QuickChangeBarItem = First_two_QuickChangeBarItem;

                Item newItem = null;

                for (int i = 0; i < InventoryConsumeItem.Count; i++)
                {
                    var item = InventoryConsumeItem.Dequeue();
                    InventoryConsumeItem.Enqueue(item);

                    if (item != First_QuickBarItem
                        && item != Second_QuickBarItem
                        && item != First_one_QuickChangeBarItem
                        && item != First_two_QuickChangeBarItem)
                    {
                        newItem = item;

                        break;
                    }
                }

                if (newItem)
                {
                    First_two_QuickChange_filter.mesh = newItem.skinedMesh.sharedMesh;
                    First_two_QuickChange_meshRenderer.materials = newItem.skinedMesh.sharedMaterials;
                    First_two_QuickChangeBarItem = newItem;
                }
                else
                {
                    First_two_QuickChange_filter.mesh = null;
                    First_two_QuickChangeBarItem = null;
                }

                for (int i = 0; i < InventoryConsumeItem.Count; i++)
                {
                    var item = InventoryConsumeItem.Dequeue();
                    InventoryConsumeItem.Enqueue(item);

                    if (item != First_QuickBarItem
                        && item != Second_QuickBarItem
                        && item != First_one_QuickChangeBarItem)
                    {
                        First_two_QuickChange_filter.mesh = item.skinedMesh.sharedMesh;
                        First_two_QuickChange_meshRenderer.materials = item.skinedMesh.sharedMaterials;
                        First_two_QuickChangeBarItem = item;

                        break;
                    }
                }
            }
        }
        else if(Second_itemQuickChangeOn)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                Second_two_QuickChange_filter.mesh = Second_one_QuickChange_filter.mesh;
                Second_two_QuickChange_meshRenderer.materials = Second_one_QuickChange_meshRenderer.materials;
                Second_two_QuickChangeBarItem = Second_one_QuickChangeBarItem;

                Second_one_QuickChange_filter.mesh = Second_QuickBar_filter.mesh;
                Second_one_QuickChange_meshRenderer.materials = Second_QuickBar_meshRenderer.materials;
                Second_one_QuickChangeBarItem = Second_QuickBarItem;

                Item newItem = null;

                for (int i = 0; i < InventoryConsumeItem.Count; i++)
                {
                    var item = InventoryConsumeItem.Dequeue();
                    InventoryConsumeItem.Enqueue(item);

                    if (item != Second_QuickBarItem
                        && item != First_QuickBarItem
                        && item != Second_one_QuickChangeBarItem
                        && item != Second_two_QuickChangeBarItem
                        )
                    {
                        newItem = item;

                        break;
                    }
                }

                if (newItem)
                {
                    Second_QuickBar_filter.mesh = newItem.skinedMesh.sharedMesh;
                    Second_QuickBar_meshRenderer.materials = newItem.skinedMesh.sharedMaterials;
                    Second_QuickBarItem = newItem;
                }
                else
                {
                    Second_QuickBar_filter.mesh = null;
                    Second_QuickBarItem = null;
                }
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {

                Second_QuickBar_filter.mesh = Second_one_QuickChange_filter.mesh;
                Second_QuickBar_meshRenderer.materials = Second_one_QuickChange_meshRenderer.materials;
                Second_QuickBarItem = Second_one_QuickChangeBarItem;

                Second_one_QuickChange_filter.mesh = Second_two_QuickChange_filter.mesh;
                Second_one_QuickChange_meshRenderer.materials = Second_two_QuickChange_meshRenderer.materials;
                Second_one_QuickChangeBarItem = Second_two_QuickChangeBarItem;

                Item newItem = null;

                for (int i = 0; i < InventoryConsumeItem.Count; i++)
                {
                    var item = InventoryConsumeItem.Dequeue();
                    InventoryConsumeItem.Enqueue(item);

                    if (item != Second_QuickBarItem
                        && item != First_QuickBarItem
                        && item != Second_one_QuickChangeBarItem
                        && item != Second_two_QuickChangeBarItem)
                    {
                        newItem = item;

                        break;
                    }
                }

                if (newItem)
                {
                    Second_two_QuickChange_filter.mesh = newItem.skinedMesh.sharedMesh;
                    Second_two_QuickChange_meshRenderer.materials = newItem.skinedMesh.sharedMaterials;
                    Second_two_QuickChangeBarItem = newItem;
                }
                else
                {
                    Second_two_QuickChange_filter.mesh = null;
                    Second_two_QuickChangeBarItem = null;
                }
            }
        }
    }

    private void OnInventoryChange()
    {
        InventoryConsumeItem.Clear();

        if (First_QuickBarItem)
        {
            if (!Inventory.instance.items.Contains(First_QuickBarItem))
            {
                First_QuickBarItem = null;
                First_QuickBar_filter.mesh = null;
                // First_QuickBar_meshRenderer.materials = null;
            }
        }

        if (Second_QuickBarItem)
        {
            if (!Inventory.instance.items.Contains(Second_QuickBarItem))
            {
                Second_QuickBarItem = null;
                Second_QuickBar_filter.mesh = null;
                //Second_QuickBar_meshRenderer.materials = null;
            }
        }

        First_one_QuickChangeBarItem = null;
        First_one_QuickChange_filter.mesh = null;
        First_two_QuickChangeBarItem = null;
        First_two_QuickChange_filter.mesh = null;
        Second_one_QuickChangeBarItem = null;
        Second_one_QuickChange_filter.mesh = null;
        Second_two_QuickChangeBarItem = null;
        Second_two_QuickChange_filter.mesh = null;

        /*if(!Inventory.instance.items.Contains(First_one_QuickChangeBarItem))
        {
            First_one_QuickChangeBarItem = null;
            First_one_QuickChange_filter.mesh = null;
            //First_one_QuickChange_meshRenderer.materials = null;
        }

        if (!Inventory.instance.items.Contains(First_two_QuickChangeBarItem))
        {
            First_two_QuickChangeBarItem = null;
            First_two_QuickChange_filter.mesh = null;
            //First_two_QuickChange_meshRenderer.materials = null;
        }

        if (!Inventory.instance.items.Contains(Second_one_QuickChangeBarItem))
        {
            Second_one_QuickChangeBarItem = null;
            Second_one_QuickChange_filter.mesh = null;
            //Second_one_QuickChange_meshRenderer.materials = null;

        }

        if (!Inventory.instance.items.Contains(Second_two_QuickChangeBarItem))
        {
            Second_two_QuickChangeBarItem = null;
            Second_two_QuickChange_filter.mesh = null;
            //Second_two_QuickChange_meshRenderer.materials = null;
        }*/

        if (!First_QuickBarItem)
        {
            foreach (Item item in Inventory.instance.items)
            {
                if (item.itemtype == ItemType.consumable
                    && item != First_QuickBarItem
                    && item != Second_QuickBarItem)
                {
                    First_QuickBar_filter.mesh = item.skinedMesh.sharedMesh;
                    First_QuickBar_meshRenderer.materials = item.skinedMesh.sharedMaterials;
                    First_QuickBarItem = item;

                    break;
                }
            }
        }
        else if (!Second_QuickBarItem)
        {
            foreach (Item item in Inventory.instance.items)
            {
                if (item.itemtype == ItemType.consumable
                    && item != First_QuickBarItem
                    && item != Second_QuickBarItem)
                {
                    Second_QuickBar_filter.mesh = item.skinedMesh.sharedMesh;
                    Second_QuickBar_meshRenderer.materials = item.skinedMesh.sharedMaterials;
                    Second_QuickBarItem = item;

                    break;
                }
            }
        }
    }

    void CleaningInventorySlot(Item item)
    {
        foreach (InventorySlot slot in Inventory.instance.slots)
        {
            if (item == slot.item)
            {
                if (!slot.ItemObject)
                {
                    slot.item = null;

                }
            }
        }
    }

    public void OnChangeControl()
    {
        if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.mouseScheme)
        {
            KeyBindindManager.instance.DisplayShortcutText(First_QuickBar_Key, first_ShortcutKeyImage, playerInputActions.UI.QuickUseFirstSlot.bindings[0].effectivePath);
            KeyBindindManager.instance.DisplayShortcutText(Second_QuickBar_Key, second_ShortcutKeyImage, playerInputActions.UI.QuickUseSecondSlot.bindings[0].effectivePath);
        }
        else if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.gamepadScheme)
        {
            KeyBindindManager.instance.DisplayCurrentControllerShortcut(First_QuickBar_Key, first_ShortcutKeyImage, playerInputActions.UI.QuickUseFirstSlot);
            KeyBindindManager.instance.DisplayCurrentControllerShortcut(Second_QuickBar_Key, second_ShortcutKeyImage, playerInputActions.UI.QuickUseSecondSlot);
        }
    }
}
