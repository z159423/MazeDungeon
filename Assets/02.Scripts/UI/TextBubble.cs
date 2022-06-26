using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class TextBubble : MonoBehaviour
{
    public GameObject TextBubblePrefab;
    public Transform TextBubblePos;
    public Vector3 offset;

    [Space]

    public bool isVisible = false;

    private Camera UI_camera;
    private RectTransform canvasRectTransform;
    private Canvas Canvas;

    [Space]

    public float popOutTime = 8f;

    private Image bubbleUI;
    private RectTransform rect;

    private void Start()
    {
        Canvas = GameObject.Find("UI").GetComponent<Canvas>();
        canvasRectTransform = Canvas.transform as RectTransform;
        UI_camera = GameObject.FindWithTag("UI_Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (bubbleUI != null)
        {
            //bubbleUI.transform.position = UI_camera.WorldToScreenPoint(TextBubblePos.position + offset);
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(TextBubblePos.position + offset);
            Vector3 localPointerPosition;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTransform, screenPosition, UI_camera, out localPointerPosition);

            bubbleUI.transform.position = localPointerPosition;

            //rect.anchoredPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, TextBubblePos.position + offset);

        }
            //bubbleUI.transform.position = Camera.main.WorldToScreenPoint(TextBubblePos.position + offset);
    }

    private void OnBecameVisible()
    {
        isVisible = true;

        if (bubbleUI != null)
        {
            bubbleUI.transform.GetChild(0).gameObject.SetActive(true);
            bubbleUI.enabled = true;
        }
    }

    private void OnBecameInvisible()
    {
        isVisible = false;

        if (bubbleUI != null)
        {
            bubbleUI.transform.GetChild(0).gameObject.SetActive(false);
            bubbleUI.enabled = false;
        }
    }

    public IEnumerator SpawnTextBubble()
    {
        if (bubbleUI != null)
            Destroy(bubbleUI.gameObject);

        var textBubble = Instantiate(TextBubblePrefab, UIManager.instance.transform).GetComponent<Image>();
        bubbleUI = textBubble;
        rect = bubbleUI.GetComponent<RectTransform>();

        ChangeTextBubbleState(textBubble.GetComponent<Animator>(), "PopUp");

        yield return new WaitForSeconds(popOutTime);

        if(textBubble != null)
            ChangeTextBubbleState(textBubble.GetComponent<Animator>(), "PopOut");

        yield return new WaitForSeconds(2);
        if (textBubble != null)
            Destroy(textBubble.gameObject);
    }

    public void ChangeText(string Key)
    {
        if (bubbleUI == null)
            return;

        bubbleUI.GetComponentInChildren<LocalizeStringEvent>().StringReference.SetReference("Conversation", Key);
    }

    public void ChangeTextBubbleState(Animator animator,string triggerName)
    {
        if (bubbleUI == null)
            return;

        animator.SetTrigger(triggerName);
    }
}
