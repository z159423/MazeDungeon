﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapDisplay))]
public class MapDisplayEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapDisplay mapDis = (MapDisplay)target;

        if(DrawDefaultInspector())
        {
            if(mapDis.autoUpdate)
            {
                mapDis.DrawMesh();
            }
        }

        if(GUILayout.Button("Generate"))
        {
            mapDis.DrawMesh();
        }
    }
}
