using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using TMPro;
using UnityEngine.InputSystem;

public class ItemPickUpToolTip : MonoBehaviour
{
    public GameObject ItemInfoBundle;
    public TextMeshProUGUI itemName;
    public LocalizeStringEvent itemNameLocalize;
    [Space]

    public TextMeshProUGUI itemTier;
    public LocalizeStringEvent itemTierLocalize;
    public ItemTierInToolTip TierValue;
    [Space]
    public TextMeshProUGUI itemDamage;
    public LocalizeStringEvent itemDamageLocalize;
    public ItemDamageInToolTip DamageValue;
    [Space]
    public TextMeshProUGUI itemDefence;
    public LocalizeStringEvent itemDefenceLocalize;
    public ItemDefenseInToolTip DefenseValue;
    [Space]
    public TextMeshProUGUI ShieldPowerText;
    public LocalizeStringEvent ShieldPowerLocalize;
    [Space]
    public TextMeshProUGUI itemType;
    public LocalizeStringEvent itemTypeLocalize;
    [Space]
    public TextMeshProUGUI MoveSpeedText;
    [Space]
    public TextMeshProUGUI pickUpText;
    public LocalizeStringEvent itemPickUpLocalize;
    public Image itemPickUpImage;
    public TextMeshProUGUI garbageText;

    public TextMeshProUGUI itemComment;
    public GameObject purchaseBundle;
    public Image CoinImage;
    public Text CoinText;
    public Text infoText;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public Transform EnchantParent;

    public GameObject inventory;
    public GameObject equipment;

    [Space]
    public GameObject ArtifactBundle;
    public MeshRenderer artifactMeshRenderer;
    public MeshFilter artifactMeshFilter;
    public TextMeshProUGUI artifactName;
    public LocalizeStringEvent artifactNameLocalize;
    public TextMeshProUGUI artifactDescription;
    public LocalizeStringEvent artifactDescriptionLocalize;
    public LocalizeStringEvent artifactAbility;

    [Space]

    public GameObject KeyBundle;
    public MeshRenderer KeyMeshRenderer;


    [Space]

    public string Tier, MinDamage, MaxDamage, Defense, HpRecoverValue, ShieldPower, MoveSpeed;
    public string PickUpKey, PurchaseKey, EnchantNameKey, EnchantExplanationKey;

    [Space]

    public bool isInventoryItem = false;
    public bool isCheckingInventoryItem = false;

    public GameObject EnchantPanelPrefab;

    public float meshRotateSpeed = 60f;

    public static ItemPickUpToolTip instance;

    private GameObject UI;
    private RectTransform canvasRectTransform;
    private Vector3 toolTipPotition;
    private Vector2 outVector2;
    private Vector3 DefaultPosition;

    private PlayerInputAction playerInputActions;

    public List<GameObject> EnchantPanels = new List<GameObject>();
    private void Awake()
    {
        instance = this;

        /*
        itemName = transform.Find("ItemNameText").gameObject.GetComponent<Text>();
        itemDamage = transform.Find("DamageText").gameObject.GetComponent<Text>();
        itemDefence = transform.Find("DefenceText").gameObject.GetComponent<Text>();
        itemType = transform.Find("TypeText").gameObject.GetComponent<Text>();
        itemComment = transform.Find("ItemComment").gameObject.GetComponent<Text>();
        */

        UI = GameObject.FindGameObjectWithTag("UI");
        canvasRectTransform = UI.GetComponent<RectTransform>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        meshFilter = GetComponentInChildren<MeshFilter>();

        DefaultPosition = transform.localPosition;

        UpdateBindKey();

        KeyBindindManager.instance.OnUpdateKeyBindsCallBack += UpdateBindKey;

        //string[] split1 = PickUpKey.Split('/');
        //string[] split2 = PurchaseKey.Split('/');

        //PickUpKey = split1[1];
        //PurchaseKey = split2[1];
    }

    private void Update()
    {
        MeshObjectRotation();

        if(isInventoryItem)
        {
            SetPositionOnItem();
        }
    }
    private void OnEnable()
    {
        //KeyBindindManager.instance.OnUpdateKeyBindsCallBack += UpdateBindKey;
    }

    private void OnDisable()
    {
        //KeyBindindManager.instance.OnUpdateKeyBindsCallBack -= UpdateBindKey;
    }

    private void UpdateBindKey()
    {
        playerInputActions = new PlayerInputAction();
        //playerInputActions.Player.Enable();

        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            playerInputActions.LoadBindingOverridesFromJson(rebinds);

        PickUpKey = InputControlPath.ToHumanReadableString(playerInputActions.UI.PickUp.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        PurchaseKey = InputControlPath.ToHumanReadableString(playerInputActions.UI.PickUp.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

        playerInputActions.Enable();
    }

    public void GetItemInfo(Item item, bool inventoryItem)
    {
        ItemInfoBundle.SetActive(true);
        ArtifactBundle.SetActive(false);
        KeyBundle.SetActive(false);

        isInventoryItem = inventoryItem;

        if(inventoryItem)
            SetPositionOnItem();

        meshFilter.sharedMesh = item.skinedMesh.sharedMesh;
        meshRenderer.sharedMaterials = item.skinedMesh.sharedMaterials;

        itemNameLocalize.StringReference.SetReference("Item", item.ItemNameKey);
        ItemNameColor(item.itemQuality);

        if (item.itemtype == ItemType.Chest || item.itemtype == ItemType.Head || item.itemtype == ItemType.Legs)
        {
            ResetInfo();
            itemDefence.enabled = true;

            Defense = ((int)item.armorModifier).ToString();
            //itemType.text = item.itemtype.ToString();
            itemTypeLocalize.StringReference.SetReference("UI", item.itemtype.ToString());
            itemDefence.gameObject.SetActive(true);

            if(item.itemtype == ItemType.Legs)
            {
                MoveSpeedText.gameObject.SetActive(true);

                MoveSpeed = item.speedModifier.ToString();
            }
        }

        if(item.itemtype == ItemType.SecondaryWeapon)
        {
            ResetInfo();
            
            if(item.secondaryWeaponType == SecondaryWeaponType.Shield)
            {
                itemDefence.enabled = true;

                Defense = ((int)item.armorModifier).ToString();
                ShieldPower = ((int)item.ShieldPointModifier).ToString();
                //itemType.text = item.secondaryWeaponType.ToString();
                itemTypeLocalize.StringReference.SetReference("UI", item.secondaryWeaponType.ToString());
                itemTypeLocalize.enabled = false;
                itemTypeLocalize.enabled = true;
                itemDefence.gameObject.SetActive(true);
                ShieldPowerText.gameObject.SetActive(true);
            }
            else if(item.secondaryWeaponType == SecondaryWeaponType.HandCrossBow)
            {
                itemDamage.enabled = true;

                MinDamage = ((int)item.minHandCrossBowDamage).ToString();
                MaxDamage = ((int)item.maxHandCrossBowDamage).ToString();
                //itemType.text = item.secondaryWeaponType.ToString();
                itemTypeLocalize.StringReference.SetReference("UI", item.secondaryWeaponType.ToString());
                itemTypeLocalize.enabled = false;
                itemTypeLocalize.enabled = true;
                itemDamage.gameObject.SetActive(true);
            }
        }

        if(item.itemtype == ItemType.Weapon)
        {
            ResetInfo();
            itemDamage.enabled = true;

            MinDamage = ((int)item.minDamageModifier).ToString();
            MaxDamage = ((int)item.maxDamageModifier).ToString();
            //itemType.text = item.weaponType.ToString();
            itemTypeLocalize.StringReference.SetReference("UI", item.weaponType.ToString());
            itemTypeLocalize.enabled = false;
            itemTypeLocalize.enabled = true;
            itemDamage.gameObject.SetActive(true);
        }

        if(item.itemtype == ItemType.consumable)
        {
            if(item.consumableType == ConsumableType.Potion)
            {
                ResetInfo();
                itemComment.enabled = true;

                HpRecoverValue = item.effectiveDose_Hp.ToString();
                //itemType.text = item.itemtype.ToString();
                itemTypeLocalize.StringReference.SetReference("UI", "Consumable");
                itemTypeLocalize.enabled = false;
                itemTypeLocalize.enabled = true;

                itemComment.gameObject.SetActive(true);
            }
            else if(item.consumableType == ConsumableType.SkillBook)
            {
                //itemType.text = "Skill Book";
            }
        }

        //Tier = item.itemTier.ToString();
        //itemTier.gameObject.SetActive(true);

        if(!isInventoryItem)
        {
            if (item.onSaleItem)
            {

                //CoinImage.enabled = true;
                //CoinText.enabled = true;
                purchaseBundle.SetActive(true);
                CoinText.text = item.Price.ToString();
                
                //infoText.text = "E : 구매";

                if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.mouseScheme)
                {
                    itemPickUpImage.gameObject.SetActive(false);
                    PurchaseKey = InputControlPath.ToHumanReadableString(playerInputActions.UI.PickUp.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                    PurchaseKey = "[" + PurchaseKey + "]";
                }
                else if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.gamepadScheme)
                {
                    PurchaseKey = "";
                    KeyBindindManager.instance.ChangeGamepadImage(garbageText, itemPickUpImage, playerInputActions.UI.PickUp);
                    
                }

                itemPickUpLocalize.StringReference.SetReference("UI", "Purchase");
                itemPickUpLocalize.gameObject.SetActive(true);

                itemPickUpLocalize.StringReference.RefreshString();
            }
            else
            {
                //CoinImage.enabled = false;
                //CoinText.enabled = false;
                purchaseBundle.SetActive(false);
                
                //infoText.text = "E : 획득";
                if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.mouseScheme)
                {
                    itemPickUpImage.gameObject.SetActive(false);
                    PickUpKey = InputControlPath.ToHumanReadableString(playerInputActions.UI.PickUp.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                    PickUpKey = "[" + PickUpKey + "]";
                }
                else if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.gamepadScheme)
                {
                    PickUpKey = "";
                    KeyBindindManager.instance.ChangeGamepadImage(garbageText, itemPickUpImage, playerInputActions.UI.PickUp);
                    
                }

                itemPickUpLocalize.StringReference.SetReference("UI", "PickUpText");
                itemPickUpLocalize.gameObject.SetActive(true);

                itemPickUpLocalize.StringReference.RefreshString();

            }

            //infoText.gameObject.SetActive(true);
        }
        else
        {
            purchaseBundle.SetActive(false);
            //pickUpText.text = "";
            itemPickUpLocalize.gameObject.SetActive(false);
            itemPickUpImage.gameObject.SetActive(false);
        }
        
        void ItemNameColor(ItemQuality itemQuality)
        {
            if (itemQuality == ItemQuality.Common)
            {
                itemName.color = new Color(100 /255f,100/255f,100/255f);
            }
            /*else if (itemQuality == ItemQuality.Uncommon)
            {
                itemName.color = Color.green;
            }*/
            else if (itemQuality == ItemQuality.Rare)
            {
                itemName.color = Color.blue;
            }
            else if (itemQuality == ItemQuality.Unique)
            {
                itemName.color = Color.magenta;
            }
            else if (itemQuality == ItemQuality.Epic)
            {
                itemName.color = Color.yellow;
            }

        }

        DeleteEnchantPanels();

        foreach (Enchant enchant in item.enchants)
        {
            var EnchantPanel = Instantiate(EnchantPanelPrefab, EnchantParent);
            EnchantPanels.Add(EnchantPanel);

            EnchantPanel.GetComponent<EnchantPanel>().SetEnchantName(enchant);
        }
        

    }

    public void GetArtifactInfo(ArtifactPickUp artifact)
    {
        ItemInfoBundle.SetActive(false);
        ArtifactBundle.SetActive(true);
        KeyBundle.SetActive(false);

        DeleteEnchantPanels();

        if (artifact.isOnSale)
        {
            purchaseBundle.SetActive(true);
            CoinText.text = artifact.PurchaseValue.ToString();
            
            if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.mouseScheme)
            {
                itemPickUpImage.gameObject.SetActive(false);
                PurchaseKey = InputControlPath.ToHumanReadableString(playerInputActions.UI.PickUp.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                PurchaseKey = "[" + PurchaseKey + "]";
            }
            else if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.gamepadScheme)
            {
                PurchaseKey = "";
                KeyBindindManager.instance.ChangeGamepadImage(garbageText, itemPickUpImage, playerInputActions.UI.PickUp);
                
            }

            itemPickUpLocalize.StringReference.SetReference("UI", "Purchase");
            itemPickUpLocalize.gameObject.SetActive(true);

            itemPickUpLocalize.StringReference.RefreshString();
        }
        else
        {
            purchaseBundle.SetActive(false);
            

            if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.mouseScheme)
            {
                itemPickUpImage.gameObject.SetActive(false);
                PickUpKey = InputControlPath.ToHumanReadableString(playerInputActions.UI.PickUp.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                PickUpKey = "[" + PickUpKey + "]";
            }
            else if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.gamepadScheme)
            {
                PickUpKey = "";
                KeyBindindManager.instance.ChangeGamepadImage(garbageText, itemPickUpImage, playerInputActions.UI.PickUp);
                
            }

            itemPickUpLocalize.StringReference.SetReference("UI", "PickUpText");
            itemPickUpLocalize.gameObject.SetActive(true);

            itemPickUpLocalize.StringReference.RefreshString();
        }

        if(artifact.artifact.ArtifactObject.GetComponent<MeshFilter>())
        {
            artifactMeshFilter.sharedMesh = artifact.artifact.ArtifactObject.GetComponent<MeshFilter>().sharedMesh;
        }
        else if(artifact.artifact.ArtifactObject.GetComponentInChildren<SkinnedMeshRenderer>())
        {
            artifactMeshFilter.sharedMesh = artifact.artifact.ArtifactObject.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
        }
        
        artifactNameLocalize.StringReference.SetReference("Artifact", artifact.artifact.nameLocalizationKey);
        artifactDescriptionLocalize.StringReference.SetReference("Artifact", artifact.artifact.descriptionLocalizationKey);

        string artifactAbilityKey = artifact.artifact.nameLocalizationKey.Replace("Name-","");
        artifactAbility.StringReference.SetReference("Artifact", "Ability-" + artifactAbilityKey);

        switch (artifact.artifact.artifactTier)
        {
            case ArtifactTier.Common:
                artifactName.color = PrefabCollect.instance.commonNameColor;
                break;

            case ArtifactTier.Rare:
                artifactName.color = PrefabCollect.instance.rareNameColor;
                break;

            case ArtifactTier.Unique:
                artifactName.color = PrefabCollect.instance.uniqueNameColor;
                break;

            case ArtifactTier.Epic:
                artifactName.color = PrefabCollect.instance.epicNameColor;
                break;
        }

        
    }

    public void GetKeyInfo(KeyPickUp keyPickUp)
    {
        ItemInfoBundle.SetActive(false);
        ArtifactBundle.SetActive(false);
        KeyBundle.SetActive(true);

        DeleteEnchantPanels();

        if (keyPickUp.isOnSale)
        {
            purchaseBundle.SetActive(true);
            CoinText.text = keyPickUp.PurchaseValue.ToString();
            
            if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.mouseScheme)
            {
                itemPickUpImage.gameObject.SetActive(false);
                PurchaseKey = InputControlPath.ToHumanReadableString(playerInputActions.UI.PickUp.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                PurchaseKey = "[" + PurchaseKey + "]";
            }
            else if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.gamepadScheme)
            {
                PurchaseKey = "";
                KeyBindindManager.instance.ChangeGamepadImage(garbageText, itemPickUpImage, playerInputActions.UI.PickUp);
                
            }

            itemPickUpLocalize.StringReference.SetReference("UI", "Purchase");
            itemPickUpLocalize.gameObject.SetActive(true);

            itemPickUpLocalize.StringReference.RefreshString();
        }
        else
        {
            purchaseBundle.SetActive(false);

            if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.mouseScheme)
            {
                itemPickUpImage.gameObject.SetActive(false);
                PickUpKey = InputControlPath.ToHumanReadableString(playerInputActions.UI.PickUp.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                PickUpKey = "[" + PickUpKey + "]";
            }
            else if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.gamepadScheme)
            {
                PickUpKey = "";
                KeyBindindManager.instance.ChangeGamepadImage(garbageText, itemPickUpImage, playerInputActions.UI.PickUp);
                
            }

            itemPickUpLocalize.StringReference.SetReference("UI", "PickUpText");
            itemPickUpLocalize.gameObject.SetActive(true);

            itemPickUpLocalize.StringReference.RefreshString();
        }
    }

    public bool CheckingToolTipEnable()
    {
        return gameObject.activeSelf;
    }

    public void ShowToolTip()
    {
        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        DeleteEnchantPanels();

        gameObject.SetActive(false);
        isInventoryItem = false;
        ReturnDefaultPosition();

        /*foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if(player.GetComponent<ItemPickupText>())
            {
                if(player.GetComponent<ItemPickupText>().ItempickupsList.Count > 0)
                {
                    if(!AllUI.instance.inventory.activeSelf)
                        ShowToolTip();
                }
            }
        }*/
    }

    public void DeleteEnchantPanels()
    {
        foreach (GameObject enchantPanel in EnchantPanels)
        {
            Destroy(enchantPanel);
        }

        EnchantPanels.Clear();
    }

    private void ResetInfo()
    {
        itemTier.gameObject.SetActive(false);
        itemDamage.gameObject.SetActive(false);
        //itemDamage.gameObject.SetActive(true);
        itemDefence.gameObject.SetActive(false);
        //itemDefence.gameObject.SetActive(true);
        itemComment.gameObject.SetActive(false);
        //itemComment.gameObject.SetActive(true);
        //infoText.gameObject.SetActive(false);
        ShieldPowerText.gameObject.SetActive(false);
        MoveSpeedText.gameObject.SetActive(false);

        itemDamage.enabled = false;
        itemDefence.enabled = false;
        //itemType.text = "";
        itemComment.enabled = false;
    }

    private void MeshObjectRotation()
    {

        if(gameObject.activeSelf == true)
        {
            meshRenderer.transform.Rotate(Vector3.down * Time.deltaTime * meshRotateSpeed);
        }
        else
        {
            meshRenderer.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
    }
    
    public void ReturnDefaultPosition()
    {
        transform.localPosition = DefaultPosition;
    }

    private void SetPositionOnItem()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, PadCursor.instance.GetCurrentCursorPosition(), GameObject.FindGameObjectWithTag("UI_Camera").GetComponent<Camera>(), out outVector2);
        toolTipPotition = outVector2;
        transform.localPosition = toolTipPotition + new Vector3(0, 0, -50);
        meshRenderer.transform.Rotate(Vector3.down * Time.deltaTime * meshRotateSpeed);
    }
}
