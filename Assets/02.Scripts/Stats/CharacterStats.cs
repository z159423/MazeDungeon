using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterStats : MonoBehaviour {

    public Stat maxHealth = new Stat();
    public int currentHealth { get; set; }

    public ShieldStat shieldStat = new ShieldStat();

    public Stat minDamage = new Stat();
    public Stat maxDamage = new Stat();
    public Stat armor = new Stat();
    public Stat reduceDamage = new Stat();
    public Stat dodge = new Stat();

    public BoolStat isStun;
    public BoolStat isRestraint;
    public BoolStat isMaimed;
    public BoolStat StunImmunity;
    public BoolStat SlowDownImmunity;

    public float knockBackPower = 1f;

    public bool isKnockBack = false;
    public bool isDead = false;
    public bool isInvincibility = false;
    public bool dodgeInvincible = false;

    public event System.Action<int, int> OnHealthChanged;
    public event System.Action<int, int> OnShieldChanged;


    List<bufforDebuff> buffAndDebuffsList02 = new List<bufforDebuff>();

    [Space]

    public Collider[] bodyParts;
    public SkinnedMeshRenderer[] skinnedParts;
    public GameObject Hips;
    public float bodyPartDeleteTime = 10f;
    public float particleSize = 1;

    private Vector3 movement;
    private Rigidbody rb;
    private AudioSource Audio;
    private NavMeshAgent Agent;
    private NPC_AI npc_AI;
    private HealthUI healthUi;
    private bool isKnocking = false;
    public SkinnedMeshRenderer skinned;
    private float AttackAnimTime = 0;
    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    [SerializeField]
    private PlayerController01 playercontroller;
    private float knockbackforce = 0;
    private PlayerArtifact playerArtifact;
    private NPC_Type npcType;

    [Space]

    [SerializeField] private string[] HitSounds;
    [SerializeField] private string[] AttackSounds;
    [SerializeField] private string[] DeadSounds;

    protected void Start()
    {
        FloatingTextController.Initialize();

        if (gameObject.GetComponent<HealthUI>() != null)
            healthUi = GetComponent<HealthUI>();

        currentHealth = (int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat());
        
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
        {
            for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
            {
                skinnedMeshRenderer.materials[i] = Instantiate<Material>(skinnedMeshRenderer.materials[i]);
            }
        }

        //ren = GetComponentsInChildren<Renderer>();
        rb = GetComponent<Rigidbody>();
        Audio = GetComponent<AudioSource>();
        npc_AI = GetComponent<NPC_AI>();

        if(npc_AI != null)
        {
            npcType = npc_AI.npcType;
        }
        else if(GetComponent<PlayerStats>() != null)
        {
            npcType = NPC_Type.friendly;
        }
        playerArtifact = GetComponent<PlayerArtifact>();

        if (!skinned)
        skinned = GetComponentInChildren<SkinnedMeshRenderer>();

        if (GetComponent<NavMeshAgent>() != null)
        {
            Agent = GetComponent<NavMeshAgent>();
        }

        if (GetComponent<PlayerController01>() != null)
        {
            playercontroller = GetComponent<PlayerController01>();
        }

        shieldStat.ownerTransform = transform;
        reduceDamage.SetBaseValue(1);
        //shieldStat.AddShield("test", 10, 100);
        //Debug.LogError(currentHealth);
    }

    private void Update()
    {
        if(Agent != null && Agent.enabled == false && isKnockBack == true)
        {
            CheckGround();
        }

        BuffDebuffTime();
    }

    private void FixedUpdate()
    {
        if(isKnockBack)
        {
            rb.velocity = movement * (20 * knockbackforce / rb.mass  );
        }
    }

    public virtual void TakeDamage(int minDamage, int maxDamage, GameObject other, bool playSound, bool playBlink
        , bool ignoreArmor, bool NotCritical = false, bool isDebuffDamage = false, bool notBackAttack = false
        , bool ownerAttack = true, BuffNDebuffObject[] Debuff = null)
    {
        if (isInvincibility || isDead)
        {
            Debug.Log("무적상태라 데미지가 들어오지 않습니다.");
            return;
        }

        if(dodgeInvincible && !isDebuffDamage)
        {
            //FloatingTextController.CreateFloatingText("Roll", transform, transform.gameObject.tag, 20, gameObject, true, owner : this);
            FloatingTextController.GenerateBuffFloatingText("Roll", transform, 15, npcType, owner: gameObject);
            return;
        }

        float damage = 0;
        float damageMultiyful = 1;
        int damageAdd = 0;

        bool isCritical = false;

        //print("mindamage : " + minDamage + " MaxDamage : " + maxDamage + " damage : " + damage);

        if (other && !notBackAttack)
        {
            if (other.GetComponent<PlayerSkill>())
            {
                if (other.GetComponent<PlayerSkill>().CheckPlayerHaveSkill(PrefabCollect.instance.BackAttack))
                {
                    var thisAngle = transform.rotation.eulerAngles.y;
                    var otherAngle = other.transform.rotation.eulerAngles.y;

                    float diff = thisAngle - otherAngle;

                    if (Mathf.Abs(diff) > 180)
                    {
                        diff = Mathf.Abs(diff) - 360;
                    }

                    print(gameObject + " 의 각도 : " + thisAngle + " " + other + " 의 각도 : " + otherAngle
                        + " 각도 차이 : " + Mathf.Abs(diff));

                    if (Mathf.Abs(diff) < 60 && !isDebuffDamage)
                    {
                        //float backAttackDamageMultifly = 1;
                        //backAttackDamageMultifly += SkillManager.instance.Sneak.skillLeveling[other.GetComponent<PlayerSkill>().GetSkillLevel(SkillManager.instance.Sneak)].damageFactor;

                        damageMultiyful += PrefabCollect.instance.BackAttack.skillLeveling[other.GetComponent<PlayerSkill>().GetSkillLevel(PrefabCollect.instance.BackAttack)].damageFactor - 1;

                        //FloatingTextController.CreateFloatingText("BackAttack" /*+ (int)Mathf.Abs(transform.localRotation.eulerAngles.y - other.transform.localRotation.eulerAngles.y)*/, transform, transform.gameObject.tag, 15, other,true, owner: this);
                        FloatingTextController.GenerateDebuffFloatingText("BackAttack", transform, 15, npcType, owner: gameObject);
                        //damage = Random.Range((int)Mathf.Round(minDamage * backAttackDamageMultifly), (int)Mathf.Round(maxDamage * backAttackDamageMultifly));
                    }
                }
            }
        }
        if (other)
        {
            if (other.GetComponent<PlayerController01>())
            {
                if (other.GetComponent<PlayerController01>().SneakGague > 70)
                {
                    //damage = damage * 2;
                    damageMultiyful += SkillManager.instance.Sneak.skillLeveling[other.GetComponent<PlayerSkill>().GetSkillLevel(SkillManager.instance.Sneak)].damageFactor - 1;
                    other.GetComponent<PlayerController01>().UseSneakGague();
                }
            }
        }

        if (other)
        {
            if (other.GetComponent<PlayerStats>())
            {
                var percent = other.GetComponent<PlayerStats>().CritialChange.GetFinalStatValueAsAddFloat();

                percent *= 100;

                int randomValue = Random.Range(0, 100);

                if (other.GetComponentInChildren<PlayerArtifact>() && !isDebuffDamage)
                {
                    if (other.GetComponentInChildren<PlayerArtifact>().GetBrokenDirkHitCount() == 6)
                        damageMultiyful += 1;
                }

                if (randomValue <= (int)percent && (int)percent != 0 && !NotCritical && !isDebuffDamage)
                {
                    //FloatingTextController.CreateFloatingText("Critical", transform, transform.gameObject.tag, 10, other,true);
                    if (other.GetComponentInChildren<PlayerArtifact>())
                    {
                        if (other.GetComponentInChildren<PlayerArtifact>().ArtifactContain(ArtifactType.FlameNeedle))
                        {
                            foreach (BuffNDebuffObject buffNDebuffObject in other.GetComponentInChildren<PlayerArtifact>().GetArtifact(ArtifactType.FlameNeedle).Debuff)
                            {
                                var instance = CreateInstanceBuffOrDebuff.CopyInstnace(buffNDebuffObject);
                                instance.buffOrDebuff.Owner = other;
                                GetComponentInChildren<CharacterBuffDeBuff>().AddBuffOrDebuff(instance);
                            }
                        }
                    }

                    damageMultiyful += 0.5f;
                    isCritical = true;
                }

                if (other.GetComponentInChildren<PlayerArtifact>())
                {
                    if (other.GetComponentInChildren<PlayerArtifact>().ArtifactContain(ArtifactType.BerserkerEyePatch))
                    {
                        if(other.GetComponentInChildren<PlayerStats>().currentHealth < (other.GetComponentInChildren<PlayerStats>().maxHealth.GetFinalStatValueAsMultiflyFloat() / 2))
                        {
                            damageMultiyful += 0.5f;
                        }
                    }
                }
            }
        }

        damage = Random.Range(minDamage * damageMultiyful, maxDamage * damageMultiyful);

        if (ignoreArmor == false)
        {
            //damage -= armor.GetValue();

            int FinalDefence = 0;

            if (playerArtifact)
            {
                if (playerArtifact.ArtifactContain(ArtifactType.AncientSnailShell) && currentHealth < (maxHealth.GetFinalStatValueAsMultiflyFloat() * .30f))
                {
                    FinalDefence += 15;
                }
            }

            FinalDefence += Mathf.RoundToInt(armor.GetFinalStatValue());

            if (FinalDefence < 0)
            {
                //Debug.LogError("FinalDefence : " + FinalDefence);
                //Debug.LogError("damage : " + damage);
                float damageMultifly = 30f / (30f - (-FinalDefence));
                //Debug.LogError("damageMultifly : " + damageMultifly);
                damage = damage * Mathf.Clamp(damageMultifly,1,2.5f);
                //Debug.LogError("finaldamage : " + damage);
            }
            else
            {
                damage = (damage * 30f) / (30f + FinalDefence);

                //Debug.LogError("damage : " + damage + " FinalDefence : " + FinalDefence);
            }

        }

        damage = Mathf.RoundToInt((damage + damageAdd));

        damage = Mathf.RoundToInt(damage / reduceDamage.GetFinalStatValue());
        
        damage = Mathf.Clamp(damage, 1, int.MaxValue);

        damage = shieldStat.DamageToShield((int)damage, other);                      //쉴드가 있는지 확인 있으면 쉴드부터 데미지 들어감

        if (OnShieldChanged != null)
        {
            OnShieldChanged((int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat()), shieldStat.GetTotalShieldValue());
        }

        int randomDodgeValue = Random.Range(0,100);

        if(randomDodgeValue < dodge.GetFinalStatValueAsMultiflyFloat() && !isDebuffDamage)
        {
            //FloatingTextController.CreateFloatingText("Dodge", transform, transform.gameObject.tag, 20, gameObject, true, owner: this);
            FloatingTextController.GenerateBuffFloatingText("Dodge", transform, 15, npcType, owner: gameObject);
        }
        else if (damage > 0)
        {
            currentHealth -= (int)damage;
            Debug.Log(transform.name + " takes " + damage + " damage.");

            if (OnHealthChanged != null)
            {
                OnHealthChanged((int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat()), currentHealth);
            }

            if (playSound == true)
            {
                StartCoroutine(HitSoundPlay());
            }

            if (isCritical)
            {
                //FloatingTextController.CreateFloatingText(damage.ToString(), transform, transform.gameObject.tag, 35, other, owner: this);
                FloatingTextController.GenerateDamageFloatingText(damage.ToString(), transform, 35, npcType, owner: gameObject, isCritical);
            }
            else
            {
                //FloatingTextController.CreateFloatingText(damage.ToString(), transform, transform.gameObject.tag, 25, other, owner: this);
                FloatingTextController.GenerateDamageFloatingText(damage.ToString(), transform, 28, npcType, owner: gameObject);
            }

            if(GetComponent<PlayerArtifact>() && !isDebuffDamage && ownerAttack)
            {
                GetComponent<PlayerArtifact>().ArtifactAttackedEffect(other);
            }

            if (other) {
                if (other.GetComponent<PlayerArtifact>() && !isDebuffDamage && ownerAttack)
                {
                    other.GetComponent<PlayerArtifact>().ArtifactAttackEffect(gameObject);
                }
            }

            if(other)
            {
                if (other.GetComponent<PlayerEnchant>() && !isDebuffDamage)
                {
                    other.GetComponent<PlayerEnchant>().CheckEnchantAttackEffect(GetComponent<CharacterBuffDeBuff>());
                }
            }
            
            if(GetComponent<ShopKeeper>())
            {
                if(other != null)
                {
                    if (other.CompareTag("Player"))
                    {
                        if (GetComponent<ShopKeeper>().betrayalText == false)
                        {
                            GetComponent<ShopKeeper>().SpawnAngryTextBubble();
                        }
                    }
                }
            }

            if(Debuff != null)
            {
                if(GetComponent<CharacterBuffDeBuff>() != null)
                {
                    foreach (BuffNDebuffObject debuff in Debuff)
                    {
                        GetComponent<CharacterBuffDeBuff>().AddBuffOrDebuff(debuff);
                    }
                }
            }
        }

        if (currentHealth <= 0)
        {
            if (OnHealthChanged != null)
            {
                OnHealthChanged( (int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat()), currentHealth);
            }

            if (GetComponent<FallenKing>())
            {
                if (GetComponent<FallenKing>().phase == 1)
                {
                    isInvincibility = true;
                    currentHealth = 1;
                    GetComponent<FallenKing>().phaseOneEnd();

                    return;
                }
                else
                {
                    Die(other);
                    isStun.ClearBoolStat();
                }

            }
            else
            {
                Die(other);
                isStun.ClearBoolStat();
            }

            
        } else if (playBlink)
        {
            Blink();
        }

        //ParticleGenerator.instance.GenerateHitParticle(transform.position, "Hit", skinned.materials, other.transform.position, skinned, particleSize);
        ParticleGenerator.instance.GenerateDeathParticle(transform.position, "Hit", skinned.materials, skinned, particleSize);
        if (other)
        {
            if (other.GetComponent<PlayerController01>() != null)
            {
                if (other.GetComponent<PlayerController01>().magicOrbs.Count > 0 && currentHealth > 0)
                {
                    Vector3 colliderCenter = GetComponent<Collider>().bounds.center;
                    other.GetComponent<PlayerController01>().magicOrbs.Pop().FireMagicOrb(colliderCenter);
                }
            }
        }

        if (npc_AI != null && other)
            npc_AI.GetDamage(other, damage);
    }

    public virtual void TakeDamage(int damage, GameObject other, bool playSound, bool playBlink, bool ignoreArmor, bool isMagicOrb)
    {
        if (ignoreArmor == false)
        {
            if (armor.GetFinalStatValue() < 0)
            {
                damage = Mathf.RoundToInt(damage * ((2 - 20) / (20 - armor.GetFinalStatValue())));
            }
            else
            {
                damage = Mathf.RoundToInt((damage * 20) / (20 + armor.GetFinalStatValue()));
            }

        }

        damage = Mathf.Clamp(damage, 1, int.MaxValue);

        currentHealth -= damage;
        Debug.Log(transform.name + " takes " + damage + " damage.");

        if (OnHealthChanged != null)
        {
            OnHealthChanged((int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat()), currentHealth);
        }

        if (playSound == true)
        {
            StartCoroutine(HitSoundPlay());
        }

        //FloatingTextController.CreateFloatingText(damage.ToString(), transform, transform.gameObject.tag, 25, other, owner: this);
        FloatingTextController.GenerateDamageFloatingText(damage.ToString(), transform, 28, npcType, owner: gameObject);

        if (currentHealth <= 0)
        {
            Debug.LogError(currentHealth);
            Die(other);
            isStun.ClearBoolStat();
        }
        else if (playBlink)
        {
            Blink();
        }

        ParticleGenerator.instance.GenerateHitParticle(transform.position, "Hit", skinned.materials, other.transform.position, skinned, particleSize);

        if (npc_AI != null)
            npc_AI.GetDamage(other, damage);

    }


    public void Heal(int HealAmount)
    {
        currentHealth += HealAmount;
        Mathf.Clamp(currentHealth, 0, (int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat()));

        //FloatingTextController.CreateFloatingText("+" +HealAmount.ToString(), transform, transform.gameObject.tag, 25, null, owner: this);
        FloatingTextController.GenerateHealFloatingText("+" + HealAmount.ToString(), transform, 25, npcType, owner: gameObject);
    }
    public void Blink()
    {
        StartCoroutine(BlinkObject());
    }

    public void knockback(GameObject gameObject, Vector3 dir, float KnockBackForce)
    {
        if(!isDead)
            StartCoroutine(KnockBack(gameObject.transform, dir, KnockBackForce));
    }

    public void knockback(GameObject gameObject, float force, float time)
    {
        if (gameObject.transform.root.gameObject.activeSelf == true)
            StartCoroutine(KnockBack2(gameObject.transform, time, force));
    }

    IEnumerator KnockBack(Transform transform, Vector3 dir, float force)
    {
        if (force > 0)
        {
            //isKnockBack = true;
            //movement = (this.transform.position - transform.position).normalized;
            movement = dir;
            knockbackforce = force;
            //NavMeshAgent Agent = GetComponent<NavMeshAgent>();
            if (Agent != null)
                rb.AddForce(movement * force, ForceMode.Impulse);

            yield return new WaitForSeconds(.1f);
            isKnockBack = false;
        }

    }

    IEnumerator KnockBack2(Transform transform, float time, float force)
    {
        if (time > 0)
        {
            isKnockBack = true;
            movement = (this.transform.position - transform.position).normalized;
            knockbackforce = force;
            //NavMeshAgent Agent = GetComponent<NavMeshAgent>();
            /*if (Agent != null)
                rb.AddForce(-movement * KnockBackForce, ForceMode.Impulse);*/

            yield return new WaitForSeconds(time);
            isKnockBack = false;
        }
    }

    protected virtual void Die(GameObject other)
    {
        Debug.Log(transform.name + " 죽음");
        //gameObject.SetActive(false);
        //Destroy(gameObject.GetComponent<Collider>());

        if(skinned)
            ParticleGenerator.instance.GenerateDeathParticle(transform.position, "Death", skinned.materials, skinned, particleSize); 

        isDead = true;

        if (bodyParts.Length > 0 && skinnedParts.Length > 0 && Hips)
        {
            Hips.transform.SetParent(null);
            Destroy(Hips, bodyPartDeleteTime);

            foreach (Collider collider in bodyParts)
            {
                collider.enabled = true;
                collider.GetComponent<Rigidbody>().useGravity = true;
            }

            foreach(SkinnedMeshRenderer skinned in skinnedParts)
            {
                skinned.transform.SetParent(null);
                Destroy(skinned.gameObject, bodyPartDeleteTime);
            }

            SkinnedMeshRenderer[] skinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (SkinnedMeshRenderer meshes in skinnedMeshes)
            {
                meshes.updateWhenOffscreen = true;
            }
        }

        if (other)
        {
            if (other.GetComponent<PlayerEnchant>())
            {
                other.GetComponent<PlayerEnchant>().CheckNPCKillEnchantEffect(gameObject);
            }
        }

    }

    void CheckGround()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position + new Vector3(0, 0.7f), Vector3.down * 0.9f, Color.red);

        if (Physics.Raycast(transform.position + new Vector3(0, 0.7f), Vector3.down, out hit, 1.3f))
        {
            if (hit.transform.GetComponent<MeshFilter>() != null)
            {
                //isGround = true;
                isKnockBack = false;

                NavMeshAgent Agent = GetComponent<NavMeshAgent>();
                Agent.enabled = true;
                
                return;
            }
        }
        //isGround = false;
    }

    float AnimationLength(string name)          //"Punch" 애니메이션클립의 길이를 알아내는 함수
    {
        RuntimeAnimatorController ac = playercontroller.animator.runtimeAnimatorController;

        for (int i = 0; i < ac.animationClips.Length; i++)
            if (ac.animationClips[i].name == name)
                AttackAnimTime = ac.animationClips[i].length;

        return AttackAnimTime;
    }

    

    IEnumerator HitSoundPlay()
    {
        //SoundManager.instance.PlaySound_Hit_Sword();
        //Audio.PlayOneShot(SoundManager.instance.ReturnSoundClip_Hit_Sound());
        //AttackAnimTime = 1f;

        AudioManager.instance.PlaySFXWithAudioSource(HitSounds[Random.Range(0, HitSounds.Length)], Audio);

        yield return null;
    }

    public void NpcAttackSoundPlay()
    {
        if (AttackSounds.Length < 1)
            return;

        if (Audio == null)
            return;

        string name = AttackSounds[Random.Range(0, AttackSounds.Length)];

        AudioManager.instance.PlaySFXWithAudioSource(name, Audio);
    }

    IEnumerator Debuff_Poision(bufforDebuff bNd)
    {
        yield return new WaitForSeconds(1);
        TakeDamage(bNd.poisonDamage,bNd.poisonDamage, bNd.fromBy, false, false, true);

        if (bNd.Duration < 0)
        {
            yield return null;
        }
        else
        {
            StartCoroutine(Debuff_Poision(bNd));
        }
    }

    IEnumerator Debuff_Stun(bufforDebuff bNd)
    {
        isStun.AddBoolModifier();
        //FloatingTextController.CreateFloatingText("Stun", transform, transform.gameObject.tag, 15, gameObject,true, owner: this);
        FloatingTextController.GenerateDebuffFloatingText("Stun", transform, 15, npcType, owner: gameObject);
        yield return new WaitForSeconds(bNd.Duration);
        isStun.RemoveBoolModifier();
    }

    IEnumerator Debuff_Maimed(bufforDebuff bNd)
    {
        isMaimed.AddBoolModifier();
        npc_AI.OnChangeNPCSpeed(-6, -.3f, -.3f, -.4f);
        //FloatingTextController.CreateFloatingText("SlowDown", transform, transform.gameObject.tag, 15, gameObject,true, owner: this);
        FloatingTextController.GenerateDebuffFloatingText("Stun", transform, 15, npcType, owner: gameObject);
        yield return new WaitForSeconds(bNd.Duration);
        isMaimed.RemoveBoolModifier();
        npc_AI.OnChangeNPCSpeed(6, .3f, .3f, .4f);
    }

    IEnumerator Debuff_Restraint(bufforDebuff bNd)
    {
        isRestraint.AddBoolModifier();
        var effect = Instantiate(PrefabCollect.instance.ChainOfPurgatoryEffect, transform);
        //FloatingTextController.CreateFloatingText("Binding", transform, transform.gameObject.tag, 15, gameObject,true, owner: this);
        FloatingTextController.GenerateDebuffFloatingText("Stun", transform, 15, npcType, owner: gameObject);
        yield return new WaitForSeconds(bNd.Duration);
        isRestraint.RemoveBoolModifier();
        Destroy(effect);
    }

    protected IEnumerator BlinkObject()
    {
        /*mpb.SetColor("_Black", Color.black);

        foreach (MeshRenderer renderer in meshRenderers)
        {
            renderer.SetPropertyBlock(mpb);
        }

        yield return new WaitForSeconds(0.1f);

        foreach (MeshRenderer renderer in meshRenderers)
        {
            renderer.SetPropertyBlock(null);
        }*/

        /*Material[] mats = null;

        foreach (SkinnedMeshRenderer skinned in skinnedMeshRenderers)
        {
            mats = skinned.materials;

            for (int i = 0; i < mats.Length; i++)
            {
                BlendMode.SetBlendMode(mats[i], BlendMode.Mode.Fade);
                //mat[i].SetFloat("_Mode", 2f);
                mats[i].color = new Color(mats[i].color.r, mats[i].color.g, mats[i].color.b, 0.5f);

            }
        }

        foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
        {
            for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
            {
                skinnedMeshRenderer.materials[i].SetFloat("_Opacity", 0.55f);
            }
        }

        yield return new WaitForSeconds(0.1f);

        foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
        {
            for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
            {
                skinnedMeshRenderer.materials[i].SetFloat("_Opacity", 1);
            }
        }

        foreach (SkinnedMeshRenderer skinned in skinnedMeshRenderers)
        {
            mats = skinned.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i].color = new Color(mats[i].color.r, mats[i].color.g, mats[i].color.b, 1f);
                //mat[i].SetFloat("_Mode", 0f);
                BlendMode.SetBlendMode(mats[i], BlendMode.Mode.Opaque);
            }

            skinned.SetPropertyBlock(null);
        }*/
        yield return null;
    }

    public void BuffDebuffTime()
    {
        for(int i = 0; i < buffAndDebuffsList02.Count; i++)
        {
            bufforDebuff TempStruct = buffAndDebuffsList02[i];
            TempStruct.Duration -= Time.deltaTime;
            buffAndDebuffsList02[i] = TempStruct;

            if (buffAndDebuffsList02[i].Duration < 0)
            {
                buffAndDebuffsList02.Remove(buffAndDebuffsList02[i]);
                Debug.Log("디버프 종료");
            }
        }
    }

    public void StartBuffAndDeBuffEffect(bufforDebuff bNd)
    {
        if(bNd.debuff == buffType.poison)
        {
            StartCoroutine(Debuff_Poision(bNd));
            //FloatingTextController.CreateFloatingText("Poison", transform, transform.gameObject.tag, 15, gameObject,true, owner: this);
            FloatingTextController.GenerateDebuffFloatingText("Poison", transform, 15, npcType, owner: gameObject);
        }
        if(bNd.debuff == buffType.stun)
        {
            StartCoroutine(Debuff_Stun(bNd));
        }
        if (bNd.debuff == buffType.Maimed)
        {
            StartCoroutine(Debuff_Maimed(bNd));
        }
        if(bNd.debuff == buffType.Restraint)
        {
            StartCoroutine(Debuff_Restraint(bNd));
        }
    }

    public void GetBuffAndDebuff(bufforDebuff bNd)
    {
        buffAndDebuffsList02.Add(bNd);
        StartCoroutine(healthUi.addBNDicon(bNd));
        StartBuffAndDeBuffEffect(buffAndDebuffsList02.Find(item => item == bNd));
    }

    public void fullHP()
    {
        currentHealth = (int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat());

        if (OnHealthChanged != null)
        {
            OnHealthChanged((int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat()), currentHealth);
        }
    }

    public void OnShieldChange()
    {
        if (OnShieldChanged != null)
        {
            OnShieldChanged((int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat()), shieldStat.GetTotalShieldValue());
        }
    }

    public AudioSource GetAudioSource()
    {
        return Audio;
    }
}

[System.Serializable]
public class bufforDebuff
{
    public buffType debuff;
    public float Duration;
    public GameObject fromBy;
    public int poisonDamage;
}
