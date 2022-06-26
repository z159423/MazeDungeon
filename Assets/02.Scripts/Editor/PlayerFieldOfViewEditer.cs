using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerFieldOfView))]
public class PlayerFieldOfViewEditer : Editor
{

    private void OnSceneGUI()
    {
        PlayerFieldOfView pfov = (PlayerFieldOfView)target;

        Handles.color = Color.white;
        Handles.DrawWireArc(pfov.transform.position, Vector3.up, Vector3.forward, 360, pfov.viewRadius);

        foreach(Collider coll in pfov.TargetsInViewRadius)
        {
            Handles.DrawLine(pfov.transform.position, coll.transform.position);
        }

        Handles.color = Color.red;
        foreach (Transform coll in pfov.visibleTargets)
        {
            Handles.DrawLine(pfov.transform.position, coll.transform.position);
        }
    }
}
