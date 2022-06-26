using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerSkill : MonoBehaviour
{
#pragma warning disable 0649
    //List<skillstruck> skillList = new List<skillstruck>();
    //public Dictionary<Skill, skillstruck> skillList = new Dictionary<Skill, skillstruck>();

    public SkillUI skillUI;
    public Skill testSkill;
    public Skill usingSkillObject;
    public int usingSkillLevel;
    public BoolStat usingSkill;

    [SerializeField]
    private SkillSlot[] skillSlot = new SkillSlot[9];

    public List<SkillData> SkillYouHave = new List<SkillData>();
    public List<Skill> PassiveSkill = new List<Skill>();
    

    private PlayerController01 playerController;
    private PlayerInputAction playerInputActions;
    private List<InputAction> skillHotKeyInputActions = new List<InputAction>();

    private void Awake()
    {
        KeyBindindManager.instance.OnUpdateKeyBindsCallBack += UpdateKeyBind;
    }

    private void Start()
    {
        skillUI = UIManager.instance.skill.GetComponent<SkillUI>();
        playerController = GetComponent<PlayerController01>();

        skillSlot = UIManager.instance.SkillSlotContent.GetComponentsInChildren<SkillSlot>();

        PadCursor.instance.OnChangeControl.AddListener(OnChangeControl);

        InitialKeyBinds();
    }

    private void InitialKeyBinds()
    {
        playerInputActions = new PlayerInputAction();

        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            playerInputActions.LoadBindingOverridesFromJson(rebinds);

        playerInputActions.Player.SkillHotKey1.performed += SkillHotKey1;
        playerInputActions.Player.SkillHotKey2.performed += SkillHotKey2;
        playerInputActions.Player.SkillHotKey3.performed += SkillHotKey3;
        playerInputActions.Player.SkillHotKey4.performed += SkillHotKey4;
        playerInputActions.Player.SkillHotKey5.performed += SkillHotKey5;
        playerInputActions.Player.SkillHotKey6.performed += SkillHotKey6;
        playerInputActions.Player.SkillHotKey7.performed += SkillHotKey7;
        playerInputActions.Player.SkillHotKey8.performed += SkillHotKey8;
        playerInputActions.Player.SkillHotKey9.performed += SkillHotKey9;
        playerInputActions.Player.SkillHotKey0.performed += SkillHotKey0;

        skillHotKeyInputActions.Add(playerInputActions.Player.SkillHotKey1);
        skillHotKeyInputActions.Add(playerInputActions.Player.SkillHotKey2);
        skillHotKeyInputActions.Add(playerInputActions.Player.SkillHotKey3);
        skillHotKeyInputActions.Add(playerInputActions.Player.SkillHotKey4);
        skillHotKeyInputActions.Add(playerInputActions.Player.SkillHotKey5);
        skillHotKeyInputActions.Add(playerInputActions.Player.SkillHotKey6);
        skillHotKeyInputActions.Add(playerInputActions.Player.SkillHotKey7);
        skillHotKeyInputActions.Add(playerInputActions.Player.SkillHotKey8);
        skillHotKeyInputActions.Add(playerInputActions.Player.SkillHotKey9);
        skillHotKeyInputActions.Add(playerInputActions.Player.SkillHotKey0);

        /*for (int i = 0; i < skillSlot.Length; i++)
        {
            KeyBindindManager.instance.DisplayCurrentControllerShortcut(skillSlot[i].bindingKeyCode, skillSlot[i].padHotKeyImage, skillHotKeyInputActions[i]);
        }*/

        OnChangeControl();

        playerInputActions.Player.Enable();
    }

    public void OnChangeControl()
    {
        if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.mouseScheme)
        {
            for (int i = 0; i < skillSlot.Length; i++)
            {
                KeyBindindManager.instance.DisplayShortcutText(skillSlot[i].bindingKeyCode, skillSlot[i].padHotKeyImage, skillHotKeyInputActions[i].bindings[0].effectivePath);
            }
        }
        else if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.gamepadScheme)
        {
            for (int i = 0; i < skillSlot.Length; i++)
            {
                KeyBindindManager.instance.DisplayCurrentControllerShortcut(skillSlot[i].bindingKeyCode, skillSlot[i].padHotKeyImage, skillHotKeyInputActions[i]);
            }
        }
    }

    private void UpdateKeyBind()
    {
        playerInputActions.Disable();
        skillHotKeyInputActions.Clear();
        InitialKeyBinds();
    }

    // Update is called once per frame

    void SkillHotKey1(InputAction.CallbackContext obj)
    {
        if (CheckPlayerHaveSkill(skillSlot[0].skill))
        {
            playerController.SkillUse(skillSlot[0].skill, skillSlot[0]);
        }
        else
        {
            Debug.Log("존재하지 않는 스킬입니다.");
        }
    }

    void SkillHotKey2(InputAction.CallbackContext obj)
    {
        if (CheckPlayerHaveSkill(skillSlot[1].skill))
        {
            playerController.SkillUse(skillSlot[1].skill, skillSlot[1]);
        }
        else
        {
            Debug.Log("존재하지 않는 스킬입니다.");
        }
    }

    void SkillHotKey3(InputAction.CallbackContext obj)
    {
        if (CheckPlayerHaveSkill(skillSlot[2].skill))
        {
            playerController.SkillUse(skillSlot[2].skill, skillSlot[2]);
        }
        else
        {
            Debug.Log("존재하지 않는 스킬입니다.");
        }
    }

    void SkillHotKey4(InputAction.CallbackContext obj)
    {
        if (CheckPlayerHaveSkill(skillSlot[3].skill))
        {
            playerController.SkillUse(skillSlot[3].skill, skillSlot[3]);
        }
        else
        {
            Debug.Log("존재하지 않는 스킬입니다.");
        }
    }

    void SkillHotKey5(InputAction.CallbackContext obj)
    {
        if (CheckPlayerHaveSkill(skillSlot[4].skill))
        {
            playerController.SkillUse(skillSlot[4].skill, skillSlot[4]);
        }
        else
        {
            Debug.Log("존재하지 않는 스킬입니다.");
        }
    }

    void SkillHotKey6(InputAction.CallbackContext obj)
    {
        if (CheckPlayerHaveSkill(skillSlot[5].skill))
        {
            playerController.SkillUse(skillSlot[5].skill, skillSlot[5]);
        }
        else
        {
            Debug.Log("존재하지 않는 스킬입니다.");
        }
    }

    void SkillHotKey7(InputAction.CallbackContext obj)
    {
        if (CheckPlayerHaveSkill(skillSlot[6].skill))
        {
            playerController.SkillUse(skillSlot[6].skill, skillSlot[6]);
        }
        else
        {
            Debug.Log("존재하지 않는 스킬입니다.");
        }
    }

    void SkillHotKey8(InputAction.CallbackContext obj)
    {
        if (CheckPlayerHaveSkill(skillSlot[7].skill))
        {
            playerController.SkillUse(skillSlot[7].skill, skillSlot[7]);
        }
        else
        {
            Debug.Log("존재하지 않는 스킬입니다.");
        }
    }

    void SkillHotKey9(InputAction.CallbackContext obj)
    {
        if (CheckPlayerHaveSkill(skillSlot[8].skill))
        {
            playerController.SkillUse(skillSlot[8].skill, skillSlot[8]);
        }
        else
        {
            Debug.Log("존재하지 않는 스킬입니다.");
        }
    }

    void SkillHotKey0(InputAction.CallbackContext obj)
    {
        if (CheckPlayerHaveSkill(skillSlot[9].skill))
        {
            playerController.SkillUse(skillSlot[9].skill, skillSlot[9]);
        }
        else
        {
            Debug.Log("존재하지 않는 스킬입니다.");
        }
    }

    /*void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (skillSlot[0].skill != null)
            {
                if (CheckPlayerHaveSkill(skillSlot[0].skill))
                {
                    playerController.SkillUse(skillSlot[0].skill, skillSlot[0]);
                }
                else
                {
                    Debug.Log("존재하지 않는 스킬입니다.");
                }
            }
            else
            {
                print("스킬 슬롯에 스킬이 존재하지 않습니다.");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (skillSlot[1].skill != null)
            {
                if (CheckPlayerHaveSkill(skillSlot[1].skill))
                {
                    playerController.SkillUse(skillSlot[1].skill, skillSlot[1]);
                }
                else
                {
                    Debug.Log("존재하지 않는 스킬입니다.");
                }
            }
            else
            {
                print("스킬 슬롯에 스킬이 존재하지 않습니다.");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (skillSlot[2].skill != null)
            {
                if (CheckPlayerHaveSkill(skillSlot[2].skill))
                {
                    playerController.SkillUse(skillSlot[2].skill, skillSlot[2]);
                }
                else
                {
                    Debug.Log("존재하지 않는 스킬입니다.");
                }
            }
            else
            {
                print("스킬 슬롯에 스킬이 존재하지 않습니다.");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (skillSlot[3].skill != null)
            {
                if (CheckPlayerHaveSkill(skillSlot[3].skill))
                {
                    playerController.SkillUse(skillSlot[3].skill, skillSlot[3]);
                }
                else
                {
                    Debug.Log("존재하지 않는 스킬입니다.");
                }
            }
            else
            {
                print("스킬 슬롯에 스킬이 존재하지 않습니다.");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (skillSlot[4].skill != null)
            {
                if (CheckPlayerHaveSkill(skillSlot[4].skill))
                {
                    playerController.SkillUse(skillSlot[4].skill, skillSlot[4]);
                }
                else
                {
                    Debug.Log("존재하지 않는 스킬입니다.");
                }
            }
            else
            {
                print("스킬 슬롯에 스킬이 존재하지 않습니다.");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (skillSlot[5].skill != null)
            {
                if (CheckPlayerHaveSkill(skillSlot[5].skill))
                {
                    playerController.SkillUse(skillSlot[5].skill, skillSlot[5]);
                }
                else
                {
                    Debug.Log("존재하지 않는 스킬입니다.");
                }
            }
            else
            {
                print("스킬 슬롯에 스킬이 존재하지 않습니다.");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            if (skillSlot[6].skill != null)
            {
                if (CheckPlayerHaveSkill(skillSlot[6].skill))
                {
                    playerController.SkillUse(skillSlot[6].skill, skillSlot[6]);
                }
                else
                {
                    Debug.Log("존재하지 않는 스킬입니다.");
                }
            }
            else
            {
                print("스킬 슬롯에 스킬이 존재하지 않습니다.");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            if (skillSlot[7].skill != null)
            {
                if (CheckPlayerHaveSkill(skillSlot[7].skill))
                {
                    playerController.SkillUse(skillSlot[7].skill, skillSlot[7]);
                }
                else
                {
                    Debug.Log("존재하지 않는 스킬입니다.");
                }
            }
            else
            {
                print("스킬 슬롯에 스킬이 존재하지 않습니다.");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            if (skillSlot[8].skill != null)
            {
                if (CheckPlayerHaveSkill(skillSlot[8].skill))
                {
                    playerController.SkillUse(skillSlot[8].skill, skillSlot[8]);
                }
                else
                {
                    Debug.Log("존재하지 않는 스킬입니다.");
                }
            }
            else
            {
                print("스킬 슬롯에 스킬이 존재하지 않습니다.");
            }
        }
    }*/

    /*
    public bool AddSkill(Skill skill)
    {
        bool succees = checkAlreadyLearn(skill);

        if (succees == true)
        {
            var Slot = skillUI.AddSkillSlot();

            var skillImage = Slot.transform.Find("Image").GetComponent<Image>();
            skillImage.sprite = skill.Sprite;
            Slot.GetComponentInChildren<DragSkill>().skill = skill;

            skillstruck temp = new skillstruck();
            temp.skill = skill;
            temp.skillLevel = 1;

            skillList.Add(skill,temp);

            if(!skill.PassiveSkill)
            {
                foreach(SkillSlot slot in skillSlot)
                {
                    if(slot.skill == null)
                    {
                        slot.ApplySkillThisSlot(skill);
                        break;
                    }
                }
            }

            return true;
        }
        else if (succees == false)
        {
            return false;
        }
        return false;

    }*/

    public void AddPassiveSkill(Skill skill)
    {
        if (!PassiveSkill.Contains(skill))
        {
            PassiveSkill.Add(skill);
        }
        else
        {

        }
    }

    public void UseSkill(Skill skill)
    {
        usingSkillObject = skill;
    }

    public void ClearUsingSkill()
    {
        usingSkillObject = null;
    }
    /*
    bool checkAlreadyLearn(Skill skill)
    {
        /*for (int i = 0; i < skillList.Count; i++)
        {
            if (skillList[i].skill == skill)
            {
                return false;
            } else
            {
                return true;
            }
                
        }
        if(skillList.ContainsKey(skill))
        {
            return false;
        } else
        {
            return true;
        }
    }
    /*
    public skillstruck GetPlayerSkillStat(Skill key)
    {
        skillstruck skillstruct;
        skillList.TryGetValue(key, out skillstruct);
        return skillstruct;
    }*/

    public void GetNewSkill(Skill skill, bool defaultSkill = false)
    {
        SkillData skillData = new SkillData();
        skillData.skill = skill;
        SkillYouHave.Add(skillData);

        if (!skill.PassiveSkill)
        {
            foreach (SkillSlot slot in skillSlot)
            {
                if (slot.skill == null)
                {
                    slot.ApplySkillThisSlot(skill, false);
                    break;
                }
            }
        }
        GetSkillLevelUpEffect(skill);

        if (skill.maxLevel == GetSkillLevel(skill))
            SkillToolTip.instance.HideToolTip();
        else if(!defaultSkill)
        {
            SkillToolTip.instance.HideToolTip();
            SkillToolTip.instance.GetSkillInfo(skill, GetSkillLevel(skill) + 1);
            SkillToolTip.instance.ShowToolTip(true);
        }

        if (GetComponent<PlayerStats>().skillLvlPoint == 0)
            SkillToolTip.instance.HideToolTip();
    }

    public void SkillLevelUp(Skill skill)
    {
        foreach(SkillData skillData in SkillYouHave)
        {
            if (skillData.skill == skill)
            {
                skillData.SkillLevelUp();

                foreach (SkillSlot slot in skillSlot)
                {
                    if (slot.skill == skill)
                    {
                        slot.ChangeSkillMaxCoolTime();
                        break;
                    }
                }

                if (skill.maxLevel == GetSkillLevel(skill))
                    SkillToolTip.instance.HideToolTip();
                else
                {
                    SkillToolTip.instance.HideToolTip();
                    SkillToolTip.instance.GetSkillInfo(skill, GetSkillLevel(skill) + 1);
                    SkillToolTip.instance.ShowToolTip(true);
                }

                if(GetComponent<PlayerStats>().skillLvlPoint == 0)
                    SkillToolTip.instance.HideToolTip();

                GetSkillLevelUpEffect(skill);
            }
        }
    }

    public void GetSkillLevelUpEffect(Skill skill)
    {
        if(skill == PrefabCollect.instance.QuickShot)
        {
            float percent = PrefabCollect.instance.QuickShot.skillLeveling[GetSkillLevel(PrefabCollect.instance.QuickShot)].value3;
            GetComponent<PlayerStats>().AttackSpeed.AddPercentModifier(percent);
        }

        if (skill == PrefabCollect.instance.ShieldReflect)
        {
            int value = PrefabCollect.instance.ShieldReflect.skillLeveling[GetSkillLevel(PrefabCollect.instance.ShieldReflect)].value2;
            GetComponent<PlayerStats>().playerMaxShieldPower.AddIntModifier(value);
        }
    }

    public int GetSkillLevel(Skill skill)
    {
        foreach (SkillData skillData in SkillYouHave)
        {
            if (skillData.skill == skill)
            {
                return skillData.skillLevel;
            }

        }
        return 0;
    }

    public bool CheckPlayerHaveSkill(Skill skill)
    {
        foreach(SkillData skillData in SkillYouHave)
        {
            if (skillData.skill == skill)
                return true;
        }

        return false;
    }

    [System.Serializable]
    public class SkillData
    {
        public int skillLevel = 0;
        public Skill skill;

        public void SkillLevelUp()
        {
            skillLevel++;
        }
    }

}

public struct skillstruck
{
    public Skill skill;
    public float skillCoolTime;
    public int skillLevel;

}
