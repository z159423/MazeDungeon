using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragSkill : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerExitHandler, IPointerEnterHandler
{
#pragma warning disable 0649
    public GameObject skillDrag;
    public Image skillImage;
    public Skill skill;
    [SerializeField] Image Cover;
    [SerializeField] Button button;
    public PlayerSkill playerSkill;
    public bool defaultSkill = false;
    public Button levelUpButton;

    [Space]
    public bool notNeedRequireSkill = false;
    public Skill[] RequireSkill;

    private RectTransform canvasRectTransform;
    private Vector2 pointerOffset;

    void Start()
    {
        skillDrag = UIManager.instance.SkillDrag;
        skillImage = skillDrag.transform.Find("SkillImage").GetComponent<Image>();
        playerSkill = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerSkill>();

        Canvas canvas = GetComponentInParent<Canvas>();

        if (canvas != null)
        {
            canvasRectTransform = canvas.transform as RectTransform;          //instantiated
        }

        //SkillParent.instance.OnSkillPointChange += SkillLevelUpButtonChecking;

        //SkillParent.instance.dragSkills.Add(this);

        /*if(playerSkill.GetSkillLevel(skill) > 0)
        {
            SkillLevelUp(true);
        }*/

        /*if (defaultSkill)
            playerSkill.GetNewSkill(skill, defaultSkill = true);*/

        CheckSkillInfoCallBack();
        SkillLevelUpButtonChecking();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData data)
    {
        if (playerSkill.CheckPlayerHaveSkill(skill) && !skill.PassiveSkill)
        {
            Debug.Log(data.pointerDrag);
            skillImage.sprite = data.pointerDrag.transform.parent.Find("Image").GetComponent<Image>().sprite;
            skillImage.raycastTarget = false;
        }
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        SkillToolTip.instance.GetSkillInfo(skill, playerSkill.GetSkillLevel(skill));
        SkillToolTip.instance.ShowToolTip(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        SkillToolTip.instance.HideToolTip();

    }

    public void OnDrag(PointerEventData data)
    {
        //Debug.Log(data);
        Vector2 localPointerPosition;
        if (playerSkill.CheckPlayerHaveSkill(skill) && !skill.PassiveSkill)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, PadCursor.instance.GetCurrentCursorPosition(), data.pressEventCamera, out localPointerPosition))
            {
                skillDrag.transform.localPosition = localPointerPosition - pointerOffset;        //아이템 드래그시 아이템이 마우스 위치로 이동하게 함                                                                                      
            }
        }
    }

    public void OnEndDrag(PointerEventData data)
    {
        if (playerSkill.CheckPlayerHaveSkill(skill) && !skill.PassiveSkill)
        {
            skillDrag.transform.localPosition = skillDrag.transform.localPosition * 1000;
            skillImage.raycastTarget = true;

            if (data.pointerEnter != null)
            {
                if (data.pointerEnter.tag == "SkillQuickSlot")
                {
                    data.pointerEnter.GetComponentInParent<SkillSlot>().ApplySkillThisSlot(skill, true);

                    /*SkillSlot[] slots = data.pointerEnter.transform.parent.parent.GetComponentsInChildren<SkillSlot>();
                    foreach (SkillSlot slot1 in slots)       //다른 슬롯에 같은 스킬이 있는지 확인
                    {
                        if (slot1.skill == skill)
                        {
                            slot1.initializationSlot();
                        }
                    }

                    Image iamge = data.pointerEnter.transform.parent.Find("SkillIcon").GetComponent<Image>();
                    iamge.sprite = skillImage.sprite;
                    SkillSlot slot = data.pointerEnter.transform.GetComponentInParent<SkillSlot>();
                    slot.skill = skill;
                    slot.slider.maxValue = skill.coolTime;
                    slot.slider.value = slot.slider.maxValue;
                    //var skillslot = data.pointerEnter.transform.GetComponentInParent<SkillSlot>();
                    //skillslot.slider.maxValue = skill.coolTime;*/
                }
            }
            else
            {
                Debug.Log("잘못된 선택입니다.");
            }
        }

    }

    public void SkillLevelUp(bool defaultSkill)
    {
        foreach (PlayerManager.PlayerInfo playerInfo in PlayerManager.instance.Players)
        {
            if (playerInfo.Skillobject == SkillUI.instance.SkillParent)
            {

                if (playerInfo.playerobject.GetComponentInChildren<PlayerStats>().skillLvlPoint > 0)
                {
                    foreach(PlayerSkill.SkillData skillData in playerSkill.SkillYouHave)
                    {
                        if(skillData.skill == skill)
                        {
                            playerInfo.playerobject.GetComponentInChildren<PlayerStats>().SkillLvlPointUse(1);
                            playerSkill.SkillLevelUp(skill);
                            CheckSkillInfoCallBack();
                            UIManager.instance.skillParent.OnSkillPointChange.Invoke();
                            return;
                        }
                        
                    }
                    playerInfo.playerobject.GetComponentInChildren<PlayerStats>().SkillLvlPointUse(1);
                    playerSkill.GetNewSkill(skill);
                    CheckSkillInfoCallBack();
                    UIManager.instance.skillParent.OnSkillPointChange.Invoke();
                    return;

                    

                    /*if (playerSkill.GetSkillLevel(skill) == 0)
                    {
                        playerSkill.GetNewSkill(skill);
                        //playerInfo.playerobject.GetComponentInChildren<PlayerSkill>().AddSkill(skill);

                        print(skill.name + " 스킬 등록됨");
                    }
                    else
                    {
                        playerSkill.SkillLevelUp(skill);
                        //skillstruck skillstruck = playerInfo.playerobject.GetComponentInChildren<PlayerSkill>().skillList[skill];
                        //skillstruck.skillLevel++;
                        //playerInfo.playerobject.GetComponentInChildren<PlayerSkill>().skillList[skill] = skillstruck;
                    }
                    playerInfo.playerobject.GetComponentInChildren<PlayerStats>().SkillLvlPointUse(1);*/
                }
                else
                {
                    print("스킬 포인트가 부족합니다.");
                }
            }
        }
    }

    private void CheckSkillInfoCallBack()
    {
        if (playerSkill.CheckPlayerHaveSkill(skill))
        {
            Cover.enabled = false;
        }
    }

    private void SkillLevelUpButtonChecking()
    {
        foreach (PlayerManager.PlayerInfo playerInfo in PlayerManager.instance.Players)
        {
            if (playerInfo.Skillobject == SkillUI.instance.SkillParent)
            {
                bool allHaveRequireSkill = notNeedRequireSkill;

                List<bool> checkPlayerSkills = new List<bool>();

                foreach(Skill requireSkill in RequireSkill)
                {
                    checkPlayerSkills.Add(playerSkill.CheckPlayerHaveSkill(requireSkill));
                }

                if (checkPlayerSkills.Count > 0 && checkPlayerSkills.Contains(true) && !checkPlayerSkills.Contains(false))
                    allHaveRequireSkill = true;

                if (playerInfo.playerobject.GetComponentInChildren<PlayerStats>().skillLvlPoint > 0 && (skill.maxLevel > playerSkill.GetSkillLevel(skill) || !playerSkill.CheckPlayerHaveSkill(skill))
                    && allHaveRequireSkill)
                {
                    button.gameObject.SetActive(true);

                }
                else
                {
                    button.gameObject.SetActive(false);
                }
            }
        }
    }


    public void InitThisSkill(SkillParent skillParent)
    {
        playerSkill = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerSkill>();
        skillParent.OnSkillPointChange += SkillLevelUpButtonChecking;

        if (defaultSkill)
            playerSkill.GetNewSkill(skill, defaultSkill = true);
    }
}
