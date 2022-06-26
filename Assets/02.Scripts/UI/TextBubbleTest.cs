using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TextBubbleTest : MonoBehaviour
{
    public Image prefabUi;
    private Image uiUse;
    public Transform canvas;
    public Transform tr_head;
    public Camera camera;
    private Vector3 offSet = new Vector3(0, 1.5f, 0);
    private RectTransform rect;

    [Space]

    public bool isVisible = false;

    // Start is called before the first frame update
    void Start()
    {
        canvas = UIManager.instance.transform;
        uiUse = Instantiate(prefabUi, canvas).GetComponent<Image>();
        rect = uiUse.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //uiUse.transform.position = camera.WorldToScreenPoint(tr_head.position + offSet);
        rect.anchoredPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, tr_head.position + offSet);
    }

    private void OnBecameVisible()
    {
        isVisible = true;
        uiUse.gameObject.SetActive(true);
    }

    private void OnBecameInvisible()
    {
        isVisible = false;
        uiUse.gameObject.SetActive(false);
    }
}
