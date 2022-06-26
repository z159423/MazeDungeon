using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGenerator_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DungeonGenerator Base = (DungeonGenerator)target;

        if (GUILayout.Button("EndStage"))
        {
            Base.EndStage();
        }
    }
}
