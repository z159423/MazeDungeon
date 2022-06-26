using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    
    public GameObject SpiderEgg;
    [Range(0,100)]
    public int EggSpawnPercent = 30;
    public Transform[] SpiderEggSpawnPoint;
    [Space]

    public GameObject WebBall;
    public Transform webBallFirePos;
    public bool enableWebBallFire = false;
    public bool webBallReady = true;
    public float webBallCoolTime = 6;
    public float webBallFireMinDist = 3f;
    public float webBallFireMaxDist = 15f;
    public int webBallAmount = 1;
    public int webBallFireAngle = 90;

    private List<GameObject> SpawnedSpiderEgg = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        SpawnRandomSpiderEgg();
    }

    public IEnumerator FireWebBall()
    {
        GetComponent<Animator>().SetTrigger("FireWebBall");

        webBallReady = false;

        yield return new WaitForSeconds(webBallCoolTime);

        webBallReady = true;
    }

    public void fireWebBall()
    {
        /*var webBall = Instantiate(WebBall, webBallFirePos.position, Quaternion.identity).GetComponent<NPC_ProjectileLogic>();
        webBall.Setting(gameObject, 5);

        webBall.Fire(GetComponent<NPC_AI>().Target.bounds.center, webBallFirePos.position, gameObject);*/

        AudioManager.instance.GenerateAudioAndPlaySFX("spiderAttack4", webBallFirePos.position);

        int Angel = 0;

        if (webBallAmount > 1)
            Angel = webBallFireAngle / (webBallAmount - 1);

        for (int i = 0; i < webBallAmount; i++)
        {
            var webBall = Instantiate(WebBall, webBallFirePos.position, Quaternion.identity).GetComponent<ProjectileLogic>();
            webBall.Setting(gameObject, Mathf.RoundToInt(GetComponent<NPCStats>().minDamage.GetFinalStatValue()), Mathf.RoundToInt(GetComponent<NPCStats>().maxDamage.GetFinalStatValue()));

            webBall.Fire(GetComponent<NPC_AI>().Target.bounds.center, webBallFirePos.position, gameObject, (Angel * (i)) - (webBallFireAngle / 2));
        }
    }

    public void fireManyWebBall()
    {
        int Angel = webBallFireAngle / (webBallAmount - 1);

        for(int i = 0; i < webBallAmount; i++)
        {
            var webBall = Instantiate(WebBall, webBallFirePos.position, Quaternion.identity).GetComponent<ProjectileLogic>();
            webBall.Setting(gameObject, Mathf.RoundToInt(GetComponent<NPCStats>().minDamage.GetFinalStatValue()), Mathf.RoundToInt(GetComponent<NPCStats>().maxDamage.GetFinalStatValue()));

            webBall.Fire(GetComponent<NPC_AI>().Target.bounds.center, webBallFirePos.position, gameObject, (Angel * (i)) - (webBallFireAngle / 2));
        }
    }

    void SpawnRandomSpiderEgg()
    {
        foreach(Transform trans in SpiderEggSpawnPoint)
        {
            int random = Random.Range(0, 100);

            if(random < EggSpawnPercent)
            {
                var egg = Instantiate(SpiderEgg, trans);
                egg.transform.localScale = new Vector3(.5f, .5f, .5f);
                egg.GetComponentInChildren<Rigidbody>().isKinematic = true;
                egg.GetComponentInChildren<Collider>().enabled = false;
                egg.GetComponentInChildren<SpiderEgg>().enabled = false;
                SpawnedSpiderEgg.Add(egg);
                egg.GetComponentInChildren<SpiderEgg>().SetEggLevel(GetComponent<NPCStats>().npcLevel);
            }
        }
    }

    public void Die()
    {
        foreach(GameObject egg in SpawnedSpiderEgg)
        {
            egg.transform.SetParent(null);
            egg.GetComponentInChildren<Collider>().enabled = true;
            egg.GetComponentInChildren<Rigidbody>().isKinematic = false;
            egg.GetComponentInChildren<SpiderEgg>().enabled = true;
        }
    }
}
