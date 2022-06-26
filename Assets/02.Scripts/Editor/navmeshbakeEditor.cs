using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NavmeshBake))]
public class navmeshbakeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        NavmeshBake baker = (NavmeshBake)target;
        if (GUILayout.Button("BakeNavMesh"))
        {
            baker.BuildNav();
        }
    }
}
