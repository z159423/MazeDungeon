using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffectFunctions : MonoBehaviour
{
    public static void explode(float ExplosionRadius, int damage, Vector3 Position, GameObject ExplodeParticle, NPC_Type ownerType, GameObject owner, float cameraShakeRadius = 25, BuffNDebuffObject[] debuffs = null, bool explosionParticle = true)
    {
        Collider[] colliders = Physics.OverlapSphere(Position, ExplosionRadius, PrefabCollect.instance.npcMask);

        if(explosionParticle)
        {
            var effect = Instantiate(ExplodeParticle, Position, ExplodeParticle.transform.rotation);
            Destroy(effect, 3.5f);
        }
        

        foreach (Collider collider in colliders)
        {
            if (ownerType == NPC_Type.enemy)
            {
                if (collider.CompareTag("Player"))
                {
                    collider.GetComponent<PlayerStats>().TakeDamage((int)Mathf.Round(damage)
                        , (int)Mathf.Round(damage), owner, true, true, false);

                    var dir = AttackEffectFunctions.GetDirection(collider.bounds.center, Position);

                    collider.GetComponentInChildren<Rigidbody>().AddForce(dir * 700,ForceMode.Acceleration);

                    if (debuffs != null)
                    {
                        foreach (BuffNDebuffObject buffNDebuffObject in debuffs)
                        {
                            collider.GetComponent<CharacterBuffDeBuff>().AddBuffOrDebuff(buffNDebuffObject);
                        }
                    }
                    
                }

                if(collider.gameObject.GetComponent<NPC_AI>())
                {
                    if (collider.gameObject.GetComponent<NPC_AI>())
                    {
                        if (collider.gameObject.GetComponent<NPC_AI>().npcType == NPC_Type.friendly || ownerType == NPC_Type.neutrality)
                        {
                            collider.gameObject.GetComponent<NPCStats>().TakeDamage((int)Mathf.Round(damage)
                            , (int)Mathf.Round(damage), owner, true, true, false);

                            if (debuffs != null)
                            {
                                foreach (BuffNDebuffObject buffNDebuffObject in debuffs)
                                {
                                    collider.GetComponent<CharacterBuffDeBuff>().AddBuffOrDebuff(buffNDebuffObject);
                                }
                            }
                        }

                        if (collider.gameObject.GetComponent<NPC_AI>().npcType == NPC_Type.Minion)
                        {
                            collider.gameObject.GetComponent<NPCStats>().TakeDamage((int)Mathf.Round(damage)
                            , (int)Mathf.Round(damage), owner, true, true, false);

                            if (debuffs != null)
                            {
                                foreach (BuffNDebuffObject buffNDebuffObject in debuffs)
                                {
                                    collider.GetComponent<CharacterBuffDeBuff>().AddBuffOrDebuff(buffNDebuffObject);
                                }
                            }
                        }
                    }
                }
            }
            else if (ownerType == NPC_Type.friendly || ownerType == NPC_Type.Minion)
            {
                if(collider.gameObject.GetComponent<NPC_AI>())
                {
                    if (collider.gameObject.GetComponent<NPC_AI>().npcType == NPC_Type.enemy || ownerType == NPC_Type.neutrality)
                    {
                        collider.GetComponent<NPCStats>().TakeDamage((int)Mathf.Round(damage)
                            , (int)Mathf.Round(damage), owner, true, true, false);

                        if (debuffs != null)
                        {
                            foreach (BuffNDebuffObject buffNDebuffObject in debuffs)
                            {
                                collider.GetComponent<CharacterBuffDeBuff>().AddBuffOrDebuff(buffNDebuffObject);
                            }
                        }
                    }
                }
                
            }

        }

        Collider[] playerFindCollider = Physics.OverlapSphere(Position, cameraShakeRadius, PrefabCollect.instance.playerMask);

        foreach (Collider collider in playerFindCollider)
        {
            if(collider.GetComponent<PlayerStats>())
                EZCameraShake.CameraShaker.Instance.ShakeOnce(4, 4, .1f, 1);
        }


    }

    public static void GenerateElectircLineRenderer(GameObject lightingRenderer, Vector3 start, Vector3 end, float endTime)
    {
        var effect = Instantiate(lightingRenderer, start, lightingRenderer.transform.rotation);
        effect.GetComponentInChildren<LineRenderer>().SetPosition(0, start);
        effect.GetComponentInChildren<LineRenderer>().SetPosition(1, end);

        Destroy(effect, endTime);
    }

    public static void ElectricStatic(float ExplosionRadius, int minDamage, int maxDamage, Vector3 Position, GameObject lightingRenderer, NPC_Type ownerType, GameObject owner, LayerMask mask, GameObject originTarget)
    {
        Collider[] colliders = Physics.OverlapSphere(Position, ExplosionRadius , mask);

        int staticCount = 0;

        var effect = Instantiate(lightingRenderer, Position, lightingRenderer.transform.rotation);
        var lineRenderer = effect.GetComponentInChildren<LineRenderer>();
        lineRenderer.SetPosition(0, Position);

        AudioManager.instance.GenerateAudioAndPlaySFX("elec2", Position);

        foreach (Collider collider in colliders)
        {
            if (staticCount > 10)
                break;


            RaycastHit hit;
            if (!Physics.Raycast(Position, GetDirection(collider.bounds.center,Position), out hit, Vector3.Distance(Position, collider.bounds.center), PrefabCollect.instance.obstacleMask))
            {
                if (ownerType == NPC_Type.enemy)
                {
                    if (collider.CompareTag("Player") || collider.gameObject.GetComponent<NPC_AI>().npcType == NPC_Type.friendly)
                    {
                        //if (collider.gameObject != originTarget)
                        //{
                        staticCount++;

                        lineRenderer.positionCount = staticCount + 1;
                        lineRenderer.SetPosition(staticCount, collider.bounds.center);

                        //Destroy(effect, 1);

                        int damage = Random.Range(minDamage, maxDamage);

                        collider.GetComponent<PlayerStats>().TakeDamage((int)Mathf.Round(damage)
                            , (int)Mathf.Round(damage), owner, false, true, false, ownerAttack: false);
                        //}
                    }
                }
                else if (ownerType == NPC_Type.friendly || ownerType == NPC_Type.Minion)
                {
                    if (collider.gameObject.GetComponent<NPC_AI>())
                    {
                        if (collider.gameObject.GetComponent<NPC_AI>().npcType == NPC_Type.enemy)
                        {
                            //if (collider.gameObject.GetComponent<NPC_AI>() != originTarget.GetComponent<NPC_AI>())
                            //{
                            /*var effect = Instantiate(lightingRenderer, Position, lightingRenderer.transform.rotation);
                            effect.GetComponentInChildren<LineRenderer>().SetPosition(0, Position);
                            effect.GetComponentInChildren<LineRenderer>().SetPosition(1, collider.bounds.center);*/
                            staticCount++;

                            lineRenderer.positionCount = staticCount + 1;
                            lineRenderer.SetPosition(staticCount, collider.bounds.center);

                            //Destroy(effect, 1);

                            int damage = Random.Range(minDamage, maxDamage);

                            collider.GetComponent<NPCStats>().TakeDamage((int)Mathf.Round(damage)
                                , (int)Mathf.Round(damage), owner, false, true, false, ownerAttack: false);
                            //}
                        }
                    }

                }
            }

            Destroy(effect, 1);
            
        }
    }

    public static bool TriggerEnterAttack(int minDamage, int maxDamage, Collider other, GameObject owner, OwnerType ownerType, List<BuffNDebuffObject> debuffs = null)
    {
        if(ownerType == OwnerType.Player)
        {
            if (other.GetComponent<NPC_AI>() != null)
            {
                if (other.GetComponentInParent<NPC_AI>().npcType == NPC_Type.enemy || other.GetComponentInParent<NPC_AI>().npcType == NPC_Type.neutrality)
                {
                    if ((other.gameObject.tag == "Enemy" || other.gameObject.tag == "enemyhitbox"))
                    {
                        NPCStats enemystats = other.GetComponentInParent<NPCStats>();

                        enemystats.TakeDamage(minDamage, maxDamage, owner.GetComponentInParent<PlayerStats>().gameObject, true, true, false, isDebuffDamage: true);

                        for(int i = 0; i < debuffs.Count; i++)
                        {
                            other.GetComponent<CharacterBuffDeBuff>().AddBuffOrDebuff(debuffs[i]);
                        }

                        return true;

                    }
                }
            }
        }
        else if(ownerType == OwnerType.Enemy)
        {
            if (other.GetComponent<PlayerStats>() != null)
            {
                PlayerStats playerStat = other.GetComponent<PlayerStats>();

                playerStat.TakeDamage(minDamage, maxDamage, owner, true, true, false);

                for (int i = 0; i < debuffs.Count; i++)
                {
                    other.GetComponent<CharacterBuffDeBuff>().AddBuffOrDebuff(debuffs[i]);
                }

                return true;
            }
        }

        return false;
        
    }

    public static Vector3 GetDirection(Vector3 target, Vector3 owner)
    {
        return (target - owner).normalized;
    }

    public static Vector3 DrawRayCast(Vector3 Start, Vector3 direction)
    {
        RaycastHit raycastHit;
        Physics.Raycast(Start, direction, out raycastHit, 10f, PrefabCollect.instance.obstacleMask);

        return raycastHit.point;
    }

    public static bool CheckRayCast(Vector3 start, Vector3 end, LayerMask layerMask, bool drawDebugLine = false)
    {
        RaycastHit hit;

        var dist = Vector3.Distance(start, end);

        if (drawDebugLine)
            Debug.DrawRay(start, (end - start).normalized * dist, Color.blue, 3f);

        return Physics.Raycast(start, (end - start).normalized, out hit, dist, layerMask);
    }

    public static Collider[] FindNearCollider(Vector3 position, float radius, LayerMask layerMask)
    {
        return Physics.OverlapSphere(position, radius, layerMask);
    }

    public static Collider FindAnAttackableTarget(Collider[] colliders, NPC_Type npcType)
    {
        foreach(Collider collider in colliders)
        {
            if (npcType == NPC_Type.enemy)
            {
                if (collider.GetComponent<NPC_AI>() != null)
                {
                    if(collider.GetComponent<NPC_AI>().npcType == NPC_Type.friendly || collider.GetComponent<NPC_AI>().npcType == NPC_Type.Minion)
                    {
                        return collider;
                    }
                    
                }
                else if(collider.GetComponent<PlayerStats>() != null)
                {
                    return collider;
                }
            }
            else if (npcType == NPC_Type.friendly || npcType == NPC_Type.Minion)
            {
                if (collider.GetComponent<NPC_AI>() != null)
                {
                    if (collider.GetComponent<NPC_AI>().npcType == NPC_Type.enemy)
                    {
                        return collider;
                    }

                }
            }
            else if (npcType == NPC_Type.neutrality)
            {
                if (collider.GetComponent<NPC_AI>() != null)
                {
                    return collider;

                }
                else if (collider.GetComponent<PlayerStats>() != null)
                {
                    return collider;
                }
            }

            return null;
        }

        return null;
    }

    public static bool CheckWhetherAttackIsPossible(NPC_Type ownerType, Collider targetCollider)
    {
        if(ownerType == NPC_Type.enemy)
        {
            if(targetCollider.GetComponent<NPC_AI>() != null)
            {
                var npctype = targetCollider.GetComponent<NPC_AI>().npcType;
                if (npctype == NPC_Type.enemy)
                {
                    return false;
                }
                else if (npctype == NPC_Type.friendly || npctype == NPC_Type.Minion)
                {
                    return true;
                }
                else if (npctype == NPC_Type.neutrality)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if(targetCollider.GetComponent<PlayerStats>() != null)
            {
                return true;
            }

            
        }
        else if(ownerType == NPC_Type.friendly || ownerType == NPC_Type.Minion)
        {
            if (targetCollider.GetComponent<NPC_AI>() != null)
            {
                var npctype = targetCollider.GetComponent<NPC_AI>().npcType;

                if (npctype == NPC_Type.enemy)
                {
                    return true;
                }
                else if (npctype == NPC_Type.friendly || npctype == NPC_Type.Minion)
                {
                    return false;
                }
                else if (npctype == NPC_Type.neutrality)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (targetCollider.GetComponent<PlayerStats>() != null)
            {
                return false;
            }
        }
        else if (ownerType == NPC_Type.neutrality)
        {
            if (targetCollider.GetComponent<NPC_AI>() != null)
            {
                var npctype = targetCollider.GetComponent<NPC_AI>().npcType;
                if (npctype == NPC_Type.enemy)
                {
                    return true;
                }
                else if (npctype == NPC_Type.friendly || npctype == NPC_Type.Minion)
                {
                    return true;
                }
                else if (npctype == NPC_Type.neutrality)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (targetCollider.GetComponent<PlayerStats>() != null)
            {
                return true;
            }
        }
        else
        {
            return false;
        }

        return false;
    }

    public static bool GetRandomResultAsInt(int num)
    {
        if (Random.Range(0, 100) < num)
            return true;
        else
            return false;
    }

    public class CameraShakerValue
    {
        public float magnitude = 4;
        public float roughness = 4;
        public float fadeInTime = 1f;
        public float fadeOutTime = 1;
    }

}

public enum OwnerType { Player, Enemy}
