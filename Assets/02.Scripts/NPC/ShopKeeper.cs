using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{

    public List<ItemPickup> onSaleItems = new List<ItemPickup>();
    public List<ArtifactPickUp> onSaleArtifacts = new List<ArtifactPickUp>();
    public List<KeyPickUp> onSaleKey = new List<KeyPickUp>();

    [Space]

    public TextBubble textBubble;
    public bool welcomeText = false;
    public bool betrayalText = false;


    public void FreeAllItem()
    {
        print("상인이 죽어서 판매아이템이 무료로 풀림");
        foreach(ItemPickup item in onSaleItems)
        {
            if (item == null)
                continue;

            item.itemOnSale = false;
            item.item.onSaleItem = false;
            item.GetComponent<Rigidbody>().isKinematic = false;
        }

        foreach (ArtifactPickUp artifact in onSaleArtifacts)
        {
            if (artifact == null)
                continue;

            artifact.isOnSale = false;
        }

        foreach (KeyPickUp key in onSaleKey)
        {
            if (key == null)
                continue;

            key.isOnSale = false;
        }

        MazeDungeonNpcSpawner.instance.SpawnShopGuards = true;
    }

    public void SpawnHelloText()
    {
        if (welcomeText)
            return;
        StopCoroutine(textBubble.SpawnTextBubble());
        StartCoroutine(textBubble.SpawnTextBubble());
        textBubble.ChangeText("ShopKeeper-Intro");
        welcomeText = true;
        
        
    }

    public void SpawnAngryTextBubble()
    {
        if (betrayalText)
            return;

        StopCoroutine(textBubble.SpawnTextBubble());
        StartCoroutine(textBubble.SpawnTextBubble());
        textBubble.ChangeText("ShopKeeper-attacked");
        betrayalText = true;
        
        
    }
}
