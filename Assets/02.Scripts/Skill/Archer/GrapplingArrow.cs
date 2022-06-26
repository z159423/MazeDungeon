using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingArrow : MonoBehaviour
{
    private Rigidbody rigidbody;
    private Collider collider;

    public Rigidbody playerRig;
    public GameObject target;
    public GameObject First, Second;
    public Vector3 DirectionAdjustment;
    public float arrowSpeed;
    public GameObject player;
    public float ditToPlayer = 0;

    Vector3 dir;
    bool isTrigger;
    float time = 0;

    LineRenderer lr;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

        target = GameObject.FindWithTag("playertarget");
        var dir = (target.transform.position + DirectionAdjustment) - player.transform.position;
        transform.LookAt(target.transform);
        transform.Rotate(new Vector3(90, 0, 0));

        rigidbody.AddForce(dir.normalized * arrowSpeed);

        lr = GetComponent<LineRenderer>();
        lr.startWidth = .25f;
        lr.endWidth = .25f;
    }

    private void Update()
    {
        lr.SetPosition(0, First.transform.position);
        lr.SetPosition(1, Second.transform.position);

        if ((Vector3.Distance(player.transform.position, transform.position) < 2 && isTrigger) || time > 3)
            Destroy(gameObject);

        ditToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (ditToPlayer > 50)
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (isTrigger)
        {
            dir = transform.position - player.transform.position;
            player.transform.position = player.transform.position + dir.normalized * Time.deltaTime * 40;
            Debug.DrawLine(player.transform.position, transform.position, Color.red);
        }
        else
        {
            time += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<NPC_AI>() != null)
        {
            if (other.GetComponentInParent<NPC_AI>().npcType == NPC_Type.enemy)
            {
                transform.SetParent(other.transform);
                rigidbody.useGravity = false;
                rigidbody.isKinematic = true;
                collider.enabled = false;
                //pull();
                isTrigger = true;
                Destroy(gameObject, 2);
            }
        }
        else if (other.tag == "Ground" || other.tag == "Environment")
        {
            transform.SetParent(other.transform);
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            //isTrigged = true;
            collider.enabled = false;
            isTrigger = true;
            //pull();
            Destroy(gameObject, 2);
        }
    }

    private void pull()
    {
        Vector3 dir = (transform.position - player.transform.position).normalized;
        player.GetComponent<Rigidbody>().AddForce(dir * 3000);
    }
}
