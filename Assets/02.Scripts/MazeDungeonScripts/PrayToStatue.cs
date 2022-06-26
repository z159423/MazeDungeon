using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrayToStatue : MonoBehaviour
{
    public StatueType statueType;

    public List<BuffNDebuffObject> confirmBuffOrDebuffs = new List<BuffNDebuffObject>();

    [Space]

    public bool randombuff = false;
    public int getBuffCount = 2;
    public List<BuffNDebuffObject> randomBuffOrDebuffs = new List<BuffNDebuffObject>();

    public bool GoldRequire = false;
    public int GoldValue;
    public bool HealthRequire = false;
    public int HealthValue;

    public bool ActiveStatue(Transform player)
    {
        if(GoldRequire)
        {
            if(Inventory.instance.coinAmount < GoldValue)
            {
                InfoMessageManager.instance.PopupInfoMessage("NotEnoughMoney");
                return false;
            }
            else
            {
                Inventory.instance.LoseCoin(GoldValue);
            }
            
        }

        if(HealthRequire)
        {
            player.GetComponent<PlayerStats>().TakeDamage(HealthValue, HealthValue, null, false, false, true, isDebuffDamage : true);
        }

        GetComponent<Collider>().enabled = false;

        switch (statueType)
        {
            case StatueType.Angel:
                print("여신상에 기도함");
                AudioManager.instance.GenerateAudioAndPlaySFX("angelStatue", player.position);

                break;

            case StatueType.Devil:
                print("악마에 기도함");
                AudioManager.instance.GenerateAudioAndPlaySFX("devilStatue", player.position);

                break;

            case StatueType.Strength:
                print("힘의석상에 기도함");

                break;

            case StatueType.Richress:
                print("풍요의석상에 기도함");


                break;

            case StatueType.Health:
                print("회복의석상에 기도함");

                break; 
        }

        if (randombuff)
        {
            List<BuffNDebuffObject> buffs = new List<BuffNDebuffObject>();

            int i = 0;
            while (i < getBuffCount)
            {
                int random = Random.Range(0, randomBuffOrDebuffs.Count);
                if (!buffs.Contains(randomBuffOrDebuffs[random]))
                {
                    player.GetComponent<CharacterBuffDeBuff>().AddBuffOrDebuff(randomBuffOrDebuffs[random]);
                    buffs.Add(randomBuffOrDebuffs[random]);
                    i++;
                }
            }

        }
        else
        {
            for (int i = 0; i < confirmBuffOrDebuffs.Count; i++)
            {
                player.GetComponent<CharacterBuffDeBuff>().AddBuffOrDebuff(confirmBuffOrDebuffs[i]);
            }
        }
        

        return true;
    }
}

public enum StatueType { none, Angel, Devil, Strength, Richress, Health }
