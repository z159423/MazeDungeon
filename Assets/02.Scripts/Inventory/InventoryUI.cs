using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour {

    public Transform itemsParent;
    public GameObject inventoryUI;

    [SerializeField]
    private GameObject prefabItem;

    Inventory inventroy;

    public InventorySlot[] slots;

	// Use this for initialization
	void Start () {
        inventroy = Inventory.instance;
        //inventroy.onInventoryChangedCallback += InventorySorting;

        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
	}
	

    void InventorySorting ()
    {
        inventroy.items.Clear();

        print("인벤토리 아이템 정렬");

        for (int i = 0; i < slots.Length; i++)
        {
            if(slots[i].GetComponentInChildren<DragItem>())
            {
                inventroy.items.Add(slots[i].GetComponentInChildren<DragItem>().thisItem);
            }
        }
    }

    public void TurnOffUi()
    {
        inventoryUI.SetActive(false);
        StatDetail.instance.HideDefaultStatUI();
    }
   
}
