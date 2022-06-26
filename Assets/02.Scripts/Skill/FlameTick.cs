using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameTick : MonoBehaviour
{
    public GameObject Owner;
    public Transform Target;
    public int minDamage = 0;
    public int maxDamage = 0;

    public List<BuffNDebuffObject> debuff = new List<BuffNDebuffObject>();

    public Vector3 rotationOffset;

    public OwnerType ownerType;

    public List<Collider> attackedEnemy = new List<Collider>();


    public void Set(GameObject Owner, int minDamage, int maxDamage, Transform dirTarget)
    {
        this.Owner = Owner;
        this.minDamage = minDamage;
        this.maxDamage = maxDamage;

        transform.root.transform.LookAt(dirTarget.position);
        transform.root.transform.Rotate(rotationOffset);


    }

    private void OnTriggerEnter(Collider other)
    {
        if(!attackedEnemy.Contains(other))
        {
            if (AttackEffectFunctions.TriggerEnterAttack(minDamage, maxDamage, other, Owner, ownerType, debuffs: debuff))
            {
                attackedEnemy.Add(other);
            }
        }
        

        if (other.GetComponent<DestroyableObject>() != null)
        {
            other.GetComponent<DestroyableObject>().GetDamage(1);
            other.GetComponent<DestroyableObject>().GenerateHitParticle(GetComponent<Collider>().bounds.center);
        }
    }

    public void StartTick()
    {
        //Owner.GetComponentInChildren<PlayerStats>().UseMana(3);

    }

    public void EndTick()
    {
        Destroy(transform.root.gameObject);
    }
}
