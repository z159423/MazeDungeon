using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingUITest : MonoBehaviour
{
    public Transform playerTransform;
    public Camera MainCamera;
    public Camera UI_camera;
    public Canvas Canvas2;

    public Text screenPositionText;
    public Text localPointerPositionText;

    public Vector3 offset;


    public RectTransform canvasRectTransform;

    private void Start()
    {
        Canvas2 = GameObject.Find("UI").GetComponent<Canvas>();
        canvasRectTransform = Canvas2.transform as RectTransform;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (playerTransform)
        {
            Vector2 screenPosition = MainCamera.WorldToScreenPoint(playerTransform.position);
            Vector3 localPointerPosition = Vector3.zero;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTransform, screenPosition, UI_camera, out localPointerPosition);

            transform.position = localPointerPosition + offset;

            if(screenPositionText)
                screenPositionText.text = screenPosition.ToString();
            if(localPointerPositionText)
                localPointerPositionText.text = localPointerPosition.ToString();
        }
            //transform.position = Camera.main.WorldToScreenPoint(playerTransform.position);
    }
}
