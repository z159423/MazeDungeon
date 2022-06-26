using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDynamicChange : MonoBehaviour
{
    private Light light;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        light.intensity = light.range - (Mathf.PerlinNoise(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 2);
    }
}
