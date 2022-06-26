using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BuffNDebuff", menuName = "BuffNDebuff/New BuffNDebuff")]
public class BuffNDebuffObject : ScriptableObject
{
    public BuffOrDebuff buffOrDebuff = new BuffOrDebuff();

}

public class CreateInstanceBuffOrDebuff
{
    public static BuffNDebuffObject createInstnace(buffType buffType, GameObject owner, float damage, float endtime, bool notimelimit, float repeattime, float percent, float value1 = 0, float value2 = 0f, float value3 = 0)
    {
        var instance = ScriptableObject.CreateInstance<BuffNDebuffObject>();

        instance.buffOrDebuff.buffDebuff = buffType;
        instance.buffOrDebuff.Damage = damage;
        instance.buffOrDebuff.EndTime = endtime;
        instance.buffOrDebuff.NoTimeLimit = notimelimit;
        instance.buffOrDebuff.RepeatTime = repeattime;
        instance.buffOrDebuff.Percent = percent;
        instance.buffOrDebuff.Owner = owner;
        instance.buffOrDebuff.value1 = value1;
        instance.buffOrDebuff.value2 = value2;
        instance.buffOrDebuff.value3 = value3;

        return instance;
    }

    public static BuffNDebuffObject CopyInstnace(BuffNDebuffObject original)
    {
        var instance = ScriptableObject.CreateInstance<BuffNDebuffObject>();

        instance.buffOrDebuff = original.buffOrDebuff;

        /*instance.buffOrDebuff.buffDebuff = original.buffOrDebuff.buffDebuff;
        instance.buffOrDebuff.Damage = original.buffOrDebuff.Damage;
        instance.buffOrDebuff.EndTime = original.buffOrDebuff.EndTime;
        instance.buffOrDebuff.NoTimeLimit = original.buffOrDebuff.NoTimeLimit;
        instance.buffOrDebuff.RepeatTime = original.buffOrDebuff.RepeatTime;
        instance.buffOrDebuff.Percent = original.buffOrDebuff.Percent;
        instance.buffOrDebuff.Owner = original.buffOrDebuff.Owner;
        instance.buffOrDebuff.value1 = original.buffOrDebuff.value1;
        instance.buffOrDebuff.value2 = original.buffOrDebuff.value2;
        instance.buffOrDebuff.value3 = original.buffOrDebuff.value3;

        instance.buffOrDebuff.iconImage = original.buffOrDebuff.iconImage;
        instance.buffOrDebuff.buffORDebuff = original.buffOrDebuff.buffORDebuff;*/

        return instance;
    }

}

[System.Serializable]
public class BuffOrDebuff
{
    public buffType buffDebuff;
    public buffORDebuff buffORDebuff;

    public GameObject Owner;

    public bool NoTimeLimit = false;
    public bool untilTheNextStage = false;
    public bool enableStack = false;
    public float EndTime = 0;

    public float RepeatTime = 1;
    public float Damage = 0;
    public float Percent = 0;

    private float currentTime = 0;

    public float value1 = 0;
    public float value2 = 0;
    public float value3 = 0;

    public GameObject ParticleEffect;
    public Sprite iconImage;
    public GameObject iconObject;

    public void UpdateThis(BuffOrDebuff newObject)
    {
        NoTimeLimit = newObject.NoTimeLimit;

        untilTheNextStage = newObject.untilTheNextStage;

        enableStack = newObject.enableStack;

        if (newObject.EndTime > EndTime)
            EndTime = newObject.EndTime;
        if (newObject.Damage > Damage)
            Damage = newObject.Damage;

        RepeatTime = newObject.RepeatTime;

        currentTime = EndTime;

        if(newObject.iconObject != null)
            newObject.iconObject.GetComponentInChildren<BuffDebuffIcon>().UpdateInfo(newObject);
    }

    public float GetCurrentRunningTime()
    {
        return currentTime;
    }

    public void SetCurrentRunningTimeToStartTime()
    {
        currentTime = EndTime;

        //if(!untilTheNextStage)
        //    currentTime = EndTime;

        //Debug.LogError(currentTime);
    }

    public void SetCurrentRunningTimeToEndTime()
    {
        currentTime = 0;
        currentTime -= RepeatTime;
    }

    public float TickRunningTime(float time)
    {
        if (NoTimeLimit || untilTheNextStage)
            return currentTime;
        else
        {
            //Debug.LogError(time + " " + currentTime);
            return currentTime -= time;
        }
            
    }


}

public enum buffType { none, poison, stun, Maimed, Restraint, Burn, SlowDown, HealthRegenerate, DamageIncrease, StunImmunity, SlowDownImmunity, DecreaseDefense, DefenceIncrease, ManaRegenerateSpeedUp, DecreaseDamage, IncreaseAttackSpeed, DecreaseAttackSpeed, MoveSpeedIncrease, MoveSpeedDecrease }
public enum buffORDebuff { Buff, Debuff }

