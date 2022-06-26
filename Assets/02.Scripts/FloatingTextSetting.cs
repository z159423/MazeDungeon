using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextSetting : MonoBehaviour
{
    public float deleteTime = 3f;
    public TextMesh textmesh;
    private Transform cam;

    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        transform.localPosition = new Vector3(0, 0, 0);
        textmesh.anchor = TextAnchor.MiddleCenter;
        Destroy(gameObject, deleteTime);
    }

    private void Update()
    {
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
    }

}
