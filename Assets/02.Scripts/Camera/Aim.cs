using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    public Transform rayTarget;
    public LayerMask mask;
    private RaycastHit Hit;

    void LateUpdate()
    {
        /*Camera mainCamera = GetComponentInChildren<Camera>();

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.white);

        Physics.Raycast(ray, out Hit, mask);

        rayTarget.position = Hit.point;*/
    }
}
