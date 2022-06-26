using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadLook : MonoBehaviour
{
    public Transform target;
    public Transform head;
    private Animator animator;
    public Vector3 offset;

    public bool correctAngle = false;

    [Range(0, 360)]
    public float viewAngle;
    public float viewRadius;
    private Vector3 gap;

    public bool active = true;

    private Quaternion defaultAngle = Quaternion.EulerAngles(0,0,0);

    private void Start()
    {
        if(target == null)
        {
            target = GameObject.FindGameObjectWithTag("playertarget").transform;
        }

        //defaultAngle = head.rotation;
    }

    private void LateUpdate()
    {
        if(!active)
        {
            return;
        }

        if (target != null)
        {
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            //print(gap);

            gap = head.localEulerAngles;

            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)      //  && !(gap.z > 90 && gap.z < 270)
            {
                //print(head.eulerAngles.z);
                head.LookAt(target.position);
                head.rotation = head.rotation * Quaternion.Euler(offset);

                if(!(gap.z > 90 && gap.z < 270) && correctAngle == true)            //머리 각도조절 이거 안하면 머리가 반대로 꺽임
                {
                    gap.z = Mathf.Clamp(gap.z, 90, 270);
                    head.localEulerAngles = new Vector3(head.localEulerAngles.x, head.localEulerAngles.y, 0);
                }

                //head.rotation = Quaternion.Euler(Mathf.Clamp(head.rotation.x, -90f, 90f), head.rotation.y, Mathf.Clamp(head.rotation.z, -90f, 90f));
            }
            else
            {
                //print("각도 초과 " + gap.z);
                //head.rotation = Quaternion.Euler(0,0,0);
            }

        }
    }

    private void OnDisable()
    {
        //head.rotation = Quaternion.EulerAngles(0,0,0);
    }

    public Vector3 dirFromAngle(float angleInDegerees, bool angelIsGlobal)
    {
        if(!angelIsGlobal)
        {
            angleInDegerees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegerees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegerees * Mathf.Deg2Rad));
    }

}
