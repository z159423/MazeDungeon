using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour {

    #region Singleton

    public static EquipmentUI instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found!");
            return;
        }
        instance = this;
    }

    #endregion

    public GameObject Equipmentui;
    public Transform equipmentParent;

    //Inventory inventroy;

    //EquipmentManager manager;
    public EquipmentSlots[] Slot;

	// Use this for initialization
	void Start () {
        //inventroy = Inventory.instance;

        //manager = EquipmentManager.instance;
        Slot = equipmentParent.GetComponentsInChildren<EquipmentSlots>();

    }
	
	// Update is called once per frame
	void Update () {
        

    }


    public void EquipItem(Item newItem)
    {
        Slot[(int)newItem.itemtype].AddItem(newItem);
        
    }

    public void UnEquipItem(int slotindex)
    {
        Slot[slotindex].ClearSlot();
    }

    public void TurnOffUi()
    {
        Equipmentui.SetActive(!Equipmentui.activeSelf);
    }
}
