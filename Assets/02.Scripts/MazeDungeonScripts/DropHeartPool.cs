using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropHeartPool : MonoBehaviour
{
    [SerializeField] GameObject dropHeart;
    [SerializeField] Transform dropHeartParent;

    Queue<GameObject> dropHeartQueue = new Queue<GameObject>();
    List<GameObject> entireDropHeart = new List<GameObject>();

    public static DropHeartPool instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject DeQueueDropHeart(Vector3 position)
    {
        if (dropHeartQueue.Count > 0)
        {
            var heart = dropHeartQueue.Dequeue();

            heart.transform.position = position + (Vector3.up * 2);
            heart.SetActive(true);
            heart.GetComponent<Rigidbody>().AddForce(new Vector3((Random.Range(-1, 2)), 1.5f, (Random.Range(-1, 2))) * 250, ForceMode.Acceleration);
            return heart;
        }
        else
        {
            var heart = GenerateNewDropHeart(position);
            heart.SetActive(true);
            heart.GetComponent<Rigidbody>().AddForce(new Vector3((Random.Range(-1, 2)), 1.5f, (Random.Range(-1, 2))) * 250, ForceMode.Acceleration);
            return heart;
        }
    }

    public void EnqueueDropHeart(GameObject heart)
    {
        heart.SetActive(false);
        dropHeartQueue.Enqueue(heart);
    }

    public GameObject GenerateNewDropHeart(Vector3 position)
    {
        var heart = Instantiate(PrefabCollect.instance.dropHeart, position + (Vector3.up * 2), Quaternion.identity, dropHeartParent);

        heart.GetComponent<Rigidbody>().AddForce(new Vector3((Random.Range(-1, 2)), 1.5f, (Random.Range(-1, 2))) * 250, ForceMode.Acceleration);

        entireDropHeart.Add(heart);
        return heart;
    }

    public void EnQueueEntireDropHeart()
    {
        for(int i = 0; i < entireDropHeart.Count; i++)
        {
            if (entireDropHeart[i].gameObject.activeSelf)
                EnqueueDropHeart(entireDropHeart[i]);
        }
    }

}
