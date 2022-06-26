using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class InfoMessageManager : MonoBehaviour
{
    public static InfoMessageManager instance;

    public Text InfoMessage;
    public Animator InfoMessageAnimator;
    public LocalizeStringEvent InfoMessageLocalize;

    private void Awake()
    {
        instance = this;
    }

    public void PopupInfoMessage(string key)
    {
        //InfoMessage.text = message;

        InfoMessageLocalize.StringReference.SetReference("UI", key);

        InfoMessageAnimator.SetTrigger("Fade");
    }
}
