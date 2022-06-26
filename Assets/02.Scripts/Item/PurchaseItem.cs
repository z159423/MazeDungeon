using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class PurchaseItem : Interactable
{
    public Item item;
    public Text ItemPickUpText;

    public int Price = 0;
    public ShopKeeper shopKeeper;

    GameObject players;
    GameObject playerEquipPoint;
    SkinnedMeshRenderer meshRenderer;
    new MeshCollider collider;

    new public string name;

    Transform[] childTrans;
    private void Start()
    {
        if (!item)
            Debug.Log("아이템 픽업에 아이템이 없습니다.");

        childTrans = GetComponentsInChildren<Transform>();
        collider = GetComponentInChildren<MeshCollider>();
        GetComponentInChildren<MeshRenderer>().materials = item.skinedMesh.sharedMaterials;
        GetComponentInChildren<MeshFilter>().sharedMesh = item.skinedMesh.sharedMesh;
        meshRenderer = item.skinedMesh;
        collider.sharedMesh = meshRenderer.sharedMesh;

        foreach (Transform child in childTrans)
        {
            child.gameObject.layer = 9;
        }

        players = GameObject.FindGameObjectWithTag("Player");
        name = item.name;
    }

    public override void Interact()
    {
        base.Interact();

        PickUp();
    }

    void PickUp()
    {
        Debug.Log("PickIng up item.");
    }

    public bool purchaseItem()
    {

        Debug.Log("아이템을 구매했습니다 :  " + item.Name);

        bool enoughInventorySpace = Inventory.instance.Add(item);
        if (enoughInventorySpace == true)
        {
            Destroy(gameObject);
            return true;
        }
        else
        {
            return false;
        }
    }
    
}
