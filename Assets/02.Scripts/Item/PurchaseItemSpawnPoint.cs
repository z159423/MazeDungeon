using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseItemSpawnPoint : MonoBehaviour
{
    public ShopKeeper shopKeeper;
    public onSaleItemType onSaleItemType;


    void Start()
    {
        if(!DungeonGenerator.instance.isShopKeeperDead)
        {
            if(onSaleItemType == onSaleItemType.Artifact)
            {
                GameObject artifact = ArtifactDropTable.instance.GenerateArtifact(transform.position + (Vector3.up * .5f));
                if (artifact)
                {
                    artifact.transform.SetParent(transform);
                    artifact.GetComponent<ArtifactPickUp>().isOnSale = true;
                    artifact.GetComponent<ArtifactPickUp>().ChangePurchaseValue();
                    shopKeeper.onSaleArtifacts.Add(artifact.GetComponent<ArtifactPickUp>());
                }
                else
                {
                    Debug.LogError("아티펙트가 생성되지 않았습니다.");
                }
                
            }
            else if(onSaleItemType == onSaleItemType.Key)
            {
                shopKeeper.onSaleKey.Add(Instantiate(PrefabCollect.instance.keyPickUp,transform.position,Quaternion.identity,transform).GetComponent<KeyPickUp>());
            }
            else
            {
                ItemPickup onSaleItem = DropItem.instance.CreateOnSaleItem(transform, GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerStats>().playerClass, true, onSaleItemType);
                StartCoroutine(GetKinematic(onSaleItem.GetComponent<Rigidbody>()));
                shopKeeper.onSaleItems.Add(onSaleItem);
            }
            
        }
    }

    IEnumerator GetKinematic(Rigidbody rigidbody)
    {
        yield return new WaitForSeconds(1);
        rigidbody.isKinematic = true;
    }
}

public enum onSaleItemType { Equipment, Consumable, Key, Artifact }
