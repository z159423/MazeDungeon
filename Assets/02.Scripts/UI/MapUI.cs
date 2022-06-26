using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUI : MonoBehaviour
{

    public GameObject MapObject;

    public static string stage;

    public void TurnOffUi()
    {
        MapObject.SetActive(!MapObject.activeSelf);
    }

    public static void ChangeDungeonCurrentStageNumber(int number)
    {
        stage = (number+1).ToString();
    }
}
