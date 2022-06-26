using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPanel : MonoBehaviour
{
    public CharacterClass thisCharacter;

    public CharacterSelect characterSelect;

    public GameObject LockPanelObject;

    [SerializeField]private GameObject LockPanel;

    public Image SelectedPanel;

    private void Start()
    {
        //CheckClassUnlock();

        if (thisCharacter == CharacterClass.Warrior)
            CharacterSelect();
    }

    public void CheckClassUnlock()
    {
        bool check = CharacterClassManager.instance.classData.checkClassUnlock(thisCharacter);

        Debug.Log(thisCharacter + " 언락되어있는 상태 : " + check);

        if (LockPanel != null)
            LockPanel.SetActive(!check);
    }

    public void CharacterSelect()
    {
        characterSelect.characterSelect(thisCharacter, SelectedPanel, gameObject);
    }
}
