using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromNoiseMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(0);

        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(1, 0, noiseMap[x, y]));
            }
        }

        return TextureFromColourMap(colorMap, width, height);

    }

    public static Texture2D TextureFromNoiseMap(float[,] noiseMap, Vector2Int PixelPosition, List<BiomeIndex> BiomeIndex, MapGenerator.TerrainChunk chunk)
    {
        int optimizationValue = 12;

        SpawnEnvironment spawnEnvironment = SpawnEnvironment.instance;

        int width = noiseMap.GetLength(0) / optimizationValue;
        int height = noiseMap.GetLength(0) / optimizationValue;

        Color[] colorMap = new Color[(width) * (height)];

        Dictionary<Vector2Int, BiomeType> BiomeSave = new Dictionary<Vector2Int, BiomeType>();
        List<BiomePointInfo> ClosestPointList = new List<BiomePointInfo>();

        #region
        //List<float> adjacentPointDst = new List<float>();
        //Dictionary<float, Vector2Int> adjacentPointDst2 = new Dictionary<float, Vector2Int>();

        //ClosestPoint = pointerVector2.Values.ToList();

        /*
        for(int i = 0; i < pointerVector2.Count; i++)
        {
            
                ClosestPoint.Add(pointerVector2[i].);
            
        }*/
        /*

        foreach (KeyValuePair<Vector2, Vector2Int> items in pointerVector2)
        {
            
            if (Vector2.Distance(PixelPosition, items.Value) < 1000)
            {
                ClosestPoint.Add(items.Value);
            }
            
            //ClosestPoint.Add(items.Value);
        }
        
    
        foreach (KeyValuePair<Vector2, Vector2Int> items in pointerVector2)
        {
            float chunkToPointDst = Vector2.Distance(PixelPosition, items.Value);
            adjacentPointDst2.Add(chunkToPointDst, items.Value);
        }
        */
        #endregion

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //colorMap[y * width + x] = BiomeIndex[GetClosestCentroidIndex(new Vector2Int(x * optimizationValue + (PixelPosition.x), y * optimizationValue + (PixelPosition.y)), ClosestPoint,)].GroundColor;
                BiomePointInfo biomePointInfo = GetClosestCentroidIndex(new Vector2Int(x * optimizationValue + (PixelPosition.x), y * optimizationValue + (PixelPosition.y)), MapGenerator.instance.BiomeCenterPoints);
                colorMap[y * width + x] = spawnEnvironment.GetColorByType(biomePointInfo.biomeType);

                if (!ClosestPointList.Contains(biomePointInfo))
                    ClosestPointList.Add(biomePointInfo);
                //BiomeSave.Add()
            }
        }

        spawnEnvironment.placementSculpture(chunk, ClosestPointList);

        return TextureFromColourMap(colorMap, width, height);
    }

    public static BiomePointInfo GetClosestCentroidIndex(Vector2Int pixelPos, Dictionary<Vector2, BiomePointInfo> BiomePointInfo)
    {
        float smallestDst = float.MaxValue;
        //int index = 0;
        Vector2 Key = new Vector2();
        /*
        for (int i = 0; i < centroids.Count; i++)
        {
            if (Vector2.Distance(pixelPos, centroids[i]) < smallestDst)
            {
                smallestDst = Vector2.Distance(pixelPos, centroids[i]);
                index = i;
            }
        }
        */

        foreach (KeyValuePair<Vector2, BiomePointInfo> items in BiomePointInfo)
        {
            if(Vector2.Distance(pixelPos,items.Value.PointVector) < smallestDst)
            {
                smallestDst = Vector2.Distance(pixelPos, items.Value.PointVector);
                Key = items.Key;
            }
        }

        BiomePointInfo returnType;
        BiomePointInfo.TryGetValue(Key, out returnType);
        return returnType;
    }

    public static int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        checked
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }

    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }
}

public struct PointStruct
{
    public BiomeType type;
    public Vector2Int pointVector2;
}
