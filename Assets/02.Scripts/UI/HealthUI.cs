using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Components;

[RequireComponent(typeof(CharacterStats))]
public class HealthUI : MonoBehaviour {

    public GameObject uiPrefab;
    public Transform target;
    public GameObject debuffIcon;
    public BuffNDebuff buffNDebuff;
    [SerializeField] Color EliteNpcNameColor;
    private Transform cam;
    float visibleTime = 7;

    float LastMadeVisibleTime;
    Transform ui;
    Image healthSlider;
    Image shieldSlider;
    Text HpText;
    public Text LevelText;
    Text NameText;
    NPCStats stats;
    NPC_AI AI;
    GameObject debuff;

    List<GameObject> debuffIconList = new List<GameObject>();

	// Use this for initialization
	void Start () {

        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        stats = GetComponent<NPCStats>();
        AI = GetComponent<NPC_AI>();

        foreach (Canvas c in FindObjectsOfType<Canvas>())
        {

            if (c.renderMode == RenderMode.WorldSpace && c.tag == "EnemyHpBar")
            {
                ui = Instantiate(uiPrefab, c.transform).transform;
                healthSlider = ui.Find("HealthSlider").GetComponent<Image>();
                shieldSlider = ui.Find("ShieldSlider").GetComponent<Image>();
                ui.transform.Find("Name").GetComponentInChildren<LocalizeStringEvent>().StringReference.SetReference("NpcName", stats.NPC_NameKey);
                ui.gameObject.SetActive(false);
                break;
            }
        }

        HpText = ui.GetComponentInChildren<Text>();
        GetComponent<NPCStats>().OnHealthChanged += OnHealthChanged;
        GetComponent<NPCStats>().OnShieldChanged += OnShieldChanged;

        if (target == null)
        {
            GameObject targetObject = new GameObject("HP UI Target");
            targetObject.transform.SetParent(transform);
            targetObject.transform.localPosition = new Vector3(0, 3, 0);

            target = targetObject.transform;
        }
        LevelText = ui.transform.Find("LevelText").GetComponent<Text>();
        LevelText.text = stats.npcLevel.ToString();

        NameText = ui.transform.Find("Name").GetComponent<Text>();
        
        //NameText.text = stats.NPC_Name;

        debuff = ui.transform.Find("buffdebuff").gameObject;

        Invoke("OnChangeNpcLevel", 1f);

        if (AI)
        {
            switch (AI.npcType)
            {
                case NPC_Type.enemy:
                    healthSlider.color = Color.red;
                    break;

                case NPC_Type.friendly:

                    break;

                case NPC_Type.neutrality:

                    break;

                case NPC_Type.Minion:
                    healthSlider.color = Color.magenta;
                    break;

                default:
                    break;

            }
        }

        EliteNpcNameColor = PrefabCollect.instance.eliteMonsterNameColor;

        Invoke("EliteMonster", 1);
       
        if (AI.npcType == NPC_Type.Minion)
            Invoke("enableHP_UI", 0.1f);
        else
        {
            ui.gameObject.SetActive(true);
            LastMadeVisibleTime = visibleTime - 0.1f;
        }

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (ui != null)
        {

            ui.position = target.position;
            ui.forward = cam.forward;

            if (LastMadeVisibleTime > visibleTime)
            {
                ui.gameObject.SetActive(false);
            }

            if (ui.gameObject.activeSelf)
                LastMadeVisibleTime += Time.deltaTime;
        }
    }

    void OnHealthChanged(int maxHealth, int currentHealth)
    {
        if (ui != null)
        {
            if (!UIManager.instance.settingMenu.NpcHpBarDisplayToggle.isOn)
                return;

                ui.gameObject.SetActive(true);
            //LastMadeVisibleTime = Time.time;
            LastMadeVisibleTime = 0;

            float healthPercent = (float)currentHealth / maxHealth;
            healthSlider.fillAmount = healthPercent;

            HpText.text = currentHealth + " / " + maxHealth;
            
            if (currentHealth <= 0)
            {
                ui.gameObject.SetActive(false);
            }
        }
    }

    void OnShieldChanged(int maxShield, int currentShield)
    {
        if (ui != null)
        {
            if (!UIManager.instance.settingMenu.NpcHpBarDisplayToggle.isOn)
                return;

            float shieldPercent = (float)currentShield / maxShield;
            shieldSlider.fillAmount = shieldPercent;

            HpText.text = stats.currentHealth + " / " + stats.maxHealth.GetFinalStatValueAsMultiflyFloat();

            ui.gameObject.SetActive(true);
            LastMadeVisibleTime = 0;
        }
    }
	

    public void GetLvlText(int value)
    {
        LevelText.text = value.ToString();

        if(stats.EliteNPC)
            LevelText.text = LevelText.text + "+";
    }

    public IEnumerator addBNDicon(bufforDebuff bod)
    {
        var ob = Instantiate(debuffIcon, debuff.transform);
        debuffIconList.Add(ob);
        var image = ob.GetComponentInChildren<Image>();
        image.sprite = buffNDebuff.GetIcon(bod.debuff);

        yield return new WaitForSeconds(bod.Duration);

        Destroy(ob);
    }

    public void enableHP_UI()
    {
        if (ui != null)
        {
            if (!UIManager.instance.settingMenu.NpcHpBarDisplayToggle.isOn)
                return;

            if (GetComponent<Wraith>())
            {
                if(GetComponent<Wraith>().appearStartDist < AI.TargetDist)
                {
                    return;
                }
            }
            
            //LastMadeVisibleTime = Time.time;
            if(!ui.gameObject.activeSelf)
            {
                ui.gameObject.SetActive(true);
                LastMadeVisibleTime = 0;
            }

            float healthPercent = (float)stats.currentHealth / stats.maxHealth.GetFinalStatValueAsMultiflyFloat();
            healthSlider.fillAmount = healthPercent;

            HpText.text = stats.currentHealth + " / " + stats.maxHealth.GetFinalStatValueAsMultiflyFloat();

            if (stats.currentHealth <= 0)
            {
                ui.gameObject.SetActive(false);
            }
        }
    }

    private void OnChangeNpcLevel()
    {
        LevelText.text = stats.npcLevel.ToString();
    }

    private void OnDisable()
    {
        //Image[] icons = debuff.GetComponentsInChildren<Image>();

        //foreach (Image image in icons)
        //{
        //   Destroy(image.gameObject);
        //}
        if (ui != null)
        {
            ui.gameObject.SetActive(false);

            foreach (GameObject ob in debuffIconList)
            {
                Destroy(ob);
            }
            debuffIconList.Clear();
        }
    }

    private void OnDestroy()
    {
        //Destroy(ui.gameObject);
    }

    public void EliteMonster()
    {
        if (stats.EliteNPC)
        {
            NameText.color = EliteNpcNameColor;
        }
            
    }

    public string GetName()
    {
        return NameText.text;
    }
}
