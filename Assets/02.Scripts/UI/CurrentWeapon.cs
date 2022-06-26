using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentWeapon : MonoBehaviour
{
    public Text R_durability;
    public MeshFilter R_filter;
    public MeshRenderer R_meshRenderer;
    private Item R_currentItem;

    public Text L_durability;
    public MeshFilter L_filter;
    public MeshRenderer L_meshRenderer;
    private Item L_currentItem;

    private void Start()
    {
        EquipmentManager.instance.onEquipmentChanged += onEquipmentChanged;
    }

    void onEquipmentChanged(Item newItem, Item oldItem)
    {
        R_durability.text = "";
        R_filter.mesh = null;
        R_currentItem = null;

        L_durability.text = "";
        L_filter.mesh = null;
        L_currentItem = null;

        var equipment = EquipmentManager.instance;


        if (equipment.currentEquipment[(int)ItemType.Weapon] != null)
        {
            R_filter.mesh = equipment.currentEquipment[(int)ItemType.Weapon].skinedMesh.sharedMesh;
            R_meshRenderer.materials = equipment.currentEquipment[(int)ItemType.Weapon].skinedMesh.sharedMaterials;
            R_durability.text = equipment.currentEquipment[(int)ItemType.Weapon].currentLimit.ToString();
            R_currentItem = equipment.currentEquipment[(int)ItemType.Weapon];
        }
        if (equipment.currentEquipment[(int)ItemType.SecondaryWeapon] != null)
        {
            L_filter.mesh = equipment.currentEquipment[(int)ItemType.SecondaryWeapon].skinedMesh.sharedMesh;
            L_meshRenderer.materials = equipment.currentEquipment[(int)ItemType.SecondaryWeapon].skinedMesh.sharedMaterials;
            L_durability.text = equipment.currentEquipment[(int)ItemType.SecondaryWeapon].currentLimit.ToString();
            L_currentItem = equipment.currentEquipment[(int)ItemType.SecondaryWeapon];
        }
    }

    private void Update()
    {
        if(R_currentItem != null)
        {
            R_durability.text = R_currentItem.currentLimit.ToString();
        }

        if(L_currentItem != null)
        {
            L_durability.text = L_currentItem.currentLimit.ToString();
        }
    }
}
