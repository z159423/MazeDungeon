using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayStat : MonoBehaviour
{
    public int realPlayTime = 0;
    public bool PlayTimerOn = false;
    public string diedBy = "";
    public int getArtifactAmount = 0;
    public int killedEnemyCount = 0;

    public static PlayStat instance;

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator PlayTimerStart()
    {
        PlayTimerOn = true;

        while (PlayTimerOn)
        {
            yield return new WaitForSeconds(1);
            realPlayTime += 1;
        }
    }

    public void DiedBy(string name)
    {
        diedBy = name;
    }

    public void GetNewArtifact()
    {
        getArtifactAmount++;
    }

    public void EnemyKilled()
    {
        killedEnemyCount++;
    }

    public void PlayerDiedEvent()
    {
        var deathMessage = UIManager.instance.DeathMessage.GetComponentInChildren<DeathMessage>();

        deathMessage.PlayMin = (realPlayTime / 60).ToString();
        deathMessage.PlaySec = (realPlayTime % 60).ToString();

        deathMessage.DiedBy = diedBy;
        deathMessage.ArtifactNumber = getArtifactAmount.ToString();
        deathMessage.KillEnemyNumber = killedEnemyCount.ToString();
    }

    public void GameClearEvent()
    {
        var Ending = UIManager.instance.EndingScene.GetComponentInChildren<GameClearUI>();

        Ending.PlayMin = (realPlayTime / 60).ToString();
        Ending.PlaySec = (realPlayTime % 60).ToString();

        Ending.ArtifactNumber = getArtifactAmount.ToString();
        Ending.KillEnemyNumber = killedEnemyCount.ToString();
    }
}
