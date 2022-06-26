using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadMovingTest : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform head;
    public Animator animator;
    public Transform Target;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        head = animator.GetBoneTransform(HumanBodyBones.Head);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        head.LookAt(Target);
        //head.rotation = head.rotation * Quaternion.Euler(Vector3.up);
    }
}
