using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiOnOff : MonoBehaviour
{
    private ToolTip toolTip;
    private ItemPickUpToolTip itemPickUpToolTip;
    void Start()
    {
        toolTip = transform.Find("ToolTip").GetComponent<ToolTip>();
        itemPickUpToolTip = transform.Find("ItemPickUpTip").GetComponent<ItemPickUpToolTip>();

        toolTip.gameObject.SetActive(true);
        toolTip.gameObject.SetActive(false);

        itemPickUpToolTip.gameObject.SetActive(true);
        itemPickUpToolTip.gameObject.SetActive(false);
    }
}
