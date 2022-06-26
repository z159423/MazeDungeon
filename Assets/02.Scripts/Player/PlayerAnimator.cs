using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAnimator : CharacterAnimator {

    PlayerController01 playerController;

    protected override void Start()
    {
        base.Start();
        EquipmentManager.instance.onEquipmentChanged += OnEquipmentChanged;
        EquipmentManager.instance.onEquipmentChanged2 += OnEquipmentChanged2;
        playerController = GetComponent<PlayerController01>();
    }

    void OnEquipmentChanged(Item newItem, Item oldItem)
    {
        /*
        if(newItem != null && newItem.itemtype == ItemType.Melee)
        {
            animator.SetLayerWeight(1, 1);
        }
        else if(newItem == null && oldItem != null && oldItem.itemtype == ItemType.Melee)
        {
            animator.SetLayerWeight(1, 0);
        }

        if (newItem != null && newItem.itemtype == ItemType.Shield)
        {
            animator.SetLayerWeight(2, 1);
        }
        else if (newItem == null && oldItem != null && oldItem.itemtype == ItemType.Shield)
        {
            animator.SetLayerWeight(2, 0);
        }
        */
    }

    public void SetPlayerAniamtorWeight()
    {
        PlayerStats playerStats = GetComponent<PlayerStats>();

        switch(playerStats.playerClass)
        {
            case CharacterClass.Warrior:
                animator.SetLayerWeight(animator.GetLayerIndex("Warrior"), 1f);
                break;

            case CharacterClass.Rogue:
                animator.SetLayerWeight(animator.GetLayerIndex("Rogue"), 1f);
                break;

            case CharacterClass.Wizard:
                animator.SetLayerWeight(animator.GetLayerIndex("Wizard"), 1f);
                break;

            case CharacterClass.Archer:
                animator.SetLayerWeight(animator.GetLayerIndex("Archer"), 1f);
                break;
        }
    }

    void OnEquipmentChanged2(EquipmentManager manager)
    {
        playerController.ResetSkillInfo();

        /*if (manager.currentEquipment[(int)ItemType.Weapon] != null && manager.currentEquipment[(int)ItemType.SecondaryWeapon] != null)
        {
            if (manager.currentEquipment[(int)ItemType.Weapon].weaponType == WeaponType.Dagger &&
               manager.currentEquipment[(int)ItemType.SecondaryWeapon].secondaryWeaponType == SecondaryWeaponType.HandCrossBow)
            {
                animator.SetLayerWeight(3, 1);
                animator.SetLayerWeight(1, 0);
                animator.SetLayerWeight(2, 0);
                animator.SetLayerWeight(4, 0);
            }
            else if (manager.currentEquipment[(int)ItemType.SecondaryWeapon].itemtype == ItemType.SecondaryWeapon &&
                (manager.currentEquipment[(int)ItemType.Weapon].weaponType == WeaponType.Dagger || manager.currentEquipment[(int)ItemType.Weapon].weaponType == WeaponType.Sword))
            {
                animator.SetLayerWeight(1, 1);
                animator.SetLayerWeight(2, 1);

                animator.SetLayerWeight(3, 0);
                animator.SetLayerWeight(4, 0);
            }
            else if(manager.currentEquipment[(int)ItemType.SecondaryWeapon].itemtype == ItemType.SecondaryWeapon && 
                manager.currentEquipment[(int)ItemType.Weapon].weaponType == WeaponType.Wand)
            {
                animator.SetLayerWeight(4, 1);
                animator.SetLayerWeight(2, 1);

                animator.SetLayerWeight(1, 0);
                animator.SetLayerWeight(3, 0);
            }
            else
            {
                animator.SetLayerWeight(3, 0);
                animator.SetLayerWeight(4, 0);
            }
        }
        else if (manager.currentEquipment[(int)ItemType.Weapon] != null && manager.currentEquipment[(int)ItemType.SecondaryWeapon] == null)
        {
            if (manager.currentEquipment[(int)ItemType.Weapon].weaponType == WeaponType.Dagger && manager.currentEquipment[(int)ItemType.SecondaryWeapon] == null
                || manager.currentEquipment[(int)ItemType.Weapon].weaponType == WeaponType.Sword || manager.currentEquipment[(int)ItemType.Weapon].weaponType == WeaponType.Bow)
            {
                animator.SetLayerWeight(1, 1);

                animator.SetLayerWeight(2, 0);
                animator.SetLayerWeight(3, 0);
                animator.SetLayerWeight(4, 0);
            }
            else if (manager.currentEquipment[(int)ItemType.Weapon].weaponType == WeaponType.Wand)
            {
                animator.SetLayerWeight(4, 1);

                animator.SetLayerWeight(1, 0);
                animator.SetLayerWeight(2, 0);
                animator.SetLayerWeight(3, 0);
            }
        }
        else if (manager.currentEquipment[(int)ItemType.Weapon] == null && manager.currentEquipment[(int)ItemType.SecondaryWeapon] != null)
        {
            if (manager.currentEquipment[(int)ItemType.SecondaryWeapon].weaponType == WeaponType.Dagger && manager.currentEquipment[(int)ItemType.Weapon] == null
                || manager.currentEquipment[(int)ItemType.SecondaryWeapon].itemtype == ItemType.SecondaryWeapon)
            {
                animator.SetLayerWeight(2, 1);

                animator.SetLayerWeight(1, 0);
                animator.SetLayerWeight(3, 0);
                animator.SetLayerWeight(4, 0);
            }
            
        }
        else
        {
            animator.SetLayerWeight(1, 0);
            animator.SetLayerWeight(2, 0);
            animator.SetLayerWeight(3, 0);
            animator.SetLayerWeight(4, 0);
        }*/
    }
    
}
