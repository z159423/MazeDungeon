using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeBothObject : MonoBehaviour
{
    LineRenderer lr;
    public GameObject First, Second;
    public Shackle FirstS, SecondS;

    public int PullPower = 2;
    public int ShackleFirePower = 1000;

    private Vector3 dir1, dir2;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.startWidth = .25f;
        lr.endWidth = .25f;

        //cube1Pos = gameObject.GetComponent<Transform>().position;
    }

    void Update()
    {
        lr.SetPosition(0, First.transform.position);
        lr.SetPosition(1, Second.transform.position);

        dir1 = Second.transform.position - First.transform.position;
        dir2 = First.transform.position - Second.transform.position;

        if (FirstS.isTrigged && FirstS.isFire && SecondS.isFire)
        {
            First.transform.parent.position = First.transform.parent.position + (dir1.normalized * Time.deltaTime * PullPower);
        }

        if (SecondS.isTrigged && FirstS.isFire && SecondS.isFire)
        {
            Second.transform.parent.position = Second.transform.parent.position + (dir2.normalized * Time.deltaTime * PullPower);
        }
            
    }

    public void FireFirstShackle()
    {
        var target = GameObject.FindWithTag("playertarget");
        var rigidbody = First.GetComponent<Rigidbody>();
        First.GetComponent<Collider>().enabled = true;
        FirstS.isFire = true;

        First.transform.SetParent(null);

        rigidbody.useGravity = true;

        var dir = target.transform.position - gameObject.transform.position;

        transform.LookAt(target.transform);
        rigidbody.AddForce(dir.normalized * ShackleFirePower);
    }

    public void FireSecondShackle()
    {
        var target = GameObject.FindWithTag("playertarget");
        var rigidbody = Second.GetComponent<Rigidbody>();
        Second.GetComponent<Collider>().enabled = true;
        SecondS.isFire = true;

        Second.transform.SetParent(null);

        rigidbody.useGravity = true;

        var dir = target.transform.position - gameObject.transform.position;

        transform.LookAt(target.transform);
        rigidbody.AddForce(dir.normalized * ShackleFirePower);
    }

    public void DestroyShackles(int time)
    {
        Destroy(First, time);
        Destroy(Second, time);
        Destroy(gameObject, time);
    }

}
