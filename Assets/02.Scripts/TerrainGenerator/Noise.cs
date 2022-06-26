using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float sampleX = (x + offset.x) * scale;
                float sampleZ = (y + offset.y) * scale;

                float height = Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1;
                //height = Mathf.Round(height * 10) / 10;
                noiseMap[x, y] = height;
            }

        }

        return noiseMap;
    }


}

public struct NoiseSetting
{
    public float scale;
    public int heightMultiply;
    public Vector2 meshOffset;

    public NoiseSetting(float Scale, int hMp, Vector2 offset)
    {
        scale = Scale;
        heightMultiply = hMp;
        meshOffset = offset;
    }
}
