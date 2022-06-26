using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour {

    //private GameObject meleeobject;
    //private BoxCollider Boxcolli;
    private Animator PlayerAnimator;

    //GameObject enemy;
    //EnemyStats enemystats;

    //EquipmentManager equipmentmanager;

    private bool isPlayerEnter;
    private int enemyhealth;

    void Awake()
    {
        //equipmentmanager = GameObject.Find("GameManager").GetComponent<EquipmentManager>();
        //meleeobject = GetComponent<GameObject>();
        //enemy = GameObject.FindGameObjectWithTag("Enemy");
        //enemyhealth = GameObject.Find("Enemy01").GetComponent<EnemyStats>().currentHealth;        // GameObject "Enemy01" 에 있는 EnemyStats 스크립트의 int 변수 currentHealth를 가져온다.(변수 자체를 가져오는게 아닌 수치만 복사해서 가져옴)
        //Boxcolli = GetComponent<BoxCollider>();
        //enemystats = GameObject.Find("Enemy01").GetComponent<EnemyStats>();     // GameObject "Enemy01" 에 있는 컴포넌트 EnemyStats를 가져온다.
        PlayerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }
    // Use this for initialization
    void Start () {
        //Boxcolli.enabled = true;       //Collider 컴포넌트는 .enabled로 껏다 킬 수 잇음

    }
	
	// Update is called once per frame
	void Update () {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemyhitbox" && PlayerAnimator.GetBool("isAttack"))
        {
            //enemystats.TakeDamage(10);
            //enemystats.Blink();
        }
    }

    private void OnTriggerExit(Collider other)
    {

    }
}
