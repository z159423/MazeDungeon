using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enchant", menuName = "Enchant/new Enchant")]
public class Enchant : ScriptableObject
{
    public Enchants enchants;

    public value value1;
    public value value2;
    public value value3;
    public value value4;
    public value value5;

    [System.Serializable]
    public class value
    {
        public string name = "valueName";
        public valueElement[] element;

        [System.Serializable]
        public class valueElement
        {
            public float value;
        }
    }

}

[System.Serializable]
public class Enchants
{
    public EnchantType enchantType;
    public int EnchantCurrentLevel;
    public int EnchantMaxLevel;
}

public enum EnchantType { DamageIncrease, AttackSpeedIncrease, LeechLife, SlowDown, Binding, Poisoning, Burn, Stun, ElectricShock, Plunder, IncreaseCriticalChance,
                            WideAttack, DoubleShot, PenetrateShot, Assassination, SturdyShield, ArmorIncrease, MaxHealthIncrease, MaxStaminaIncrease, MaxManaIncrease,
                            QuickStealth, Berserker, ManaRecoveryIncrease, Thorn, FrostArmor, MoveSpeedIncrease, sneakMoveSpeedIncrease, AttackAbsorption, ManaDrain,DecreaseDefense,
                            ManaRecoverySpeedIncrease}
