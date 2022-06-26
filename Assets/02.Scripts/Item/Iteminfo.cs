using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Iteminfo : MonoBehaviour
{


    public string itemName;
    public int itemID, itemSeed, effectiveDose_Hp, effectiveDose_Mp, MaxStack, IndexItemInList = 999;
    public float armorModifier, damageModifier;

    [SerializeField]
    public Item item;
    [SerializeField]
    public Sprite icon = null;
    [SerializeField]
    public bool isDefaultItem = false;
    [SerializeField]
    public ItemType itemtype;
    [SerializeField]
    public GameObject pickupprefab;
    [SerializeField]
    public SkinnedMeshRenderer mesh;


    private PlayerStats playerstat;

    private void Awake()
    {

    }

    public void GetItemSeed()
    {
        itemSeed = Random.Range(1,1000000);
    }

    public virtual void Use()
    {

        if (this.itemtype == ItemType.consumable)
        {
            Debug.Log(itemName + "소모품사용");
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>().Usingpotion(effectiveDose_Hp, effectiveDose_Mp);

        }
    }

    public Item getCopy()
    {
        return (Item)this.MemberwiseClone();
    }

}