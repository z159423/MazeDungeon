using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class CharacterSelect : MonoBehaviour
{
    public CharacterClass selectedCharacter;

    public LocalizeStringEvent CharacterNameLocalize;

    public Text CharacterNameText;

    public Image[] Panels;

    public CharacterSelectPanel[] characterSelects;

    public GameObject warriorDetail;
    public GameObject rougeDetail;
    public GameObject WizardDetail;
    public GameObject ArcherDetail;

    [Space]

    public string killedSkeletonArcherCount;

    [Space]
    
    public GameObject UnlockConditionPanel;
    public bool pointerEnter = false;
    public RectTransform canvasRectTransform;
    public Camera UICamera;
    public LocalizeStringEvent unlockDetail;
    private Vector3 unlockPanelPotition;
    private Vector2 outVector2;

    public static CharacterSelect instance;

    private void Awake()
    {
        instance = this;

        characterSelects = GetComponentsInChildren<CharacterSelectPanel>();
    }

    private void OnEnable()
    {
        CharacterClassManager.instance.LoadCharacterClassUnlock();

        foreach(CharacterSelectPanel characterSelect in characterSelects)
        {
            characterSelect.CheckClassUnlock();
        }
    }

    private void Update()
    {
        if(pointerEnter)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, PadCursor.instance.GetCurrentCursorPosition(), UICamera, out outVector2);
            unlockPanelPotition = outVector2;
            UnlockConditionPanel.transform.localPosition = unlockPanelPotition + new Vector3(0, 0, -200);
        }
        
    }


    public void characterSelect(CharacterClass character, Image selectedPanel, GameObject point)
    {
        selectedCharacter = character;

        CharacterNameLocalize.StringReference.SetReference("UI", character.ToString());

        foreach (Image panel in Panels)
        {
            panel.enabled = true;
        }

        foreach(Animator animator in GetComponentsInChildren<Animator>())
        {
            animator.SetBool("Warrior", false);
            animator.SetBool("Rogue", false);
            animator.SetBool("Archer", false);
            animator.SetBool("Mage", false);
        }

        switch(character)
        {
            case CharacterClass.Adventurer:
                break;

            case CharacterClass.Warrior:
                point.GetComponentInChildren<Animator>().SetBool("Warrior", true);
                HideAllDetail();
                warriorDetail.gameObject.SetActive(true);
                break;

            case CharacterClass.Rogue:
                point.GetComponentInChildren<Animator>().SetBool("Rogue", true);
                HideAllDetail();
                rougeDetail.gameObject.SetActive(true);
                break;

            case CharacterClass.Archer:
                point.GetComponentInChildren<Animator>().SetBool("Archer", true);
                HideAllDetail();
                WizardDetail.gameObject.SetActive(true);
                break;

            case CharacterClass.Wizard:
                point.GetComponentInChildren<Animator>().SetBool("Mage", true);
                HideAllDetail();
                ArcherDetail.gameObject.SetActive(true);
                break;

            default:

                break;
        }

        selectedPanel.enabled = false;
    }

    private void HideAllDetail()
    {
        warriorDetail.gameObject.SetActive(false);
        rougeDetail.gameObject.SetActive(false);
        WizardDetail.gameObject.SetActive(false);
        ArcherDetail.gameObject.SetActive(false);
    }

    public void GameStart()
    {
        SceneChanger.instance.characterclass = selectedCharacter;
     
    }

    public void ShowUnlockPanel(CharacterClass Class)
    {
        ChangePanelInfo(Class);
        UnlockConditionPanel.SetActive(true);
        pointerEnter = true;
    }

    public void HideUnlockPanel()
    {
        pointerEnter = false;
        UnlockConditionPanel.SetActive(false);
    }

    public void ChangePanelInfo(CharacterClass Class)
    {
        switch(Class)
        {
            case CharacterClass.Archer:
                killedSkeletonArcherCount = CharacterClassManager.GetSkeletonArcherKillCount().ToString();
                unlockDetail.StringReference.SetReference("UI", "Unlock_Archer");
                break;

            case CharacterClass.Rogue:
                unlockDetail.StringReference.SetReference("UI", "Unlock_Rogue");
                break;

            case CharacterClass.Wizard:
                unlockDetail.StringReference.SetReference("UI", "Unlock_Mage");
                break;
        }
    }
}

public enum CharacterClass { Adventurer, Warrior, Rogue, Wizard, Archer, Alchemist, Necromancer, Tinker }
