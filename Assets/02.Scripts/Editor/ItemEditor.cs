using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{

    private Item item;
    private bool _initialized;

    public override void OnInspectorGUI()
    {
        _Initialize();

        EditorGUILayout.TextField("Item Instance Id", item.GetInstanceID().ToString());

        EditorGUILayout.Space();

        item.Name = EditorGUILayout.TextField("Item 이름", item.Name);
        item.ItemNameKey = EditorGUILayout.TextField("item 현지화 Key", item.ItemNameKey);
        item.itemTier = EditorGUILayout.IntField("Item 레밸", item.itemTier);
        item.itemID = EditorGUILayout.IntField("Item ID", item.itemID);
        EditorGUILayout.Space();
        item.MaxStack = EditorGUILayout.IntField("MaxStack", item.MaxStack);
        //item.IndexItemInList = EditorGUILayout.IntField("IndexItemInList", item.IndexItemInList);
        EditorGUILayout.Space();
        item.icon = (Sprite)EditorGUILayout.ObjectField("Icon", item.icon, typeof(Sprite), false);

        EditorGUILayout.Space();

        item.itemtype = (ItemType)EditorGUILayout.EnumPopup(item.itemtype);

        item.itemQuality = (ItemQuality)EditorGUILayout.EnumPopup(item.itemQuality);

        item.canBreakable = EditorGUILayout.Toggle("부서질수 있음", false);

        if (item.itemtype == ItemType.consumable)
        {
            item.consumableType = (ConsumableType)EditorGUILayout.EnumPopup(item.consumableType);

            if (item.consumableType == ConsumableType.Potion)
            {
                item.effectiveDose_Hp = EditorGUILayout.IntField("효과량_HP", item.effectiveDose_Hp);
                item.effectiveDose_Mp = EditorGUILayout.IntField("효과량_MP", item.effectiveDose_Mp);
            }

            if (item.consumableType == ConsumableType.SkillBook)
            {
                item.skill = (Skill)EditorGUILayout.ObjectField("Skill", item.skill, typeof(Skill), false);
            }
        }
        if (item.itemtype == ItemType.Head || item.itemtype == ItemType.Chest || item.itemtype == ItemType.Legs || item.itemtype == ItemType.SecondaryWeapon || item.itemtype == ItemType.Weapon)
        {
            item.armorModifier = EditorGUILayout.FloatField("방어력", (float)item.armorModifier);
            item.minDamageModifier = EditorGUILayout.FloatField("최소 공격력", (float)item.minDamageModifier);
            item.maxDamageModifier = EditorGUILayout.FloatField("최대 공격력", (float)item.maxDamageModifier);
            item.speedModifier = EditorGUILayout.FloatField("추가 이동속도", (float)item.speedModifier);
        }

        if (item.itemtype == ItemType.Weapon)
        {
            item.weaponType = (WeaponType)EditorGUILayout.EnumPopup(item.weaponType);
            item.limit = EditorGUILayout.IntField("사용횟수", item.limit);
        }

        if (item.itemtype == ItemType.SecondaryWeapon)
        {
            item.secondaryWeaponType = (SecondaryWeaponType)EditorGUILayout.EnumPopup(item.secondaryWeaponType);

            if (item.secondaryWeaponType == SecondaryWeaponType.Shield)
            {
                item.ShieldPointModifier = EditorGUILayout.FloatField("방패 강도", (float)item.ShieldPointModifier);
            }
            else if(item.secondaryWeaponType == SecondaryWeaponType.HandCrossBow)
            {
                item.minHandCrossBowDamage = EditorGUILayout.FloatField("한손 석궁 최소 공격력", (float)item.minHandCrossBowDamage);
                item.maxHandCrossBowDamage = EditorGUILayout.FloatField("한손 석궁 최대 공격력", (float)item.maxHandCrossBowDamage);
            }
            
        }

        for (int i = 0; i < item.enchants.Count; i++)
        {
            string fieldName = "Enchant" + i;
            item.enchants[i] = (Enchant)EditorGUILayout.ObjectField(item.enchants[i], typeof(Enchant));
        }

        EditorGUILayout.Space();

        item.pickupprefab = (GameObject)EditorGUILayout.ObjectField("PickUpPrefab", item.pickupprefab, typeof(GameObject), false);
        item.skinedMesh = (SkinnedMeshRenderer)EditorGUILayout.ObjectField("Mesh", item.skinedMesh, typeof(SkinnedMeshRenderer), false);


        EditorUtility.SetDirty(item);
    }


    private void _Initialize()
    {
        if (!_initialized)
        {
            item = (Item)target;
            _initialized = true;
        }

    }
}