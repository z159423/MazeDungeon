using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ItemDataBase", menuName = "Inventory/List")]
public class ItemDataBase : ScriptableObject
{
    public GameObject itemPickupPrefab;

    public SkinnedMeshRenderer nullMesh;

    [SerializeField]
    public List<Item> totalItemlist = new List<Item>();

    

    [SerializeField] public List<dropableitemOnClass> classList = new List<dropableitemOnClass>();

    [SerializeField] public List<Item> ConsumeableItem = new List<Item>();

    [Space]
    [Header("Warrior----------------------------------------------------")]
    [SerializeField] public List<Item> Sword = new List<Item>();
    [SerializeField] public List<Item> Shield = new List<Item>();
    [SerializeField] public List<Item> Helmet = new List<Item>();
    [SerializeField] public List<Item> Chest = new List<Item>();
    [SerializeField] public List<Item> Pants = new List<Item>();

    [Header("Rogue---------------------------------------------------------")]
    [SerializeField] public List<Item> Dagger = new List<Item>();
    [SerializeField] public List<Item> Hood = new List<Item>();
    [SerializeField] public List<Item> LeatherArmor = new List<Item>();

    [Header("Archer----------------------------------------------------------")]
    [SerializeField] public List<Item> Bow = new List<Item>();

    [Header("Mage---------------------------------------------------------")]
    [SerializeField] public List<Item> Wand = new List<Item>();
    [SerializeField] public List<Item> Robe = new List<Item>();
    [SerializeField] public List<Item> MageHat = new List<Item>();

    [Header("Accessory-----------------------------------------------")]
    [SerializeField] public List<Item> Accessory = new List<Item>();

    [Space]
    [Space]

    [SerializeField]
    public List<SkinnedMeshRenderer> MeleeMesh = new List<SkinnedMeshRenderer>();
    [SerializeField]
    public List<SkinnedMeshRenderer> DaggerMesh = new List<SkinnedMeshRenderer>();
    [SerializeField]
    public List<SkinnedMeshRenderer> DaggerLeftMesh = new List<SkinnedMeshRenderer>();
    [SerializeField]
    public List<SkinnedMeshRenderer> BowMesh = new List<SkinnedMeshRenderer>();
    [SerializeField]
    public List<SkinnedMeshRenderer> ShieldMesh = new List<SkinnedMeshRenderer>();
    [SerializeField]
    public List<SkinnedMeshRenderer> WandMesh = new List<SkinnedMeshRenderer>();

    [SerializeField]
    public List<SkinnedMeshRenderer> HelmetMesh = new List<SkinnedMeshRenderer>();
    [SerializeField]
    public List<SkinnedMeshRenderer> ChestArmorMesh = new List<SkinnedMeshRenderer>();
    [SerializeField]
    public List<SkinnedMeshRenderer> PantsMesh = new List<SkinnedMeshRenderer>();
    [SerializeField]
    public List<SkinnedMeshRenderer> AccessoryMesh = new List<SkinnedMeshRenderer>();

    [System.Serializable]
    public struct DropableItemInfo
    {
        public Item item;
        public int dropWeight;
    }

    [SerializeField]
    public List<Item> HpPotions = new List<Item>();

    [SerializeField]
    public List<DropableItemInfo> HpPotions2 = new List<DropableItemInfo>();

    public GameObject Key;

    public Item GetItemByID(int id)
    {
        for(int i = 0;i < totalItemlist.Count; i++)
        {
            if (id == totalItemlist[i].itemID)
                return totalItemlist[i].getCopy();
        }
        return null;
    }

    public SkinnedMeshRenderer GetNullMesh()
    {
        return nullMesh;
    }
    public SkinnedMeshRenderer GetMeleeMesh()
    {
        int rCount = Random.Range(0, MeleeMesh.Count);
        return MeleeMesh[rCount];
    }

    public SkinnedMeshRenderer GetBowMesh()
    {
        int rCount = Random.Range(0, BowMesh.Count);
        return BowMesh[rCount];
    }

    public SkinnedMeshRenderer GetDaggerMesh(Item item)
    {
        int rCount = Random.Range(0, DaggerMesh.Count);

        item.daggerLeftMesh = DaggerLeftMesh[rCount];
        return DaggerMesh[rCount];
    }

    public SkinnedMeshRenderer GetShieldMesh()
    {
        int rCount = Random.Range(0, ShieldMesh.Count);
        return ShieldMesh[rCount];
    }

    public SkinnedMeshRenderer GetWandMesh()
    {
        int rCount = Random.Range(0, WandMesh.Count);
        return WandMesh[rCount];
    }

    public SkinnedMeshRenderer GetHelmetMesh()
    {
        int rCount = Random.Range(0, HelmetMesh.Count);
        return HelmetMesh[rCount];
    }

    public SkinnedMeshRenderer GetChestArmorMesh()
    {
        int rCount = Random.Range(0, ChestArmorMesh.Count);
        return ChestArmorMesh[rCount];
    }

    public SkinnedMeshRenderer GetShoseMesh()
    {
        int rCount = Random.Range(0, PantsMesh.Count);
        return PantsMesh[rCount];
    }

    public SkinnedMeshRenderer GetAccessoryMesh()
    {
        int rCount = Random.Range(0, AccessoryMesh.Count);
        return AccessoryMesh[rCount];
    }

    public Item GetConsumableItem()
    {
        int rCount = Random.Range(0, HpPotions.Count);
        return HpPotions[rCount];
    }

}

[System.Serializable]
public class dropableitemOnClass
{
    public CharacterClass characterClass;
    public DropableItemList itemList;
}
