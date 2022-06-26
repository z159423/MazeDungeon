using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLogic : MonoBehaviour
{
    private GameObject owner;
    private NPC_AI ai;
    private NPCStats stats;

    private NPC_Type NPC_Type;
    private int minDamage;
    private int maxDamage;
    private bool activated = false;
    private Vector3 contectPoint;
    public Vector3 dirToAim;
    private Vector3 dirToEnemy;
    private Rigidbody rigid;

    public Collider Target;

    public float moveForce = 0;
    public float arrowSpeed = 10f;
    public float turningSpeed = 0.1f;

    [Space]
    public bool canBlock = true;
    public bool possibleRefelct = true;
    public Vector3 reflectRotationOffset;

    [Space]
    public bool nailedAtObstacle = true;
    public bool dontDelete = false;
    public bool destoryAtHit = true;
    public bool destroyAtHitObstacle = true;
    public float destroyAtHitTime = 5;

    [Space]
    public bool nailedAtHitEnemy = false;
    public float nailedAtHitDestroyTime = 5;

    [Space]
    public bool guidedMissile = false;
    public bool endGuide = false;
    public float endGuideTime = 3f;
    public bool findNearTarget = false;
    public float findNearRadius = 4f;
    public float findNearTargetLate = 0.5f;
    
    public GameObject hitParticle = null;

    [Space]
    public bool destroyInTime = true;
    public float DestroyTime = 4f;

    [Space]
    public bool explode = false;
    public float ExplodeDamageMultiply = 2;
    public float explodeRange = 7;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public GameObject ExplodeParticle;
    public bool isExplodeKnockBack = false;
    public List<BuffNDebuffObject> explodeEffects = new List<BuffNDebuffObject>();
    public bool explodeInTime = false;
    public float explodeInTimeValue = 4f;

    private bool isExplode = false;

    public bool zeroDamage = false;
    public bool notBackAttack = false;

    private float savedDamage = 0;

    public BuffNDebuffObject projectileEffect = null;
    public ParticleSystem FlyingParticle;

    [Space]

    public bool ownerAttack = true;
    ParticleSystem[] allParticle;

    [Space]

    public string FireSound = "";
    public string HitSound = "";

    public void Setting(GameObject owner, int minDamage, int maxDamage)
    {
        this.owner = owner;
        if(owner.GetComponentInChildren<PlayerStats>() != null)
        {
            NPC_Type = NPC_Type.friendly;
            savedDamage = Random.Range(minDamage, maxDamage);

            //damage = owner.GetComponentInChildren<PlayerStats>().damage.GetValue();
        }
        else
        {
            ai = owner.GetComponentInChildren<NPC_AI>();
            stats = owner.GetComponentInChildren<NPCStats>();
            NPC_Type = ai.npcType;
        }

        var value = Random.Range(minDamage, maxDamage);

        if (stats)
            savedDamage = value;

        this.minDamage = minDamage;
        this.maxDamage = maxDamage;

        if(projectileEffect)
        {
            var instance = Instantiate(projectileEffect);
            projectileEffect = instance;
            projectileEffect.buffOrDebuff.Owner = owner;
        }
        
    }

    public void Start()
    {
        rigid = GetComponent<Rigidbody>();

        if (endGuide)
            Invoke("EndGuideMode", endGuideTime);

        allParticle = GetComponentsInChildren<ParticleSystem>();

        if (!FireSound.Equals(""))
            AudioManager.instance.GenerateAudioAndPlaySFX(FireSound, GetComponent<Collider>().transform.position);

        if (findNearTarget)
            StartCoroutine(FindNearTarger());

        if (explodeInTime)
            StartCoroutine(ExplodeInTime());
            
    }

    private void FixedUpdate()
    {
        if(guidedMissile)
        {
            if (Target != null)
            {
                if (Target != null)
                {
                    dirToEnemy = (Target.bounds.center - transform.position).normalized;

                    rigid.velocity = transform.up * arrowSpeed * Time.deltaTime;
                    transform.up = Vector3.Lerp(transform.up, dirToEnemy, turningSpeed * Time.deltaTime);
                }
                else
                {
                    transform.up = Vector3.Lerp(transform.up, dirToAim, turningSpeed * Time.deltaTime);
                    //transform.position += dirToAim * 0.5f;
                    rigid.velocity = dirToAim * arrowSpeed * Time.deltaTime;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(activated == true)
        {
            return;
        }
        if (collision.gameObject.CompareTag("Shield") && !owner.CompareTag("Player") && canBlock)
        {
            print("발사체가 방패에 충돌함");

            Rigidbody rigidbody = GetComponent<Rigidbody>();
            contectPoint = collision.ClosestPointOnBounds(transform.position);
            //collision.GetComponentInParent<AudioSource>().PlayOneShot(SoundManager.instance.ShieldBash);

            AudioManager.instance.GenerateAudioAndPlaySFX("block1", transform.position);
            
            Invoke("GenerateParticle", 0.01f);
            collision.GetComponentInParent<PlayerStats>().UseShieldPower(minDamage, maxDamage);

            var value = Random.Range(1, 100);

            if (collision.GetComponentInParent<PlayerSkill>().CheckPlayerHaveSkill(SkillManager.instance.ShieldReflect) 
                && (value < PrefabCollect.instance.ShieldReflect.skillLeveling[collision.GetComponentInParent<PlayerSkill>().GetSkillLevel(SkillManager.instance.ShieldReflect)].value1)
                && possibleRefelct)    //만약 ShieldReflect스킬이 있는 경우 반사
            {
                print("발사체를 반사함");
                //FloatingTextController.CreateFloatingText("Reflection", transform, transform.gameObject.tag, 25, gameObject,true, owner: stats);
                FloatingTextController.GenerateDebuffFloatingText("Reflection", transform, 15, NPC_Type, owner: gameObject);
                EndGuideMode();
                owner = collision.GetComponentInParent<PlayerSkill>().gameObject;
                NPC_Type = NPC_Type.friendly;
                rigidbody.isKinematic = true;
                rigidbody.isKinematic = false;
                //rigidbody.AddForce((owner.transform.forward).normalized * moveForce);
                //transform.rotation = owner.transform.rotation;

                transform.LookAt(owner.GetComponent<PlayerController01>().target);
                transform.rotation = transform.rotation * Quaternion.Euler(reflectRotationOffset);
                rigidbody.AddForce(AttackEffectFunctions.GetDirection(owner.GetComponent<PlayerController01>().target.position, transform.position) * moveForce);

                minDamage = Mathf.RoundToInt(minDamage *PrefabCollect.instance.ShieldReflect.skillLeveling[collision.GetComponentInParent<PlayerSkill>().GetSkillLevel(SkillManager.instance.ShieldReflect)].damageFactor);
                maxDamage = Mathf.RoundToInt(maxDamage *PrefabCollect.instance.ShieldReflect.skillLeveling[collision.GetComponentInParent<PlayerSkill>().GetSkillLevel(SkillManager.instance.ShieldReflect)].damageFactor);

                if (FlyingParticle)
                    FlyingParticle.Stop();
            }
            else
            {
                GetComponent<Collider>().isTrigger = false;
                activated = true;
                guidedMissile = false;
                //FloatingTextController.CreateFloatingText("Block", transform, transform.gameObject.tag, 25, gameObject,true, owner: stats);
                FloatingTextController.GenerateDebuffFloatingText("Block", transform, 15, NPC_Type, owner: gameObject);

                EndGuideMode();
                rigidbody.mass = 0.001f;
                rigidbody.useGravity = true;
                rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                rigidbody.isKinematic = true;
                rigidbody.isKinematic = false;

                if (hitParticle != null)
                {
                    var generatedParticle = Instantiate(hitParticle, GetComponentInChildren<Collider>().bounds.center, Quaternion.identity);
                    Destroy(generatedParticle, 5);
                }

                foreach(ParticleSystem particleSystem in allParticle)
                {
                    particleSystem.Stop();
                }

                if (FlyingParticle)
                    FlyingParticle.Stop();

                if(GetComponent<AudioSource>())
                    GetComponent<AudioSource>().Stop();

                if (!HitSound.Equals(""))
                    AudioManager.instance.GenerateAudioAndPlaySFX(HitSound, collision.transform.position);


                //GetComponent<TrailRenderer>().enabled = false;
            }

            
            //Destroy(gameObject);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("obstacle"))
        {
            if (hitParticle != null)
            {
                var generatedParticle = Instantiate(hitParticle, GetComponentInChildren<Collider>().bounds.center, Quaternion.identity);
                Destroy(generatedParticle, 5);
            }
            //Destroy(gameObject);

            if (nailedAtObstacle)
            {
                GetComponent<Rigidbody>().isKinematic = true;
                gameObject.GetComponent<Collider>().enabled = false;
            }
            else if(dontDelete)
            {
                gameObject.GetComponent<Collider>().isTrigger = false;
                gameObject.GetComponent<Rigidbody>().useGravity = true;
                Destroy(gameObject, 5);
                //deleteProjectile();
            }
            else if(destroyAtHitObstacle)
            {
                //Destroy(gameObject);

                deleteProjectile();
            }

            if(GetComponent<GurosSpear>())
            {
                GetComponent<GurosSpear>().isReadyPull = true;

                if(GetComponentInChildren<TrailRenderer>())
                    GetComponentInChildren<TrailRenderer>().emitting = false;
            }

            if(explode)
            {
                Explode();
            }

            if (FlyingParticle)
                FlyingParticle.Stop();

            if (!HitSound.Equals(""))
                AudioManager.instance.GenerateAudioAndPlaySFX(HitSound, collision.transform.position);
        }

        if (AttackEffectFunctions.CheckWhetherAttackIsPossible(NPC_Type, collision))
        {
            if (projectileEffect)
            {
                var instanceEffect = CreateInstanceBuffOrDebuff.CopyInstnace(projectileEffect);
                collision.gameObject.GetComponentInChildren<CharacterBuffDeBuff>().AddBuffOrDebuff(instanceEffect);
            }

            if (!zeroDamage)
                collision.gameObject.GetComponent<CharacterStats>().TakeDamage(minDamage, maxDamage, owner, true, true, false, ownerAttack: ownerAttack);

            if (hitParticle != null)
            {
                var generatedParticle = Instantiate(hitParticle, GetComponentInChildren<Collider>().bounds.center, Quaternion.identity);
                Destroy(generatedParticle, 5);
            }

            if (explode)
            {
                Explode();
            }
            //Destroy(gameObject);

            if (FlyingParticle)
                FlyingParticle.Stop();

            if (!HitSound.Equals(""))
                AudioManager.instance.GenerateAudioAndPlaySFX(HitSound, collision.transform.position);

            if (nailedAtHitEnemy)
                NailedAtEnemy(collision);
            else if (destoryAtHit)
                deleteProjectile();
        }

        /*if (NPC_Type == NPC_Type.enemy)
        {
            

            if (collision.gameObject.GetComponent<PlayerStats>() != null)
            {

                if (projectileEffect)
                {
                    var instanceEffect = CreateInstanceBuffOrDebuff.CopyInstnace(projectileEffect);
                    collision.gameObject.GetComponentInChildren<CharacterBuffDeBuff>().AddBuffOrDebuff(instanceEffect);
                }

                if (!zeroDamage)
                    collision.gameObject.GetComponent<PlayerStats>().TakeDamage(minDamage, maxDamage, owner, true, true, false);

                if (hitParticle != null)
                {
                    var generatedParticle = Instantiate(hitParticle, GetComponentInChildren<Collider>().bounds.center, Quaternion.identity);
                    Destroy(generatedParticle, 5);
                }

                if (explode)
                {
                    Explode();
                }
                //Destroy(gameObject);

                if (FlyingParticle)
                    FlyingParticle.Stop();

                if (!HitSound.Equals(""))
                    AudioManager.instance.GenerateAudioAndPlaySFX(HitSound, collision.transform.position);
                if (nailedAtHitEnemy)
                    NailedAtEnemy(collision);
                else if (destoryAtHit)
                    deleteProjectile();

            }
            else if (collision.gameObject.GetComponent<NPC_AI>() != null)
            {
                if (collision.gameObject.GetComponent<NPC_AI>().npcType == NPC_Type.friendly ||
                   collision.gameObject.GetComponent<NPC_AI>().npcType == NPC_Type.neutrality ||
                   collision.gameObject.GetComponent<NPC_AI>().npcType == NPC_Type.Minion)
                {
                    if (projectileEffect)
                    {
                        var instanceEffect = CreateInstanceBuffOrDebuff.CopyInstnace(projectileEffect);
                        collision.gameObject.GetComponentInChildren<CharacterBuffDeBuff>().AddBuffOrDebuff(instanceEffect);
                    }

                    if (!zeroDamage)
                        collision.gameObject.GetComponent<NPCStats>().TakeDamage(minDamage, maxDamage, owner, true, true, false);
                    if (hitParticle != null)
                    {
                        var generatedParticle = Instantiate(hitParticle, GetComponentInChildren<Collider>().bounds.center, Quaternion.identity);
                        Destroy(generatedParticle, 5);
                    }

                    if (explode)
                    {
                        Explode();
                    }

                    if (FlyingParticle)
                        FlyingParticle.Stop();

                    //Destroy(gameObject);
                    if (nailedAtHitEnemy)
                        NailedAtEnemy(collision);
                    else if (destoryAtHit)
                        deleteProjectile();

                    if (!HitSound.Equals(""))
                        AudioManager.instance.GenerateAudioAndPlaySFX(HitSound, collision.transform.position);
                }
            }
        }
        else if (NPC_Type == NPC_Type.friendly || NPC_Type == NPC_Type.neutrality || NPC_Type == NPC_Type.Minion)
        {
            if (collision.gameObject.GetComponent<NPC_AI>() != null)
            {
                if (collision.gameObject.GetComponent<NPC_AI>().npcType == NPC_Type.enemy ||
                   collision.gameObject.GetComponent<NPC_AI>().npcType == NPC_Type.neutrality)
                {
                    if (!zeroDamage)
                        collision.gameObject.GetComponent<NPCStats>().TakeDamage(minDamage,maxDamage, owner, true, true, false, notBackAttack : true, ownerAttack : false);
                    if (hitParticle != null)
                    {
                        var generatedParticle = Instantiate(hitParticle, GetComponentInChildren<Collider>().bounds.center, Quaternion.identity);
                        Destroy(generatedParticle, 5);
                    }

                    if (explode)
                    {
                        Explode();
                    }

                    if (projectileEffect)
                    {
                        var instanceEffect = CreateInstanceBuffOrDebuff.CopyInstnace(projectileEffect);
                        collision.gameObject.GetComponentInChildren<CharacterBuffDeBuff>().AddBuffOrDebuff(instanceEffect);
                    }

                    if (FlyingParticle)
                        FlyingParticle.Stop();

                    if (nailedAtHitEnemy)
                        NailedAtEnemy(collision);
                    else if (destoryAtHit)
                        deleteProjectile();

                    if (!HitSound.Equals(""))
                        AudioManager.instance.GenerateAudioAndPlaySFX(HitSound, collision.transform.position);
                }
            }

            
        }*/

        if (collision.GetComponent<DestroyableObject>() != null)
        {
            collision.GetComponent<DestroyableObject>().GetDamage(1);
            collision.GetComponent<DestroyableObject>().GenerateHitParticle(GetComponent<Collider>().bounds.center);
        }
    }

    public void Explode()                //범위공격
    {
        if (isExplode)
            return;

        isExplode = true;

        BuffNDebuffObject[] buffNDebuffObjects = explodeEffects.ToArray();

        AttackEffectFunctions.explode(explodeRange, Mathf.RoundToInt(savedDamage * ExplodeDamageMultiply), transform.position, ExplodeParticle, NPC_Type, owner, debuffs: buffNDebuffObjects);

        /*Collider[] colliders = Physics.OverlapSphere(transform.position, explodeRange, targetMask);

        var effect = Instantiate(ExplodeParticle, transform.position, ExplodeParticle.transform.rotation);

        Destroy(effect, 3.5f);

        foreach (Collider collider in colliders)
        {

            if (NPC_Type == NPC_Type.enemy)
            {
                if (collider.CompareTag("Player"))
                {
                    collider.GetComponent<PlayerStats>().TakeDamage((int)Mathf.Round(savedDamage * ExplodeDamageMultiply)
                        ,(int)Mathf.Round(savedDamage * ExplodeDamageMultiply), gameObject, true, true, false);
                    EZCameraShake.CameraShaker.Instance.ShakeOnce(4, 4, .1f, 1);
                }

                if(collider.CompareTag("Enemy"))
                {
                    if(collider.gameObject.GetComponent<NPC_AI>() != null)
                    {
                        if (collider.gameObject.GetComponent<NPC_AI>().npcType == NPC_Type.Minion || collider.gameObject.GetComponent<NPC_AI>().npcType == NPC_Type.neutrality)
                        {
                            collider.GetComponent<NPCStats>().TakeDamage((int)Mathf.Round(savedDamage * ExplodeDamageMultiply)
                                , (int)Mathf.Round(savedDamage * ExplodeDamageMultiply), gameObject, true, true, false);
                        }
                    }
                }
            }
            else if (NPC_Type == NPC_Type.friendly || NPC_Type == NPC_Type.Minion)
            {
                if (collider.gameObject.GetComponent<NPC_AI>().npcType == NPC_Type.enemy)
                {
                    collider.GetComponent<NPCStats>().TakeDamage((int)Mathf.Round(savedDamage * ExplodeDamageMultiply)
                        , (int)Mathf.Round(savedDamage * ExplodeDamageMultiply), gameObject, true, true, false );

                }
            }

        /*else if (collider.CompareTag("Enemy") && collider.gameObject != gameObject)
        {
            collider.GetComponent<NPCStats>().TakeDamage((int)(stats.damage.GetValue() * ExplodeDamageMultiply), gameObject, true, true, false);
        }*/
    }

    public void Fire(Vector3 target, Vector3 firePos, GameObject owner = null, float extraAngleY = 0, float extraAngleX = 0)
    {
        transform.LookAt(target);
        transform.Rotate(new Vector3(90, extraAngleY, 0));

        GetComponentInChildren<Rigidbody>().AddForce(transform.up * moveForce);

        transform.Rotate(new Vector3(extraAngleX, 0, 0));
        //Setting(owner, Mathf.RoundToInt(owner.GetComponent<NPCStats>().minDamage.GetFinalStatValue()), Mathf.RoundToInt(owner.GetComponent<NPCStats>().maxDamage.GetFinalStatValue()));
        if (destroyInTime)
            Invoke("deleteProjectile", DestroyTime);
    }

    public void GenerateParticle()
    {
        ParticleGenerator.instance.GenerateHitEffect(contectPoint, "Hit2");
    }

    void TakeDamage(NPCStats stats,Vector3 position)
    {
        stats.TakeDamage(minDamage, maxDamage, owner, true, true, false);

        if(hitParticle != null)
        {
            var generatedParticle = Instantiate(hitParticle, position, Quaternion.identity);
            Destroy(generatedParticle, 5);
        }
    }

    void deleteProjectile()
    {
        activated = true;

        EndGuideMode();
        rigid.isKinematic = true;
        //rigid.isKinematic = false;
        //rigid.useGravity = true;
        //GetComponent<Collider>().enabled = false;

        var particles = GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem particle in particles)
        {
            particle.Stop();
        }

        var renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        GetComponent<Collider>().enabled = false;

        if (GetComponent<AudioSource>())
            GetComponent<AudioSource>().enabled = false;
        //GetComponentInChildren<MeshRenderer>().enabled = false;
        Destroy(gameObject, destroyAtHitTime);
    }

    void NailedAtEnemy(Collider other)
    {
        transform.SetParent(other.transform);
        rigid.isKinematic = true;
        GetComponent<Collider>().enabled = false;
    }

    void EndGuideMode()
    {
        guidedMissile = false;
    }

    public enum afterHit
    {
        DestroyInstant, Fall, Nailed
    }

    IEnumerator FindNearTarger()
    {
        while(true)
        {
            if(Target == null)
            {
                var nearTarget = AttackEffectFunctions.FindNearCollider(transform.position, findNearRadius, PrefabCollect.instance.targetMask);

                Target = AttackEffectFunctions.FindAnAttackableTarget(nearTarget, NPC_Type);
            }
            
            yield return new WaitForSeconds(findNearTargetLate);
        }
    }

    IEnumerator ExplodeInTime()
    {
        yield return new WaitForSeconds(explodeInTimeValue);

        Explode();
        deleteProjectile();
    }
}
