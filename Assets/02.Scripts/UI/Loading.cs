using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public static Loading instance;

    public GameObject LoadingIcon;
    public Text LoadingText;

    public bool loading = true;

    int dotCount = 0;

    private void Start()
    {
        instance = this;
    }
    void Update()
    {
        LoadingIcon.transform.Rotate(new Vector3(0, 0, 1) * (Time.deltaTime * 40));
    }

    void TextChanging()
    {
        if (dotCount > 3)
            dotCount = 0;
        
        string text = "Loading";
        for(int i = 0; i > dotCount; i++)
        {
            text += ".";
        }
        LoadingText.text = text;
        dotCount++;
    }

    public void StartLoading()
    {
        gameObject.SetActive(true);
    }

    public void EndLoading()
    {
        gameObject.SetActive(false);
    }
}
