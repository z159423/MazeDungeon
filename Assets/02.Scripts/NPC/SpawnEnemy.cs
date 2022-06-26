using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public EnemyPrefabs[] NPCInfo;
    public GameObject Player;
    public Transform NPCPoolTransform;
    public int SpawnRange;

    private GameObject SpawnPoint;
    private IsColliderTrigger isColliderTrigger;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(MakeNpcPool());
        StartCoroutine(CheckNpcNumber());
        SpawnPoint = GameObject.Find("SpawnPoint");
        isColliderTrigger = SpawnPoint.transform.GetComponent<IsColliderTrigger>();
    }

    IEnumerator SpawnNpc(int num)
    {
        yield return new WaitForSeconds(0.5f);

        if (NPCPoolTransform.Find(NPCInfo[num].NPCPrefabs.name) == null)
        {
            GameObject parent = new GameObject(NPCInfo[num].NPCPrefabs.name);
            parent.transform.SetParent(NPCPoolTransform);
            //parent.name = NPCInfo[0].NPCPrefabs.name;
        }
        //if (NPCInfo[0].MaxStack >= GameObject.FindGameObjectsWithTag(NPCInfo[0].NPCPrefabs.name))
        while (true)
        {

            if (NPCInfo[num].MaxStack <= NPCPoolTransform.Find(NPCInfo[num].NPCPrefabs.name).transform.childCount)
            {
                Debug.Log(NPCInfo[num].NPCPrefabs.name + "스폰중단");
                break;
            }

            yield return new WaitForSeconds(NPCInfo[num].SpawnTime);

            Vector3 Rvector = new Vector3(Random.Range(-SpawnRange, SpawnRange), 0, Random.Range(-SpawnRange, SpawnRange));
            GameObject child;

            //Debug.Log("레이케스트 위치" + Rvector);
            Debug.DrawRay(Player.transform.position + Rvector + new Vector3(0, 1000f, 0), Vector3.down * 2000f, Color.red);

            RaycastHit hit;

            SpawnPoint.transform.position = Player.transform.position + Rvector + new Vector3(0,5,0);
            if (Physics.Raycast(Player.transform.position + Rvector + new Vector3(0, 1000f, 0), Vector3.down, out hit, 2000f,5))
            {
                //Debug.Log("레이케스트 성공" + hit.point);
                if (hit.transform.tag == "Ground" && isColliderTrigger.isTrigged == false)
                {
                    SpawnPoint.transform.position = hit.point;
                    child = Instantiate(NPCInfo[num].NPCPrefabs, hit.point + new Vector3(0, 5, 0), Player.transform.rotation) as GameObject;
                    child.transform.SetParent(NPCPoolTransform.Find(NPCInfo[num].NPCPrefabs.name));
                    //Debug.Log(child + " " + hit.transform.tag + " " +  child.transform.position);
                }
            }
        }
        yield return null;
    }

    IEnumerator CheckNpcNumber()
    {
            for (int i = 0; i < NPCInfo.Length; i++)
            {
                if (NPCInfo[i].MaxStack > NPCPoolTransform.Find(NPCInfo[i].NPCPrefabs.name).transform.childCount)
                {
                    Debug.Log(NPCInfo[i].NPCPrefabs.name + "스폰 시작");
                    StartCoroutine(SpawnNpc(i));
                }
            }
            yield return null ;
    }

    

    IEnumerator MakeNpcPool()
    {
        for (int i = 0; i < NPCInfo.Length; i++)
        {
            if (NPCPoolTransform.Find(NPCInfo[i].NPCPrefabs.name) == null)
            {
                Debug.Log(NPCInfo[i].NPCPrefabs.name + "스폰풀 생성");
                GameObject parent = new GameObject(NPCInfo[i].NPCPrefabs.name);
                parent.transform.SetParent(NPCPoolTransform);
                //parent.name = NPCInfo[0].NPCPrefabs.name;
            }
        }
        yield return null;
    }

    [System.Serializable]
    public class EnemyPrefabs
    {
        public GameObject NPCPrefabs;
        public float SpawnTime;
        public int MaxStack;
    }
}
