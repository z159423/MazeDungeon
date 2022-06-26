using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GurosSpear : MonoBehaviour
{
    public bool isPulling = false;
    public bool isReadyPull = false;

    public Transform Hand;

    public float arrowSpeed;
    public float turningSpeed;

    public float DistToHand;

    private Rigidbody rigid;
    public Guros guros;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(Hand != null)
        {
            DistToHand = Vector3.Distance(transform.position, Hand.position);

            if (isPulling)
            {
                var dirToEnemy = (Hand.position - transform.position).normalized;

                rigid.velocity = transform.up * arrowSpeed;
                transform.up = Vector3.Lerp(transform.up, dirToEnemy, turningSpeed);

                if (DistToHand < 1.5f)
                {
                    StickSpearToHand();
                }
            }
        }
           
    }

    public void StickSpearToHand()
    {
        transform.SetParent(Hand);
        transform.localPosition = guros.OriginSpearPotition;
        transform.localRotation = guros.OriginSpearRotation;

        rigid.isKinematic = true;

        isPulling = false;
        guros.SpearReady = true;
    }

    
}
