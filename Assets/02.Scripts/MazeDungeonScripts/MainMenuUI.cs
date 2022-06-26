using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject Console;
    public GameObject CharacterSelectMenu;

    public Settings settingMenu;

    public GameObject Credits;
    

    private void Start()
    {
        settingMenu.InitiallizeAudioSetting();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        AudioManager.instance.FindBGM("bgm6");

        var settingData = Settings.RoadSettingData();
        if(settingData != null)
            StartCoroutine(settingMenu.ReadSetting(settingData));
    }

    void Update()
    {
        if (Input.GetButtonDown("Console") && UIManager.instance.settingMenu.AllowConsoleToggle.isOn)
        {
            Console.SetActive(!Console.activeSelf);
        }
    }

    public void StartGameButton()
    {
        CharacterSelectMenu.SetActive(true);
    }

    public void LoadScene(int num)
    {
        SceneChanger.instance.LoadScene(num);
    }

    public void CloseCharacterSelectMenu()
    {
        CharacterSelectMenu.SetActive(false);
    }

    public void PlaySound(string clipName)
    {
        AudioManager.instance.PlaySFX(clipName);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    public void CreditsBtn()
    {
        Credits.SetActive(!Credits.activeSelf);
    }
}
