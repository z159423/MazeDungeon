using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    public static ToolTip instance;

    public Text tooltipText;
    public Text itemLevel;
    public Text Damage;
    public Text Defence;
    public Text itemType;
    public Text itemComment;
    public Text itemLimit;
    public GameObject TargetObject;

    private RectTransform canvasRectTransform;
    private GameObject UI;
    private Vector3 toolTipPotition;
    private Vector2 outVector2;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private void Awake()
    {
        instance = this;

        UI = GameObject.FindGameObjectWithTag("UI");
        canvasRectTransform = UI.GetComponent<RectTransform>();
        /*
        tooltipText = GameObject.Find("ItemNameText").GetComponent<Text>();
        Damage = GameObject.Find("DamageText").GetComponent<Text>();
        Defence = GameObject.Find("DefenceText").GetComponent<Text>();
        Type = GameObject.Find("TypeText").GetComponent<Text>();
        */
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        meshFilter = GetComponentInChildren<MeshFilter>();
    }

    void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, PadCursor.instance.GetCurrentCursorPosition(), GameObject.FindGameObjectWithTag("UI_Camera").GetComponent<Camera>(), out outVector2);
        toolTipPotition = outVector2;
        transform.localPosition = toolTipPotition + new Vector3(0, 0, -50);
        meshRenderer.transform.Rotate(Vector3.down * 2);

    }

    public void GetItem(Item item)
    {
        GetMesh(item.skinedMesh);

        ItemNameColor(item.itemQuality);

        itemLevel.text = "ItemLevel " + item.itemTier.ToString();

        if (item.itemtype == ItemType.Chest || item.itemtype == ItemType.Head || item.itemtype == ItemType.Legs || item.itemtype == ItemType.SecondaryWeapon)
        {
            ResetInfo();
            Defence.text = "방어력 " + item.armorModifier;
            itemType.text = item.itemtype.ToString();
        }
        else if (item.itemtype == ItemType.Weapon)
        {
            ResetInfo();
            Damage.text = "공격력 " + item.minDamageModifier + " ~ " + item.maxDamageModifier;
            itemType.text = item.itemtype.ToString();
        }
        else if (item.itemtype == ItemType.consumable)
        {
            ResetInfo();
            

            if (item.consumableType == ConsumableType.Potion)
            {
                itemType.text = item.itemtype.ToString();
                itemComment.text = "Hp를 " + item.effectiveDose_Hp + " 만큼 회복합니다. \n " + "Mp를 " + item.effectiveDose_Mp + " 만큼 회복합니다. ";
                itemType.text = item.itemtype.ToString();
                itemComment.text = "Hp를 " + item.effectiveDose_Hp + " 만큼 회복합니다. \n " + "Mp를 " + item.effectiveDose_Mp + " 만큼 회복합니다. ";
            }
            else if (item.consumableType == ConsumableType.SkillBook)
            {
                itemType.text = "Skill Book";
                itemLevel.text = "";
                itemComment.text = "오른쪽 클릭으로 스킬을 배웁니다.";
            }
        }

        if (item.itemtype == ItemType.Weapon || item.itemtype == ItemType.SecondaryWeapon)
        {
            itemLimit.text = "내구도 " + item.limit + " / " + item.currentLimit;
        }

        void ItemNameColor(ItemQuality itemQuality)
        {
            if(itemQuality == ItemQuality.Common)
            {
                tooltipText.color = Color.gray;
            } /*else if(itemQuality == ItemQuality.Uncommon)
            {
                tooltipText.color = Color.green;
            }*/
            else if (itemQuality == ItemQuality.Rare)
            {
                tooltipText.color = Color.blue;
            }
            else if (itemQuality == ItemQuality.Unique)
            {
                tooltipText.color = Color.magenta;
            }
            else if (itemQuality == ItemQuality.Epic)
            {
                tooltipText.color = Color.yellow;
            }

        }
    }

    private void ResetInfo()
    {
        Damage.text = "";
        Defence.text = "";
        itemType.text = "";
        itemComment.text = "";
        itemLimit.text = "";
    }

    public void GetMesh(SkinnedMeshRenderer mesh)
    {
        meshFilter.sharedMesh = mesh.sharedMesh;
        meshRenderer.sharedMaterials = mesh.sharedMaterials;
    }

    public void SetItemName(string S)
    {
        tooltipText.text = S;
    }

    public void ShowToolTip()
    {
        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        meshRenderer.transform.rotation = new Quaternion(0, 0, 0,0);
        transform.localPosition = new Vector2(1000,1000);
        gameObject.SetActive(false);
    }
}
