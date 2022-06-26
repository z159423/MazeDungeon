using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSpear : MonoBehaviour
{
    public GameObject target;
    public GameObject owner;
    public float arrowSpeed;
    public float deleteTime = 5f;
    public int damage = 0;
    public BuffNDebuffObject slowdown;
    public Vector3 DirectionAdjustment;

    private new Rigidbody rigidbody;
    private Collider collider;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<BoxCollider>();
        target = GameObject.FindWithTag("playertarget");

        var dir = (target.transform.position + DirectionAdjustment) - gameObject.transform.position;

        transform.LookAt(target.transform);
        rigidbody.AddForce(dir.normalized * arrowSpeed);
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
                StartCoroutine("DeleteObject");

                other.transform.GetComponentInParent<CharacterStats>().TakeDamage(damage,damage, owner, true, true, false);
                other.transform.GetComponentInParent<CharacterBuffDeBuff>().AddBuffOrDebuff(slowdown);
            }
        }
        else if (other.tag == "Ground" || other.tag == "Environment")
        {
            //transform.SetParent(other.transform);
            rigidbody.isKinematic = true;
            collider.enabled = false;
            Destroy(transform.gameObject, deleteTime);
        }
    }

    IEnumerator DeleteObject()
    {
        yield return new WaitForSeconds(deleteTime);
        Destroy(transform.gameObject);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
