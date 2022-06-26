using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueScripts : MonoBehaviour
{
    public List<BearTrap> BearTrapList = new List<BearTrap>();

    public List<ExplosionTrap> ExplosionTrapList = new List<ExplosionTrap>();

    private PlayerSkill playerSkill;

    private void Start()
    {
        playerSkill = GetComponent<PlayerSkill>();
    }

    public void AddNewBearTrap(BearTrap newTrap)
    {
        /*
        for(int i = 0; i < BearTrapList.Count; i++)
        {
            Destroy(BearTrapList[i].gameObject);
            BearTrapList.RemoveAt(i);
            break;
        }*/

        int trapNumber = 0;
        
        foreach(BearTrap trap in BearTrapList)
        {
            trapNumber++;

            if(PrefabCollect.instance.BearTrapSkill.skillLeveling[playerSkill.GetSkillLevel(PrefabCollect.instance.BearTrapSkill)].value1 <= trapNumber)
            {
                BearTrapList.Remove(trap);
                Destroy(trap.gameObject);
                break;
            }
        }

        BearTrapList.Add(newTrap);
    }

    public void RemoveBearTrap(BearTrap deleteTrap)
    {
        foreach (BearTrap trap in BearTrapList)
        {
            if(trap == deleteTrap)
            {
                BearTrapList.Remove(deleteTrap);
                //Destroy(trap);
                break;
            }
                
        }
    }

    public void AddNewExplosionTrap(ExplosionTrap newTrap)
    {
        int trapNumber = 0;

        foreach (ExplosionTrap trap in ExplosionTrapList)
        {
            trapNumber++;

            if (PrefabCollect.instance.ExplosionTrapSkill.skillLeveling[playerSkill.GetSkillLevel(PrefabCollect.instance.ExplosionTrapSkill)].value1 <= trapNumber)
            {
                ExplosionTrapList.Remove(trap);
                Destroy(trap.gameObject);
                break;
            }
        }

        ExplosionTrapList.Add(newTrap);
    }

    public void RemoveExplosionTrap(ExplosionTrap deleteTrap)
    {
        foreach (ExplosionTrap trap in ExplosionTrapList)
        {
            if (trap == deleteTrap)
            {
                ExplosionTrapList.Remove(deleteTrap);
                //Destroy(trap);
                break;
            }
                
        }
    }
}
