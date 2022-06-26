using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Test))]
public class TestEdior : Editor {

    private Test test;

    private bool _initialized;

    public override void OnInspectorGUI()
    {
        _Initialize();

        EditorGUILayout.IntField("Item ID", 0);

        test.test02  = EditorGUILayout.IntField("Item ID 2", 0);
    }

    private void _Initialize()
    {
        if (!_initialized)
        {
            test = (Test)target;
            _initialized = true;
        }
        
    }
}
