using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleTrapTrigger : MonoBehaviour
{
    public Animator trapAnimator;

    public int needleDamage = 20;

    public bool trapReady = true;

    [Space]

    public bool periodicallyTrigger = false;
    public float periodicallyTriggerStartTime = 0;
    public float periodicallyTriggerRate = 4f;
    public bool periodicallyTrigged = false;


    private void Start()
    {
        if(periodicallyTrigger)
            StartCoroutine(PeriodicallyTriggerStart());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!periodicallyTrigger)
        {
            if (other.CompareTag("Player"))
            {
                TriggedTrap();
            }
            else if (other.CompareTag("Enemy"))
            {
                if (other.GetComponentInChildren<NPC_AI>().npcType == NPC_Type.friendly || other.GetComponentInChildren<NPC_AI>().npcType == NPC_Type.Minion)
                {
                    TriggedTrap();
                }
            }
        }
    }

    IEnumerator PeriodicallyTriggerStart()
    {
        periodicallyTrigged = true;
        yield return new WaitForSeconds(periodicallyTriggerStartTime);

        while(periodicallyTrigged)
        {
            yield return new WaitForSeconds(periodicallyTriggerRate);
            TriggedTrap();
        }
    }

    public void TriggedTrap()
    {
        if(trapReady)
        {
            trapReady = false;
            trapAnimator.SetTrigger("Trigged");
        }
    }
}
