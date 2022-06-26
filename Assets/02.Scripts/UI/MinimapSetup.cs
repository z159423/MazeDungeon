using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapSetup : MonoBehaviour
{
    public Camera MinimapCamera;
    public Text SizeText;

    public float MaxMinimapSize = 300;
    public float MinMinimapSize = 100;
    public float SizeValue = 50;

    private void Start()
    {
        SizeText.text = MinimapCamera.orthographicSize.ToString();
    }

    public void MinimapSizePlus()
    {
        if((MinimapCamera.orthographicSize + SizeValue) > MaxMinimapSize)
        {
            MinimapCamera.orthographicSize = MinMinimapSize;
        }
        else
        {
            MinimapCamera.orthographicSize += SizeValue;
        }

        SizeText.text = MinimapCamera.orthographicSize.ToString();
    }

    public void MinimapSizeMinus()
    {
        if ((MinimapCamera.orthographicSize - SizeValue) < MinMinimapSize)
        {
            MinimapCamera.orthographicSize = MaxMinimapSize;
        }
        else
        {
            MinimapCamera.orthographicSize -= SizeValue;
        }

        SizeText.text = MinimapCamera.orthographicSize.ToString();
    }
}
