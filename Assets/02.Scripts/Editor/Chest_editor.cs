using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OpenChest))]
public class Chest_editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        OpenChest Base = (OpenChest)target;

        if (GUILayout.Button("Open"))
        {
            Base.ChestOpen();
        }
    }
}
