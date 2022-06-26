using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionDisplay : MonoBehaviour
{
    public Text text;
    void Start()
    {
        text.text = "V" + Application.version;
    }
}
