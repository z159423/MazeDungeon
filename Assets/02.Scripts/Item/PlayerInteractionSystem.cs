using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Localization.Components;

public class PlayerInteractionSystem : MonoBehaviour
{
    public GameObject ItemPickupToolTip;

    private ItemPickUpToolTip PickupToolTip;
    private GameObject CenterMessage;
    public LocalizeStringEvent CenterMessageLocalize;

    public List<ItemPickup> ItempickupsList = new List<ItemPickup>();
    public List<OpenChest> Chest = new List<OpenChest>();
    public List<Door> Door = new List<Door>();
    public List<PrayToStatue> Statue = new List<PrayToStatue>();
    public List<ArtifactPickUp> Artifact = new List<ArtifactPickUp>();
    public List<KeyPickUp> Key = new List<KeyPickUp>();

    private PlayerInputAction playerInputActions;
    private ItemPickUpToolTip itemPickUpToolTip;

    private void Awake()
    {
        InitialKeyBind();

        KeyBindindManager.instance.OnUpdateKeyBindsCallBack += UpdateKeyBind;
    }

    private void InitialKeyBind()
    {
        playerInputActions = new PlayerInputAction();

        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            playerInputActions.LoadBindingOverridesFromJson(rebinds);

        playerInputActions.UI.PickUp.started += Interaction;

        playerInputActions.UI.Enable();
    }

    private void UpdateKeyBind()
    {
        playerInputActions.Disable();

        InitialKeyBind();
    }

    private void Start()
    {
        PickupToolTip = ItemPickUpToolTip.instance;
        ItemPickupToolTip = UIManager.instance.itemPickupToolTip;
        CenterMessage = UIManager.instance.CenterMessageText;
        CenterMessageLocalize = UIManager.instance.CenterMessageLocalize;

        itemPickUpToolTip = ItemPickupToolTip.GetComponent<ItemPickUpToolTip>();
    }

    private void Update()
    {
        if(itemPickUpToolTip.CheckingToolTipEnable() && !UIManager.instance.inventory.activeSelf)
        {
            if (ItempickupsList.Count == 0 && Artifact.Count == 0 && Key.Count == 0)
                itemPickUpToolTip.HideToolTip();
        }

        CheckingInteractions();
    }

    private void Interaction(InputAction.CallbackContext obj)
    {
        if (ItempickupsList.Count != 0)
        {
            if (ItempickupsList[0] == null)
            {
                ItempickupsList.RemoveAt(0);
                OnChangeElimentCount();

                return;
            }

            bool isSuccess = ItempickupsList[0].Itempickup();
            if (isSuccess == true)
            {
                ItempickupsList.RemoveAt(0);
            }
            OnChangeElimentCount();
        }
        else if (Chest.Count != 0)
        {

            if (Chest[0] == null)
            {
                Chest.RemoveAt(0);
                OnChangeElimentCount();

                return;
            }

            bool success = Chest[0].ChestOpen(gameObject);
            if (success)
            {
                Chest.RemoveAt(0);
            }

            OnChangeElimentCount();
        }
        else if (Door.Count != 0)
        {
            if (Door[0] == null)
            {
                Door.RemoveAt(0);
                OnChangeElimentCount();

                return;
            }

            bool success = Door[0].OpenDoor(gameObject);
            if (success)
            {
                Door.RemoveAt(0);
            }

            OnChangeElimentCount();
        }
        else if (Statue.Count != 0)
        {
            if (Statue[0] == null)
            {
                Statue.RemoveAt(0);
                OnChangeElimentCount();

                return;
            }

            bool success = Statue[0].ActiveStatue(transform);

            if(success)
            {
                //GetComponent<Animator>().SetTrigger("Pray");
                Statue.RemoveAt(0);
            }

            OnChangeElimentCount();
        }
        else if (Artifact.Count != 0)
        {
            if (Artifact[0] == null)
            {
                Artifact.RemoveAt(0);
                OnChangeElimentCount();

                return;
            }

            bool isSuccess = Artifact[0].PickUp(gameObject);
            if (isSuccess == true)
            {
                Artifact.RemoveAt(0);
            }

            OnChangeElimentCount();
        }
        else if (Key.Count != 0)
        {
            if (Key[0] == null)
            {
                Key.RemoveAt(0);
                OnChangeElimentCount();

                return;
            }

            bool isSuccess = Key[0].PickUp();
            if (isSuccess == true)
            {
                Key.RemoveAt(0);
            }

            OnChangeElimentCount();
        }
    }

    private void OnChangeElimentCount()
    {
        if(!PickupToolTip)
        {
            PickupToolTip = ItemPickupToolTip.GetComponent<ItemPickUpToolTip>();
        }

        //print("ItempickupsList.Count : " + ItempickupsList.Count);

        CheckingInteractions();

        if (ItempickupsList.Count > 0)
        {
            if(!UIManager.instance.inventory.activeSelf)
            {
                if (ItempickupsList[0] == null)
                {
                    ItempickupsList.RemoveAt(0);
                    OnChangeElimentCount();

                    return;
                }

                ItemPickupToolTip.GetComponent<ItemPickUpToolTip>().ShowToolTip();
                PickupToolTip.GetItemInfo(ItempickupsList[0].item, false);
                CheckingInteractions();
            }
            
        }
        else if(Artifact.Count > 0)
        {
            if (!UIManager.instance.inventory.activeSelf)
            {
                if (Artifact[0] == null)
                {
                    Artifact.RemoveAt(0);
                    OnChangeElimentCount();

                    return;
                }

                ItemPickupToolTip.GetComponent<ItemPickUpToolTip>().ShowToolTip();
                PickupToolTip.GetArtifactInfo(Artifact[0]);
                CheckingInteractions();
            }
        }
        else if (Key.Count > 0)
        {
            if (!UIManager.instance.inventory.activeSelf)
            {
                if (Key[0] == null)
                {
                    Key.RemoveAt(0);
                    OnChangeElimentCount();

                    return;
                }

                ItemPickupToolTip.GetComponent<ItemPickUpToolTip>().ShowToolTip();
                PickupToolTip.GetKeyInfo(Key[0]);
                CheckingInteractions();
            }
        }
        else
        {
            if(!ItemPickUpToolTip.instance.isCheckingInventoryItem)
            {
                ItemPickupToolTip.GetComponent<ItemPickUpToolTip>().HideToolTip();
            }
                
        }

        UIManager.instance.GoldRequire.SetActive(false);
        UIManager.instance.HealthRequire.SetActive(false);
        UIManager.instance.InteractionHotKey.SetActive(false);

        if (Statue.Count > 0)
        {
            CenterMessage.gameObject.SetActive(true);

            if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.mouseScheme)
            {
                UIManager.instance.interactionHotKeyImage.gameObject.SetActive(false);
                UIManager.instance.interactionHotKeyText.gameObject.SetActive(true);
                UIManager.instance.InteractionHotKey.SetActive(true);
                UIManager.instance.interactionHotKey = InputControlPath.ToHumanReadableString(playerInputActions.UI.PickUp.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                UIManager.instance.interactionHotKeyLocalize.StringReference.RefreshString();
            }
            else if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.gamepadScheme)
            {
                UIManager.instance.interactionHotKeyImage.gameObject.SetActive(true);
                UIManager.instance.interactionHotKeyText.gameObject.SetActive(false);
                UIManager.instance.InteractionHotKey.SetActive(true);
                KeyBindindManager.instance.ChangeGamepadImage(UIManager.instance.interactionHotKeyText, UIManager.instance.interactionHotKeyImage, playerInputActions.UI.PickUp);

            }

            switch (Statue[0].statueType)
            {
                case StatueType.Health:
                    CenterMessageLocalize.StringReference.SetReference("UI", "PrayToHealthStatue");
                    break;

                case StatueType.Strength:
                    CenterMessageLocalize.StringReference.SetReference("UI", "PrayToStrengthStatue");
                    break;

                default:
                    CenterMessageLocalize.StringReference.SetReference("UI", "Pray");
                    break;
            }
            
            if (Statue[0].GoldRequire)
            {
                UIManager.instance.GoldRequire.SetActive(true);
                UIManager.instance.GoldValue.text = "- " + Statue[0].GoldValue.ToString();
            }
            else if(Statue[0].HealthRequire)
            {
                UIManager.instance.HealthRequire.SetActive(true);
                UIManager.instance.HealthValue.text = "- " + Statue[0].HealthValue.ToString();
            }

        }
        else if (Door.Count > 0)
        {
            if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.mouseScheme)
            {
                UIManager.instance.interactionHotKeyImage2.gameObject.SetActive(false);
                UIManager.instance.InteractionKey = InputControlPath.ToHumanReadableString(playerInputActions.UI.PickUp.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

                UIManager.instance.InteractionKey = "[" + UIManager.instance.InteractionKey + "]";
            }
            else if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.gamepadScheme)
            {
                UIManager.instance.InteractionKey = "";

                UIManager.instance.interactionHotKeyImage2.gameObject.SetActive(true);
                KeyBindindManager.instance.ChangeGamepadImage(UIManager.instance.interactionHotKeyText, UIManager.instance.interactionHotKeyImage2, playerInputActions.UI.PickUp);
            }

            CenterMessage.gameObject.SetActive(true);
            CenterMessageLocalize.StringReference.SetReference("UI", "OpenDoor");

            UIManager.instance.interactionHotKeyLocalize.StringReference.RefreshString();
        }
        else if (Chest.Count > 0)
        {
            if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.mouseScheme)
            {
                UIManager.instance.interactionHotKeyImage2.gameObject.SetActive(false);
                UIManager.instance.InteractionKey = InputControlPath.ToHumanReadableString(playerInputActions.UI.PickUp.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

                UIManager.instance.InteractionKey = "[" + UIManager.instance.InteractionKey + "]";
            }
            else if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.gamepadScheme)
            {
                UIManager.instance.InteractionKey = "";

                UIManager.instance.interactionHotKeyImage2.gameObject.SetActive(true);
                KeyBindindManager.instance.ChangeGamepadImage(UIManager.instance.interactionHotKeyText, UIManager.instance.interactionHotKeyImage2, playerInputActions.UI.PickUp);
            }

            CenterMessage.gameObject.SetActive(true);
            CenterMessageLocalize.StringReference.SetReference("UI", "OpenChest");

            UIManager.instance.interactionHotKeyLocalize.StringReference.RefreshString();
        }
        else
        {
            UIManager.instance.interactionHotKeyImage2.gameObject.SetActive(false);
            CenterMessage.gameObject.SetActive(false);
        }
    }

    private void CheckingInteractions()
    {
        for (int i = 0; i < ItempickupsList.Count; i++)
        {
            if (ItempickupsList[i] == null)
            {
                ItempickupsList.RemoveAt(i);
                OnChangeElimentCount();
            }

        }

        for (int i = 0; i < Chest.Count; i++)
        {
            if (Chest[i] == null)
            {
                Chest.RemoveAt(i);
                OnChangeElimentCount();
            }

        }

        for (int i = 0; i < Door.Count; i++)
        {
            if (Door[i] == null)
            {
                Door.RemoveAt(i);
                OnChangeElimentCount();
            }

        }

        for (int i = 0; i < Statue.Count; i++)
        {
            if (Statue[i] == null)
            {
                Statue.RemoveAt(i);
                OnChangeElimentCount();
            }

        }

        for (int i = 0; i < Artifact.Count; i++)
        {
            if (Artifact[i] == null)
            {
                Artifact.RemoveAt(i);
                OnChangeElimentCount();
            }

        }

        for (int i = 0; i < Key.Count; i++)
        {
            if (Key[i] == null)
            {
                Key.RemoveAt(i);
                OnChangeElimentCount();
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ItemPickup>() != null)
        {
            ItempickupsList.Add(other.GetComponent<ItemPickup>());
        }

        if (other.GetComponent<OpenChest>() != null)
        {
            Chest.Add(other.GetComponent<OpenChest>());
        }

        if (other.GetComponent<Door>() != null)
        {
            Door.Add(other.GetComponent<Door>());
        }

        if (other.GetComponent<PrayToStatue>() != null)
        {
            Statue.Add(other.GetComponent<PrayToStatue>());
        }

        if (other.GetComponent<ArtifactPickUp>() != null)
        {
            Artifact.Add(other.GetComponent<ArtifactPickUp>());
        }

        if (other.GetComponent<KeyPickUp>() != null)
        {
            Key.Add(other.GetComponent<KeyPickUp>());
        }

        OnChangeElimentCount();

    }

    private void OnTriggerExit(Collider other)
    {

        if (other.GetComponent<ItemPickup>() != null)
        {
            ItempickupsList.Remove(other.GetComponent<ItemPickup>());
        }

        if (other.GetComponent<OpenChest>() != null)
        {
            Chest.Remove(other.GetComponent<OpenChest>());
        }

        if (other.GetComponent<Door>() != null)
        {
            Door.Remove(other.GetComponent<Door>());
        }

        if (other.GetComponent<PrayToStatue>() != null)
        {
            Statue.Remove(other.GetComponent<PrayToStatue>());
        }

        if (other.GetComponent<ArtifactPickUp>() != null)
        {
            Artifact.Remove(other.GetComponent<ArtifactPickUp>());
        }

        if (other.GetComponent<KeyPickUp>() != null)
        {
            Key.Remove(other.GetComponent<KeyPickUp>());
        }

        OnChangeElimentCount();
    }


}
