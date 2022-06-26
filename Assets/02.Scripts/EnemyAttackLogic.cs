using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackLogic : MonoBehaviour
{
    public Animator animator;
    Collider colli;
    CharacterStats CharStat;
    NPCStats EnemyStat;
    //AnimationClip animclip;
    Collider other;

    float AttackAnimTime = 0;


    // Use this for initialization
    void Start () {
        colli = GetComponent<Collider>();
        //animator = GetComponent<Animator>();
        CharStat = GameObject.Find("Player").GetComponent<CharacterStats>(); // ★★★ 다른 스크립트 사용 가능하게 해주는 함수
        //animclip = GetComponent<AnimationClip>();

        EnemyStat = transform.gameObject.GetComponentInParent<NPCStats>();

        AttackAnimTime = AnimationLength("Punch"); // 애니메이션클립(공격모션)의 길이를 알아내는 부분 "애니메이션클립이름"을 변경하면 됨
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "Player" && animator.GetBool("IsAttack") == true)
        {
            StartCoroutine(AttackTrigger(AttackAnimTime * 1f));
            //colli.isTrigger = false;
            Debug.Log("Player 영역 안으로 진입");
            //CharStat.TakeDamage(10);
        }
    }

 

    IEnumerator AttackTrigger(float animLength)
    {
        Debug.Log("코루틴 시작");
        colli.isTrigger = false;
        CharStat.TakeDamage(Mathf.RoundToInt(EnemyStat.minDamage.GetFinalStatValue())
            , Mathf.RoundToInt(EnemyStat.maxDamage.GetFinalStatValue()), transform.gameObject, true, true, false);
        yield return new WaitForSeconds(AttackAnimTime);
        Debug.Log("코루틴 종료");
        colli.isTrigger = true;
    }

    float AnimationLength(string name)          //"Punch" 애니메이션클립의 길이를 알아내는 함수
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;

        for (int i = 0; i < ac.animationClips.Length; i++)
            if (ac.animationClips[i].name == name)
                AttackAnimTime = ac.animationClips[i].length;

        return AttackAnimTime;
    }
}
