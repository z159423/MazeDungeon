﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.layer != 8)
            gameObject.layer = 8;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.layer != 8)
            gameObject.layer = 8;
    }
}
