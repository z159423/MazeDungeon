using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int count;
    public CoinType coinType;

    private bool playSound = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Inventory.instance.GetCoin(count);
            AudioManager.instance.GenerateAudioAndPlaySFX("coinCollect", transform.position + Vector3.up);
            //other.GetComponent<AudioSource>().PlayOneShot(SoundManager.instance.CoinCollect);

            EnqueueCoin(coinType);

            //Destroy(gameObject);
        }

        if(other.CompareTag("Reposition"))
            EnqueueCoin(coinType);
    }

    void EnqueueCoin(CoinType coinType)
    {
        switch (coinType)
        {
            case CoinType.copper:
                CoinObjectPool.instance.EnqueueCopperCoin(gameObject);
                break;

            case CoinType.silver:
                CoinObjectPool.instance.EnqueueSilverCoin(gameObject);

                break;

            case CoinType.gold:
                CoinObjectPool.instance.EnqueueGoldCoin(gameObject);
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!playSound)
        {
            GetComponent<AudioSource>().PlayOneShot(SoundManager.instance.CoinDrop);
            playSound = true;
        }
        
    }
}

public enum CoinType {copper, silver,gold }
