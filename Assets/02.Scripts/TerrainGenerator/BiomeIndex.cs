using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Biome", menuName = "Biome")]
public class BiomeIndex : ScriptableObject
{
    public string BiomeName;
    public BiomeType biomeType;

    public Color GroundColor;
    public List<Sculpture> sculpture = new List<Sculpture>();

    
}
[System.Serializable]
public class Sculpture
{

    public List<GameObject> sculptureGameobject = new List<GameObject>();

    public int population;
    public float minSculptureSize;
    public float maxSculptureSize;

    public GameObject GetSculpture()
    {
        int randomNumber = Random.Range(0, sculptureGameobject.Count);

        return sculptureGameobject[randomNumber];
    }

    public float getMinSculptureSize()
    {
        return minSculptureSize;
    }

    public float getMaxSculptureSize()
    {
        return maxSculptureSize;
    }
}

public enum BiomeType {Grassland, Desert, Snowland,Deadland }