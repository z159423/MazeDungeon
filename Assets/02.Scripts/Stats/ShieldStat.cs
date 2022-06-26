using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShieldStat
{
    [SerializeField]
    private float baseValue = 0;

    [SerializeField]
    public List<ShieldInfo> Shields = new List<ShieldInfo>();

    public Transform ownerTransform;
    private CharacterStats stat;
    private NPC_Type npcType;

    /// <summary>
    /// 쉴드를 충전하는 함수
    /// </summary>
    /// <param name="divisionName">쉴드를 구분하는 이름</param>
    /// <param name="ShieldValue">충전하는 쉴드량</param>
    /// <param name="MaxShieldValue">최대로 충전되는 쉴드량</param>
    public void AddShield(string divisionName, int ShieldValue, int MaxShieldValue, CharacterStats stat)
    {
        this.stat = stat;
        foreach(ShieldInfo shieldInfo in Shields)
        {
            if(shieldInfo.ShieldDivisionName.Equals(divisionName))      //이미 같은 쉴드가 있는 경우 그 쉴드에 추가로 쉴드를 충전함
            {
                shieldInfo.UpdateThisShield(ShieldValue, MaxShieldValue);
                return;
            }
        }

        ShieldInfo newShieldInfo = new ShieldInfo();
        newShieldInfo.SetShieldInfo(divisionName, MaxShieldValue, ShieldValue);

        AddNewShield(newShieldInfo);
    }

    void AddNewShield(ShieldInfo shieldInfo)
    {
        Shields.Add(shieldInfo);
    }

    public int DamageToShield(int damageValue, GameObject other)        //보유중인 쉴드에서 데미지만큼 소모함 반환값은 쉴드를 소모 하고 남은 데미지
    {
        int remainDamage = damageValue;

        while(Shields.Count > 0)
        {
            remainDamage = Shields[0].DamageThisShield(remainDamage);

            if (remainDamage > 0)
            {
                Shields.RemoveAt(0);
                
            }
            else
            {
                break;
            }
        }

        if(remainDamage > 0)
        {
            if ((damageValue - remainDamage) > 0)
            {
                if(stat.GetComponent<NPC_AI>() != null)
                {
                    npcType = stat.GetComponent<NPC_AI>().npcType;
                }
                else if(stat.GetComponent<PlayerStats>() != null)
                {
                    npcType = NPC_Type.friendly;
                }

                FloatingTextController.GenerateShieldFloatingText((damageValue - remainDamage).ToString(), ownerTransform, 25, npcType, ownerTransform.gameObject);
                //AudioManager.instance.GenerateAudioAndPlaySFX("shieldHit", ownerTransform.position);
            }
            

            return remainDamage;
        }
        else
        {
            if(damageValue > 0)
            {
                if (stat.GetComponent<NPC_AI>() != null)
                {
                    npcType = stat.GetComponent<NPC_AI>().npcType;
                }
                else if (stat.GetComponent<PlayerStats>() != null)
                {
                    npcType = NPC_Type.friendly;
                }

                FloatingTextController.GenerateShieldFloatingText(damageValue.ToString(), ownerTransform, 25, npcType, ownerTransform.gameObject);
                //AudioManager.instance.GenerateAudioAndPlaySFX("shieldHit", ownerTransform.position);
            }

            return 0;
        }
    }

    public void DeleteShield(ShieldInfo shieldInfo)
    {
        Shields.Remove(shieldInfo);
    }

    public void DeleteShieldAsName(string name)
    {
        for(int i = 0; i < Shields.Count; i++)
        {
            if (Shields[i].ShieldDivisionName.Equals(name))
                DeleteShield(Shields[i]);
        }

        /*foreach(ShieldInfo shieldInfo in Shields)
        {
            if (shieldInfo.ShieldDivisionName.Equals(name))
                DeleteShield(shieldInfo);
        }*/
    }

    public int GetTotalShieldValue()
    {
        int totalValue = 0;
        foreach(ShieldInfo shieldInfo in Shields)
        {
            totalValue += shieldInfo.CurrentShieldValue;
        }

        return totalValue;
    }

    [SerializeField]
    public class ShieldInfo
    {
        public string ShieldDivisionName;
        public int MaxShield;
        public int CurrentShieldValue;

        public bool limitTime = false;
        public float shieldEndTime;

        public GameObject shieldParticle;

        public void SetShieldInfo(string divisionName, int maxShield, int beginningShieldValue)
        {
            ShieldDivisionName = divisionName;
            MaxShield = maxShield;

            UpdateThisShield(beginningShieldValue, maxShield);
        }

        public void UpdateThisShield(int shieldValue, int maxShield)
        {
            CurrentShieldValue += shieldValue;

            if (MaxShield < maxShield)
                MaxShield = maxShield;

            CurrentShieldValue = Mathf.Clamp(CurrentShieldValue, 0, MaxShield);
        }

        public int DamageThisShield(int damageValue)
        {
            int remainDamage = damageValue - CurrentShieldValue;

            CurrentShieldValue -= damageValue;

            return remainDamage;
        }
    }
}
