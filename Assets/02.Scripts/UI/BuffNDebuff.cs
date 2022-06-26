using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "BuffNDebuff", menuName = "Debuff")]
public class BuffNDebuff : ScriptableObject
{
    public List<buffandDebuff> bufforDebuff = new List<buffandDebuff>();

    [System.Serializable]
    public class buffandDebuff
    {
        public buffType debuff;
        public Sprite icon;
    }

    public Sprite GetIcon(buffType debuff)
    {
        foreach(buffandDebuff Debuff in bufforDebuff)
        {
            if (Debuff.debuff == debuff)
                return Debuff.icon;
        }

        Debug.Log("알맞는 아이콘이 없습니다.");
        return null;
    }

}
