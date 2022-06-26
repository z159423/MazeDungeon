using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour {

    public Transform target;
    public Vector3 cameraposition;
    [Space]
    public float smoothSpeed = 0.125f;
    public float MoveSpeed;     // 플레이어를 따라오는 카메라 맨의 스피드.

    void Start()
    {
        // Player라는 태그를 가진 오브젝트의 transform을 가져온다.
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // 플레이어를 따라다님.
    private void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + cameraposition;
        Vector3 smoothedPositon = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPositon;
        //transform.position += ((target.position - Pos) * MoveSpeed) + cameraposition;
    }


}
