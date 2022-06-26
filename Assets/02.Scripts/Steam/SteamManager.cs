using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class SteamManager : MonoBehaviour
{
    public uint appId;

    public static bool Initialized = false;

    public static SteamManager instance;

    public static string SteamLanguage = "";
    public int SteamLanguageNumber = 0;

    public static bool isInit = false;

    private void Awake()
    {
        if(!Initialized)
        {
            DontDestroyOnLoad(this);
            Initialized = true;
            instance = this;
        }
        
        try
        {
            if(!isInit)
            {
                Steamworks.SteamClient.Init(appId, true);
                Debug.Log("스팀 연동 완료 " + Steamworks.SteamClient.Name);
                isInit = true;
            }
            
        }
        catch(System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void OnApplicationQuit()
    {
        try
        {
            Steamworks.SteamClient.Shutdown();
        }
        catch
        {

        }
    }

    public static string GetSteamUILanguage()
    {
        return Steamworks.SteamUtils.SteamUILanguage;
    }

    public void LocalInit(string steamLanguage)
    {
        StartCoroutine(LocalInitCoroutine(steamLanguage));
    }

    public int GetLocalNumber(string steamLanguage)
    {
        switch (steamLanguage)
        {
            case "english":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "en")
                    {
                        return i;
                    }

                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "koreana":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "ko-KR")
                    {
                        return i;
                    }

                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "french":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "fr-FR")
                    {
                        return i;
                    }
                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "german":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "de-DE")
                    {
                        return i;
                    }
                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "japanese":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "ja")
                    {
                        return i;
                    }
                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "polish":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "pl-PL")
                    {
                        return i;
                    }
                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "portuguese":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "pt")
                    {
                        return i;
                    }
                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "brazilian":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "pt")
                    {
                        return i;
                    }
                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "russian":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "ru-RU")
                    {
                        return i;
                    }
                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "spanish":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "es")
                    {
                        return i;
                    }
                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "turkish":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "tr-TR")
                    {
                        return i;
                    }
                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;
        }
        return 0;
    }

    public static IEnumerator LocalInitCoroutine(string steamLanguage)
    {
        yield return LocalizationSettings.InitializationOperation;

        Debug.LogError(steamLanguage);

        SteamLanguage = steamLanguage;

        switch (steamLanguage)
        {
            case "english":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "en")
                    {
                        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[i];
                        SteamManager.instance.SteamLanguageNumber = i;
                    }
                        
                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "koreana":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "ko-KR")
                    {
                        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[i];
                        SteamManager.instance.SteamLanguageNumber = i;
                    }
                        
                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "schinese":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "zh")
                    {
                        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[i];
                        SteamManager.instance.SteamLanguageNumber = i;
                    }

                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "tchinese":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "zh")
                    {
                        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[i];
                        SteamManager.instance.SteamLanguageNumber = i;
                    }

                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "french":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "fr-FR")
                    {
                        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[i];
                        SteamManager.instance.SteamLanguageNumber = i;
                    }
                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "german":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "de-DE")
                    {
                        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[i];
                        SteamManager.instance.SteamLanguageNumber = i;
                    }
                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "japanese":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "ja")
                    {
                        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[i];
                        SteamManager.instance.SteamLanguageNumber = i;
                    }
                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "polish":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "pl-PL")
                    {
                        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[i];
                        SteamManager.instance.SteamLanguageNumber = i;
                    }
                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "portuguese":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "pt")
                    {
                        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[i];
                        SteamManager.instance.SteamLanguageNumber = i;
                    }
                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "brazilian":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "pt-BR")
                    {
                        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[i];
                        SteamManager.instance.SteamLanguageNumber = i;
                    }
                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "russian":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "ru-RU")
                    {
                        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[i];
                        SteamManager.instance.SteamLanguageNumber = i;
                    }
                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "spanish":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "es")
                    {
                        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[i];
                        SteamManager.instance.SteamLanguageNumber = i;
                    }
                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "latam":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "es-AR")
                    {
                        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[i];
                        SteamManager.instance.SteamLanguageNumber = i;
                    }
                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            case "turkish":

                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
                {
                    if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == "tr-TR")
                    {
                        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[i];
                        SteamManager.instance.SteamLanguageNumber = i;
                    }
                    //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code);
                }
                break;

            default:
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
                break;
        }

    }
}

