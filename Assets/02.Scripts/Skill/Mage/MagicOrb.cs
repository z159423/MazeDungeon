using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicOrb : MonoBehaviour
{
    public GameObject owner;
    public Transform target;
    public float fireSpeed;
    public float flyingSpeed;
    public float deleteTime = 5f;
    public int damage = 0;
    public Vector3 DirectionAdjustment;

    private new Rigidbody rigidbody;
    private TrailRenderer trailRenderer;
    private bool isFire = false;
    private float x = 0;
    private float rotation = 0;
    private float num = 0;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        x += Time.deltaTime;

        rotation = Mathf.PerlinNoise(x, x);

        rotation *= 5;

        num += rotation + (Time.deltaTime * rotation);
        transform.rotation = Quaternion.Euler(num, num, num);

        if(num < 0)
        {
            num *= -1;
        }

        if (!isFire)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, flyingSpeed);
        }
        
    }
    public void FireMagicOrb(Vector3 targetPosition)
    {
        isFire = true;
        trailRenderer.emitting = true;
        var dir = (targetPosition + DirectionAdjustment) - gameObject.transform.position;

        owner.GetComponent<PlayerController01>().magicOrbBeacons.Pop();
        StartCoroutine(owner.GetComponent<PlayerController01>().ChangeMagicOrbsPosition());

        rigidbody.AddForce(dir.normalized * fireSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<NPC_AI>() != null)
        {
            if ((other.GetComponentInParent<NPC_AI>().npcType == NPC_Type.enemy || other.GetComponentInParent<NPC_AI>().npcType == NPC_Type.neutrality) && isFire)
            {
                other.transform.GetComponentInParent<CharacterStats>().TakeDamage(damage, owner, true, true, false, true);
                Destroy(transform.gameObject);
            }
        }
        else if ((other.tag == "Ground" || other.tag == "Environment") && isFire)
        {
            Destroy(transform.gameObject);
        }
    }
}
