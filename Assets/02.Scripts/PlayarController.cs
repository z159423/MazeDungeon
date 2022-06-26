using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayarController : MonoBehaviour {

    public Transform Axis;

    public float speed = 10f;               //이동속도
    public float jumpPower = 5f;
    public float rotateSpeed = 5f;

    public float moveSpeed = 10.0f;

    public float rotSpeed = 100.0f;

    Rigidbody rigdbody;
    Animator animator;

    private Transform tr;

    Vector3 movement;
    float horizontalMove;
    float verticalMove;

    bool isJumping;                         //점프

    void Awake()
    {
        rigdbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        tr = GetComponent<Transform>();
        StartCoroutine("WhatHappen");
        
    }


    void Update()
    {
        StopCoroutine("WhatHappen");

        horizontalMove = Input.GetAxisRaw("Horizontal");   // Horizontal = 왼쪽, 오른쪽 방향키
        verticalMove = Input.GetAxisRaw("Vertical");     // Vertical = 위, 아래 방향키

        Vector3 moveDir = (Vector3.forward * verticalMove) + (Vector3.right * horizontalMove);

        tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed, Space.Self);

        //tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));

        //Run();

        if (Input.GetButtonDown("Jump"))
            isJumping = true;

        AnimationUpdate();
    }

    void FixedUpdate()
    {
        
        Jump();
        //Turn();
    }

    void Run()
    {
        movement.Set(horizontalMove, 0, verticalMove);
        movement = movement.normalized * speed * Time.deltaTime;        //(speed = 이동속도) * (Time.deltaTime = 프레임 보정시간)  두방향키를 동시에 눌러 대각선 이동이 가능해짐

        rigdbody.MovePosition(transform.position + movement);
    }

    void Turn ()
    {
        if (horizontalMove == 0 && verticalMove == 0)
            return;

        Quaternion newRotation = Quaternion.LookRotation(movement);

        rigdbody.rotation = Quaternion.Slerp(rigdbody.rotation, newRotation, rotateSpeed * Time.deltaTime);
    }

    void Jump()
    {
        if (!isJumping)
            return;

        if (animator.GetBool("isJumping") == true)
            return;

        rigdbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);

        isJumping = false;
    }

    void AnimationUpdate()
    {
        if(horizontalMove == 0 && verticalMove == 0)
        {
            animator.SetBool("isRunning", false);
        }
        else
        {
            animator.SetBool("isRunning", true);
        }
        /*
        if(isJumping == true)
        {
            animator.SetBool("isJumping", true);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }*/
    }

    void OnTriggerStay()
    {
        animator.SetBool("isJumping", false);
    }

    void OnTriggerExit()
    {
        animator.SetBool("isJumping", true);
    }

    IEnumerator WhatHappen()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            movement.x += 1;
            tr.position = movement;
        }
       }


}
