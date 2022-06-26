using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingsChamber : MonoBehaviour
{
    public GameObject Boss;

    public void EnableBoss()
    {
        Boss.SetActive(true);
    }
}
