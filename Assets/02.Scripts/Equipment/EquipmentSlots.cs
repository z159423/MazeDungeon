using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EquipmentSlots : MonoBehaviour {

    public Image icon;
    Item equipment;
    public EquipmentManager equipmentmanager ;

    public ItemType equipSlot;
    public WeaponType weaponType;

    public UnityEvent onchangeEquipment;

    void Start()
    {
        equipmentmanager = EquipmentManager.instance;

        EquipmentManager.instance.onEquipmentChanged2 += new EquipmentManager.OnEquipmentChanged2(OnEquipmentChanged);

        checkingEquipmentSlotIconEnable();
    }

    public void OnEquipmentChanged(EquipmentManager manager)
    {
        checkingEquipmentSlotIconEnable();
    }

    private void checkingEquipmentSlotIconEnable()
    {
        if (EquipmentManager.instance.currentEquipment[(int)equipSlot] != null)
        {
            if (!icon)
                icon = GetComponentInChildren<Image>();

            icon.enabled = false;
        }
        else
        {
            if (!icon)
                icon = GetComponentInChildren<Image>();

            icon.enabled = true;
        }
    }

    public void AddItem(Item newItem)
    {
        equipment = newItem;

        //icon.sprite = equipment.icon;
        //icon.enabled = true;
    }

    public void ClearSlot()
    {
        if(equipment != null)
        equipmentmanager.Unequip((int)equipment.itemtype);
        equipment = null;
        //icon.sprite = null;
        //icon.enabled = false;
    }
	
}
