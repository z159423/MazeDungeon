using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Test01 : MonoBehaviour {

    public Transform target;     //추적할 타겟 게임 오브젝트의 Transform 변수
    public float dist = 10.0f;     // 카메라와의 일정 거리
    public float height = 5.0f;     // 카메라의 높이 설정
    public float dampRotate = 5.0f;     // 부드러운 회전을 위한 변수
    public float TurnSpeed;
    public float camPos;

    Vector3 V3;

    private Transform tr;
    // Use this for initialization
    void Start()
    {
        //카메라 자신의 transform 컴포넌트를 tr에 할당
        tr = GetComponent<Transform>();
        TurnSpeed = 2f;
        camPos = 2f;
    }

    void Update()
    {
        Vector3 PositionInfo = tr.position - target.position;     // 주인공과 카메라 사이의 백터정보
        PositionInfo = Vector3.Normalize(PositionInfo);     // 화면 확대가 너무 급격히 일어나지 않도록 정규화~

        target.transform.Rotate(0, Input.GetAxis("Horizontal") * TurnSpeed, 0);      //마우스 입력이 감지되면 오브젝트 회전
        transform.RotateAround(target.position, Vector3.right, Input.GetAxis("Mouse Y") * TurnSpeed);     //마우스 Y(Pitch) 움직임 인지하여 화면 회전
        tr.position = tr.position - (PositionInfo * Input.GetAxis("Mouse ScrollWheel") * TurnSpeed);     // 마우스 휠로 화면확대 축고
    }

}
