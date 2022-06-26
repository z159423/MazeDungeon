using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger instance;

    public CharacterClass characterclass;
    public GameObject characterSelectMenu;

    public delegate void StageEnd();
    public StageEnd OnStageEnd;

    private bool Initialized = false;

    private void Awake()
    {
        //roomTemplates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        OnStageEnd += EndStage;

        if(!Initialized)
        {
            DontDestroyOnLoad(gameObject);
            Initialized = true;
        }
        
    }

    public void LoadScean_NewStart()
    {
        SceneManager.LoadScene("MazeDungeonNextStage");
    }

    public void LoadScean_Title()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("StartScean");
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    public void CharacterSelectMenuOn()
    {
        characterSelectMenu.SetActive(true);
    }

    public void CharacterSelectMenuOff()
    {
        characterSelectMenu.SetActive(false);
    }

    public void LoadScean_MainMenu()
    {
        SceneManager.LoadScene("StartScean");
    }

    public void EndStage()
    {
        Debug.Log("스테이지 클리어 : 스테이지 초기화작업");
        //LoadScean_NewStart();

        MazeDungeonNpcSpawner.instance.ClearStage();
        //RoomTemplates.instance.ClearStage();
        //RoomTemplates.instance.SpawnNewEntryRoom();
        StageManager.instance.ClearStage();
        //roomTemplates.FirstRoomCount = roomTemplates.FirstRoomCount + 2;

        UIManager.instance.StageClear.SetActive(false);
    }

    public void OnEndStage()
    {
        OnStageEnd.Invoke();
    }

    public void Quit()
    {
        Application.Quit();
    }


    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operaion = SceneManager.LoadSceneAsync(sceneIndex);

        while(!operaion.isDone)
        {
            float progress = Mathf.Clamp01(operaion.progress / .9f);
            //Debug.Log(progress);

            yield return null;
        }
    }
}
