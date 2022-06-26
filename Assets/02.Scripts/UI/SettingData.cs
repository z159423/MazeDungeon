using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
public class SettingData
{
    public bool firstSave = true;

    //Game

    public int SeletedLocale;
    public bool DamagePopupTextEnable = true;
    public bool EffectPopupTextEnable = true;
    public bool NpcHpBarDisplay = true;
    public bool AllowConsole = false;

    //Audio

    public float MasterVolume = 0.7f;
    public float MusicVolume = 1;
    public float SFXVolume = 1;


    //Video

    public int resolutionNum = 0;
    public int graphicQuality = 5;
    public bool fullScreen = true;

    public bool fpsDisplay = false;
    public float FOV = 55;
    public float LOD = 200;
    public float fogDensity = 0.01f;
    public float frameLateLimit = 144;
    public float UISize = 1f;

    //Input

    public float mouseSensitive = 1f;
}
