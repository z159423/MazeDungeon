using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropHeart : MonoBehaviour
{
    public int hpRecovery = 10;
    [SerializeField]private Transform root;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerStats>() != null)
        {
            var stat = other.GetComponent<PlayerStats>();

            if (stat.currentHealth != stat.maxHealth.GetFinalStatValue())
            {
                stat.Heal(hpRecovery);
                AudioManager.instance.GenerateAudioAndPlaySFX("dropHeart", transform.position);
                DropHeartPool.instance.EnqueueDropHeart(root.gameObject);
            }
        }

        if (other.CompareTag("Reposition"))
            DropHeartPool.instance.EnqueueDropHeart(root.gameObject);
    }

    public static void GenerateDropHeart(Vector3 position)
    {
        DropHeartPool.instance.DeQueueDropHeart(position);

        return;
    }
}
