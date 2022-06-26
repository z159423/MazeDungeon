using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    Vector3[] vertices;
    int[] triangles;

    const float viewerMoveThresholdForChunkUpdate = 25f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;
    Vector2 viewerPositionOld;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    public const float maxViewDst = 850;        //맵 최대 렌더링 거리
    public bool autoUpdate;
    public bool useFlatShading;
    public Transform viewer;
    public Renderer textureRender;
    public static Vector2 viewerPosition;

    [Range(0, 6)]
    public int levelOfDetail;
    [Range(0, 1)]
    public float scale = 0.01f;
    public Vector2 offSet;
    public int heightMultiply = 1;
    public AnimationCurve heightCurve;

    public bool EnvironmentGenerate = true;

    int chunkSize = 241;
    int chunksVisibleInViewDst;

    public List<Vector2Int> BiomeCenterPoint = new List<Vector2Int>();
    //public List<Color> BiomesColor = new List<Color>();

    //public int regionAmount;
    public Vector2Int BiomesMapSize;

    //Dictionary<Vector2, Vector2Int> BiomeCenterPoints = new Dictionary<Vector2, Vector2Int>();
    public Dictionary<Vector2, BiomePointInfo> BiomeCenterPoints = new Dictionary<Vector2, BiomePointInfo>();

    public int maxGeneratePointDst = 1500;
    int trueMaxGeneratePointDst;
    public int PointGap = 2000;

    public static MapGenerator instance;


    NoiseSetting setting;
    void Start()
    {
        instance = this;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
        trueMaxGeneratePointDst = Mathf.RoundToInt(maxGeneratePointDst / chunkSize);

        if (viewer == null)
            viewer = GameObject.FindWithTag("Player").transform;

        //GenerateBiomePoint();
        updateBiomeCenterPointer();
        updateVisibleChunks();
    }

    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        updateBiomeCenterPointer();
       // updateVisibleChunks();

        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            updateVisibleChunks();
        }
    }

    void updateVisibleChunks()
    {
        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize - 1);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize - 1);

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    if (terrainChunkDictionary[viewedChunkCoord].IsVisible())
                    {
                        terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
                    }
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize - 1, transform, scale, heightMultiply, levelOfDetail, heightCurve, offSet, useFlatShading
                        ,SpawnEnvironment.instance.BiomeList));

                }

            }
        }
    }

    private void updateBiomeCenterPointer()
    {
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / PointGap - 1);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / PointGap - 1);


        for(int y = -trueMaxGeneratePointDst; y < trueMaxGeneratePointDst; y ++)
        {
            for(int x = -trueMaxGeneratePointDst; x < trueMaxGeneratePointDst; x ++)
            {
                Vector2 viewedBiomePointCoord = new Vector2(currentChunkCoordX + x, currentChunkCoordY + y);

                if (BiomeCenterPoints.ContainsKey(viewedBiomePointCoord))
                {

                }
                else
                {
                    Vector2Int BiomeCenterPointVecter2 = new Vector2Int(Random.Range(0, currentChunkCoordX * PointGap), Random.Range(0, currentChunkCoordY * PointGap));
                    Vector2 currentCenterPointVector = new Vector2(currentChunkCoordX + x, currentChunkCoordY + y);
                    BiomeCenterPointVecter2 += new Vector2Int((int)currentCenterPointVector.x * PointGap, (int)currentCenterPointVector.y * PointGap);

                    BiomePointInfo info = new BiomePointInfo();
                    int BiomeType = Random.Range(0, System.Enum.GetNames(typeof(BiomeType)).Length);
                    info.biomeType = (BiomeType)BiomeType;
                    info.PointVector = BiomeCenterPointVecter2;

                    int levelTemp = 0;

                    levelTemp = (int)Vector2.Distance(new Vector2(0,0), currentCenterPointVector);
                    //levelTemp += (currentCenterPointVector.x < 0) ? (int)(currentCenterPointVector.x * -1) : (int)currentCenterPointVector.x;
                    //levelTemp += (currentCenterPointVector.y < 0) ? (int)(currentCenterPointVector.y * -1) : (int)currentCenterPointVector.y;

                    info.BiomeLevel = levelTemp;

                    //BiomeCenterPoints.Add(currentCenterPointVector, BiomeCenterPointVecter2);
                    BiomeCenterPoints.Add(currentCenterPointVector, info);


                    print("지형 포인트 추가 테스트 : " + currentCenterPointVector + " 좌표 : " + BiomeCenterPointVecter2 + " 지형타입 : " + info.biomeType + " 지형레벨 " + info.BiomeLevel);
                }
            }
        }

    }

    public BiomePointInfo GetClosestCentroidIndex(Vector2Int pixelPos)
{
    float smallestDst = float.MaxValue;
    Vector2 Key = new Vector2();


    foreach (KeyValuePair<Vector2, BiomePointInfo> items in BiomeCenterPoints)
    {
        if (Vector2.Distance(pixelPos, items.Value.PointVector) < smallestDst)
        {
            smallestDst = Vector2.Distance(pixelPos, items.Value.PointVector);
            Key = items.Key;
        }
    }

    BiomePointInfo returnType;
    BiomeCenterPoints.TryGetValue(Key, out returnType);
    return returnType;
}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        /*
        for (int i = 0; i < BiomeCenterPoints.Count; i++)
        {
            Gizmos.DrawSphere(new Vector3(BiomeCenterPoints[i].x, 0, ), 10f);
        }*/

        foreach(KeyValuePair<Vector2, BiomePointInfo> items in BiomeCenterPoints)
        {
            Gizmos.DrawSphere(new Vector3(items.Value.PointVector.x, 0, items.Value.PointVector.y), 50f);
        }
    }

    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        MeshCollider meshCollider;
        SpawnEnvironment spawnEnvironment;

        int size;

        public TerrainChunk(Vector2 coord, int size, Transform parent, float scale, int heightMultiply, int levelOfDetail, AnimationCurve animationCurve, Vector2 offset, bool useFlatShading
            , List<BiomeIndex> biomeIndex)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);
            this.size = size;

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = Resources.Load("Mat/White") as Material;

            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;
            meshObject.name = meshObject.transform.position.x + " - " + meshObject.transform.position.z;
            meshObject.tag = "Ground";

            MeshGenerator.GenerateTerrainMesh(size + 1, size + 1, scale, heightMultiply, position, meshFilter, levelOfDetail, animationCurve, offset, useFlatShading);
            meshCollider = meshObject.AddComponent<MeshCollider>();
            meshObject.AddComponent<NavMeshSourceTag>();

            //노이즈맵 기반 텍스쳐 적용
            //meshRenderer.material.mainTexture = TextureGenerator.TextureFromNoiseMap(Noise.GenerateNoiseMap(size + 1, size + 1, scale, position + offset));
            meshRenderer.material.mainTexture = TextureGenerator.TextureFromNoiseMap(Noise.GenerateNoiseMap(size + 1, size + 1, scale, position + offset)
                , new Vector2Int((int)positionV3.x, (int)positionV3.z), biomeIndex, this);

            SpawnEnvironment.terrainChunk2.Add(this);
            //SpawnEnvironment.instance.GenerateEnvironment(this);

            //NavmeshBakeTest.instance.AddNavMeshSurface(meshObject);
            //NavmeshBakeTest.instance.BakeNavMesh(meshObject);
            //SetVisible(false);
        }

        

        public BiomePointInfo GetClosestBiomePoint(Vector3 vector3)
        {
            return null;
        }

        public Vector2 GetPosition()
        {
            return position;
        }

        public void UpdateTerrainChunk() 
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDstFromNearestEdge <= maxViewDst;
            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }

        public Vector2 GetVector()
        {
            return position;
        }

        public int GetSize()
        {
            return size;
        }
    }
}


public class BiomePointInfo
{
    public Vector2Int PointVector;
    public BiomeType biomeType;
    public int BiomeLevel;

}

public class Biome
{
    public List<Vector2Int> BiomeCenterPoint = new List<Vector2Int>();
    public List<Color> BiomesColor = new List<Color>();

    public int BiomeCount;
    public Vector2Int BiomesMapSize;

    Biome(int biomeCount, Vector2Int BiomeMapSize)
    {
        this.BiomeCount = biomeCount;
        this.BiomesMapSize = BiomeMapSize;
    }

}

