using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulSkull : MonoBehaviour
{
    public Transform TargetPlayer;
    public float m_currentSpeed = 0;
    public int soulValue = 0;

    private Rigidbody rigid;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.AddForce(new Vector3(0, 1, 0) * 1000);

    }

    private void LateUpdate()
    {
        if(m_currentSpeed < 2)
        {
            m_currentSpeed += Time.deltaTime;

            rigid.velocity = transform.up * m_currentSpeed;
        }
        else
        {
            if (TargetPlayer != null)
            {
                CapsuleCollider PlayerCollider = TargetPlayer.GetComponent<CapsuleCollider>();

                rigid.velocity = transform.up * m_currentSpeed;

                m_currentSpeed += Time.deltaTime;
                //startM_CurrentSpeed = startM_CurrentSpeed - Time.deltaTime;

                Vector3 t_dir = (PlayerCollider.bounds.center - transform.position).normalized;
                transform.up = Vector3.Lerp(transform.up, t_dir, 0.1f);
            }
        }
  
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Destroy(transform.parent.gameObject);
            other.GetComponent<PlayerStats>().GetSoul(soulValue);
        }
    }
}
