using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockBomb : MonoBehaviour
{
    public float StunDistance = 7;
    public LayerMask TriggerMask;
    public BuffNDebuffObject Stun;
    public GameObject ShockBombParticle;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("obstacle") || collision.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            FlashBang();
        }
        
    }

    void FlashBang()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, StunDistance, PrefabCollect.instance.npcMask);

        var particle = Instantiate(ShockBombParticle, transform.position, Quaternion.identity);

        Destroy(particle, 5);

        foreach (Collider collider in colliders)
        {
            if(collider.GetComponentInChildren<NPC_AI>())
            {
                if (collider.GetComponentInChildren<NPC_AI>().npcType == NPC_Type.enemy || collider.GetComponentInChildren<NPC_AI>().npcType == NPC_Type.neutrality)
                {
                    var clone = Instantiate(Stun) as BuffNDebuffObject;
                    collider.GetComponentInChildren<CharacterBuffDeBuff>().AddBuffOrDebuff(clone);
                }
            }
        }

        Destroy(gameObject);
    }
}
