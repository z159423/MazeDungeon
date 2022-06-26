using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

    public Image icon;
    //public Button removeButton;
    public Item item;
    public GameObject ItemObject;

    public Material[] stencilVertexMat;

    public void AddItem (Item newItem)
    {
        item = newItem;

        if (icon != null)
        {
            icon.sprite = item.icon;
            icon.enabled = true;
        }//removeButton.interactable = true;

        ItemObject = GetComponentInChildren<DragItem>().gameObject;
    }

    public void ClearSlot (bool DeleteItemObject)
    {
        item = null;
        ItemObject = null;

        if (icon != null)
        {
            icon.sprite = null;
            icon.enabled = false;
            icon = null;
        }

        if(DeleteItemObject)
        {
            //Destroy(ItemObject);
        }
        else
        {
            //ItemObject = null;
        }
            
    }


    public void UseItem()
    {
        if (item != null)
        {
            if (transform.childCount != 0)
            {
                //Destroy(transform.GetChild(0).gameObject);
                //Destroy(transform.Find("Item(Clone)").gameObject); // 아이템을 이동하면 원래 있던 아이템클론을 지우는 코드. 이름으로 검색해서 지움
                item.Use();
            }
            
         }

        
    }

    public void Getcomponent()
    {
        //icon = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        icon = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        //removeButton = transform.GetChild(0).GetChild(1).GetComponent<Button>();
        ItemObject = GetComponentInChildren<DragItem>().gameObject;
    }

    public void Getcomponent_Hotkey()
    {
        icon = transform.GetChild(2).GetChild(0).GetComponent<Image>();
    }

    public void CopyValuesFromOtherSlot(InventorySlot otherSlot)
    {
        icon = otherSlot.icon;
        item = otherSlot.item;
        ItemObject = otherSlot.ItemObject;
    }

    public void CopyValuesFromItem(Item otherItem)
    {
        item = otherItem;
        ItemObject = GetComponentInChildren<DragItem>().gameObject;
        //icon = GetComponentInChildren<DragItem>().
    }
}
