using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonBomb : MonoBehaviour
{
    public GameObject owner;
    public int mindamage;
    public int maxdamage;
    public BuffNDebuffObject poison;
    public float DeleteTime = 7;

    private void OnCollisionEnter(Collision collision)
    {
        PoisonGas();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            if (other.GetComponent<NPC_AI>() != null)
            {
                if (other.GetComponent<NPC_AI>().npcType == NPC_Type.enemy)
                {
                    PoisonGas();
                }
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("obstacle"))
        {
            PoisonGas();
        }
    }

    public void PoisonGas()
    {
        var gas = Instantiate(PrefabCollect.instance.PoisonGas, GetComponent<Collider>().bounds.center, new Quaternion(0, 0, 0, 0));
        var gasS = gas.GetComponent<PoisonGas>();
        gasS.owner = owner;
        gasS.minDamage = mindamage;
        gasS.maxDamage = maxdamage;
        gasS.poison = poison;
        gasS.DeleteTime = DeleteTime;
        Destroy(gameObject);
    }
}
