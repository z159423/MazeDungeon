using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GetAngle
{

    public static float GetAngleBetween3DVector(Vector3 vec1, Vector3 vec2)
    {
        float theta = Vector3.Dot(vec1, vec2) / (vec1.magnitude * vec2.magnitude);
        Vector3 dirAngle = Vector3.Cross(vec1, vec2);
        float angle = Mathf.Acos(theta) * Mathf.Rad2Deg;
        if (dirAngle.z < 0.0f) angle = 360 - angle;
        Debug.Log("사잇각 : " + angle);
        return angle;
    }

}
