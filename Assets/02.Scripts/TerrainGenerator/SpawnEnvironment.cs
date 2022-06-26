using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnvironment : MonoBehaviour
{

    #pragma warning disable 0618
    public static SpawnEnvironment instance;

    public List<BiomeIndex> BiomeList = new List<BiomeIndex>();
    
    public EnvironmentList List;

    public static Dictionary<Vector2, MapGenerator.TerrainChunk> terrainChunk = new Dictionary<Vector2, MapGenerator.TerrainChunk>();
    public static List<MapGenerator.TerrainChunk> terrainChunk2 = new List<MapGenerator.TerrainChunk>();

    public static int frequency;
    public int treeCount = 1;
    public int RockCount = 1;

    public bool GenerateTree;
    public bool GenerateRock;

    
    private void Awake()
    {
        instance = this;
    }

    public void GenerateEnvironment(MapGenerator.TerrainChunk chunk)
    {
        if (GenerateTree == true)
        {
            placementTree(chunk, List, treeCount);
        }
        else
        {
            Debug.Log("나무생성이 false로 되어 있습니다.");
        }

        if (GenerateRock == true)
        {
            placementRock(chunk, List, RockCount);
        }
        else
        {
            Debug.Log("돌생성이 false로 되어 있습니다.");
        }
    }

    public Color GetColorByType(BiomeType type)
    {
        for(int i = 0; i < BiomeList.Count; i++)
        {
            if (type == BiomeList[i].biomeType)
                return BiomeList[i].GroundColor;
        }
        return new Color(0, 0, 0, 0);
    }

    public Sculpture GetSculptureByType(BiomeType type)
    {
        for(int i = 0; i < BiomeList.Count; i++)
        {
            if(BiomeList[i].biomeType == type)
            {
                return BiomeList[i].sculpture[0];
            }
        }

        return null;
    }

    public List<Sculpture> GetSculptureListByType(BiomeType type)
    {
        for (int i = 0; i < BiomeList.Count; i++)
        {
            if (BiomeList[i].biomeType == type)
            {
                return BiomeList[i].sculpture;
            }
        }

        return null;
    }

    public void placementSculpture(MapGenerator.TerrainChunk chunk, List<BiomePointInfo> biomePointInfo)
    {
        Vector3 chunkdata = chunk.GetVector();
        Vector3 chunkCenter = new Vector3(chunkdata.x, 0, chunkdata.y);
        int chunkSize = chunk.GetSize();
        int PopulationCount = 1;
        int SculputureCount = 1;
        //int halfChunkSize = chunkSize / 2 - 1;

        //print(biomePointInfo.Count);
        for (int k = 0; k < SculputureCount; k++)
        {
            for (int i = 0; i < PopulationCount; i++)
            {
                Vector3 randomV3 = new Vector3(Random.Range(0, chunkSize), 0, Random.Range(0, chunkSize));

                RaycastHit hit;
                if (Physics.Raycast(chunkCenter + randomV3 + new Vector3(0, 1000f, 0), Vector3.down, out hit, 2000))
                {

                    if (hit.transform.tag == "Ground")
                    {
                        int RandomRotation = Random.Range(0, 4);
                        float shortDist = float.MaxValue;
                        BiomeType type = BiomeType.Grassland;

                        for (int j = 0; j < biomePointInfo.Count; j++)
                        {
                            float num = Vector2.Distance(biomePointInfo[j].PointVector, hit.transform.position);
                            if (num < shortDist)
                            {
                                shortDist = num;
                                type = biomePointInfo[j].biomeType;
                            }
                        }
                        Sculpture sculpture = GetSculptureByType(type);
                        List<Sculpture> sculptureList = new List<Sculpture>();

                        sculptureList = GetSculptureListByType(type);
                        SculputureCount = sculptureList.Count;
                        PopulationCount = sculptureList[k].population;
                        if (SculputureCount <= 0)
                            break;

                        if (PopulationCount <= 0)
                            break;

                        GameObject gameObject = Instantiate(sculptureList[k].GetSculpture(), hit.point, Quaternion.Euler(0, RandomRotation * 90, 0)) as GameObject;
                        gameObject.gameObject.transform.SetParent(hit.transform);

                        float RValue = Random.Range(sculptureList[k].minSculptureSize, sculptureList[k].maxSculptureSize);
                        gameObject.transform.localScale = new Vector3(RValue, RValue, RValue);

                        gameObject.tag = "Environment";
                        if(gameObject.GetComponentInChildren<MeshCollider>() != null)
                            gameObject.GetComponentInChildren<MeshCollider>().gameObject.AddComponent<NavMeshSourceTag>();
                        //gameObject.AddComponent<NavMeshSourceTag>();

                    }

                }
            }
        }
    }

    public void placementTree(MapGenerator.TerrainChunk chunk, EnvironmentList list, int treeCount)
    {
        Vector3 chunkdata = chunk.GetVector();
        Vector3 chunkCenter = new Vector3(chunkdata.x, 0, chunkdata.y);
        int chunkSize = chunk.GetSize();
        int halfChunkSize = chunkSize / 2 - 1;

        for (int i = 0; i < treeCount; i++)
        {
            Vector3 randomV3 = new Vector3(Random.Range(0, chunkSize), 0, Random.Range(0, chunkSize));

            RaycastHit hit;
            if (Physics.Raycast(chunkCenter + randomV3 + new Vector3(0, 1000f, 0), Vector3.down, out hit, 2000))
            {

                if (hit.transform.tag == "Ground")
                {
                    int RandomRotation = Random.Range(0, 4);

                    GameObject gameObject = Instantiate(list.GetTree(Random.Range(0, list.GetTreeListSize())), hit.point, Quaternion.Euler(0, RandomRotation * 90, 0)) as GameObject;
                    gameObject.gameObject.transform.SetParent(hit.transform);

                    int RValue = Random.Range(list.minTreeSize, list.maxTreeSize);
                    gameObject.transform.localScale = new Vector3(RValue, RValue, RValue);

                    gameObject.tag = "Environment";
                    //gameObject.AddComponent<NavMeshSourceTag>();
                }

            }
        }

    }

    public void placementRock(MapGenerator.TerrainChunk chunk, EnvironmentList list, int rockCount)
    {
        Vector3 chunkdata = chunk.GetVector();
        Vector3 chunkCenter = new Vector3(chunkdata.x, 0, chunkdata.y);
        int chunkSize = chunk.GetSize();
        int halfChunkSize = chunkSize / 2 - 1;

        for (int i = 0; i < rockCount; i++)
        {
            Vector3 randomV3 = new Vector3(Random.Range(0, chunkSize), 0, Random.Range(0, chunkSize));

            RaycastHit hit;
            if (Physics.Raycast(chunkCenter + randomV3 + new Vector3(0, 1000f, 0), Vector3.down, out hit, 2000))
            {
                if (hit.transform.tag == "Ground")
                {
                    GameObject gameObject = Instantiate(list.GetRock(Random.Range(0, list.GetRockListSize())), hit.point, Quaternion.EulerAngles(0, Random.rotation.y, 0)) as GameObject;
                    gameObject.gameObject.transform.SetParent(hit.transform);

                    int RValue = Random.Range(list.minRockSize, list.maxRockSize);
                    gameObject.transform.localScale = new Vector3(RValue, RValue, RValue);
                    gameObject.AddComponent<NavMeshSourceTag>();

                    gameObject.tag = "Environment";
                }
            }
        }

    }
}
