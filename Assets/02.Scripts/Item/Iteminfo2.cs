using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Iteminfo2 {

    public string itemName;
    public int itemID, itemSeed, effectiveDose_Hp, effectiveDose_Mp, MaxStack, IndexItemInList = 999;
    public float armorModifier, damageModifier;

    public Item item;
    public Sprite icon = null;
    public bool isDefaultItem = false;
    public ItemType itemtype;
    public GameObject pickupprefab;
    public SkinnedMeshRenderer mesh;

}
