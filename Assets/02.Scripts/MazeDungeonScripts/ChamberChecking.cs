using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChamberChecking : MonoBehaviour
{
    public bool IsSpawnedChamber = false;

    public bool IsTriggered = false;
    public bool NotAllowDeleteThisRoom = false;

    public bool IsStartingRoom = false;

    public ChamberSize chamberType;
    public ChamberConnectionPoint parentRoomConnectionPoint = null;
    public List<ChamberChecking> childernRoom = new List<ChamberChecking>();

    public List<GameObject> ShowOnMapObject = new List<GameObject>();

    public List<GameObject> nextChamberMarker = new List<GameObject>();

    [Space]
    [SerializeField] private GameObject ChamberIcon;

    private void Start()
    {
        if (IsStartingRoom)
            DungeonGenerator.instance.StartingRoom = transform.root.gameObject;

        var classification = GetComponentsInChildren<MeshRenderer>();

        foreach(MeshRenderer mesh in classification)
        {
            if (mesh.CompareTag("Ceiling"))
                ShowOnMapObject.Add(mesh.gameObject);
        }

        GenerateChamberIcon();
        //ShowOnMapObject = gameObject.find.FindGameObjectsWithTag("Ceiling");
    }

    void addroom()
    {
        if (!DungeonGenerator.instance.RoomStack.Contains(transform.root.gameObject))
            DungeonGenerator.instance.RoomStack.Push(transform.root.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rooms"))
        {
            if (other.GetComponent<ChamberChecking>().IsSpawnedChamber && !IsTriggered && !IsStartingRoom)
            {
                IsTriggered = true;

                print("================ 방이 겹쳐서 삭제함 ===============" + other.transform.root.name + " + " + this.transform.root.name);
                /*print("본인 오브젝트 : " + transform.gameObject.name);
                print("other 오브젝트 : " + transform.gameObject.name);*/
                DungeonGenerator.instance.LastChamberCreateFailed();

                switch (chamberType)
                {
                    case ChamberSize.Small:
                        DungeonGenerator.instance.currentChamberNumber.currentSmallChamberNum--;
                        if (DungeonGenerator.instance.ChamberStack.Count > 0)
                            DungeonGenerator.instance.ChamberStack.Pop();
                        break;

                    case ChamberSize.Medium:
                        DungeonGenerator.instance.currentChamberNumber.currentMediumChamberNum--;
                        if (DungeonGenerator.instance.ChamberStack.Count > 0)
                            DungeonGenerator.instance.ChamberStack.Pop();
                        break;

                    case ChamberSize.Large:
                        DungeonGenerator.instance.currentChamberNumber.currentLargeChamberNum--;
                        if (DungeonGenerator.instance.ChamberStack.Count > 0)
                            DungeonGenerator.instance.ChamberStack.Pop();
                        break;

                    case ChamberSize.Boss:
                        DungeonGenerator.instance.currentChamberNumber.currentBossChambercount--;
                        if (DungeonGenerator.instance.ChamberStack.Count > 0)
                            DungeonGenerator.instance.ChamberStack.Pop();
                        break;

                    case ChamberSize.Tresure:
                        DungeonGenerator.instance.currentChamberNumber.currentTreasureChamberCount--;
                        if (DungeonGenerator.instance.ChamberStack.Count > 0)
                            DungeonGenerator.instance.ChamberStack.Pop();
                        break;

                    case ChamberSize.Shop:
                        DungeonGenerator.instance.currentChamberNumber.currentShopChambercount--;
                        if (DungeonGenerator.instance.ChamberStack.Count > 0)
                            DungeonGenerator.instance.ChamberStack.Pop();
                        break;

                    case ChamberSize.Special:
                        DungeonGenerator.instance.currentChamberNumber.currentSpecialChambercount--;
                        if (DungeonGenerator.instance.ChamberStack.Count > 0)
                            DungeonGenerator.instance.ChamberStack.Pop();
                        break;

                    case ChamberSize.Trap:
                        DungeonGenerator.instance.currentChamberNumber.currentTrapChamberCount--;
                        if (DungeonGenerator.instance.ChamberStack.Count > 0)
                            DungeonGenerator.instance.ChamberStack.Pop();
                        break;
                }
                if (DungeonGenerator.instance.RoomStack.Count > 0)
                    DungeonGenerator.instance.RoomStack.Pop();


                Destroy(transform.root.gameObject);

                //====================================방이 겹칠시 그 방의 parentRoom까지 같이 삭제해서 겹치는것을 거 방치하는 부분========================================
                if (parentRoomConnectionPoint)
                {
                    if (parentRoomConnectionPoint.thisChamberChecking.chamberType == ChamberSize.halls
                    && parentRoomConnectionPoint.thisChamberChecking.childernRoom.Count < 2)
                    {
                        if (parentRoomConnectionPoint.thisChamberChecking.parentRoomConnectionPoint)
                        {
                            //parentRoomConnectionPoint.transform.root.GetComponentInChildren<ChamberChecking>().NotAllowDeleteThisRoom = false;
                            //DungeonGenerator.instance.RoomStack.
                            parentRoomConnectionPoint.thisChamberChecking.parentRoomConnectionPoint.thisChamberChecking.childernRoom.Remove(parentRoomConnectionPoint.thisChamberChecking);

                            //Debug.LogError(DungeonGenerator.instance.ChamberQueue.Count);

                            DungeonGenerator.instance.ChamberQueue.Enqueue(parentRoomConnectionPoint.thisChamberChecking.parentRoomConnectionPoint);
                            //StartCoroutine(parentRoomConnectionPoint.thisChamberChecking.parentRoomConnectionPoint.enQueueThisPoint());

                            //Debug.LogError(DungeonGenerator.instance.ChamberQueue.Count);

                            Destroy(parentRoomConnectionPoint.thisChamberChecking.transform.root.gameObject);


                        }
                    }
                }
            }
        }

        if(other.CompareTag("Player"))
        {
            RevealMap();
        }
    }

    public void RevealMap()
    {
        foreach (GameObject obj in ShowOnMapObject)
        {
            if (obj)
                obj.layer = LayerMask.NameToLayer("ShownOnMap");
        }

        foreach (GameObject obj in nextChamberMarker)
        {
            if (obj)
                obj.layer = LayerMask.NameToLayer("ShownOnMap");
        }

        if(ChamberIcon != null)
            ChamberIcon.layer = LayerMask.NameToLayer("ShownOnMap");
    }

    public void GenerateChamberIcon()
    {
        /*switch(chamberType)
        {
            case ChamberSize.Shop:

                ChamberIcon = Instantiate(PrefabCollect.instance.ShopIcon, transform.position + new Vector3(0,100,0), Quaternion.Euler(90, transform.root.rotation.y, 0), transform.root);

                break;
        }*/

        if(ChamberIcon != null)
        {
            ChamberIcon.transform.rotation = Quaternion.Euler(90, transform.root.rotation.y, 0);
        }
    }

    private void OnDestroy()
    {
        if (parentRoomConnectionPoint != null)
        {
            if (parentRoomConnectionPoint.thisChamberChecking.childernRoom.Contains(this))
                parentRoomConnectionPoint.thisChamberChecking.childernRoom.Remove(this);
        }

        if(DungeonGenerator.instance)
        {
            if (DungeonGenerator.instance.WallSpawnedChambers.Contains(this))
                DungeonGenerator.instance.WallSpawnedChambers.Remove(this);
        }
        

    }
}
