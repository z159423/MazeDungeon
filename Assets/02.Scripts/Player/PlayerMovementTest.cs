using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementTest : MonoBehaviour
{
    Rigidbody rigidbody;
    private float horizontalMove;
    private float verticalMove;

    private Vector3 moveDir;

    public float MoveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");   // Horizontal = 왼쪽, 오른쪽 방향키
        verticalMove = Input.GetAxisRaw("Vertical");     // Vertical = 위, 아래 방향키

        moveDir = new Vector3(horizontalMove, 0, verticalMove);
    }

    private void FixedUpdate()
    {
        transform.position += moveDir.normalized * MoveSpeed * Time.fixedDeltaTime;
        //rigidbody.AddForce(moveDir * MoveSpeed * Time.fixedDeltaTime);
    }
}
