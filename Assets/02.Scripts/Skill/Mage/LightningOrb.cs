using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningOrb : MonoBehaviour
{
    public GameObject target;
    public GameObject owner;
    public float arrowSpeed;
    public Vector3 DirectionAdjustment;
    public float deleteTime = 5f;
    public int minDamage = 0;
    public int maxDamage = 0;
    public float tickTime = .3f;
    public float serchingDist = 3.5f;
    public int maxTargetCount = 5;
    public NPC_Type npcType;

    public float OrbSize = 2;

    private float orbLocalSize = 0;
    private new Rigidbody rigidbody;
    private Collider collider;

    private List<CharacterStats> DamageTarget = new List<CharacterStats>();

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        target = GameObject.FindWithTag("playertarget");

        var dir = (target.transform.position + DirectionAdjustment) - gameObject.transform.position;

        transform.LookAt(target.transform);
        rigidbody.AddForce(dir.normalized * arrowSpeed);

        StartCoroutine(TickDamage());

        Destroy(gameObject, deleteTime);
    }

    private void Update()
    {
        if (orbLocalSize < OrbSize)
        {
            orbLocalSize += Time.deltaTime * 1.5f;
            transform.localScale = new Vector3(orbLocalSize, orbLocalSize, orbLocalSize);
        }
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<NPC_AI>() != null)
        {
            if (other.GetComponent<NPC_AI>().npcType == NPC_Type.enemy || other.GetComponent<NPC_AI>().npcType == NPC_Type.neutrality)
            {
                DamageTarget.Add(other.transform.GetComponentInParent<CharacterStats>());

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<NPC_AI>() != null)
        {
            if (other.GetComponent<NPC_AI>().npcType == NPC_Type.enemy || other.GetComponent<NPC_AI>().npcType == NPC_Type.neutrality)
            {
                if(DamageTarget.Contains(other.transform.GetComponent<CharacterStats>()))
                    DamageTarget.Remove(other.transform.GetComponent<CharacterStats>());
            }
        }
    }*/

    IEnumerator TickDamage()
    {
        while(true)
        {
            var Colliders = AttackEffectFunctions.FindNearCollider(transform.position, serchingDist, PrefabCollect.instance.npcMask);

            int currentCount = 0;

            foreach (Collider target in Colliders)
            {
                if (currentCount == maxTargetCount)
                    break;

                if (AttackEffectFunctions.CheckWhetherAttackIsPossible(npcType, target))
                {
                    if (!AttackEffectFunctions.CheckRayCast(transform.position, target.bounds.center, PrefabCollect.instance.obstacleMask, drawDebugLine: true))
                    {
                        if (target.GetComponent<NPCStats>() != null)
                        {
                            target.GetComponent<NPCStats>().TakeDamage(minDamage, maxDamage, owner, true, true, false, isDebuffDamage : true);

                            AttackEffectFunctions.GenerateElectircLineRenderer(PrefabCollect.instance.LightingRenderer, transform.position, target.bounds.center, 0.5f);

                            currentCount++;
                        }

                        if (target.GetComponent<PlayerStats>() != null)
                        {
                            target.GetComponent<PlayerStats>().TakeDamage(minDamage, maxDamage, owner, false, true, false, isDebuffDamage: true);

                            AttackEffectFunctions.GenerateElectircLineRenderer(PrefabCollect.instance.LightingRenderer, transform.position, target.bounds.center, 0.5f);

                            currentCount++;
                        }
                    }
                }
            }

            yield return new WaitForSeconds(tickTime);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
