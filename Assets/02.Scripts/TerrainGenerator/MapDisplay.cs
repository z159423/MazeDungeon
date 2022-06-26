using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public int width = 241;
    public int height = 241;
    public float scale = 0.03f;
    public Vector2 offSet;
    public int heightMultiply = 10;
    public bool autoUpdate;
    public bool useFlatShading;
    [Range(0,6)]
    public int levelOfDetail;
    public AnimationCurve heightCurve;

    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void DrawMesh()
    {
        
        MeshGenerator.GenerateTerrainMesh(width, height, scale, heightMultiply, offSet, meshFilter, levelOfDetail, heightCurve, offSet, useFlatShading);
    }
}
