using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC : MonoBehaviour {

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

    // Use this for initialization
    void Start () {
        tr = GetComponent<Transform>();
    }
	
	// Update is called once per frame
	void Update () {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");

        tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));
    }

    void FixedUpdate()
    {

        Jump();
        Turn();
        Run();
    }

    void Run()
    {
        movement.Set(horizontalMove, 0, verticalMove);
        movement = movement.normalized * speed * Time.deltaTime;        //(speed = 이동속도) * (Time.deltaTime = 프레임 보정시간)  두방향키를 동시에 눌러 대각선 이동이 가능해짐

        rigdbody.MovePosition(transform.position + movement);
    }

    void Turn()
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
}
