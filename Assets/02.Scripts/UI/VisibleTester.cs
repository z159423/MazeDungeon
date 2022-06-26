using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleTester : MonoBehaviour
{
    public float cutoff = 45f;

    public static VisibleTester instance;

    private void Awake()
    {
        instance = this;
    }

    public bool VisibleTest(Vector3 inputPoint)
    {
        float cosAngle = Vector3.Dot((inputPoint - this.transform.position).normalized,
            this.transform.forward);
        float angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;
        return angle < cutoff;
    }
}
