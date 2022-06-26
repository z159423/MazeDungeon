using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillQuickSlot : MonoBehaviour
{
    public static SkillQuickSlot instance;

    public SkillSlot[] skillSlot = new SkillSlot[9];

    public GameObject ShieldBlockSlot;
    public GameObject FlameSlot;
    public GameObject SneakSlot;
    public GameObject ChargingShotSlot;


    private void Awake()
    {
        instance = this;
    }

    public void ActiveDefaultSkillSlot(CharacterClass characterClass)
    {
        switch(characterClass)
        {
            case CharacterClass.Warrior:
                ShieldBlockSlot.SetActive(true);
                break;

            case CharacterClass.Rogue:
                SneakSlot.SetActive(true);
                break;

            case CharacterClass.Wizard:
                FlameSlot.SetActive(true);
                break;

            case CharacterClass.Archer:
                ChargingShotSlot.SetActive(true);
                break;
        }
    }

}
