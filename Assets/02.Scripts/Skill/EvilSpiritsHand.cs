using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilSpiritsHand : MonoBehaviour
{
    List<NPC_AI> OverlapCheck = new List<NPC_AI>();

    public int SkillLevel = 1;
    public Skill skill = null;
    public GameObject owner = null;

    private float y = 0;
    private float size = 0.1f;

    private void Start()
    {
        GameObject target = GameObject.FindWithTag("playertarget");
        Vector3 dirToAim = (target.transform.position - transform.position).normalized;
        Rigidbody rigid = GetComponentInParent<Rigidbody>();

        rigid.AddForce(dirToAim * 750);

        transform.LookAt(target.transform);
    }

    private void Update()
    {
        /*y += Time.deltaTime * 150;
        transform.root.rotation = Quaternion.Euler(0, y, 0);*/

        if(size < 1)
        {
            size += Time.deltaTime * 0.7f;
            transform.root.localScale = new Vector3(size, size, size);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInChildren<NPC_AI>() != null)
        {
            if(other.GetComponentInChildren<NPC_AI>().npcType == NPC_Type.enemy && !OverlapCheck.Contains(other.GetComponentInChildren<NPC_AI>()))
            {
                OverlapCheck.Add(other.GetComponentInChildren<NPC_AI>());

                bufforDebuff restraint = new bufforDebuff();

                restraint.Duration = 4;
                restraint.debuff = buffType.Restraint;
                restraint.fromBy = owner;

                other.GetComponentInChildren<NPCStats>().GetBuffAndDebuff(restraint);

                other.GetComponentInChildren<NPCStats>().TakeDamage(0,0, owner, false, false, false);
            }
        }
        
    }
}
