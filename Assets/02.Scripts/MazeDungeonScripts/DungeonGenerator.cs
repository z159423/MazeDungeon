using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityScript.Scripting.Pipeline;
using UnityEngine.AI;
using UnityEngine.UI;

public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator instance;

    [SerializeField] private float currentGenerateTime = 0;
    private bool isMapGenerating = false;
    public float limitGenerateTime = 15;

    public Stages CurrentStage = Stages.Jail;
    public int CurrentStageNumber = 0;
    [Space]
    public int FinalStageNumber = 5;
    public GameObject FinalBossChamber = null;
    [Space]
    public DungeonGenerateTester Tester;
    [Space]
    public bool StartCreateChamber = false;

    public float CreateChamberRepeatTime = 0.1f;

    public GameObject ZeroStageStartingRoom;

    public StagesRoom[] stagesRooms;

    [Space]
    public GameObject WallPrefab = null;
    [Space]
    public GameObject NextChamberMarker;
    [Space]
    public GameObject BronzeChest;
    public GameObject SilverChest;
    public GameObject GoldChest;
    [Space]
    [Range(0, 100)]
    public float bronzeChestper;
    [Range(0, 100)]
    public float silverChestper;
    [Range(0, 100)]
    public float goldChestper;

    public List<ChestPointBeacon> chestPointBeacons = new List<ChestPointBeacon>();
    public List<GameObject> SpawndTresureChest = new List<GameObject>();
    [Space]

    public Queue<ChamberConnectionPoint> ChamberQueue = new Queue<ChamberConnectionPoint>();

    public Stack<GameObject> ChamberStack = new Stack<GameObject>();
    public Stack<GameObject> RoomStack = new Stack<GameObject>();
    public List<ChamberChecking> WallSpawnedChambers = new List<ChamberChecking>();
    public List<ChamberConnectionPoint> NeedWallConnections = new List<ChamberConnectionPoint>();
    public List<GameObject> SpawnedWall = new List<GameObject>();
    public List<ChamberConnectionPoint> allConnectionPoint = new List<ChamberConnectionPoint>();

    public ChamberConnectionPoint dequeueConnectionPoint = null;

    public CurrentChamber currentChamberNumber;                                             //현재 생성된 방 갯수

    //public ChamberManager.ChamberCount ChamberSetting;

    public GameObject BossChamber = null;
    public GameObject TreasureChamber = null;
    public GameObject ShopChamber = null;
    public GameObject SpecialChamber = null;
    public GameObject TrapChamber = null;

    public GameObject StartingRoom = null;

    [Space]
    public List<PlayerSpawnPoint> playerSpawnPoints = new List<PlayerSpawnPoint>();

    [Space]
    public bool isShopKeeperDead = false;

    [Space]
    public bool RegenerateAtFailPathFinding = true;


    [System.Serializable]
    public class StagesRoom
    {
        public string stageName;
        public Stages stage;
        public List<GameObject> StartChambers = new List<GameObject>();
        public List<GameObject> SmallChambers = new List<GameObject>();
        public List<GameObject> MediumChambers = new List<GameObject>();
        public List<GameObject> LargeChambers = new List<GameObject>();
        public List<GameObject> BossChambers = new List<GameObject>();
        public List<GameObject> TreasureChambers = new List<GameObject>();
        public List<GameObject> ShopChambers = new List<GameObject>();
        public List<GameObject> SpecialChambers = new List<GameObject>();
        public List<GameObject> Halls = new List<GameObject>();
        public List<GameObject> Corners = new List<GameObject>();
        public List<GameObject> TrapChambers = new List<GameObject>();
    }

    [System.Serializable]
    public class CurrentChamber
    {
        public int currentSmallChamberNum = 0;
        public int currentMediumChamberNum = 0;
        public int currentLargeChamberNum = 0;
        public int currentBossChambercount = 0;
        public int currentTreasureChamberCount = 0;
        public int currentShopChambercount = 0;
        public int currentSpecialChambercount = 0;
        public int currentTrapChamberCount = 0;

        /// <summary>
        /// 현재 생성된 방 넘버 초기화
        /// </summary>
        public void ClearCurrentChamberNumber()
        {
            currentSmallChamberNum = 0;
            currentMediumChamberNum = 0;
            currentLargeChamberNum = 0;
            currentBossChambercount = 0;
            currentTreasureChamberCount = 0;
            currentShopChambercount = 0; 
            currentSpecialChambercount = 0;
            currentTrapChamberCount = 0;
        }
    }


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (StartCreateChamber)
        {
            //InvokeRepeating("GenerateChamberSequence", 0.2f, CreateChamberRepeatTime);
            StartCoroutine(GenerateChamberSequence());
        }
    }

    private void Update()
    {
        if (isMapGenerating)
            currentGenerateTime += Time.deltaTime;
    }

    private IEnumerator GenerateChamberSequence()
    {
        yield return new WaitForSeconds(0.2f);

        isMapGenerating = true;

        if (Tester.GenerateTest)
            Tester.GenerateTestPanel.SetActive(true);

        while (true)
        {

            if (currentGenerateTime > limitGenerateTime)
            {
                print("스테이지 생성 시간을 초과하여 초기화후 다시 시작합니다.");
                Tester.OverTimeCount++;
                Tester.OverTimeText.text = Tester.OverTimeCount.ToString();
                ReGenerateStage();
                break;
            }

            //print("현재 대기중인 방 커넥션 포인트 개수 = " + ChamberQueue.Count);

            if (ChamberQueue.Count == 0)//===================== 모든 방을 다 만들었을때 ===================
            {
                //BossChamber = ChamberStack.Pop();
                //TreasureChamber = ChamberStack.Pop();

                //CancelInvoke("GenerateChamberSequence");

                MazeDungeonNpcSpawner.instance.SpawnNPCAtTime();
                CreateTresureChest();
                MazeDungeonNpcSpawner.instance.SpawnMimic();

                //Debug.LogError(WallSpawnedChambers.Count);

                foreach (ChamberChecking chamber in WallSpawnedChambers)
                {
                    if (chamber.childernRoom.Count == 0 && chamber.chamberType == ChamberSize.halls && chamber.childernRoom.Count <= 1)
                    {
                        deleteChamber(chamber.transform.root.gameObject);
                    }
                    else
                    {
                        ChamberConnectionPoint[] chamberConnectionPoints = chamber.transform.parent.GetComponentsInChildren<ChamberConnectionPoint>();
                        foreach (ChamberConnectionPoint point in chamberConnectionPoints)
                        {
                            if (!NeedWallConnections.Contains(point) && point.childChamber == null)
                                NeedWallConnections.Add(point);
                        }
                    }
                }

                //Debug.LogError(NeedWallConnections.Count);

                foreach (ChamberConnectionPoint connectionPoint in NeedWallConnections)
                {
                    GenerateWall(connectionPoint);
                }

                StatueSpawnManager.instance.SpawnStatues();                          //석상 생성

                print("모든 방을 생성하였습니다." + ChamberQueue.Count);
                isMapGenerating = false;

                Tester.AverageGenerateTime += currentGenerateTime;
                currentGenerateTime = 0;

                //SkillUI.instance.Skill.SetActive(!SkillUI.instance.Skill.activeSelf);

                if (Loading.instance)
                    Loading.instance.EndLoading();

                Transform StartPoint = GameObject.FindGameObjectWithTag("StartPoint").transform;
                Transform EndPoint = GameObject.FindGameObjectWithTag("EndPoint").transform;

                Tester.Tester = Instantiate(Tester.TesterPrefab, StartPoint.position, StartPoint.rotation);

                bool allChamberGenerated = true;

                if (BossChamber != null && TreasureChamber != null && ShopChamber != null && TrapChamber != null &&
                   currentChamberNumber.currentBossChambercount == ChamberManager.Instantce.ChamberSettings[CurrentStageNumber].MaxChamberCount.BossChamberCount &&
                   currentChamberNumber.currentTreasureChamberCount == ChamberManager.Instantce.ChamberSettings[CurrentStageNumber].MaxChamberCount.TreasureChamberCount &&
                   currentChamberNumber.currentShopChambercount == ChamberManager.Instantce.ChamberSettings[CurrentStageNumber].MaxChamberCount.ShopChambercount &&
                   currentChamberNumber.currentSpecialChambercount == ChamberManager.Instantce.ChamberSettings[CurrentStageNumber].MaxChamberCount.SpecialChambercount &&
                   currentChamberNumber.currentTrapChamberCount == ChamberManager.Instantce.ChamberSettings[CurrentStageNumber].MaxChamberCount.TrapChamberCount)
                {
                    allChamberGenerated = true;
                }
                else
                {
                    Debug.LogError("currentChamberNumber.currentBossChambercount : " + currentChamberNumber.currentBossChambercount +
                        "\n currentChamberNumber.currentTreasureChamberCount : " + currentChamberNumber.currentBossChambercount +
                        "\n currentChamberNumber.currentShopChambercount : " + currentChamberNumber.currentShopChambercount +
                        "\n currentChamberNumber.currentSpecialChambercount : " + currentChamberNumber.currentSpecialChambercount +
                        "\n currentChamberNumber.currentTrapChamberCount : " + currentChamberNumber.currentTrapChamberCount);

                    allChamberGenerated = false;
                    Tester.GereratedChamberCountCheck++;
                    Tester.FailCount++;
                }

                NavmeshBake.instance.BuildNav();                                    //네브매쉬 빌드

                Vector3 Target = EndPoint.position;
                NavMeshAgent navMeshAgent = Tester.Tester.GetComponent<NavMeshAgent>();
                NavMeshPath navMeshPath = new NavMeshPath();

                bool pathFindingSuccess = true;

                if (navMeshAgent.CalculatePath(Target, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    //move to target
                    //navMeshAgent.SetPath(navMeshPath);

                    pathFindingSuccess = true;
                }
                else
                {
                    //Fail condition here

                    pathFindingSuccess = false;
                }

                if (RegenerateAtFailPathFinding && !Tester.GenerateTest && !allChamberGenerated)
                {
                    Debug.LogError("맵 생성 오류로 재생성함 -> RegenerateAtFailPathFinding : " + pathFindingSuccess + "/ allChamberGenerated : " + allChamberGenerated);
                    ReGenerateStage();
                }

                //================================================== 던전 생성 테스터 ===========================================================
                if (Tester.GenerateTest)
                {
                    if (pathFindingSuccess)
                    {
                        Tester.GenerateTestPanel.active = true;
                        Tester.successCount++;
                    }
                    else
                    {
                        Tester.GenerateTestPanel.active = true;
                        Tester.FailCount++;
                    }

                    Tester.SuccessText.text = Tester.successCount.ToString();
                    Tester.FailText.text = Tester.FailCount.ToString();
                    Tester.AverageTimeText.text = (Tester.AverageGenerateTime / (Tester.successCount + Tester.FailCount)).ToString();

                    if (!Tester.StopOnFail.isOn)
                        pathFindingSuccess = true;

                    Tester.GereratedChamberCountCheckText.text = Tester.GereratedChamberCountCheck.ToString();

                    if (Tester.AutoRepeat.isOn && pathFindingSuccess && allChamberGenerated)
                        ReGenerateStage();
                }

                Destroy(Tester.Tester);

                AudioManager.instance.FindBGM("bgm1");

                foreach (Animator animator in BossChamber.GetComponentsInChildren<Animator>())
                {
                    animator.SetTrigger("Close");
                }

                StartCoroutine(GenerateNextChamberMarker());       //방이 생성되고 마커가 활성화 되도록

                if(!PlayStat.instance.PlayTimerOn)
                    StartCoroutine(PlayStat.instance.PlayTimerStart());         //모든방이 생성된 후 플레이 타임 측정

                if (ShopChamber != null && !DungeonGenerator.instance.isShopKeeperDead)            //상점주인 모든 방이 생성되고 활성화 되도록
                    ShopChamber.GetComponentInChildren<Shop>().EnableShopKeeper();

                if(BossChamber != null)             //마지막 보스가 모든 방이 생성되고 활성화 되도록
                {
                    if (BossChamber.GetComponentInChildren<KingsChamber>() != null)
                        BossChamber.GetComponentInChildren<KingsChamber>().EnableBoss();
                }

                MapUI.ChangeDungeonCurrentStageNumber(CurrentStageNumber);

                

                break;
            }

            if (RoomStack.Count > 0)
            {
                if (RoomStack.Peek() == null)
                {
                    RoomStack.Pop();
                }

                if (RoomStack.Peek())
                {
                    if (RoomStack.Peek().GetComponentInChildren<ChamberChecking>())
                        RoomStack.Peek().GetComponentInChildren<ChamberChecking>().IsSpawnedChamber = true;
                }

                if (RoomStack.Peek())
                {
                    if (!RoomStack.Peek().GetComponentInChildren<ChamberChecking>().parentRoomConnectionPoint.transform.root.GetComponentInChildren<ChamberChecking>().childernRoom.Contains(RoomStack.Peek().GetComponentInChildren<ChamberChecking>()))
                        RoomStack.Peek().GetComponentInChildren<ChamberChecking>().parentRoomConnectionPoint.transform.root.GetComponentInChildren<ChamberChecking>().childernRoom.Add(RoomStack.Peek().GetComponentInChildren<ChamberChecking>());
                    //RoomStack.Peek().GetComponentInChildren<ChamberChecking>().parentRoomConnectionPoint.transform.root.GetComponentInChildren<ChamberChecking>().NotAllowDeleteThisRoom = true;
                }
            }


            var lastConnectionPoint = ChamberQueue.Dequeue();
            dequeueConnectionPoint = lastConnectionPoint;
            if (lastConnectionPoint != null)
                lastConnectionPoint.GenerateChamber();

            yield return new WaitForSeconds(CreateChamberRepeatTime);
        }
    }

    public void LastChamberCreateFailed()
    {
        ChamberQueue.Enqueue(dequeueConnectionPoint);
    }

    public void MoveToNextStage()
    {
        DestroyAllRoom();
        
    }

    public void DestroyAllRoom()
    {
        while (true)
        {
            if (RoomStack.Count > 0)
            {
                var room = RoomStack.Pop();
                Destroy(room);
            }
            else
            {
                print("남은 방의 개수는 0개입니다.");
                break;
            }
        }
    }
    private void CreateTresureChest()
    {
        for(int i = 0; i < chestPointBeacons.Count; i++)
        {
            if (chestPointBeacons[i] == null)
                continue;

            List<float> p = new List<float>();

            p.Add(bronzeChestper);
            p.Add(silverChestper);
            p.Add(goldChestper);

            var result = choose(p);

            GameObject chest = null;

            if (result == 0)
            {
                SpawndTresureChest.Add(chest = Instantiate(BronzeChest, chestPointBeacons[i].transform.position, chestPointBeacons[i].transform.rotation));
            }
            else if (result == 1)
            {
                SpawndTresureChest.Add(chest = Instantiate(SilverChest, chestPointBeacons[i].transform.position, chestPointBeacons[i].transform.rotation));
            }
            else if (result == 2)
            {
                SpawndTresureChest.Add(chest = Instantiate(GoldChest, chestPointBeacons[i].transform.position, chestPointBeacons[i].transform.rotation));
            }

            chest.transform.parent = chestPointBeacons[i].transform.root;
        }
        
    }

    float choose(List<float> probs)
    {
        float total = 0;

        foreach (float elem in probs)
        {
            total += elem;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < probs.Count; i++)
        {
            if (randomPoint < probs[i])
            {
                return i;
            }
            else
            {
                randomPoint -= probs[i];
            }
        }
        return probs.Count - 1;
    }

    void deleteChamber(GameObject chamberObj)
    {
        ChamberChecking parentChamber = chamberObj.GetComponentInChildren<ChamberChecking>().parentRoomConnectionPoint.transform.root.GetComponentInChildren<ChamberChecking>();
        parentChamber.childernRoom.Remove(chamberObj.GetComponentInChildren<ChamberChecking>());
        if (parentChamber.childernRoom.Count == 0 && parentChamber.chamberType == ChamberSize.halls && parentChamber.childernRoom.Count <= 1)
        {
            deleteChamber(parentChamber.transform.root.gameObject);
        }
        else
        {
            NeedWallConnections.Add(chamberObj.GetComponentInChildren<ChamberChecking>().parentRoomConnectionPoint);
        }
        Destroy(chamberObj);
    }

    void GenerateWall(ChamberConnectionPoint point)
    {
        print("벽 생성됨");

        var wall = Instantiate(WallPrefab, point.transform.position, Quaternion.Euler(0, point.ConnectionRotation, 0), point.transform);
        wall.transform.localPosition = new Vector3(0, 0, 0);
        SpawnedWall.Add(wall);
        if (!WallSpawnedChambers.Contains(transform.root.GetComponentInChildren<ChamberChecking>()))
            WallSpawnedChambers.Add(transform.root.GetComponentInChildren<ChamberChecking>());
        return;
    }

    public void EndStage()
    {
        ClearStage();
        GenerateNextStage(true);
        MazeDungeonNpcSpawner.instance.ClearStage();
    }

    public void ReGenerateStage()
    {
        //CancelInvoke("GenerateChamberSequence");

        ClearStage();
        MazeDungeonNpcSpawner.instance.ClearStage();
        /*foreach(ChamberConnectionPoint point in StartingRoom.GetComponentsInChildren<ChamberConnectionPoint>())
        {
            point.enQueueThisPoint();
        }*/
        GenerateNextStage(false);
    }

    public void ClearStage()
    {
        Loading.instance.StartLoading();

        currentGenerateTime = 0;

        currentChamberNumber.ClearCurrentChamberNumber();

        /*foreach (GameObject wall in SpawnedWall)
        {
            Destroy(wall);
        }*/
        SpawnedWall.Clear();

        foreach (GameObject room in RoomStack)
        {
            Destroy(room);
        }
        ChamberStack.Clear();
        RoomStack.Clear();

        Destroy(StartingRoom);

        WallSpawnedChambers.Clear();
        NeedWallConnections.Clear();

        NavmeshBake.instance.DeleteNavData();

        DropHeartPool.instance.EnQueueEntireDropHeart();

        //Invoke("GenerateNextStage", 0.1f);
    }

    void GenerateNextStage(bool goNextStage)
    {

        if (CurrentStageNumber == 2)
            CharacterClassManager.ClearStage3();

        if(goNextStage)
        {
            CurrentStageNumber++;
        }
        
        ChamberQueue.Clear();
        int currentStage = 0;

        for (int i = 0; i > stagesRooms.Length; i++)
        {
            if (stagesRooms[i].stage == CurrentStage)
            {
                currentStage = i;
                break;
            }
        }

        playerSpawnPoints.Clear();

        if(CurrentStageNumber == 0)
        {
            StartingRoom = Instantiate(ZeroStageStartingRoom, transform.position, Quaternion.Euler(0, 0, 0));
        }
        else
        {
            int value2 = Random.Range(0, stagesRooms[currentStage].StartChambers.Count);
            StartingRoom = Instantiate(stagesRooms[currentStage].StartChambers[value2], transform.position, Quaternion.Euler(0, 0, 0));
        }

        var psps = StartingRoom.GetComponentsInChildren<PlayerSpawnPoint>();
        foreach(PlayerSpawnPoint psp in psps)
        {
            psp.AddPlayerSpawnPoint();
        }

        foreach (ChamberConnectionPoint point in StartingRoom.GetComponentsInChildren<ChamberConnectionPoint>())
        {
            //point.enQueueThisPoint();
            DungeonGenerator.instance.ChamberQueue.Enqueue(point);
            point.reGeneratedRoom = true;
        }

        for (int i = 0; i < PlayerManager.instance.Players.Count; i++)
        {
            PlayerManager.instance.Players[i].playerobject.GetComponentInChildren<Rigidbody>().isKinematic = true;
            int Rnum = Random.Range(0, playerSpawnPoints.Count);
            PlayerManager.instance.Players[i].playerobject.GetComponentInChildren<PlayerController01>().transform.position = playerSpawnPoints[Rnum].transform.position;
            print("캐릭터 리스폰 포인트 : " + playerSpawnPoints[Rnum].transform.position);
            PlayerManager.instance.Players[i].playerobject.GetComponentInChildren<Rigidbody>().isKinematic = false;

            PlayerManager.instance.Players[i].playerobject.GetComponentInChildren<CharacterBuffDeBuff>().UntilStageEndBuffEnd();
        }

        if (StartCreateChamber)
            StartCoroutine(GenerateChamberSequence());
        //InvokeRepeating("GenerateChamberSequence", 0.2f, CreateChamberRepeatTime);
    }

    IEnumerator GenerateNextChamberMarker()
    {
        yield return new WaitForSeconds(1f);

        foreach (var connectionPoint1 in allConnectionPoint)
        {
            if(connectionPoint1 != null)
            {
                if (connectionPoint1.childChamber != null)
                {
                    var marker = Instantiate(NextChamberMarker, connectionPoint1.transform.position + (Vector3.down * 5), Quaternion.Euler(0, connectionPoint1.ConnectionRotation, 0), connectionPoint1.transform);
                    if (connectionPoint1.transform.root.GetComponentInChildren<ChamberChecking>())
                        connectionPoint1.transform.root.GetComponentInChildren<ChamberChecking>().nextChamberMarker.Add(marker);
                }
            }
        }
    }

    public bool GetDungeonGenerating()
    {
        return isMapGenerating;
    }
}
