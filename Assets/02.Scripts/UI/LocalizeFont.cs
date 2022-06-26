using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocalizeFont : MonoBehaviour
{
    public TextMeshProUGUI text;

    protected virtual void OnEnable()
    {
        if(!text)
        {
            text = GetComponent<TextMeshProUGUI>();
        }
        
        LocalizationSettings.SelectedLocaleChanged += ChangeHandler;

        ChangeFont(LocalizationSettings.SelectedLocale);
    }
    void OnDisable() => LocalizationSettings.SelectedLocaleChanged -= ChangeHandler;
    IEnumerator Start()
    {
        // Wait for the localization system to initialize, loading Locales, preloading etc.
        yield return LocalizationSettings.InitializationOperation;
        ChangeHandler(LocalizationSettings.SelectedLocale);
    }

    protected virtual void ChangeHandler(Locale value)
    {
        print(value.Identifier.Code);

        ChangeFont(value);
    }

    void ChangeFont(Locale value)
    {
        if (!value)
            return;

        if (value.Identifier.Code.Equals("ko-KR"))
            text.font = LocalizationScript.Instance.korFont;
        else if (value.Identifier.Code.Equals("en"))
            text.font = LocalizationScript.Instance.engFont;
        else if (value.Identifier.Code.Equals("zh"))
            text.font = LocalizationScript.Instance.CN_Font;
        else if (value.Identifier.Code.Equals("fr-FR"))
            text.font = LocalizationScript.Instance.Fr_Font;
        else if (value.Identifier.Code.Equals("de-DE"))
            text.font = LocalizationScript.Instance.De_Font;
        else if (value.Identifier.Code.Equals("ja"))
            text.font = LocalizationScript.Instance.Jp_Font;
        else if (value.Identifier.Code.Equals("pl-PL"))
            text.font = LocalizationScript.Instance.Pl_Font;
        else if (value.Identifier.Code.Equals("pt"))
            text.font = LocalizationScript.Instance.Pt_Font;
        else if (value.Identifier.Code.Equals("ru-RU"))
            text.font = LocalizationScript.Instance.ru_Font;
        else if (value.Identifier.Code.Equals("es"))
            text.font = LocalizationScript.Instance.es_Font;
        else if (value.Identifier.Code.Equals("tr-TR"))
            text.font = LocalizationScript.Instance.tr_Font;
        else if (value.Identifier.Code.Equals("es-AR"))
            text.font = LocalizationScript.Instance.es_Font;
        else if (value.Identifier.Code.Equals("pt-BR"))
            text.font = LocalizationScript.Instance.Pt_Font;
        else
            text.font = LocalizationScript.Instance.engFont;
    }
}