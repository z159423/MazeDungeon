using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightFadeOut : MonoBehaviour
{
    new Light light;
    void Start()
    {
        light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        light.range -= Time.deltaTime * 10;
    }
}
