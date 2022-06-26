using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Components;
using TMPro;

public class UIManager : MonoBehaviour
{

    public Slider Hpbar;
    public Slider ShieldBar;
    public Slider Manabar;
    public Slider Soulbar;
    public Slider Ragebar;
    public Slider Expbar;
    public Slider StaminaBarSlider;
    public Slider ChargeBar;
    public Slider DodgeBar;
    public Slider SneakBar;
    public Slider WarriorShieldBar;
    public GameObject ArcherArrowStack;
    public Transform ArcherArrowLayout;

    [Space]

    public Text HpText;
    public Text ManaText;
    public Text SoulText;
    public Text RageText;
    public Text AttackStat;
    public Text DeffendStat;
    public Text CriticalChanceStat;
    public Text SpeedStat;
    public Text DodgeChangeStat;
    public Text MaxHpStat;
    public Text MaxSteaminaStat;
    public Text AttackSpeed;

    public Text ExpText;
    public Text LvlText;
    public Text StaminaText;
    //public Text CoinText;
    public Animator CoinAnimator;
    public Text CoinText;
    //public Text KeyText;
    public Animator KeyAnimator;
    public Text KeyText;
    public GameObject CenterMessageText;
    public LocalizeStringEvent CenterMessageLocalize;

    [Space]

    public GameObject inventory;
    public GameObject equipment;
    public GameObject skill;
    public GameObject ESCMenu;
    public GameObject Console;
    public GameObject Minimap;
    public GameObject DeathMessage;
    public GameObject Map;
    public GameObject Dodge;
    public GameObject Artifact;
    public GameObject CrossHair;
    public GameObject SkillQuickSlots;
    public GameObject CharactorState;
    public GameObject CunsumableQuickSlot;
    public GameObject CurrentWeapon;
    public GameObject QuickUI;
    public Transform buffDebuffIconParent;

    public GameObject EndingScene;

    [Space]

    public InventoryUI inventoryUI;

    [Space]

    public GameObject ToolTip;
    public GameObject itemPickupToolTip;
    public GameObject SkillDrag;
    public GameObject SkillSlotContent;
    public GameObject StageClear;
    public GameObject Coin;
    public GameObject GoldRequire;
    public TextMeshProUGUI GoldValue;
    public GameObject HealthRequire;
    public GameObject InteractionHotKey;
    public TextMeshProUGUI HealthValue;
    public TextMeshProUGUI interactionHotKeyText;
    public LocalizeStringEvent interactionHotKeyLocalize;
    public Image interactionHotKeyImage;
    public Image interactionHotKeyImage2;
    public string interactionHotKey;

    public GameObject padCursor;

    [Space]

    public Settings settingMenu;
    [Space]

    public bool GameEnd = false;

    [Space]

    public string InteractionKey = "";

    [Space]

    public SkillParent skillParent;

    [Space]

    public static UIManager instance;

    private PlayerInputAction playerInputActions;

    private void Awake()
    {
        instance = this;

        InitialBindKey();

        KeyBindindManager.instance.OnUpdateKeyBindsCallBack += UpdateBindKey;
    }

    private void OnEnable()
    {
        //KeyBindindManager.instance.OnUpdateKeyBindsCallBack += UpdateBindKey;
    }

    private void OnDisable()
    {
        //KeyBindindManager.instance.OnUpdateKeyBindsCallBack -= UpdateBindKey;
    }

    private void Start()
    {
        Inventory.instance.OnCoinChangedCallBack += CoinTextChanged;
        Inventory.instance.OnKeyChangedCallBack += KeyTextChanged;

        settingMenu.InitiallizeAudioSetting();

        var settingData = Settings.RoadSettingData();
        if (settingData != null)
            StartCoroutine(settingMenu.ReadSetting(settingData));

        InteractionKey = InputControlPath.ToHumanReadableString(playerInputActions.UI.PickUp.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    private void InitialBindKey()
    {
        playerInputActions = new PlayerInputAction();

        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            playerInputActions.LoadBindingOverridesFromJson(rebinds);

        playerInputActions.UI.Inventory.started += Inventory_started;
        playerInputActions.UI.Skill.started += Skill_started;
        playerInputActions.UI.EscMenu.started += EscMenu_started;
        playerInputActions.UI.Consloe.started += Consloe_started;
        playerInputActions.UI.Minimap.started += Minimap_started;
        playerInputActions.UI.Map.started += Map_started;
        playerInputActions.UI.Artifact.started += Artifact_started;
        //playerInputActions.UI.UI.started += UiOnOff;

        playerInputActions.UI.Enable();
    }

    private void UpdateBindKey()
    {
        playerInputActions.Disable();

        InitialBindKey();

        InteractionKey = InputControlPath.ToHumanReadableString(playerInputActions.UI.PickUp.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    private void Map_started(InputAction.CallbackContext obj)
    {
        MapUIActive();
    }

    public void MapUIActive()
    {
        if (GameEnd)
            return;

        MainUIOff(Map);
        Map.SetActive(!Map.activeSelf);

        IsAnyUiOn();
    }

    private void Minimap_started(InputAction.CallbackContext obj)
    {
        if (GameEnd)
            return;

        Minimap.SetActive(!Minimap.activeSelf);

        IsAnyUiOn();
    }

    private void Consloe_started(InputAction.CallbackContext obj)
    {
        if (GameEnd)
            return;

        if(settingMenu.AllowConsoleToggle.isOn)
        {
            Console.SetActive(!Console.activeSelf);
            IsAnyUiOn();
        }
        
    }

    private void EscMenu_started(InputAction.CallbackContext obj)
    {
        if (GameEnd)
            return;

        if (inventory.activeSelf)
        {
            InventoryUIActive();
        }
        else if (Artifact.activeSelf)
        {
            ArtifactUIActive();
        }
        else if (Map.activeSelf)
        {
            MapUIActive();
        }
        else if(skill.activeSelf)
        {
            SkillUIActive();
        }
        else
        {
            ESCMenu.SetActive(!ESCMenu.activeSelf);

            if (ESCMenu.activeSelf)
            {
                ESCMenu.GetComponentInChildren<escMenu1>().Pause();
            }
            else if (!ESCMenu.activeSelf)
            {
                ESCMenu.GetComponentInChildren<escMenu1>().Resume();
            }
        }

        IsAnyUiOn();
    }

    private void Skill_started(InputAction.CallbackContext obj)
    {
        SkillUIActive();
    }

    public void SkillUIActive()
    {
        if (GameEnd)
            return;

        MainUIOff(skill);

        if (skill.activeSelf == true)
        {
            if (SkillToolTip.instance.gameObject.activeSelf && SkillToolTip.instance.isSkillTab)
            {
                SkillToolTip.instance.HideToolTip();
            }

            skill.SetActive(false);
        }
        else if (skill.activeSelf == false)
        {
            skill.SetActive(true);
        }

        IsAnyUiOn();
    }

    private void Inventory_started(InputAction.CallbackContext obj)
    {
        InventoryUIActive();
    }

    public void InventoryUIActive()
    {
        if (GameEnd)
            return;

        MainUIOff(inventory);

        if (inventory.activeSelf)
        {
            if (ItemPickUpToolTip.instance.isCheckingInventoryItem)
            {
                ItemPickUpToolTip.instance.isCheckingInventoryItem = false;
                ItemPickUpToolTip.instance.HideToolTip();
            }

            if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteractionSystem>())
            {
                if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteractionSystem>().ItempickupsList.Count > 0)
                {
                    ItemPickUpToolTip.instance.ShowToolTip();
                    ItemPickUpToolTip.instance.GetItemInfo(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteractionSystem>().ItempickupsList[0].item, false);
                }
            }

            StatDetail.instance.HideDefaultStatUI();
        }

        if (!inventory.activeSelf)
        {
            ItemPickUpToolTip.instance.HideToolTip();
        }

        inventory.SetActive(!inventory.activeSelf);

        IsAnyUiOn();
    }

    private void Artifact_started(InputAction.CallbackContext obj)
    {
        ArtifactUIActive();
    }

    public void ArtifactUIActive()
    {
        if (GameEnd)
            return;

        MainUIOff(Artifact);

        Artifact.SetActive(!Artifact.activeSelf);

        IsAnyUiOn();
    }

    public void GameEnding()
    {
        GameEnd = true;

        PlayStat.instance.GameClearEvent();

        inventory.SetActive(false);
        equipment.SetActive(false);
        skill.SetActive(false);
        ESCMenu.SetActive(false);
        Console.SetActive(false);
        Minimap.SetActive(false);
        DeathMessage.SetActive(false);
        Map.SetActive(false);
        Dodge.SetActive(false);
        Artifact.SetActive(false);
        CrossHair.SetActive(false);
        SkillQuickSlots.SetActive(false);
        CharactorState.SetActive(false);
        CunsumableQuickSlot.SetActive(false);
        CurrentWeapon.SetActive(false);
        QuickUI.SetActive(false);

        EndingScene.SetActive(true);
        IsAnyUiOn();

        foreach (GameObject Players in GameObject.FindGameObjectsWithTag("Player"))                                      //플레이어중에 네크로멘서가 있으면 영혼수집용 영혼이 소환됨
        {
            Players.GetComponentInChildren<PlayerStats>().isInvincibility = true;
        }
    }

    public void MainUIOff(GameObject UI)
    {
        if (UI != inventory)
            inventoryUI.TurnOffUi();

        if (UI != skill)
        {
            skill.SetActive(false);
            SkillToolTip.instance.HideToolTip();
        }
            
        if (UI != Map)
            Map.SetActive(false);

        if (UI != Artifact)
            Artifact.SetActive(false);

    }

    public void UiOnOff(InputAction.CallbackContext obj)
    {
        //gameObject.SetActive(!gameObject.activeSelf);

        if (GetComponent<Canvas>().planeDistance == 150)
            GetComponent<Canvas>().planeDistance = 0;
        else if(GetComponent<Canvas>().planeDistance == 0)
        {
            GetComponent<Canvas>().planeDistance = 150;
        }


    }

    void CoinTextChanged()
    {
        CoinText.text = Inventory.instance.coinAmount.ToString();
    }

    void KeyTextChanged()
    {
        KeyText.text = Inventory.instance.keyAmount.ToString();
    }

    public bool IsAnyUiOn()
    {
        if(equipment.activeSelf || inventory.activeSelf || skill.activeSelf || 
            ESCMenu.activeSelf || Console.activeSelf || DeathMessage.activeSelf || 
            Map.activeSelf || StageClear.activeSelf || Artifact.activeSelf || EndingScene.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;

            if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.mouseScheme)
            {
                PadCursor.instance.MouseCursorActive();
            }
            else if(PadCursor.instance.GetCurrentCursorScheme() == PadCursor.gamepadScheme)
            {
                PadCursor.instance.GamePadCursorActive();
            }
            
            return false;
        }else
        {

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            padCursor.SetActive(false);

            return true;
        }
    }

}
