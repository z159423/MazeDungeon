using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;
using System;

public class DropItem : MonoBehaviour
{
    public ItemDataBase itemDataBase;
    public GameObject itemPickupPrefab;
    [Space]
    public LayerMask DropItemLayer;
    [Space]

    public bool statUpgradeByStage = true;
    public float damageUpgradeByStagePercent = .1f;
    public float defenceUpgradeByStagePercent = .1f;


    [System.Serializable]
    public struct DropTable
    {
        [Tooltip("경험치 개수")]
        public int EXPDropAmount;

        [Tooltip("코인 드롭 개수")]
        public int DropCoinMinAmount;
        public int DropCoinMaxAmount;

        [Tooltip("장비 아이템 드랍 확률")]
        [Range(0, 100)]
        public int EquipmentItemDropPercent;

        [Tooltip("장비 아이템 드랍 최소 개수")]
        [Range(0, 10)]
        public int MinEquipmentItemDropCount;

        [Tooltip("장비 아이템 드랍 최대 개수")]
        [Range(0, 10)]
        public int MaxEquipmentItemDropCount;

        [Tooltip("소모품 드랍 확률")]
        [Range(0, 100)]
        public int ConsumeableItemDropPercent;

        [Tooltip("열쇠 드랍 확률")]
        [Range(0, 100)]
        public int KeyDropPercent;

        [Tooltip("열쇠 드랍 확률")]
        [Range(0, 100)]
        public int ArtifactDropPercent;

        [Tooltip("체력회복하트 드랍 확률")]
        [Range(0, 100)]
        public int HeartDropPercent;


        public void Copy(DropTable table)
        {
            EXPDropAmount = table.EXPDropAmount;
            DropCoinMinAmount = table.DropCoinMinAmount;
            DropCoinMaxAmount = table.DropCoinMaxAmount;
            EquipmentItemDropPercent = table.EquipmentItemDropPercent;
            MinEquipmentItemDropCount = table.MinEquipmentItemDropCount;
            MaxEquipmentItemDropCount = table.MaxEquipmentItemDropCount;
            ConsumeableItemDropPercent = table.ConsumeableItemDropPercent;
            KeyDropPercent = table.KeyDropPercent;
            ArtifactDropPercent = table.ArtifactDropPercent;
            HeartDropPercent = table.HeartDropPercent;
        }
    }


    [Space]
    [Header("=============드롭 태이블================")]
    [Space]

    [Header("노말 몬스터 드롭 태이블")]
    [Space]

    public DropTable normalMonsterDropTable;

    [Header("엘리트 몬스터 드롭 태이블")]
    [Space]

    public DropTable eliteMonsterDropTable;

    [Header("미니보스 몬스터 드롭 태이블")]
    [Space]

    public DropTable minibossDropTable;


    [Header ("보스 몬스터 드롭 태이블")]
    [Space]

    public DropTable bossMonsterDropTable;

    public EnchantClassification enchantClassification;

    public static DropItem instance;


    private void Start()
    {
        instance = this;
    }

    public void dropItem(ItemDataBase itemDataBase, Transform DropPosition, NPCStats stat , NPC_AI ai, PlayerStats playerStats)
    {
        DropTable tempDropTable = new DropTable();

        if(ai.monsterType == MonsterType.Boss)
        {
            if(ai.miniBoss)
                tempDropTable.Copy(minibossDropTable);
            else
                tempDropTable.Copy(bossMonsterDropTable);
        }
        else if(stat.EliteNPC)
        {
            tempDropTable.Copy(eliteMonsterDropTable);
        }
        else
        {
            tempDropTable.Copy(normalMonsterDropTable);
        }

        if (ai.npcType == NPC_Type.enemy)
        {
            //=====================================  장비아이템 드랍  ======================================

            int EquipmentDropAmount;

            if (stat.IndividualEquipmentDropPercent)
            {
                EquipmentDropAmount = UnityEngine.Random.Range(stat.IndividualDropTable.MinEquipmentItemDropCount, stat.IndividualDropTable.MaxEquipmentItemDropCount);
            }
            else
            {
                EquipmentDropAmount = UnityEngine.Random.Range(tempDropTable.MinEquipmentItemDropCount, tempDropTable.MaxEquipmentItemDropCount);
            }

            for (int i = 0; i < EquipmentDropAmount; i++)
            {
                int DropEquipmentItemPercent = UnityEngine.Random.Range(0, 100);
                int RandomPercent;

                if (stat.IndividualEquipmentDropPercent)
                {
                    RandomPercent = stat.IndividualDropTable.EquipmentItemDropPercent;
                }
                else
                {
                    RandomPercent = tempDropTable.EquipmentItemDropPercent;
                }

                if (RandomPercent > DropEquipmentItemPercent)
                {
                    GameObject dropItem = Instantiate(itemDataBase.itemPickupPrefab, DropPosition.position + (DropPosition.up * 2), Quaternion.identity);
                    ItemPickup itemPickUp = dropItem.GetComponent<ItemPickup>();

                    itemPickUp.item = SpawnDropItem(playerStats.playerClass, onSaleItemType.Equipment);
                    ParticleGenerator.instance.GenerateLootEffect(dropItem.transform, itemPickUp.item.itemQuality);
                    
                }
            }

            //=====================================  소모 아이템 드랍  ======================================

            int ConsumableDropPercent = UnityEngine.Random.Range(0, 100);
            int RandomPercent2;

            if (stat.IndividualConsumableDropPercent)
            {
                RandomPercent2 = stat.IndividualDropTable.ConsumeableItemDropPercent;
            }
            else
            {
                RandomPercent2 = tempDropTable.ConsumeableItemDropPercent;
            }

            if (RandomPercent2 > ConsumableDropPercent)
            {
                GameObject dropItem = Instantiate(itemDataBase.itemPickupPrefab, DropPosition.position + (DropPosition.up * 2), Quaternion.identity);
                ItemPickup itemPickUp = dropItem.GetComponent<ItemPickup>();

                itemPickUp.item = CreateConsumeAbleItem();

                ParticleGenerator.instance.GenerateConsumableEffect(dropItem.transform);
            }
            //=====================================  열쇠 드랍  ======================================

            int KeyDropPercent = UnityEngine.Random.Range(0, 100);
            int RandomPercent3;

            if (stat.IndividualKeyDropPercent)
            {
                RandomPercent3 = stat.IndividualDropTable.KeyDropPercent;
            }
            else
            {
                RandomPercent3 = tempDropTable.KeyDropPercent;
            }

            if (RandomPercent3 > KeyDropPercent)
            {
                KeyGenerate(DropPosition.position + (DropPosition.up * 2));
                //Instantiate(itemDataBase.Key, DropPosition.position + (DropPosition.up * 2), Quaternion.identity);
            }

            //=============================================== 아티펙트 드랍 ============================================

            int artifactDropPercent = UnityEngine.Random.Range(0, 100);
            int RandomPercent4;

            if (stat.IndividualArtifactDropPercent)
            {
                RandomPercent4 = stat.IndividualDropTable.ArtifactDropPercent;
            }
            else
            {
                RandomPercent4 = tempDropTable.ArtifactDropPercent;
            }

            if (RandomPercent4 > artifactDropPercent)
            {
                var artifact = ArtifactDropTable.instance.GenerateArtifact(stat.transform.position + new Vector3(0,1,0));

                artifact.GetComponentInChildren<ArtifactPickUp>().isOnSale = false;
            }

            //=============================================== 하트 드랍 ============================================

            int HeartDropPercent = UnityEngine.Random.Range(0, 100);
            int RandomPercent5;

            if (stat.IndividualHeartDropPercent)
            {
                RandomPercent5 = stat.IndividualDropTable.HeartDropPercent;
            }
            else
            {
                RandomPercent5 = tempDropTable.HeartDropPercent;
            }

            if (RandomPercent5 > HeartDropPercent)
            {
                DropHeart.GenerateDropHeart(DropPosition.position);
            }
        }
    }

    public GameObject CreateEquipmentDropItem(Transform DropPosition, CharacterClass characterClass, int tier
        ,bool IndividualPercent = false, int dropPercent = 100)
    {
        if(IndividualPercent)
        {
            int Value = UnityEngine.Random.Range(0, 100);

            if (Value > dropPercent)
                return null;
        }

        GameObject dropItem = Instantiate(itemDataBase.itemPickupPrefab, DropPosition.position + (DropPosition.up * 2), Quaternion.identity);
        ItemPickup itemPickUp = dropItem.GetComponent<ItemPickup>();

        //dropItem.layer = DropItemLayer;

        //itemPickUp.item = SpawnDropItemWithTier(characterClass, false, tier);

        //itemPickUp.item = CreateEquipmentItem(characterClass);

        itemPickUp.item = SpawnDropItem(characterClass, onSaleItemType.Equipment);

        //enchantClassification.AddEnchantToEquipment(itemPickUp.item, characterClass);

        ParticleGenerator.instance.GenerateLootEffect(dropItem.transform, itemPickUp.item.itemQuality);

        print(itemPickUp.item.Name);

        return dropItem;
    }

    public ItemPickup CreateOnSaleItem(Transform DropPosition, CharacterClass characterClass, bool parent, onSaleItemType type)
    {
        GameObject dropItem;

        if (parent)
        {
            dropItem = Instantiate(itemDataBase.itemPickupPrefab, DropPosition.position + (DropPosition.up * 2), Quaternion.identity, DropPosition);
        }
        else
        {
            dropItem = Instantiate(itemDataBase.itemPickupPrefab, DropPosition.position + (DropPosition.up * 2), Quaternion.identity);
        }

        ItemPickup itemPickUp = dropItem.GetComponent<ItemPickup>();

        itemPickUp.item = SpawnDropItem(characterClass, type);

        try
        {
            if (itemPickUp.item.Price == 0)
            {
                switch(itemPickUp.item.itemQuality)
                {
                    case ItemQuality.Common:
                        itemPickUp.item.Price += 50;
                        break;

                    /*case ItemQuality.Uncommon:
                        itemPickUp.item.Price += 60;
                        break;*/

                    case ItemQuality.Rare:
                        itemPickUp.item.Price += 100;
                        break;

                    case ItemQuality.Unique:
                        itemPickUp.item.Price += 250;
                        break;

                    case ItemQuality.Epic:
                        itemPickUp.item.Price += 400;
                        break;
                }
            }
        }
        catch(Exception el)
        {
            Debug.LogError("에러 발생 : " + itemPickUp.item + " " + el);
        }

        
        itemPickUp.item.onSaleItem = true;

        ParticleGenerator.instance.GenerateLootEffect(dropItem.transform, itemPickUp.item.itemQuality);

        print("판매 아이템 생성 : " + itemPickUp.item.Name);

        return itemPickUp;
    }

    public ItemPickup CreateConsumeableItem(Transform DropPosition)
    {
        GameObject dropItem;

        dropItem = Instantiate(itemDataBase.itemPickupPrefab, DropPosition.GetComponent<Collider>().bounds.center, Quaternion.identity);

        ItemPickup itemPickUp = dropItem.GetComponent<ItemPickup>();

        int num = UnityEngine.Random.Range(0, itemDataBase.ConsumeableItem.Count);

        itemPickUp.item = itemDataBase.ConsumeableItem[num];

        //print("소모품 생성 : " + itemPickUp.item.Name);

        ParticleGenerator.instance.GenerateConsumableEffect(itemPickUp.transform);

        return itemPickUp;
    }

    public Item SpawnDropItemWithTier(CharacterClass characterClass, bool SpawnOnlyConsumeable, int tier)
    {
        if (SpawnOnlyConsumeable)
        {
            var value = UnityEngine.Random.Range(0, itemDataBase.ConsumeableItem.Count);
            var cloneItem = Instantiate(itemDataBase.ConsumeableItem[value]);


            return cloneItem;
        }

        int value1 = 0;
        int value2 = 0;
        try
        {
            foreach (dropableitemOnClass Class in itemDataBase.classList)
            {
                if (Class.characterClass == characterClass)
                {
                    value1 = UnityEngine.Random.Range(0, Class.itemList.itemlist.Count);                            //플레이어의 클래스 관련 장비만 나오도록 설정되어 있음

                    value2 = UnityEngine.Random.Range(0, Class.itemList.itemlist[value1].itemlist2[tier - 1].items.Count);           //아이템의 등급과 상관없이 동등한 확률로 스폰되게 설정되어있음

                    print("value1 : " + value1 + " value2 : " + value2);

                    var cloneItem = Instantiate(Class.itemList.itemlist[value1].itemlist2[tier - 1].items[value2].item);

                    cloneItem.Name = Class.itemList.itemlist[value1].itemlist2[tier - 1].items[value2].name;
                    cloneItem.itemTier = tier;
                    cloneItem.minDamageModifier = Class.itemList.itemlist[value1].itemlist2[tier - 1].items[value2].MinDamage;
                    cloneItem.maxDamageModifier = Class.itemList.itemlist[value1].itemlist2[tier - 1].items[value2].MaxDamage;
                    cloneItem.armorModifier = Class.itemList.itemlist[value1].itemlist2[tier - 1].items[value2].Defence;
                    cloneItem.speedModifier = Class.itemList.itemlist[value1].itemlist2[tier - 1].items[value2].Speed;
                    cloneItem.ShieldPointModifier = Class.itemList.itemlist[value1].itemlist2[tier - 1].items[value2].ShieldPoint;

                    cloneItem.itemQuality = setUpItemQuality();

                    return cloneItem;
                }
            }
        }
        catch (System.Exception e)
        {

            Debug.Log("에러발생 : " + "value1 : " + value1 + " value2 : " + value2);
        }



        return null;
    }

    /// <summary>
    ///  장비에 인첸트 부여 장비 퀄리티 마다 부여되는 인첸트의 개수가 달라짐
    ///  [Common = 0개, Uncommon = 1개, Rare = 2개, Unique = 3개, Epic = 4개]
    /// </summary>
    

    

    public Item SpawnDropItem(CharacterClass characterClass, onSaleItemType type)
    {
        try
        {
            if (type == onSaleItemType.Consumable)
            {
                var value1 = UnityEngine.Random.Range(0, itemDataBase.ConsumeableItem.Count);
                var cloneItem = Instantiate(itemDataBase.ConsumeableItem[value1]);

                return cloneItem;
            }

            if(type == onSaleItemType.Equipment)
            {
                var item = CreateEquipmentItem(characterClass);
                enchantClassification.AddEnchantToEquipment(item, characterClass);
                UpgrageEquipmentStatByStageNumber(item);
                return item;

                /*foreach (dropableitemOnClass Class in itemDataBase.classList)
                {
                    if (Class.characterClass == characterClass)
                    {
                        var value1 = UnityEngine.Random.Range(0, Class.itemList.itemlist.Count);                            //플레이어의 클래스 관련 장비만 나오도록 설정되어 있음
                        var value2 = UnityEngine.Random.Range(0, Class.itemList.itemlist[value1].itemlist.Count);           //아이템의 등급과 상관없이 동등한 확률로 스폰되게 설정되어있음

                        print("value1 : " + value1 + " value2 : " + value2);

                        val1 = value1;
                        val2 = value2;

                        var cloneItem = Instantiate(Class.itemList.itemlist[value1].itemlist[value2]);
                        //cloneItem = Class.itemList.itemlist[value1].itemlist[value2];
                        cloneItem.itemQuality = setUpItemQuality();

                        return cloneItem;
                    }
                }*/
            }

            if(type == onSaleItemType.Key)
            {

            }

            if(type == onSaleItemType.Artifact)
            {
                
            }
            
        }
        catch(Exception ex)
        {
            Debug.LogError("DropItem 생성중 오류 발생 아이템  : " + ex);
            return null;
        }

        return null;
    }

    public Item CreateEquipmentItem(CharacterClass characterClass)
    {
        foreach (dropableitemOnClass Class in itemDataBase.classList)
        {
            if (Class.characterClass == characterClass)
            {
                var value1 = UnityEngine.Random.Range(0, Class.itemList.itemlist.Count);
                int value2 = 0;
                Item item;

                switch (setUpItemQuality())
                {
                    case ItemQuality.Common:
                        value2 = UnityEngine.Random.Range(0, Class.itemList.itemlist[value1].itemlist2[0].items.Count);
                        item = Instantiate(Class.itemList.itemlist[value1].itemlist2[0].items[value2].item);
                        item.itemQuality = ItemQuality.Common;
                        return item;

                    case ItemQuality.Rare:
                        value2 = UnityEngine.Random.Range(0, Class.itemList.itemlist[value1].itemlist2[1].items.Count);
                        item = Instantiate(Class.itemList.itemlist[value1].itemlist2[1].items[value2].item);
                        item.itemQuality = ItemQuality.Rare;
                        return item;

                    case ItemQuality.Unique:
                        value2 = UnityEngine.Random.Range(0, Class.itemList.itemlist[value1].itemlist2[2].items.Count);
                        item = Instantiate(Class.itemList.itemlist[value1].itemlist2[2].items[value2].item);
                        item.itemQuality = ItemQuality.Unique;
                        return item;

                    case ItemQuality.Epic:
                        value2 = UnityEngine.Random.Range(0, Class.itemList.itemlist[value1].itemlist2[3].items.Count);
                        item = Instantiate(Class.itemList.itemlist[value1].itemlist2[3].items[value2].item);
                        item.itemQuality = ItemQuality.Epic;
                        return item;
                }
            }

        }

        return null;   
    }

    public void UpgrageEquipmentStatByStageNumber(Item item)        //스테이지마다 장비의 스탯을 업그레이드
    {
        int stageNumber = DungeonGenerator.instance.CurrentStageNumber+1;

        if (statUpgradeByStage)          
        {
            if (item.itemtype == ItemType.Weapon)
            {
                item.minDamageModifier += item.minDamageModifier * (damageUpgradeByStagePercent * stageNumber);
                item.maxDamageModifier += item.maxDamageModifier * (damageUpgradeByStagePercent * stageNumber);
            }
            else if (item.itemtype == ItemType.SecondaryWeapon && item.secondaryWeaponType == SecondaryWeaponType.Shield)
            {
                item.armorModifier += item.armorModifier * (defenceUpgradeByStagePercent * stageNumber);
            }
            else if (item.armorModifier > 0)
            {
                item.armorModifier += item.armorModifier * (defenceUpgradeByStagePercent * stageNumber);
            }
        }
    }

    public static Item CreateItemInstance(ItemDataBase itemDataBase, CharacterStats NPCStat)
    {
        var item = CreatItemInfo(itemDataBase, NPCStat);
        return item;
    }

    public Item CreateConsumeAbleItem()
    {
        int total = 0;

        foreach(var item in itemDataBase.HpPotions2)
        {
            total += item.dropWeight;
        }

        int RandomNumber = UnityEngine.Random.Range(0, total);

        foreach(var item in itemDataBase.HpPotions2)
        {
            if(RandomNumber <= item.dropWeight)
            {
                return item.item;
            }
            else
            {
                RandomNumber -= item.dropWeight;
            }
        }

        return null;
    }

    public void DropCoin(int coinValue,Vector3 position ,Vector3 ForcePower, float eachTime)
    {
        StartCoroutine(CreateCoin(coinValue, position, ForcePower, eachTime));
    }

    IEnumerator CreateCoin(int coinValue, Vector3 position, Vector3 ForcePower ,float Time)
    {
        int GoldCoinValue = coinValue / 10;
        coinValue = coinValue % 10;
        int SilverCoinValue = coinValue / 5;
        coinValue = coinValue % 5;
        int CopperCoinValue = coinValue / 1;

        //Vector3 randomVec3;
        //Vector3 randomForce;

        for (int i = 0; i < GoldCoinValue; i++)
        {
            /*
            randomVec3 = new Vector3(UnityEngine.Random.Range(-0.25f, 0.25f), 0, UnityEngine.Random.Range(-0.25f, 0.25f));
            randomForce = new Vector3(UnityEngine.Random.Range(-3f, 3f), 0, UnityEngine.Random.Range(-3f, 3f));
            var coin = Instantiate(PrefabCollect.instance.GoldCoin, position + randomVec3, Quaternion.identity);
            coin.GetComponent<Rigidbody>().velocity = ForcePower + randomForce;*/
            CoinObjectPool.instance.DeQueueGoldCoin(position);
            yield return new WaitForSeconds(Time);
        }

        for (int i = 0; i < SilverCoinValue; i++)
        {
            /*
            randomVec3 = new Vector3(UnityEngine.Random.Range(-0.25f, 0.25f), 0, UnityEngine.Random.Range(-0.25f, 0.25f));
            randomForce = new Vector3(UnityEngine.Random.Range(-3f, 3f), 0, UnityEngine.Random.Range(-3f, 3f));
            var coin = Instantiate(PrefabCollect.instance.SilverCoin, position + randomVec3, Quaternion.identity);
            coin.GetComponent<Rigidbody>().velocity = ForcePower + randomForce;*/
            CoinObjectPool.instance.DeQueueSilverCoin(position);
            yield return new WaitForSeconds(Time);
        }

        for (int i = 0; i < CopperCoinValue; i++)
        {
            /*
            randomVec3 = new Vector3(UnityEngine.Random.Range(-0.25f, 0.25f), 0, UnityEngine.Random.Range(-0.25f, 0.25f));
            randomForce = new Vector3(UnityEngine.Random.Range(-3f, 3f), 0, UnityEngine.Random.Range(-3f, 3f));
            var coin = Instantiate(PrefabCollect.instance.CopperCoin, position + randomVec3, Quaternion.identity);
            coin.GetComponent<Rigidbody>().velocity = ForcePower + randomForce;*/
            CoinObjectPool.instance.DeQueueCopperCoin(position);
            yield return new WaitForSeconds(Time);
        }
    }

    /*private Item CreateEquipmentItem(CharacterClass characterClass)
    {
        foreach (dropableitemOnClass Class in itemDataBase.classList)
        {
            if (Class.characterClass == characterClass)
            {
                var value1 = UnityEngine.Random.Range(0, Class.itemList.itemlist.Count);                            //플레이어의 클래스 관련 장비만 나오도록 설정되어 있음
                var value2 = UnityEngine.Random.Range(0, Class.itemList.itemlist[value1].itemlist.Count);           //아이템의 등급과 상관없이 동등한 확률로 스폰되게 설정되어있음

                print("value1 : " + value1 + " value2 : " + value2);

                var cloneItem = Instantiate(Class.itemList.itemlist[value1].itemlist[value2]);
                //cloneItem = Class.itemList.itemlist[value1].itemlist[value2];

                return cloneItem;
            }
        }

        return null;
    }*/

    ItemType setUpItemType()
    {
        int R = UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(ItemType)).Length);
        return (ItemType)R;

    }

    WeaponType setUpWeaponType()
    {
        int R = UnityEngine.Random.Range(1, System.Enum.GetNames(typeof(WeaponType)).Length);
        return (WeaponType)R;
    }

    ItemQuality setUpItemQuality()
    {
        int R = UnityEngine.Random.Range(1, 1000);

        //Debug.Log("R = " + R);

        if (1 <= R && R < 450)
        {
            return ItemQuality.Common;
        }
        /*else if (600 <= R && R < 850)
        {
            return ItemQuality.Uncommon;
        }*/
        else if (450 <= R && R < 925)
        {
            return ItemQuality.Rare;
        }
        else if (925 <= R && R < 985)
        {
            return ItemQuality.Unique;
        }
        else if (985 <= R && R < 999)
        {
            return ItemQuality.Epic;
        }
        else
        {
            return ItemQuality.Common;
        }

    }

    public void KeyGenerate(Vector3 position)
    {
        Instantiate(itemDataBase.Key, position, Quaternion.identity);
    }

    static Item CreatItemInfo(ItemDataBase itemData, CharacterStats NPCStat)
    {
        var itemAsset = Item.CreateInstance<Item>();
        //itemAsset = itemDataBase.GetItemByID(5);
        itemAsset.pickupprefab = itemData.itemPickupPrefab;
        itemAsset.itemtype = setUpItemType();               //아이템 타입 설정
        if (itemAsset.itemtype == ItemType.Weapon)        
        {
            itemAsset.weaponType = setUpWeaponType();       //무기 타입 설정
        }

        if(itemAsset.itemtype != ItemType.consumable)       
        {
            itemAsset.itemQuality = setUpItemQuality();     //아이템 등급 설정
        }

        Debug.Log(itemAsset.itemtype);

        if(NPCStat == null)
        {
            int level = UnityEngine.Random.Range(-5, 5) + 1;
            //itemAsset.itemLevel = (level < 1) ? 1 : level;
        } else
        {
            //itemAsset.itemLevel = ItemLevelSetUp(NPCStat.npcLevel);
        }

        itemAsset.skinedMesh = setUpItemMesh(itemAsset);
        if(itemAsset.itemtype == ItemType.consumable)
        {
            itemAsset = itemData.GetConsumableItem();
        }

        try
        {
            itemAsset.name = itemAsset.skinedMesh.name;
            itemAsset.Name = itemAsset.skinedMesh.name;
        }
        catch(Exception el)
        {
            Debug.LogError("에러 발생 : " + itemAsset.ItemNameKey + " " + el);
        }

        if (itemAsset.consumableType == ConsumableType.SkillBook)
            itemAsset.Name = itemAsset.skill.Name + " Skill Book";
        itemAsset.MaxStack = SetUpItemMaxStack(itemAsset);

        if(itemAsset.itemtype == ItemType.Weapon)
        {
            if(itemAsset.weaponType == WeaponType.Dagger)
            {
                //itemAsset.maxDamageModifier = 1 + itemAsset.itemLevel;
            }
            else
            {
                //itemAsset.maxDamageModifier = 1 + (itemAsset.itemLevel * 2);
            }
            itemAsset.maxDamageModifier = (int)SetUpItemDamageWithQuality(itemAsset.itemQuality, itemAsset.maxDamageModifier);

            itemAsset.limit = 10;
            itemAsset.currentLimit = itemAsset.limit;
        }
        else if(itemAsset.itemtype == ItemType.SecondaryWeapon)
        {
            //itemAsset.armorModifier = (1 + itemAsset.itemLevel) / 2;
            itemAsset.armorModifier = (int)SetUpItemDamageWithQuality(itemAsset.itemQuality, itemAsset.armorModifier);
        }
        else if(itemAsset.itemtype == ItemType.Head || itemAsset.itemtype == ItemType.Chest || itemAsset.itemtype == ItemType.Legs)
        {
            //itemAsset.armorModifier = (1 + itemAsset.itemLevel) / 2;
            itemAsset.armorModifier = (int)SetUpItemDamageWithQuality(itemAsset.itemQuality, itemAsset.armorModifier);
        }
        else
        {
            itemAsset.maxDamageModifier = 0;
            itemAsset.armorModifier = 0;
        }



        ItemType setUpItemType()
        {
            int R = UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(ItemType)).Length);
            return (ItemType)R;
        }

        WeaponType setUpWeaponType()
        {
            int R = UnityEngine.Random.Range(1, System.Enum.GetNames(typeof(WeaponType)).Length);
            return (WeaponType)R;
        }

        ItemQuality setUpItemQuality()
        {
            int R = UnityEngine.Random.Range(1, 1000);

            //Debug.Log("R = " + R);

            if (1 <= R && R < 600)      
            {
                return ItemQuality.Common;
            } /*else if(600 <= R && R < 850)
            {
                return ItemQuality.Uncommon;
            }*/ else if(600 <= R && R < 850)
            {
                return ItemQuality.Rare;
            } else if(850 <= R && R < 950)
            {
                return ItemQuality.Unique;
            }
            else if (950 <= R && R < 999)
            {
                return ItemQuality.Epic;
            }
            else
            {
                return ItemQuality.Common;
            }
            
        }

        SkinnedMeshRenderer setUpItemMesh(Item item)
        {
            switch (item.itemtype)
            {
                case ItemType.Weapon:
                    switch (item.weaponType)
                    {
                        case WeaponType.Sword:
                            return itemData.GetMeleeMesh();
                            
                        case WeaponType.Bow:
                            return itemData.GetBowMesh();

                        case WeaponType.Dagger:
                            return itemData.GetDaggerMesh(item);

                        case WeaponType.Wand:
                            return itemData.GetWandMesh();

                        default:
                            Debug.Log("접근할수 없는 무기타입입니다. nullMesh가 반환됨");
                            return itemData.GetNullMesh();
                    }

                case ItemType.Head:
                    return itemData.GetHelmetMesh();

                case ItemType.Chest:
                    return itemData.GetChestArmorMesh();

                case ItemType.Legs:
                    return itemData.GetShoseMesh();

                case ItemType.SecondaryWeapon:
                    return itemData.GetShieldMesh();

                case ItemType.Accessory:
                    return itemData.GetAccessoryMesh();

                case ItemType.consumable:
                    return null;

                default:
                    Debug.Log("접근할 수 없는 아이템 타입입니다.");
                    return null;
            }
            
        }

        return itemAsset;
    }

    static int SetUpItemMaxStack(Item item)
    {
        if(item.itemtype == ItemType.consumable && item.consumableType == ConsumableType.Potion)
        {
            return 99;
        }
        else
        {
            return 1;
        }
    }

    static int ItemLevelSetUp(int DroperLevel)
    {
        int level = UnityEngine.Random.Range(-5, 5) + DroperLevel;
        return (level < 1) ? 1 : level;
    }

    static double SetUpItemDamageWithQuality(ItemQuality itemQuality, double damage)
    {
        /*if(itemQuality == ItemQuality.Uncommon)
        {
            return damage * 1.25f;
        } else*/ if(itemQuality == ItemQuality.Rare)
        {
            return damage * 1.3f;
        }
        else if (itemQuality == ItemQuality.Unique)
        {
            return damage * 1.65f;
        }
        else if (itemQuality == ItemQuality.Epic)
        {
            return damage * 2f;
        }
        else
        {
            return damage;
        }
    }

    
}
