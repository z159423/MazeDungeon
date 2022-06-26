using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChamberConnectionPoint : MonoBehaviour
{
    [SerializeField]public float ConnectionRotation;
    [SerializeField] List<ValidConnectionType> ValidConnectionTypes = new List<ValidConnectionType>();
    public GameObject Wall;

    public ChamberChecking thisChamberChecking;

    public ChamberChecking childChamber;

    public bool reGeneratedRoom = false;

    private void Start()
    {
        //Invoke("GenerateChamber", 0.5f);

        //Invoke("enQueueThisPoint", 0.01f);
        StartCoroutine(enQueueThisPoint());

        thisChamberChecking = transform.parent.parent.GetComponentInChildren<ChamberChecking>();
    }

    public IEnumerator enQueueThisPoint()
    {
        yield return new WaitForSeconds(0.01f);

        if(!reGeneratedRoom)
        {
            //print("enQueue됨");
            DungeonGenerator.instance.ChamberQueue.Enqueue(this);
        }
    }

    public void GenerateChamber()
    {
        int value2;
        GameObject chamber = null;
        ChamberConnectionPoint[] connections;

        int currentStage = 0;
        int StageLevel = DungeonGenerator.instance.CurrentStageNumber;

        for (int i = 0; i > DungeonGenerator.instance.stagesRooms.Length; i++)
        {
            if(DungeonGenerator.instance.stagesRooms[i].stage == DungeonGenerator.instance.CurrentStage)
            {
                currentStage = i;
                break;
            }
        }

        if (DungeonGenerator.instance.currentChamberNumber.currentSmallChamberNum >= ChamberManager.Instantce.ChamberSettings[StageLevel].MaxChamberCount.SmallChamberCount
            && DungeonGenerator.instance.currentChamberNumber.currentMediumChamberNum >= ChamberManager.Instantce.ChamberSettings[StageLevel].MaxChamberCount.MediumChamberCount
            && DungeonGenerator.instance.currentChamberNumber.currentLargeChamberNum >= ChamberManager.Instantce.ChamberSettings[StageLevel].MaxChamberCount.LargeChamberCount)
        {
            if ((DungeonGenerator.instance.currentChamberNumber.currentBossChambercount >= ChamberManager.Instantce.ChamberSettings[StageLevel].MaxChamberCount.BossChamberCount &&
            DungeonGenerator.instance.currentChamberNumber.currentTreasureChamberCount >= ChamberManager.Instantce.ChamberSettings[StageLevel].MaxChamberCount.TreasureChamberCount &&
            DungeonGenerator.instance.currentChamberNumber.currentShopChambercount >= ChamberManager.Instantce.ChamberSettings[StageLevel].MaxChamberCount.ShopChambercount &&
            DungeonGenerator.instance.currentChamberNumber.currentSpecialChambercount >= ChamberManager.Instantce.ChamberSettings[StageLevel].MaxChamberCount.SpecialChambercount &&
            DungeonGenerator.instance.currentChamberNumber.currentTrapChamberCount >= ChamberManager.Instantce.ChamberSettings[StageLevel].MaxChamberCount.TrapChamberCount) &&
            !ValidConnectionTypes.Contains(ValidConnectionType.Chamber))
            {
                GenerateWall();                                         //더이상 생성할 방과 통로가 없을때 남은 ConnectionPoint를 Wall로 막음
                return;
            }
        }

        var value = Random.Range(0, ValidConnectionTypes.Count);

        switch (ValidConnectionTypes[value])
        {
            case ValidConnectionType.Hallway:
                value2 = Random.Range(0, DungeonGenerator.instance.stagesRooms[currentStage].Halls.Count);
                chamber = Instantiate(DungeonGenerator.instance.stagesRooms[currentStage].Halls[value2], transform.position, Quaternion.Euler(0, ConnectionRotation, 0));
                connections = chamber.GetComponentsInChildren<ChamberConnectionPoint>();

                foreach (ChamberConnectionPoint chamberConnectionPoint in connections)
                {
                    chamberConnectionPoint.ConnectionRotation += ConnectionRotation;
                }

                DungeonGenerator.instance.RoomStack.Push(chamber);
                break;

            case ValidConnectionType.Chamber:

                List<ChamberSize> chamberSize = new List<ChamberSize>();

                int value3 = 0;

                if (DungeonGenerator.instance.currentChamberNumber.currentSmallChamberNum < ChamberManager.Instantce.ChamberSettings[StageLevel].MaxChamberCount.SmallChamberCount)
                {
                    chamberSize.Add(ChamberSize.Small);
                }
                if (DungeonGenerator.instance.currentChamberNumber.currentMediumChamberNum < ChamberManager.Instantce.ChamberSettings[StageLevel].MaxChamberCount.MediumChamberCount)
                {
                    chamberSize.Add(ChamberSize.Medium);
                }
                if (DungeonGenerator.instance.currentChamberNumber.currentLargeChamberNum < ChamberManager.Instantce.ChamberSettings[StageLevel].MaxChamberCount.LargeChamberCount)
                {
                    chamberSize.Add(ChamberSize.Large);
                }

                // 1. 모든 일반방 스폰됬어야함
                // 2. 

                if ((DungeonGenerator.instance.currentChamberNumber.currentBossChambercount < ChamberManager.Instantce.ChamberSettings[StageLevel].MaxChamberCount.BossChamberCount ||
                   DungeonGenerator.instance.currentChamberNumber.currentTreasureChamberCount < ChamberManager.Instantce.ChamberSettings[StageLevel].MaxChamberCount.TreasureChamberCount ||
                   DungeonGenerator.instance.currentChamberNumber.currentShopChambercount < ChamberManager.Instantce.ChamberSettings[StageLevel].MaxChamberCount.ShopChambercount ||
                   DungeonGenerator.instance.currentChamberNumber.currentSpecialChambercount < ChamberManager.Instantce.ChamberSettings[StageLevel].MaxChamberCount.SpecialChambercount ||
                   DungeonGenerator.instance.currentChamberNumber.currentTrapChamberCount < ChamberManager.Instantce.ChamberSettings[StageLevel].MaxChamberCount.TrapChamberCount)
                   && chamberSize.Count == 0)
                {

                    if (DungeonGenerator.instance.currentChamberNumber.currentBossChambercount < ChamberManager.Instantce.ChamberSettings[StageLevel].MaxChamberCount.BossChamberCount)
                    {
                        chamberSize.Add(ChamberSize.Boss);
                    }
                    else if (DungeonGenerator.instance.currentChamberNumber.currentTreasureChamberCount < ChamberManager.Instantce.ChamberSettings[StageLevel].MaxChamberCount.TreasureChamberCount)
                    {
                        chamberSize.Add(ChamberSize.Tresure);
                    }
                    else if (DungeonGenerator.instance.currentChamberNumber.currentShopChambercount < ChamberManager.Instantce.ChamberSettings[StageLevel].MaxChamberCount.ShopChambercount)
                    {
                        chamberSize.Add(ChamberSize.Shop);
                    }
                    else if(DungeonGenerator.instance.currentChamberNumber.currentSpecialChambercount < ChamberManager.Instantce.ChamberSettings[StageLevel].MaxChamberCount.SpecialChambercount)
                    {
                        chamberSize.Add(ChamberSize.Special);
                    }
                    else if (DungeonGenerator.instance.currentChamberNumber.currentTrapChamberCount < ChamberManager.Instantce.ChamberSettings[StageLevel].MaxChamberCount.TrapChamberCount)
                    {
                        chamberSize.Add(ChamberSize.Trap);
                    }
                }

                value3 = Random.Range(0, chamberSize.Count);

                if(chamberSize.Count == 0)
                {
                    GenerateWall();
                    return;
                }

                switch (chamberSize[value3])
                {
                    case ChamberSize.Small:
                        value2 = Random.Range(0, DungeonGenerator.instance.stagesRooms[currentStage].SmallChambers.Count);
                        chamber = Instantiate(DungeonGenerator.instance.stagesRooms[currentStage].SmallChambers[value2], transform.position, Quaternion.Euler(0, ConnectionRotation, 0));
                        connections = chamber.GetComponentsInChildren<ChamberConnectionPoint>();

                        foreach (ChamberConnectionPoint chamberConnectionPoint in connections)
                        {
                            chamberConnectionPoint.ConnectionRotation += ConnectionRotation;
                        }

                        DungeonGenerator.instance.currentChamberNumber.currentSmallChamberNum++;
                        DungeonGenerator.instance.ChamberStack.Push(chamber);
                        DungeonGenerator.instance.RoomStack.Push(chamber);
                        break;

                    case ChamberSize.Medium:
                        value2 = Random.Range(0, DungeonGenerator.instance.stagesRooms[currentStage].MediumChambers.Count);
                        chamber = Instantiate(DungeonGenerator.instance.stagesRooms[currentStage].MediumChambers[value2], transform.position, Quaternion.Euler(0, ConnectionRotation, 0));
                        connections = chamber.GetComponentsInChildren<ChamberConnectionPoint>();

                        foreach (ChamberConnectionPoint chamberConnectionPoint in connections)
                        {
                            chamberConnectionPoint.ConnectionRotation += ConnectionRotation;
                        }

                        DungeonGenerator.instance.currentChamberNumber.currentMediumChamberNum++;
                        DungeonGenerator.instance.ChamberStack.Push(chamber);
                        DungeonGenerator.instance.RoomStack.Push(chamber);
                        break;

                    case ChamberSize.Large:
                        value2 = Random.Range(0, DungeonGenerator.instance.stagesRooms[currentStage].LargeChambers.Count);
                        chamber = Instantiate(DungeonGenerator.instance.stagesRooms[currentStage].LargeChambers[value2], transform.position, Quaternion.Euler(0, ConnectionRotation, 0));
                        connections = chamber.GetComponentsInChildren<ChamberConnectionPoint>();

                        foreach (ChamberConnectionPoint chamberConnectionPoint in connections)
                        {
                            chamberConnectionPoint.ConnectionRotation += ConnectionRotation;
                        }

                        DungeonGenerator.instance.currentChamberNumber.currentLargeChamberNum++;
                        DungeonGenerator.instance.ChamberStack.Push(chamber);
                        DungeonGenerator.instance.RoomStack.Push(chamber);
                        break;



                    case ChamberSize.Boss:
                        //마지막 스테이지라면 마지막 보스방으로 생성
                        if (DungeonGenerator.instance.FinalStageNumber <= DungeonGenerator.instance.CurrentStageNumber)
                        {
                            chamber = Instantiate(DungeonGenerator.instance.FinalBossChamber, transform.position, Quaternion.Euler(0, ConnectionRotation, 0));
                            DungeonGenerator.instance.currentChamberNumber.currentBossChambercount++;
                            DungeonGenerator.instance.ChamberStack.Push(chamber);
                            DungeonGenerator.instance.RoomStack.Push(chamber);

                            DungeonGenerator.instance.BossChamber = chamber;
                        }
                        else
                        {
                            value2 = Random.Range(0, DungeonGenerator.instance.stagesRooms[currentStage].BossChambers.Count);
                            chamber = Instantiate(DungeonGenerator.instance.stagesRooms[currentStage].BossChambers[value2], transform.position, Quaternion.Euler(0, ConnectionRotation, 0));
                            DungeonGenerator.instance.currentChamberNumber.currentBossChambercount++;
                            DungeonGenerator.instance.ChamberStack.Push(chamber);
                            DungeonGenerator.instance.RoomStack.Push(chamber);

                            DungeonGenerator.instance.BossChamber = chamber;

                        }

                        break;

                    case ChamberSize.Tresure:
                        value2 = Random.Range(0, DungeonGenerator.instance.stagesRooms[currentStage].TreasureChambers.Count);
                        chamber = Instantiate(DungeonGenerator.instance.stagesRooms[currentStage].TreasureChambers[value2], transform.position, Quaternion.Euler(0, ConnectionRotation, 0));
                        DungeonGenerator.instance.currentChamberNumber.currentTreasureChamberCount++;
                        DungeonGenerator.instance.ChamberStack.Push(chamber);
                        DungeonGenerator.instance.RoomStack.Push(chamber);

                        DungeonGenerator.instance.TreasureChamber = chamber;
                        break;

                    case ChamberSize.Shop:
                        value2 = Random.Range(0, DungeonGenerator.instance.stagesRooms[currentStage].ShopChambers.Count);
                        chamber = Instantiate(DungeonGenerator.instance.stagesRooms[currentStage].ShopChambers[value2], transform.position, Quaternion.Euler(0, ConnectionRotation, 0));
                        DungeonGenerator.instance.currentChamberNumber.currentShopChambercount++;
                        DungeonGenerator.instance.ChamberStack.Push(chamber);
                        DungeonGenerator.instance.RoomStack.Push(chamber);

                        DungeonGenerator.instance.ShopChamber = chamber;

                        /*if(DungeonGenerator.instance.isShopKeeperDead)
                        {
                            Destroy(chamber.GetComponentInChildren<ShopKeeper>().gameObject);
                        }*/
                        break;

                    case ChamberSize.Special:
                        value2 = Random.Range(0, DungeonGenerator.instance.stagesRooms[currentStage].SpecialChambers.Count);
                        chamber = Instantiate(DungeonGenerator.instance.stagesRooms[currentStage].SpecialChambers[value2], transform.position, Quaternion.Euler(0, ConnectionRotation, 0));
                        DungeonGenerator.instance.currentChamberNumber.currentSpecialChambercount++;
                        DungeonGenerator.instance.ChamberStack.Push(chamber);
                        DungeonGenerator.instance.RoomStack.Push(chamber);

                        DungeonGenerator.instance.SpecialChamber = chamber;
                        break;

                    case ChamberSize.Trap:
                        value2 = Random.Range(0, DungeonGenerator.instance.stagesRooms[currentStage].TrapChambers.Count);
                        chamber = Instantiate(DungeonGenerator.instance.stagesRooms[currentStage].TrapChambers[value2], transform.position, Quaternion.Euler(0, ConnectionRotation, 0));
                        DungeonGenerator.instance.currentChamberNumber.currentTrapChamberCount++;
                        DungeonGenerator.instance.ChamberStack.Push(chamber);
                        DungeonGenerator.instance.RoomStack.Push(chamber);

                        DungeonGenerator.instance.TrapChamber = chamber;
                        break;
                }


                break;

            case ValidConnectionType.Corner:
                value2 = Random.Range(0, DungeonGenerator.instance.stagesRooms[currentStage].Corners.Count);
                chamber = Instantiate(DungeonGenerator.instance.stagesRooms[currentStage].Corners[value2], transform.position, Quaternion.Euler(0, ConnectionRotation, 0));
                connections = chamber.GetComponentsInChildren<ChamberConnectionPoint>();

                foreach (ChamberConnectionPoint chamberConnectionPoint in connections)
                {
                    chamberConnectionPoint.ConnectionRotation += ConnectionRotation;
                }
                DungeonGenerator.instance.RoomStack.Push(chamber);
                break;

        }

        DungeonGenerator.instance.allConnectionPoint.Add(this);

        chamber.GetComponentInChildren<ChamberChecking>().parentRoomConnectionPoint = this;
        if (!chamber.GetComponentInChildren<ChamberChecking>().parentRoomConnectionPoint.transform.root.GetComponentInChildren<ChamberChecking>().childernRoom.Contains(chamber.transform.root.GetComponentInChildren<ChamberChecking>()))
            chamber.GetComponentInChildren<ChamberChecking>().parentRoomConnectionPoint.transform.root.GetComponentInChildren<ChamberChecking>().childernRoom.Add(chamber.transform.root.GetComponentInChildren<ChamberChecking>());
        chamber.GetComponentInChildren<ChamberChecking>().parentRoomConnectionPoint.transform.root.GetComponentInChildren<ChamberChecking>().NotAllowDeleteThisRoom = true;

        childChamber = chamber.GetComponentInChildren<ChamberChecking>();
    }

    void GenerateWall()
    {
        //print("벽 생성됨");
        //Wall = DungeonGenerator.instance.WallPrefab;
        //DungeonGenerator.instance.SpawnedWall.Add(Instantiate(Wall, transform.position, Quaternion.Euler(0, ConnectionRotation, 0)));

        if (!DungeonGenerator.instance.WallSpawnedChambers.Contains(thisChamberChecking))
            DungeonGenerator.instance.WallSpawnedChambers.Add(thisChamberChecking);
        return;
    }

    public void CancelEnQueueInvoke()
    {
        CancelInvoke("enQueueThisPoint");
    }
}

public enum ValidConnectionType { Hallway, Chamber, Corner }
public enum ChamberSize {halls, Small, Medium, Large, Boss, Tresure, Shop, Special, Trap}