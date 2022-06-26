using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New DropAbleItemList", menuName = "DropAbleItemList")]
public class DropableItemList : ScriptableObject
{
    [SerializeField] public CharacterClass characterclass;
    [SerializeField] public List<dropableitemlist> itemlist = new List<dropableitemlist>();
}


[System.Serializable]
public class dropableitemlist
{
    [SerializeField] public ItemType itemType;
    [SerializeField] public SecondaryWeaponType leftWeaponType;
    [SerializeField] public WeaponType rightWeaponType;

    [SerializeField] public List<Item> itemlist = new List<Item>();
    [SerializeField] public List<dropAbleItem> itemlist2 = new List<dropAbleItem>();
}

[System.Serializable]
public class dropAbleItem
{
    //public int Tier;
    public ItemQuality itemQuality;
    public List<ItemInfo> items = new List<ItemInfo>();
}

[System.Serializable]
public class ItemInfo
{
    public string name;
    public Item item;
    public int MinDamage;
    public int MaxDamage;
    public int Defence;
    public int Speed;
    public int ShieldPoint;

}
