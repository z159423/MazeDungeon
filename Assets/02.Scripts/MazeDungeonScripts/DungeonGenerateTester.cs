using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonGenerateTester : MonoBehaviour
{
    public bool GenerateTest = false;
    public GameObject GenerateTestPanel;
    public GameObject TesterPrefab;
    public Text SuccessText;
    public Text FailText;
    public Text OverTimeText;
    public Text AverageTimeText;
    public Text GereratedChamberCountCheckText;
    public int successCount = 0;
    public int FailCount = 0;
    public int OverTimeCount = 0;
    public int GereratedChamberCountCheck = 0;

    public Toggle AutoRepeat;
    public Toggle StopOnFail;
    public GameObject Tester;
    public float AverageGenerateTime = 0;

    public void ClosePanel()
    {
        GenerateTestPanel.SetActive(false);
    }
}
