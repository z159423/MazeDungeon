using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public Animator animator;
    private Text damageText;

    public Camera MainCamera, UI_camera;
    public Vector3 targetPosition;
    public RectTransform canvasRectTransform;
    public Canvas Canvas;

    public Vector3 randomOffset;

    public GameObject owner;

    void Start()
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        damageText = animator.GetComponentInChildren<Text>();
        UI_camera = GameObject.FindWithTag("UI_Camera").GetComponent<Camera>();
        Canvas = GameObject.Find("UI").GetComponent<Canvas>();
        canvasRectTransform = Canvas.transform as RectTransform;

        MainCamera = Camera.main;

        randomOffset = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f));

        Destroy(gameObject, clipInfo[0].clip.length);

        print(clipInfo.Length);
    }

    private void LateUpdate()
    {
        Vector2 screenPosition = MainCamera.WorldToScreenPoint(targetPosition);
        Vector3 localPointerPosition;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTransform, screenPosition, UI_camera, out localPointerPosition);

        transform.position = localPointerPosition + randomOffset;

        if(owner != null)
        {
            if (VisibleTester.instance.VisibleTest(owner.transform.position))
                damageText.enabled = true;
            else
                damageText.enabled = false;
        }
        
    }



    public void SetText(string text)
    {
        animator.GetComponentInChildren<Text>().text = text;
    }

    public void SetColor(string color, GameObject other)
    {
        Debug.Log(color + "  " + other);

        if (color == "Player")
        {
            animator.GetComponentInChildren<Text>().color = Color.red;
        }
        else if (color == "Shield")
        {
            animator.GetComponentInChildren<Text>().color = Color.cyan;
        }
        else if (other)
        {
            if (other.GetComponent<NPC_AI>() != null)
            {
                if (other.GetComponent<NPC_AI>().npcType == NPC_Type.friendly)
                {
                    animator.GetComponentInChildren<Text>().color = Color.red;
                }
                else if (other.GetComponent<NPC_AI>().npcType == NPC_Type.enemy)
                {
                    animator.GetComponentInChildren<Text>().color = Color.green;
                }
            }
        }
        else if (color == "Enemy")
        {
            animator.GetComponentInChildren<Text>().color = Color.green;
        }
        else
        {
            animator.GetComponentInChildren<Text>().color = Color.green;
        }
        
    }

    public void SetColorByNpcType(NPC_Type npcType, bool isCritical = false)
    {
        var text = animator.GetComponentInChildren<Text>();

        switch (npcType)
        {
            case NPC_Type.enemy:
                if(isCritical)
                {
                    text.color = Color.magenta;
                }
                else
                {
                    text.color = Color.green;
                }
                

                break;

            case NPC_Type.friendly:
                text.color = Color.red;

                break;

            case NPC_Type.Minion:
                text.color = Color.red;

                break;

            case NPC_Type.neutrality:
                text.color = Color.yellow;

                break;
        }
    }

    public void SetBuffColor(NPC_Type npcType)
    {
        var text = animator.GetComponentInChildren<Text>();

        switch (npcType)
        {
            case NPC_Type.enemy:
                text.color = Color.red;

                break;

            case NPC_Type.friendly:
                text.color = Color.green;

                break;

            case NPC_Type.Minion:
                text.color = Color.green;

                break;

            case NPC_Type.neutrality:
                text.color = Color.yellow;

                break;
        }
    }

    public void SetShieldColor()
    {
        animator.GetComponentInChildren<Text>().color = Color.cyan;
    }

    public void setTextSize(int size)
    {
        animator.GetComponentInChildren<Text>().fontSize = size;
    }

    public void setOwner(GameObject owner)
    {
        this.owner = owner;
    }
}
