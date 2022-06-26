using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocalizationScript : MonoBehaviour
{

    public static LocalizationScript Instance;

    public TMP_FontAsset engFont;
    public TMP_FontAsset korFont;
    public TMP_FontAsset CN_Font;
    public TMP_FontAsset Fr_Font;
    public TMP_FontAsset De_Font;
    public TMP_FontAsset Jp_Font;
    public TMP_FontAsset Pl_Font;
    public TMP_FontAsset Pt_Font;
    public TMP_FontAsset ru_Font;
    public TMP_FontAsset es_Font;
    public TMP_FontAsset tr_Font;


    private void Awake()
    {
        Instance = this;
    }


}
