using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameTrap : MonoBehaviour
{
    public FlameTrapTrigger trap;

    public List<Collider> ColliderList = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ColliderList.Add(other);
        }
        else if (other.CompareTag("Enemy"))
        {
            ColliderList.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (ColliderList.Contains(other))
            ColliderList.Remove(other);
    }

    public IEnumerator startDamaging()
    {
        while(trap.firing)
        {
            foreach (Collider collider in ColliderList)
            {
                if (collider.CompareTag("Player"))
                {
                    collider.GetComponent<PlayerStats>().TakeDamage(trap.flameMinDamage, trap.flameMaxDamage, null, false, true, true);
                }
                else if (collider.CompareTag("Enemy"))
                {
                    collider.GetComponent<NPCStats>().TakeDamage(trap.flameMinDamage / 3, trap.flameMaxDamage / 3, null, false, true, true);
                }
            }
            yield return new WaitForSeconds(trap.takeDamageRate);
        }

    }
}
