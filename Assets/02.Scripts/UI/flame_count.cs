using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class flame_count : MonoBehaviour
{
    float deltaTime = 0.0f;

    public TextMeshProUGUI TMP;

    public static flame_count instance;

    private void Awake()
    {
        instance = this;

        //Application.targetFrameRate = 60;
    }

    void Update()
    {
        if (Time.timeScale > 0 && TMP.enabled)

            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

        TMP.text = text;
    }

    public void FrameDisplay(bool Select)
    {
        if(TMP != null)
            TMP.enabled = Select;
    }
}