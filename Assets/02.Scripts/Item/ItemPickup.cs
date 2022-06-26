using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ItemPickup: Interactable
{
    public Item item;
    public Text ItemPickUpText;

    public bool itemOnSale = false; 
    public int Price = 0;
    public ShopKeeper shopKeeper = null;

    GameObject players;
    GameObject playerEquipPoint;
    SkinnedMeshRenderer meshRenderer;
    new MeshCollider collider;

    new public string name;

    [Space]

    public Material[] pickupItemMaterial;

    Transform[] childTrans;
    private void Start()
    {
        if (!item)
            Debug.Log("아이템 픽업에 아이템이 없습니다.");

        childTrans = GetComponentsInChildren<Transform>();
        collider = GetComponentInChildren<MeshCollider>();
        //GetComponentInChildren<MeshRenderer>().materials = pickupItemMaterial;                  //아이템이 플레이어가 장착중인 아이템과 같은 메쉬면 커져보이는 문제 해결용
        Mesh temp_Mesh = Instantiate(item.skinedMesh.sharedMesh);
        GetComponentInChildren<MeshFilter>().sharedMesh = temp_Mesh;
        meshRenderer = item.skinedMesh;
        collider.sharedMesh = meshRenderer.sharedMesh;

        players = GameObject.FindGameObjectWithTag("Player");
        name = item.name;

        Price = item.Price;
        itemOnSale = item.onSaleItem;

        GetComponentInChildren<MeshRenderer>().materials = item.skinedMesh.sharedMaterials;
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

    public bool Itempickup()
    {
        if(itemOnSale)
        {
            if (Inventory.instance.coinAmount < Price)
            {
                InfoMessageManager.instance.PopupInfoMessage("NotEnoughMoney");
                Debug.Log("코인이 부족하여 구매할 수 없습니다 코인갯수 :  " + Inventory.instance.coinAmount + ", 아이템 이름 : " + item.Name);
                return false;
            }
            else
            {
                Debug.Log("아이템을 구매하였습니다 :  " + item.Name);

                bool enoughInventorySpace = Inventory.instance.Add(item);
                if (enoughInventorySpace == true)
                {
                    Inventory.instance.LoseCoin(Price);
                    AudioManager.instance.GenerateAudioAndPlaySFX("purchase", GetComponentInChildren<Collider>().bounds.center);
                    Destroy(gameObject);
                    return true;
                }
                else
                {

                    return false;
                }
            }
        }
        else
        {
            Debug.Log("Picking up " + item.Name);

            bool enoughInventorySpace = Inventory.instance.Add(item);
            if (enoughInventorySpace == true)
            {
                AudioManager.instance.GenerateAudioAndPlaySFX("pickup1", GetComponentInChildren<Collider>().bounds.center);
                Destroy(gameObject);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    
}
