using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

public class BearTrap : MonoBehaviour
{
    public GameObject owner;
    public int damage = 0;
    public int stunTime = 3;
    public int endTrapTime = 30;

    public BuffNDebuffObject stun;

    public RogueScripts rougeScripts;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("EndTrap", endTrapTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<NPC_AI>() != null && other.CompareTag("Enemy"))
        {
            if (other.GetComponentInParent<NPC_AI>().npcType == NPC_Type.enemy)
            {
                GetComponentInChildren<Animator>().SetTrigger("Trigged");
                other.GetComponentInParent<NPCStats>().TakeDamage(damage,damage, owner, true, true, false, notBackAttack: true);

                var clone = Instantiate(stun) as BuffNDebuffObject;
                other.GetComponentInParent<CharacterBuffDeBuff>().AddBuffOrDebuff(clone);

                GetComponent<BoxCollider>().enabled = false;

                transform.root.SetParent(other.transform);

                if(rougeScripts != null)
                    rougeScripts.RemoveBearTrap(this);

                Destroy(transform.gameObject, stunTime);
            }
        }
    }

    public void EndTrap()
    {
        if (rougeScripts != null)
            rougeScripts.RemoveBearTrap(this);

        Destroy(transform.gameObject);
    }
}
