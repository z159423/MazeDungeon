using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWaveTransparencAmount : MonoBehaviour
{
    public Material material;
    public float Transparency;

    // Update is called once per frame
    void Update()
    {
        Transparency += (float)(Time.deltaTime * 0.5);
        material.SetFloat("Vector1_B8955192", Transparency);
    }
}
