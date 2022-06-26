using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class CharacterCombat : MonoBehaviour {

    CharacterStats myStats;

    void Start ()
    {
        myStats = GetComponent<CharacterStats>();
    }

    public void Attack (CharacterStats targetStats)
    {
        targetStats.TakeDamage(Mathf.RoundToInt(myStats.minDamage.GetFinalStatValue())
            , Mathf.RoundToInt(myStats.maxDamage.GetFinalStatValue()), transform.gameObject, true, true, false);
    }
}
