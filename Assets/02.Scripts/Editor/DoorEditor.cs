using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Door))]
public class DoorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Door door = (Door)target;

        if (GUILayout.Button("Open Door"))
        {
            door.OpenDoor();
        }
    }
}