using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UseItem : MonoBehaviour, IPointerDownHandler
{

    public void OnPointerDown(PointerEventData data)
    {
            if (data.button == PointerEventData.InputButton.Right)
            {
                transform.parent.GetComponent<InventorySlot>().UseItem();
            }
    }
}
