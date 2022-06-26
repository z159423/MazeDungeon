using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HeadLook))]
public class HeadLookEditor : Editor
{
    private void OnSceneGUI()
    {
        HeadLook headLook = (HeadLook)target;

        Handles.color = Color.white;

        //Handles.DrawWireArc(headLook.transform.position, Vector3.up, Vector3.forward, headLook.viewAngle, headLook.viewRadius);

        Vector3 viewAngelA = headLook.dirFromAngle(-headLook.viewAngle / 2, false);
        Vector3 viewAngelB = headLook.dirFromAngle(headLook.viewAngle / 2, false);

        Handles.DrawLine(headLook.head.position, headLook.transform.position + viewAngelA * headLook.viewRadius);
        Handles.DrawLine(headLook.head.position, headLook.transform.position + viewAngelB * headLook.viewRadius);


        if (headLook.target != null)
        {
            Handles.DrawLine(headLook.head.position,headLook.target.position);
        }
    }
}
