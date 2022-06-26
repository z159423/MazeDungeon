using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class escMenu1 : MonoBehaviour
{

    public GameObject toMainMenuConfirmMessage;
    public GameObject exitGameConfirmMessage;
    public GameObject Option;

    public void Resume()
    {
        //gameObject.SetActive(true);
        Time.timeScale = 1;
    }

    public void Pause()
    {
        //gameObject.SetActive(false);
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        toMainMenuConfirmMessage.gameObject.SetActive(false);
        exitGameConfirmMessage.gameObject.SetActive(false);
        Option.gameObject.SetActive(false);
    }

    public void ToMainMenuButton()
    {
        if (Loading.instance)
            Loading.instance.StartLoading();

        SceneChanger.instance.LoadScean_MainMenu();
    }

    public void ClickToMainMenuBtn()
    {
        toMainMenuConfirmMessage.gameObject.SetActive(true);
        exitGameConfirmMessage.gameObject.SetActive(false);
    }

    public void ClickExitBtn()
    {
        toMainMenuConfirmMessage.gameObject.SetActive(false);
        exitGameConfirmMessage.gameObject.SetActive(true);
    }

    public void ClickCancleBtn()
    {
        toMainMenuConfirmMessage.gameObject.SetActive(false);
        exitGameConfirmMessage.gameObject.SetActive(false);
    }
}
