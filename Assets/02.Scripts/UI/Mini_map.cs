﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mini_map : MonoBehaviour
{

    public Transform player;

    private void LateUpdate()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

    }

}
