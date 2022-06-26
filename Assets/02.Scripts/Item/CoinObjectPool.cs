using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject copperCoin;
    [SerializeField] private GameObject silverCoin;
    [SerializeField] private GameObject goldCoin;

    [SerializeField] private Vector3 PopUpForce;

    Queue<GameObject> copperCoinQueue = new Queue<GameObject>();
    Queue<GameObject> silverCoinQueue = new Queue<GameObject>();
    Queue<GameObject> goldCoinQueue = new Queue<GameObject>();

    [SerializeField] Transform poolParent;

    public static CoinObjectPool instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject GenerateNewCopperCoin(Vector3 position)
    {
        var randomVec3 = new Vector3(UnityEngine.Random.Range(-0.25f, 0.25f), 0, UnityEngine.Random.Range(-0.25f, 0.25f));
        var randomForce = new Vector3(UnityEngine.Random.Range(-3f, 3f), 0, UnityEngine.Random.Range(-3f, 3f));
        var coin = Instantiate(copperCoin, position + randomVec3, copperCoin.transform.rotation, poolParent);
        coin.GetComponent<Rigidbody>().velocity = PopUpForce + randomForce;

        return coin;
    }

    public GameObject GenerateNewSilverCoin(Vector3 position)
    {
        var randomVec3 = new Vector3(UnityEngine.Random.Range(-0.25f, 0.25f), 0, UnityEngine.Random.Range(-0.25f, 0.25f));
        var randomForce = new Vector3(UnityEngine.Random.Range(-3f, 3f), 0, UnityEngine.Random.Range(-3f, 3f));
        var coin = Instantiate(silverCoin, position + randomVec3, silverCoin.transform.rotation, poolParent);
        coin.GetComponent<Rigidbody>().velocity = PopUpForce + randomForce;

        return coin;
    }

    public GameObject GenerateNewGoldCoin(Vector3 position)
    {
        var randomVec3 = new Vector3(UnityEngine.Random.Range(-0.25f, 0.25f), 0, UnityEngine.Random.Range(-0.25f, 0.25f));
        var randomForce = new Vector3(UnityEngine.Random.Range(-3f, 3f), 0, UnityEngine.Random.Range(-3f, 3f));
        var coin = Instantiate(goldCoin, position + randomVec3, goldCoin.transform.rotation, poolParent);
        coin.GetComponent<Rigidbody>().velocity = PopUpForce + randomForce;

        return coin;
    }

    public void EnqueueCopperCoin(GameObject coin)
    {
        coin.SetActive(false);
        copperCoinQueue.Enqueue(coin);
    }

    public GameObject DeQueueCopperCoin(Vector3 position)
    {
        if (copperCoinQueue.Count > 0)
        {
            var coin = copperCoinQueue.Dequeue();

            while (coin == null)
            {
                coin = copperCoinQueue.Dequeue();

                if (coin != null)
                    break;
            }

            var randomVec3 = new Vector3(UnityEngine.Random.Range(-0.25f, 0.25f), 0, UnityEngine.Random.Range(-0.25f, 0.25f));
            var randomForce = new Vector3(UnityEngine.Random.Range(-3f, 3f), 0, UnityEngine.Random.Range(-3f, 3f));
            coin.transform.position = position + randomVec3;
            coin.SetActive(true);
            coin.GetComponent<Rigidbody>().velocity = PopUpForce + randomForce;
            return coin;
        }
        else
        {
            var randomVec3 = new Vector3(UnityEngine.Random.Range(-0.25f, 0.25f), 0, UnityEngine.Random.Range(-0.25f, 0.25f));
            var randomForce = new Vector3(UnityEngine.Random.Range(-3f, 3f), 0, UnityEngine.Random.Range(-3f, 3f));
            var coin = GenerateNewCopperCoin(position + randomVec3);
            coin.SetActive(true);
            coin.GetComponent<Rigidbody>().velocity = PopUpForce + randomForce;
            return coin;
        }
    }

    public void EnqueueSilverCoin(GameObject coin)
    {
        coin.SetActive(false);
        silverCoinQueue.Enqueue(coin);
    }

    public GameObject DeQueueSilverCoin(Vector3 position)
    {
        if (silverCoinQueue.Count > 0)
        {
            var coin = silverCoinQueue.Dequeue();

            var randomVec3 = new Vector3(UnityEngine.Random.Range(-0.25f, 0.25f), 0, UnityEngine.Random.Range(-0.25f, 0.25f));
            var randomForce = new Vector3(UnityEngine.Random.Range(-3f, 3f), 0, UnityEngine.Random.Range(-3f, 3f));
            coin.transform.position = position + randomVec3;
            coin.SetActive(true);
            coin.GetComponent<Rigidbody>().velocity = PopUpForce + randomForce;
            return coin;
        }
        else
        {
            var randomVec3 = new Vector3(UnityEngine.Random.Range(-0.25f, 0.25f), 0, UnityEngine.Random.Range(-0.25f, 0.25f));
            var randomForce = new Vector3(UnityEngine.Random.Range(-3f, 3f), 0, UnityEngine.Random.Range(-3f, 3f));
            var coin = GenerateNewSilverCoin(position + randomVec3);
            coin.SetActive(true);
            coin.GetComponent<Rigidbody>().velocity = PopUpForce + randomForce;
            return coin;
        }
    }

    public void EnqueueGoldCoin(GameObject coin)
    {
        coin.SetActive(false);
        goldCoinQueue.Enqueue(coin);
    }

    public GameObject DeQueueGoldCoin(Vector3 position)
    {
        if (goldCoinQueue.Count > 0)
        {
            var coin = goldCoinQueue.Dequeue();

            var randomVec3 = new Vector3(UnityEngine.Random.Range(-0.25f, 0.25f), 0, UnityEngine.Random.Range(-0.25f, 0.25f));
            var randomForce = new Vector3(UnityEngine.Random.Range(-3f, 3f), 0, UnityEngine.Random.Range(-3f, 3f));
            coin.transform.position = position + randomVec3;
            coin.SetActive(true);
            coin.GetComponent<Rigidbody>().velocity = PopUpForce + randomForce;
            return coin;
        }
        else
        {
            var randomVec3 = new Vector3(UnityEngine.Random.Range(-0.25f, 0.25f), 0, UnityEngine.Random.Range(-0.25f, 0.25f));
            var randomForce = new Vector3(UnityEngine.Random.Range(-3f, 3f), 0, UnityEngine.Random.Range(-3f, 3f));
            var coin = GenerateNewGoldCoin(position + randomVec3);
            coin.SetActive(true);
            coin.GetComponent<Rigidbody>().velocity = PopUpForce + randomForce;
            return coin;
        }
    }

}
