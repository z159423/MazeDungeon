using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FlameTrapTrigger : MonoBehaviour
{
    public List<VisualEffect> flameTrapVFX = new List<VisualEffect>();

    public FlameTrap flameTrap;

    public Animator trapAnimator;

    public float takeDamageRate = .5f;
    public int flameMinDamage;
    public int flameMaxDamage;
    public bool trapReady = true;
    public bool firing = false;

    [Space]
    public AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
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

    public void TriggedTrap()
    {
        if(trapReady)
        {
            trapReady = false;
            trapAnimator.SetTrigger("Triggred");
        }
    }

    public void StartTrap()
    {
        firing = true;
        foreach (VisualEffect visualEffect in flameTrapVFX)
        {
            visualEffect.Play();
            audioSource.Play();
        }

        StartCoroutine(flameTrap.startDamaging());
    }

    public void StopTrap()
    {
        firing = false;
        foreach (VisualEffect visualEffect in flameTrapVFX)
        {
            visualEffect.Stop();
            audioSource.Stop();
        }
    }

    public void TrapReady()
    {
        trapReady = true;
    }
}
