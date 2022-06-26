using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(NavmeshBake))]
public class NavMeshBakeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        NavmeshBake navmeshBakeTest = (NavmeshBake)target;

        if (GUILayout.Button("NavMesh 재생성"))
        {
            navmeshBakeTest.BuildNav();
        }
    }
}