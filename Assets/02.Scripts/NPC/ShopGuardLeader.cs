using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopGuardLeader : MonoBehaviour
{
    public TextBubble guardLeaderTextBubble;

    [Space]

    public bool FindThiefText = false;

    public void GenerateFindThiefTextBubble()
    {
        if (FindThiefText)
            return;

        StopCoroutine(guardLeaderTextBubble.SpawnTextBubble());
        StartCoroutine(guardLeaderTextBubble.SpawnTextBubble());
        int randomValue = Random.Range(1, 3);
        guardLeaderTextBubble.ChangeText("ShopGuard-FindThief"+ randomValue);
        FindThiefText = true;

    }
}
