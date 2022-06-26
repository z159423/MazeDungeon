using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerStats))]
public class PlayerStat_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PlayerStats stat = (PlayerStats)target;

        if (GUILayout.Button("LevelUP"))
        {
            stat.LevelUp();
        }
    }

}
