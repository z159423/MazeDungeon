using UnityEngine;
using UnityEngine.Localization.Components;
using TMPro;

public class BuffDebuffDetail : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent nameLocalize;
    [SerializeField] private TextMeshProUGUI nameTMP;
    [SerializeField] private LocalizeStringEvent detailLocalize;
    [SerializeField] private TextMeshProUGUI detailTMP;
    [SerializeField] private LocalizeStringEvent timeRemainLocalize;
    [SerializeField] private TextMeshProUGUI timeRemainTMP;

    [Space]

    public BuffNDebuffObject buffNDebuffObject;

    public string BT1 = "";
    public string BD1 = "";
    public string BRT1 = "";
    public string value1 = "";
    public string value2 = "";
    public string value3 = "";
    public string TimeRemain = "";

    [SerializeField] private GameObject UI;
    [SerializeField] private Camera Camera;
    [SerializeField] private Vector3 offset;


    private RectTransform canvasRectTransform;
    private Vector3 UIposiiton;
    private Vector2 outVector2;
    private Vector3 DefaultPosition;
    private bool detailDisplay = false;

    public static BuffDebuffDetail instance;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        canvasRectTransform = UI.GetComponent<RectTransform>();

        DefaultPosition = transform.localPosition;

        HideBuffDebuffDetailUI();
    }

    void Update()
    {
        if(buffNDebuffObject != null)
        {
            if (buffNDebuffObject.buffOrDebuff.iconObject == null)
                HideBuffDebuffDetailUI();

            if (detailDisplay)
            {
                if (buffNDebuffObject.buffOrDebuff.untilTheNextStage)
                {
                    timeRemainLocalize.StringReference.SetReference("BuffDebuff", "RemainTimeToNextStage");
                }
                else
                {
                    if(TimeRemain != buffNDebuffObject.buffOrDebuff.GetCurrentRunningTime().ToString())
                    {
                        TimeRemain = buffNDebuffObject.buffOrDebuff.GetCurrentRunningTime().ToString();

                        timeRemainLocalize.StringReference.SetReference("BuffDebuff", "RemainTime");

                        timeRemainLocalize.enabled = false;
                        timeRemainLocalize.enabled = true;
                    }
                }
            }
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, PadCursor.instance.GetCurrentCursorPosition(), Camera, out outVector2);
        UIposiiton = outVector2;
        transform.localPosition = UIposiiton + new Vector3(0, 0, -50) + offset;
    }

    public void ChangeText(BuffNDebuffObject Object)
    {
        buffNDebuffObject = Object;

        BT1 = Object.buffOrDebuff.EndTime.ToString();
        BD1 = Object.buffOrDebuff.Damage.ToString();
        BRT1 = Object.buffOrDebuff.RepeatTime.ToString();
        value1 = Object.buffOrDebuff.value1.ToString();
        value2 = Object.buffOrDebuff.value2.ToString();
        value3 = Object.buffOrDebuff.value3.ToString();


        nameLocalize.StringReference.SetReference("BuffDebuff", Object.buffOrDebuff.buffDebuff.ToString());
        detailLocalize.StringReference.SetReference("BuffDebuff", Object.buffOrDebuff.buffDebuff.ToString() + "-Detail");
    }

    public void ShowBuffDebuffDetailStatUI()
    {
        gameObject.SetActive(true);
        detailDisplay = true;
    }

    public void HideBuffDebuffDetailUI()
    {
        gameObject.SetActive(false);
        transform.localPosition = DefaultPosition;
        detailDisplay = false;
    }
}
