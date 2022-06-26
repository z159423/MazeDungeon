using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item_fix{

    public Item item;
    public string Name = "New Item";
    public int itemID, effectiveDose_Hp, effectiveDose_Mp, MaxStack, IndexItemInList = 999;
    public float armorModifier, damageModifier;

    public Sprite icon = null;
    public bool isDefaultItem = false;
    public ItemType itemtype;
    public GameObject pickupprefab;
    public SkinnedMeshRenderer mesh;

    private PlayerStats playerstat;

    private void Awake()
    {

    }

    public virtual void Use()
    {


        if (this.itemtype == ItemType.consumable)
        {
            Debug.Log(Name + "소모품사용");
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>().Usingpotion(effectiveDose_Hp, effectiveDose_Mp);

        }
    }



    public Item getCopy()
    {
        return (Item)this.MemberwiseClone();
    }
}

//public enum ItemType { Head, Chest, Legs, Weapon, Shield, Feet, consumable }