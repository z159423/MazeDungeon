using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetting : MonoBehaviour
{

    public GameObject MainCamera;

    private float camera_dist = 0f;
    public float camera_width = -10f;
    public float camera_height = 4f;

    Vector3 dir;

    // Start is called before the first frame update
    void Start()
    {
        camera_dist = Mathf.Sqrt(camera_width * camera_width + camera_height * camera_height);
        dir = new Vector3(0, camera_height, camera_width).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 ray_target = transform.up * camera_height + transform.forward * camera_width;
        RaycastHit hitinfo;
        Physics.Raycast(transform.position, ray_target, out hitinfo, camera_dist);

        if (hitinfo.point != Vector3.zero)//레이케스트 성공시
        {
            //point로 옮긴다.
            MainCamera.transform.position = hitinfo.point;
        }
        else
        {
            //로컬좌표를 0으로 맞춘다. (카메라리그로 옮긴다.)
            MainCamera.transform.localPosition = Vector3.zero;
            //카메라위치까지의 방향벡터 * 카메라 최대거리 로 옮긴다.
            MainCamera.transform.Translate(dir * camera_dist);

        }
    }
}
