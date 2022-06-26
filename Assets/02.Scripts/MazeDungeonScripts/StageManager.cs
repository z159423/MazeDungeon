using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager instance;

    public int stageLevel = 1;

    public List<GameObject> dontDestroyObjects = new List<GameObject>();

    public GameObject Player;

    private void Awake()
    {
        instance = this;

        foreach(GameObject Object in dontDestroyObjects)
        {
            //DontDestroyOnLoad(Object);
        }
    }

    public void ClearStage()
    {
        stageLevel += 1;
        Player.transform.position = new Vector3(0, 0, 0);
    }
}
