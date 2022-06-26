using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingKnife : MonoBehaviour
{
    public GameObject target;
    public GameObject owner;
    public float flyingSpeed = 1000;
    public float deleteTime = 5f;
    public int minDamage = 0;
    public int maxDamage = 0;
    public BuffNDebuffObject debuff;

    private Rigidbody rigidbody;
    private Collider collider;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<BoxCollider>();
        target = GameObject.FindWithTag("playertarget");

        var dir = target.transform.position - gameObject.transform.position;

        transform.LookAt(target.transform);
        rigidbody.AddForce(dir.normalized * flyingSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<NPC_AI>() != null)
        {
            if (other.GetComponentInParent<NPC_AI>().npcType == NPC_Type.enemy || other.GetComponentInParent<NPC_AI>().npcType == NPC_Type.neutrality)
            {
                transform.SetParent(other.transform);
                rigidbody.isKinematic = true;
                collider.enabled = false;
                // GetComponentInChildren<Animator>().SetBool("Spin", false);
                GetComponentInChildren<Animator>().speed = 0;

                Destroy(transform.gameObject, deleteTime);

                other.transform.GetComponentInParent<CharacterStats>().TakeDamage(minDamage, maxDamage, owner, true, true, false);

                if(other.GetComponentInChildren<CharacterBuffDeBuff>() != null && debuff != null)
                    other.GetComponentInChildren<CharacterBuffDeBuff>().AddBuffOrDebuff(debuff);

                GetComponentInChildren<MeleeWeaponTrail>().Emit = false;
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("obstacle"))
        {
            //transform.SetParent(other.transform);
            rigidbody.useGravity = true;
            collider.isTrigger = false;
            //GetComponentInChildren<Animator>().SetBool("Spin", false);
            GetComponentInChildren<Animator>().speed = 0;

            Destroy(transform.gameObject, deleteTime);
        }
    }
}
