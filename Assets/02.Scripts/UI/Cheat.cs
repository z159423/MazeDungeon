using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Cheat : MonoBehaviour
{

    public static Cheat instance;

    public InputField itemTierInputField;

    public GameObject Ob;

    public InputField NPCNumber;
    public GameObject[] NPCList;

    public GameObject[] ArtifactList;
    public InputField ArtifactNumber;

    public GameObject[] Chests;
    public InputField ChestNumber;

    private GameObject player;

    private void Start()
    {
        instance = this;

        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerStats>().gameObject;
    }

    private void Update()
    {
        var playerangle = player.transform.localRotation.eulerAngles.y;

        if (playerangle > 180)
            playerangle -= 360;

        PlayerAngle.text = "각도 : " + playerangle;
    }

    public Text PlayerAngle;
    
    public void TeleportToBoss()
    {
        try
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in players)
            {
                player.transform.position = DungeonGenerator.instance.BossChamber.transform.position;
            }
        }
        catch(NullReferenceException ie)
        {
            print(ie);
        }
        
    }

    public void TeleportToShop()
    {
        try
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in players)
            {
                player.transform.position = DungeonGenerator.instance.ShopChamber.transform.position;
            }
        }
        catch (NullReferenceException ie)
        {
            print(ie);
        }
    }

    public void TeleportToTresureRoom()
    {
        try
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in players)
            {
                player.transform.position = DungeonGenerator.instance.TreasureChamber.transform.position;
            }
        }
        catch (NullReferenceException ie)
        {
            print(ie);
        }
    }

    public void CompleteStage()
    {
        DungeonGenerator.instance.EndStage();
    }

    public void KillAllEnemy()
    {

        GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemys)
        {
            if (enemy.GetComponentInChildren<NPC_AI>().npcType == NPC_Type.enemy)
            {
                //enemy.GetComponentInChildren<NPCStats>().KillThisNPC();
                enemy.GetComponentInChildren<NPCStats>().TakeDamage(10000, 10000, enemy, false, false, false);
            }
        }

    }

    public void KillAllNPC()
    {

        GameObject[] npcs = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject npc in npcs)
        {
            //npc.GetComponentInChildren<NPCStats>().KillThisNPC();
            if(npc.GetComponentInChildren<NPCStats>())
                npc.GetComponentInChildren<NPCStats>().TakeDamage(10000, 10000, npc, false, false, false);
        }
    }

    public void SummonItem()
    {
        var tieratring = itemTierInputField.text;
        int tier;
        int.TryParse(tieratring, out tier);

        DropItem.instance.CreateEquipmentDropItem(GameObject.FindGameObjectWithTag("Player").transform, GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerStats>().playerClass, tier);
    }

    public void SummonPurchaseItem()
    {
        DropItem.instance.CreateOnSaleItem(GameObject.FindGameObjectWithTag("Player").transform, GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerStats>().playerClass, false, onSaleItemType.Equipment);
    }

    public void SummonConsumeableItem()
    {
        DropItem.instance.CreateConsumeableItem(GameObject.FindGameObjectWithTag("Player").transform);
    }
    public void AddCoin()
    {
        Inventory.instance.GetCoin(10);
    }

    public void PlayerTakeDamage()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerStats>().TakeDamage(10, 10, null, true, true, false);
    }

    public void GetKey()
    {
        Inventory.instance.GetKey(1);
    }

    public void PlayerLevelUp()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerStats>().LevelUp();
    }

    public void SpawnRandomNormalNpc()
    {
        List<Transform> spawnPoint = new List<Transform>();
        spawnPoint.Add(GameObject.FindGameObjectWithTag("Player").transform);
        MazeDungeonNpcSpawner.instance.SpawnNpc(spawnPoint);
    }

    public void RevealAllMap()
    {
        foreach (GameObject rooms in GameObject.FindGameObjectsWithTag("Rooms"))
        {
            rooms.GetComponentInChildren<ChamberChecking>().RevealMap();
        }
    }

    public void CreateObject()
    {
        Instantiate(Ob, GameObject.FindGameObjectWithTag("Player").transform.position, Quaternion.identity);
    }

    public void NpcSpawn()
    {
        Instantiate(NPCList[Int32.Parse(NPCNumber.text)], GameObject.FindGameObjectWithTag("Player").transform.position, Quaternion.identity);
    }

    public void ArtifactSpawn()
    {
        Instantiate(ArtifactList[Int32.Parse(ArtifactNumber.text)], GameObject.FindGameObjectWithTag("Player").transform.position, Quaternion.identity);
    }

    public void GenerateChest()
    {
        Instantiate(Chests[Int32.Parse(ChestNumber.text)], GameObject.FindGameObjectWithTag("Player").transform.position + Vector3.forward, Quaternion.identity);
    }

    public void PlayerInvincible()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerStats>().isInvincibility = true;
    }

    public void PlayerFullHp()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerStats>().fullHP();
    }

    public void UnlockArhcer()
    {
        CharacterClassManager.UnlockArcher();
    }

    public void UnlockRouge()
    {
        CharacterClassManager.ClearStage3();
    }

    public void UnlockWizard()
    {
        CharacterClassManager.KillTheLich();
    }

    public void GameSpeedUp()
    {
        Time.timeScale += 0.1f;
    }

    public void GameSpeedDown()
    {
        Time.timeScale -= 0.1f;
    }

    public void UIOff()
    {

    }
    
}

