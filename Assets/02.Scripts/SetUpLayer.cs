using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpLayer : MonoBehaviour
{
    public int layerNumber = 2;
    void Start()
    {
        ChangeLayer(transform, layerNumber);
    }


    void ChangeLayer(Transform trans, int layer)
    {
        trans.gameObject.layer = layer;

        for (int i = 0; i < trans.childCount; i++)
        {
            Transform child = trans.GetChild(i);

            child.gameObject.layer = layer;

            ChangeLayer(child, layer);
        }
    }
}
