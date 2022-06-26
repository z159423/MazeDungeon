using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string Name = "New Item";
    public string ItemNameKey = "ItemNameKey";
    public int itemID, itemTier, MaxStack;

    public int effectiveDose_Hp, effectiveDose_Mp;

    public double armorModifier, minDamageModifier, maxDamageModifier, speedModifier, ShieldPointModifier, minHandCrossBowDamage, maxHandCrossBowDamage;

    public int limit = 10;
    public int currentLimit = 10;

    public bool canBreakable = false;

    public float attackSpeed;

    public Sprite icon = null;
    public bool isDefaultItem = false;

    public ItemType itemtype;
    public WeaponType weaponType;
    public SecondaryWeaponType secondaryWeaponType;
    public ConsumableType consumableType;
    public ItemQuality itemQuality;
    
    public GameObject pickupprefab;
    public SkinnedMeshRenderer skinedMesh;
    public SkinnedMeshRenderer daggerLeftMesh;
    public Mesh mesh2;

    public List<Enchant> enchants = new List<Enchant>();
    public Item() { }

    public Skill skill;

    private PlayerStats playerstat;

    public int Price = 0;
    public bool onSaleItem = false;


    public virtual bool Use()
    {
        if (this.itemtype == ItemType.consumable)
        {
            if(this.consumableType == ConsumableType.Potion)
            {
                Debug.Log(Name + "포션 사용시도");
                
                return GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>().Usingpotion(effectiveDose_Hp, effectiveDose_Mp);

            }
            else
            {
                return false;
            }
            

        }
        else
        {
            Debug.Log("Using " + Name);
            EquipmentManager.instance.Equip(this);
            return false;
        }
    }

    public void RemoveFormInventory()
    {
        Inventory.instance.Remove(this);
    }

    public Item getCopy()
    {
        return (Item)this.MemberwiseClone();
    }

    public void CreatInstance()
    {
        var Itemasset = CreateInstance<Item>();
    }
}

public enum ItemType {Head, Chest, Legs, Weapon, SecondaryWeapon, Accessory, consumable }
public enum ConsumableType {Potion, SkillBook }
public enum WeaponType {None, Sword, Bow, Dagger, Wand }
public enum SecondaryWeaponType {None, Shield, HandCrossBow }
public enum ItemQuality {Common, Rare, Unique, Epic}
