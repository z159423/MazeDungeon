using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowLogic : MonoBehaviour
{

    public Vector3 flyingOffset;
    public FlyingType flyingType;
    public GameObject owner;

    public float moveSpeed;

    [Space]

    public bool lookAtPlayerTarger = false;
    public Vector3 lookAtOffset;

    private Vector3 currentOffset;
    private Vector3 vectorToTarget;

    [Space]

    public float changeOffsetTime = 5f;
    public Vector3 changeMinOffset;
    public Vector3 changeMaxOffset;
    private Vector3 cangeOffset;

    private void OnEnable()
    {
        StartCoroutine(ChangeOffset());
    }

    private void OnDisable()
    {
        StopCoroutine(ChangeOffset());
    }

    // Update is called once per frame
    void Update()
    {
        if(owner)
            currentOffset = owner.transform.position + flyingOffset + cangeOffset;

        transform.localPosition = Vector3.Lerp(transform.localPosition, currentOffset, moveSpeed);

        if(owner && lookAtPlayerTarger)
        {
            vectorToTarget = owner.transform.position - transform.position;

            vectorToTarget.y = 0;
            vectorToTarget.Normalize();

            Quaternion targetAngle = Quaternion.LookRotation(vectorToTarget);

            //transform.LookAt(targetAngle.eulerAngles);
            

            transform.rotation = Quaternion.LookRotation(vectorToTarget) * Quaternion.Euler(lookAtOffset);
            //transform.rotation = transform.rotation * Quaternion.Euler(lookAtOffset);

        }
    }

    public IEnumerator ChangeOffset()
    {
        while(true)
        {
            yield return new WaitForSeconds(changeOffsetTime);
            cangeOffset = new Vector3(Random.Range(changeMinOffset.x, changeMaxOffset.x), Random.Range(changeMinOffset.y, changeMaxOffset.y), Random.Range(changeMinOffset.z, changeMaxOffset.z));
        }

    }

    public void SetFollower(GameObject owner)
    {
        this.owner = owner;
    }

    public enum FlyingType { flying, land }
}
