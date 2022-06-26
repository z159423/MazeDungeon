using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NPCStats))]
public class NPCStatsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        NPCStats stats = (NPCStats)target;

        if(GUILayout.Button("Die"))
        {
            stats.KillThisNPC();
        }
    }
}
