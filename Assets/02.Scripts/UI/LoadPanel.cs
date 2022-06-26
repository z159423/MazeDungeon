using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class LoadPanel : MonoBehaviour
{
    public GameObject button;
    private GameObject contents;

    void CreatLoadButton()
    {
        contents = GameObject.Find("Contents");

        string saveFilePath = Application.dataPath + "/Resources/ItemData.json";
        FileInfo saveFile = new FileInfo(saveFilePath);

        string JsonString = File.ReadAllText(saveFilePath);
        DataMannager.Data loadData = JsonConvert.DeserializeObject<DataMannager.Data>(JsonString);

        if (saveFile.Exists)
        {
            button = Instantiate(button, contents.transform);
            Debug.Log(loadData.playerName);
            button.GetComponentInChildren<Text>().text = (loadData.playerName + " LVL : " + loadData.playerLvl);
        }
    }

    private void Start()
    {
        CreatLoadButton();
    }


    public void PanelOn()
    {
        gameObject.SetActive(true);
    }

    public void PanelOff()
    {
        gameObject.SetActive(false);
    }

}
