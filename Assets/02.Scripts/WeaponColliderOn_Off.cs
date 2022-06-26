using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponColliderOn_Off : MonoBehaviour {

    EquipmentManager equipmentmanager;
    private BoxCollider Boxcolli;

    void Awake()
    {
        equipmentmanager = GameObject.Find("GameManager").GetComponent<EquipmentManager>();
        Boxcolli = GetComponent<BoxCollider>();
    }

}
