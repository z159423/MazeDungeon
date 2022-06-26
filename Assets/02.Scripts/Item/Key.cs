using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public int count;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory.instance.GetKey(count);
            other.GetComponent<AudioSource>().PlayOneShot(SoundManager.instance.CoinCollect);
            Destroy(gameObject);
        }

        if (other.CompareTag("Reposition"))
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<AudioSource>().PlayOneShot(SoundManager.instance.CoinDrop);
    }
}
