using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine.InputSystem;


public class Settings : MonoBehaviour
{

    public Button Game, Video, Audio, Input;
    public GameObject GameObject, VideoObject, AudioObject, InputObject;

    [Space]
    [Header("Game")]

    public Dropdown LanguageDropdown;
    public Toggle DamagePopUpToggle;
    public Toggle EffectPopUpToggle;
    public Toggle NpcHpBarDisplayToggle;
    public Toggle AllowConsoleToggle;


    [Space]
    [Header("Video")]

    public Dropdown resolutionDropdown;
    public Dropdown graphicQualitydropdown;
    public Toggle fullscreenBtn;
    public int resolutionNum;
    private FullScreenMode screenMode;
    private List<Resolution> resolutions = new List<Resolution>();
    public Toggle frameDisplayToggle;
    public flame_count frameDisplay;
    public Slider FOVslider;
    public Text FOVText;
    public Slider LODslider;
    public Text LODText;
    public Slider frameLateLimitSlider;
    public Text frameLateLimitText;
    public Slider UISizeSlider;
    public Text UISizeText;
    public CanvasScaler canvasScaler;

    public Camera mainCamera;

    [Space]
    [Header("Audio")]

    public AudioMixer audioMixer;

    public Slider masterVolumeSlider;
    public Text masterVolumeText;

    public Slider musicVolumeSlider;
    public Text musicVolumeText;

    public Slider sfxVolumeSlider;
    public Text sfxVolumeText;

    public LocalizeStringEvent localizeString;

    [Space]

    [Header("Input")]

    public Slider mouseSensitiveSlider;
    public Text mouseSensitiveNumber;

    [Space]

    public InputActionAsset actions;
    private PlayerInputAction playerInputActions;
    [SerializeField] private InputActionAsset inputActions;

    [Space]

    [SerializeField]  private LocalizeStringEvent infoMessageLocalize;
    [SerializeField] private Animator infoMessageAnimator;

    public static Settings instance;


    private void Awake()
    {
        instance = this;
    }

    public void OnEnable()
    {
        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            actions.LoadBindingOverridesFromJson(rebinds);

        var settingData = RoadSettingData();
        if (settingData != null)
            StartCoroutine(ReadSetting(settingData));

        InitUI();
    }

    public void InitiallizeAudioSetting()
    {
        float master = masterVolumeSlider.value;
        audioMixer.SetFloat("Master", Mathf.Log10(master) * 20);
        masterVolumeText.text = ((int)Mathf.Round(master * 100)).ToString();

        float music = musicVolumeSlider.value;
        audioMixer.SetFloat("Music", Mathf.Log10(music) * 20);
        musicVolumeText.text = ((int)Mathf.Round(music * 100)).ToString();

        float sfx = sfxVolumeSlider.value;
        audioMixer.SetFloat("SFX", Mathf.Log10(sfx) * 20);
        sfxVolumeText.text =  ((int)Mathf.Round(sfx * 100)).ToString();
    }

    public void SetMasterVolume (float sliderValue)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(sliderValue) * 20 );
        int value = Mathf.RoundToInt(sliderValue * 100);
        masterVolumeText.text = value.ToString();
    }

    public void SetMusicVolume(float sliderValue)
    {
        audioMixer.SetFloat("Music", Mathf.Log10(sliderValue) * 20);
        int value = Mathf.RoundToInt(sliderValue * 100);
        musicVolumeText.text = value.ToString();
    }

    public void SetSFXVolume(float sliderValue)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(sliderValue) * 20);
        int value = Mathf.RoundToInt(sliderValue * 100);
        sfxVolumeText.text = value.ToString();
    }

    IEnumerator Start()
    {
        // Wait for the localization system to initialize, loading Locales, preloading etc.
        yield return LocalizationSettings.InitializationOperation;

        // Generate list of available Locales
        var options = new List<Dropdown.OptionData>();
        int selected = 0;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            if (LocalizationSettings.SelectedLocale == locale)
                selected = i;
            options.Add(new Dropdown.OptionData(locale.name));
        }
        LanguageDropdown.options = options;

        LanguageDropdown.value = selected;
        LanguageDropdown.onValueChanged.AddListener(LocaleSelected);
    }

    static void LocaleSelected(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }

    public void Close()
    {
        gameObject.SetActive(false);

        var settingData = RoadSettingData();

        if (settingData.UISize < 0.5f)
            settingData.UISize = 1f;

        UISizeSlider.value = settingData.UISize;
        UISizeText.text = settingData.UISize.ToString();

        OnChangeUISize();
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void OptionButtonClick(string button)
    {
        TurnOffAllUi();

        switch(button)
        {
            case "game":

                if (GameObject)
                {
                    GameObject.SetActive(true);
                }
                break;

            case "video":

                if (VideoObject)
                {
                    VideoObject.SetActive(true);
                }
                break;

            case "audio":
                if (AudioObject)
                {
                    AudioObject.SetActive(true);
                }

                break;

            case "input":
                if (InputObject)
                {
                    InputObject.SetActive(true);
                }

                break;
        }
    }


    private void TurnOffAllUi()
    {
        if(GameObject)
        {
            GameObject.SetActive(false);
        }

        if (VideoObject)
        {
            VideoObject.SetActive(false);
        }

        if (AudioObject)
        {
            AudioObject.SetActive(false);
        }

        if (InputObject)
        {
            InputObject.SetActive(false);
        }

        //localizeString.StringReference.TableEntryReference
    }

    public void OnMouseSensitiveChange()
    {
        mouseSensitiveNumber.text = mouseSensitiveSlider.value.ToString("F3");
    }

    public void SaveSetting()
    {
        SettingData settingData = new SettingData();

        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            if (LocalizationSettings.AvailableLocales.Locales[i] == LocalizationSettings.SelectedLocale)
            {
                settingData.SeletedLocale = i;
                //Debug.LogError(i);
                //Debug.LogError(LocalizationSettings.SelectedLocale);
                //Debug.LogError(LocalizationSettings.AvailableLocales.Locales[i]);
            }
                
        }
        //Game
        settingData.firstSave = false;
        settingData.DamagePopupTextEnable = DamagePopUpToggle.isOn;
        settingData.EffectPopupTextEnable = EffectPopUpToggle.isOn;
        settingData.NpcHpBarDisplay = NpcHpBarDisplayToggle.isOn;
        settingData.AllowConsole = AllowConsoleToggle.isOn;


        //Audio
        settingData.MasterVolume = masterVolumeSlider.value;
        settingData.MusicVolume = musicVolumeSlider.value;
        settingData.SFXVolume = sfxVolumeSlider.value;


        //Input
        settingData.mouseSensitive = mouseSensitiveSlider.value;


        //Video
        settingData.resolutionNum = resolutionNum;
        //settingData.graphicQuality = graphicQualitydropdown.value;
        settingData.fullScreen = fullscreenBtn.isOn;
        settingData.fpsDisplay = frameDisplayToggle.isOn;
        settingData.FOV = FOVslider.value;
        settingData.LOD = LODslider.value;
        settingData.frameLateLimit = frameLateLimitSlider.value;
        Application.targetFrameRate = (int)frameLateLimitSlider.value;
        settingData.UISize = UISizeSlider.value;

        if (mainCamera != null)
        {
            mainCamera.fieldOfView = FOVslider.value;
            mainCamera.farClipPlane = LODslider.value;
            RenderSettings.fogDensity = GetFogDensity(LODslider.value);
            Debug.LogError(RenderSettings.fogDensity);
        }

        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/setting.mazedungeon";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, settingData);
        stream.Close();

        playerInputActions = new PlayerInputAction();

        var rebinds = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);

        if(KeyBindindManager.instance.OnUpdateKeyBindsCallBack != null)
            KeyBindindManager.instance.OnUpdateKeyBindsCallBack.Invoke();

        //QualitySettings.SetQualityLevel(settingData.graphicQuality);

        

        ShowInfoMessage("SettingSaved");
    }

    public static void SaveDate(SettingData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/setting.mazedungeon";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public IEnumerator ReadSetting(SettingData settingData)
    {
        yield return LocalizationSettings.InitializationOperation;

        //LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[settingData.SeletedLocale];

        //Game

        if(!settingData.firstSave)
            LocaleSelected(settingData.SeletedLocale);

        //Debug.LogError(settingData.SeletedLocale);

        DamagePopUpToggle.isOn = settingData.DamagePopupTextEnable;
        EffectPopUpToggle.isOn = settingData.EffectPopupTextEnable;
        NpcHpBarDisplayToggle.isOn = settingData.NpcHpBarDisplay;
        AllowConsoleToggle.isOn = settingData.AllowConsole;

        //Audio

        masterVolumeSlider.value = settingData.MasterVolume;
        musicVolumeSlider.value = settingData.MusicVolume;
        sfxVolumeSlider.value = settingData.SFXVolume;

        mouseSensitiveSlider.value = settingData.mouseSensitive;

        //Video

        resolutionNum = settingData.resolutionNum;
        //graphicQualitydropdown.value = settingData.graphicQuality;
        fullscreenBtn.isOn = settingData.fullScreen;
        frameDisplayToggle.isOn = settingData.fpsDisplay;
        FOVslider.value = settingData.FOV;
        FOVText.text = settingData.FOV.ToString();
        LODslider.value = settingData.LOD;
        LODText.text = settingData.LOD.ToString();

        if (settingData.UISize < 0.5f)
            settingData.UISize = 1f;

        UISizeSlider.value = settingData.UISize;
        UISizeText.text = settingData.UISize.ToString();

        OnChangeUISize();

        if (mainCamera != null)
        {
            mainCamera.fieldOfView = FOVslider.value;
            mainCamera.farClipPlane = LODslider.value;
            RenderSettings.fogDensity = GetFogDensity(LODslider.value);
            //Debug.LogError(RenderSettings.fogDensity);
        }

        frameLateLimitSlider.value = settingData.frameLateLimit;
        frameLateLimitText.text = settingData.frameLateLimit.ToString();
        Application.targetFrameRate = (int)frameLateLimitSlider.value;

        if (frameDisplay == null)
            frameDisplay = flame_count.instance;

        frameDisplay.FrameDisplay(frameDisplayToggle.isOn);
    }

    public static SettingData RoadSettingData()
    {
        string path = Application.persistentDataPath + "/setting.mazedungeon";

        Debug.Log(path);

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SettingData settingData = formatter.Deserialize(stream) as SettingData;
            stream.Close();

            return settingData;
        }
        else
        {
            Debug.Log("세팅 파일을 불러올수 없습니다. " + path + "\n새로운 세팅파일을 생성합니다.");

            SettingData settingData = new SettingData();

            settingData.resolutionNum = GetNowResolution();

            SteamManager.instance.LocalInit(SteamManager.GetSteamUILanguage());

            SaveDate(settingData);

            return settingData;
        }
    }

    public SettingData CreateNewData()
    {
        SettingData settingData = new SettingData();

        resolutions.Clear();

        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].refreshRate == 60)
                resolutions.Add(Screen.resolutions[i]);
        }

        resolutionDropdown.options.Clear();

        int optionNum = 0;
        foreach(Resolution item in resolutions)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = item.width + " x " + item.height;
            resolutionDropdown.options.Add(option);

            if (item.width == Screen.width && item.height == Screen.height)
                resolutionDropdown.value = optionNum;
            optionNum++;
        }
        resolutionDropdown.RefreshShownValue();

        fullscreenBtn.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
        return settingData;
}

    public void PlaySound(string clipName)
    {
        AudioManager.instance.PlaySFX(clipName);
    }

    public void ResetAllBindings()
    {
        foreach(InputActionMap map in inputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }

        foreach(KeyRebind keyRebind in GetComponentsInChildren<KeyRebind>())
        {
            keyRebind.CheckingBindingKey();
        }

        ShowInfoMessage("SettingReset");

        SettingData settingData = new SettingData();

        mouseSensitiveSlider.value = settingData.mouseSensitive;
    }

    public void ShowInfoMessage(string LocalizeKey)
    {
        infoMessageLocalize.StringReference.SetReference("UI", LocalizeKey);

        infoMessageAnimator.SetTrigger("Show");
    }

    private void InitUI()
    {
        if (resolutions.Count > 0)
            return;

        resolutions.Clear();

        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].refreshRate == 60)
                resolutions.Add(Screen.resolutions[i]);
        }

        resolutionDropdown.options.Clear();

        int optionNum = 0;

        foreach(Resolution item in resolutions)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = item.width + " x " + item.height;
            resolutionDropdown.options.Add(option);

            if (item.width == Screen.width && item.height == Screen.height)
                resolutionDropdown.value = optionNum;
            optionNum++;
        }
        resolutionDropdown.RefreshShownValue();
    }

    public void DropboxOptionChange(int x)
    {
        resolutionNum = x;
    }

    public void FullScreenBtn(bool isFull)
    {
        screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void SaveVideoBtnClick()
    {
        frameDisplay.FrameDisplay(frameDisplayToggle.isOn);
        Screen.SetResolution(resolutions[resolutionNum].width, resolutions[resolutionNum].height, screenMode);
    }

    public void SetFov(float sliderValue)
    {
        FOVText.text = sliderValue.ToString();

        if (mainCamera != null)
        {
            mainCamera.fieldOfView = sliderValue;
        }
    }

    public void SetLOD(float sliderValue)
    {
        LODText.text = sliderValue.ToString();
    }

    public void SetFrameLateLimit(float sliderValue)
    {
        frameLateLimitText.text = sliderValue.ToString();
    }

    private float GetFogDensity(float LODvalue)
    {
        var i = LODslider.value * 0.01f;

        float j = Mathf.Pow(3, i);

        float z = 2222 * j;

        return LODvalue / z;
    }

    public void ResetVideoSetting()
    {
        SettingData settingData = new SettingData();

        //fullscreenBtn.isOn = settingData.fullScreen;
        frameDisplayToggle.isOn = settingData.fpsDisplay;
        FOVslider.value = settingData.FOV;
        LODslider.value = settingData.LOD;
        frameLateLimitSlider.value = settingData.frameLateLimit;

        UISizeSlider.value = settingData.UISize;
        UISizeText.text = settingData.UISize.ToString();
    }

    public void ResetGameSetting()
    {
        SettingData settingData = new SettingData();

        DamagePopUpToggle.isOn = settingData.DamagePopupTextEnable;
        EffectPopUpToggle.isOn = settingData.EffectPopupTextEnable;
    }

    public void ResetAudioSetting()
    {
        SettingData settingData = new SettingData();

        masterVolumeSlider.value = settingData.MasterVolume;
        musicVolumeSlider.value = settingData.MusicVolume;
        sfxVolumeSlider.value = settingData.SFXVolume;

    }

    public static int GetNowResolution()
    {
        List<Resolution> resolutions = new List<Resolution>();

        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].refreshRate == 60)
                resolutions.Add(Screen.resolutions[i]);
        }

        for (int i = 0; i < resolutions.Count; i++)
        {
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                return i;
        }

        return resolutions.Count;
    }

    public void SelectSteamUILanguages(string language)
    {
        switch(language)
        {
            case "english":

                break;
        }
    }

    public void OnChangeUISize()
    {
        canvasScaler.referenceResolution = new Vector2(800 / UISizeSlider.value, 600 / UISizeSlider.value);
        UISizeText.text = string.Format("{0:N2}", UISizeSlider.value);
    }
    
}
