using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypingEffect : MonoBehaviour
{
    public TextMeshProUGUI DialogText;

    //Main Text
    public string typingText = "";

    //SubString Variable
    string subText;


    public void StartTyping()
    {
        StartCoroutine(TypingAction());
    }


    IEnumerator TypingAction()
    {
        typingText = DialogText.text + " ";
        DialogText.text = "";
        DialogText.enabled = true;

        for (int i = 0; i < typingText.Length; i++)
        {
            yield return new WaitForSeconds(0.1f);

            subText += typingText.Substring(0, i);
            DialogText.text = subText;
            subText = "";
        }
    }
}
