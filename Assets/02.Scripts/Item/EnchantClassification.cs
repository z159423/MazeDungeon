using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantClassification : MonoBehaviour
{

    //==============================================================Weapon=====================================================================
    [Header("Weapon")]

    public Enchant[] WeapomCommual;

    public Enchant[] Sword;
    public Enchant[] Bow;
    public Enchant[] Dagger;
    public Enchant[] Wand;
    public Enchant[] Shield;
    public Enchant[] HandCrossBow;

    [Space]

    //==============================================================Armor=====================================================================
    [Header("Armor")]

    public Enchant[] ArmorCommual;

    public Enchant[] Helmet;
    public Enchant[] Armor;
    public Enchant[] Foots;


    [Space]
    public ClassIdentityEnchant[] ArmorIdentityEnchant;


    public void AddEnchantToEquipment(Item item, CharacterClass characterClass)
    {
        switch (item.itemQuality)
        {
            case ItemQuality.Common:

                break;

            /*case ItemQuality.Uncommon:
                AddEnchant(item, 1);
                break;*/

            case ItemQuality.Rare:
                AddEnchant(item, 1, characterClass);
                break;

            case ItemQuality.Unique:
                AddEnchant(item, 2, characterClass);
                break;

            case ItemQuality.Epic:
                AddEnchant(item, 3, characterClass);
                break;
        }
    }

    [System.Serializable]
    public class ClassIdentityEnchant
    {
        public CharacterClass CharacterClass;

        public Enchant[] Helmet;
        public Enchant[] Armor;
        public Enchant[] Foots;
    }

    Item AddEnchant(Item item, int EnchantCount, CharacterClass characterClass)
    {
        List<Enchant> AddableEnchantStack = new List<Enchant>();

        if (item.itemtype == ItemType.Weapon)
        {
            foreach (Enchant enchant in WeapomCommual)
            {
                AddableEnchantStack.Add(enchant);
            }

            if (item.weaponType == WeaponType.Sword)
            {
                foreach (Enchant enchant in Sword)
                {
                    AddableEnchantStack.Add(enchant);
                }
            }
            else if (item.weaponType == WeaponType.Bow)
            {
                foreach (Enchant enchant in Bow)
                {
                    AddableEnchantStack.Add(enchant);
                }
            }
            else if (item.weaponType == WeaponType.Dagger)
            {
                foreach (Enchant enchant in Dagger)
                {
                    AddableEnchantStack.Add(enchant);
                }
            }
            else if (item.weaponType == WeaponType.Wand)
            {
                foreach (Enchant enchant in Wand)
                {
                    AddableEnchantStack.Add(enchant);
                }
            }
        }

        if (item.itemtype == ItemType.SecondaryWeapon)
        {
            if (item.secondaryWeaponType == SecondaryWeaponType.HandCrossBow)
            {
                foreach (Enchant enchant in HandCrossBow)
                {
                    AddableEnchantStack.Add(enchant);
                }
            }
            else if (item.secondaryWeaponType == SecondaryWeaponType.Shield)
            {
                foreach (Enchant enchant in Shield)
                {
                    AddableEnchantStack.Add(enchant);
                }
            }
        }

        if (item.itemtype == ItemType.Head)
        {
            foreach (Enchant enchant in ArmorCommual)
            {
                AddableEnchantStack.Add(enchant);
            }

            foreach (Enchant enchant in Helmet)
            {
                AddableEnchantStack.Add(enchant);
            }

            foreach(ClassIdentityEnchant identityEnchant in ArmorIdentityEnchant)
            {
                if(characterClass == identityEnchant.CharacterClass)
                {
                    foreach (Enchant enchant in identityEnchant.Helmet)
                    {
                        AddableEnchantStack.Add(enchant);
                    }
                }
            }
        }

        if (item.itemtype == ItemType.Chest)
        {
            foreach (Enchant enchant in ArmorCommual)
            {
                AddableEnchantStack.Add(enchant);
            }

            foreach (Enchant enchant in Armor)
            {
                AddableEnchantStack.Add(enchant);
            }

            foreach (ClassIdentityEnchant identityEnchant in ArmorIdentityEnchant)
            {
                if (characterClass == identityEnchant.CharacterClass)
                {
                    foreach (Enchant enchant in identityEnchant.Armor)
                    {
                        AddableEnchantStack.Add(enchant);
                    }
                }
            }
        }

        if (item.itemtype == ItemType.Legs)
        {
            foreach (Enchant enchant in ArmorCommual)
            {
                AddableEnchantStack.Add(enchant);
            }

            foreach (Enchant enchant in Foots)
            {
                AddableEnchantStack.Add(enchant);
            }

            foreach (ClassIdentityEnchant identityEnchant in ArmorIdentityEnchant)
            {
                if (characterClass == identityEnchant.CharacterClass)
                {
                    foreach (Enchant enchant in identityEnchant.Foots)
                    {
                        AddableEnchantStack.Add(enchant);
                    }
                }
            }
        }

        for(int i = 0; i < EnchantCount; i++)
        {
            try
            {
                int num = Random.Range(0, AddableEnchantStack.Count);

                //var instanceEnchant = ScriptableObject.CreateInstance<Enchant>();
                var instanceEnchant = Instantiate<Enchant>(AddableEnchantStack[num]);

                instanceEnchant.enchants.enchantType = AddableEnchantStack[num].enchants.enchantType;

                instanceEnchant.enchants.EnchantCurrentLevel = SetEncahntLevel(item);

                item.enchants.Add(instanceEnchant);

                AddableEnchantStack.RemoveAt(num);
            }
            catch(System.Exception el)
            {
                Debug.LogError("아이템에 넣을 인첸트 개수가 부족합니다. + " + el);
                return item;
            }
        }

        return item;
    }

    int SetEncahntLevel(Item item)
    {
        switch (item.itemQuality)
        {
            case ItemQuality.Common:
                return 1;

            /*case ItemQuality.Uncommon:

                int num = Random.Range(1, 3);

                return num;*/

            case ItemQuality.Rare:
                int value = Random.Range(1, 3);
                return value;

            case ItemQuality.Unique:
                value = Random.Range(2, 4);
                return value;

            case ItemQuality.Epic:
                value = Random.Range(2, 4);
                return value;
        }

        return 1;
    }

    [System.Serializable]
    public class ItemTypeClassification
    {
        public bool itemTypeCommual = false;
        public ItemType itemType;

        public bool weaponTypeCommual = false;
        public WeaponType weaponType;

        public bool secondaryWeaponTypeCommual = false;
        public SecondaryWeaponType secondaryWeaponType;

        public bool characterClassCommual = false;
        public CharacterClass characterClass;

        public Enchant enchant;

    }
}
