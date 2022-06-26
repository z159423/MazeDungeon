using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Needles : MonoBehaviour
{
    public bool NeedleReady = true;

    public NeedleTrapTrigger needleTrap;

    private List<Collider> attackedTarget = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if(!attackedTarget.Contains(other))
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponentInChildren<PlayerStats>().TakeDamage(needleTrap.needleDamage, needleTrap.needleDamage, null, true, true, true);
                attackedTarget.Add(other);
            }
            else if (other.CompareTag("Enemy"))
            {
                other.GetComponentInChildren<NPCStats>().TakeDamage(needleTrap.needleDamage / 3, needleTrap.needleDamage / 3, null, true, true, true);
                attackedTarget.Add(other);
            }
        }
    }

    public void ClearAttackList()
    {
        needleTrap.trapReady = true;
        attackedTarget.Clear();
    }
}
