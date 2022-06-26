using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearUI : MonoBehaviour
{
    public TypingEffect[] AllTypingEffect;

    public string PlayMin = "";
    public string PlaySec = "";
    public string ArtifactNumber = "";
    public string KillEnemyNumber = "";

    void Start()
    {
        AllTypingEffect = GetComponentsInChildren<TypingEffect>();
    }

    public void StartEnding()
    {
        StartCoroutine(startEndingScene());
    }

    public IEnumerator startEndingScene()
    {
        for (int i = 0; i < AllTypingEffect.Length; i++)
        {
            AllTypingEffect[i].StartTyping();
            yield return new WaitForSeconds(2f);
        }
    }
}
