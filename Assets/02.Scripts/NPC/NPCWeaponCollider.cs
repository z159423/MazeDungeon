using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWeaponCollider : MonoBehaviour
{
    Collider weaponCollider;

    void Start()
    {
        weaponCollider = GetComponent<Collider>();
    }

    public void ColliderOn()
    {
        weaponCollider.enabled = true;
    }

    public void ColliderOff()
    {
        weaponCollider.enabled = false;
    }
}
