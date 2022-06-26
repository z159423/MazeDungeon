using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnvironmentList", menuName = "Environment/List")]
public class EnvironmentList : ScriptableObject
{
    [SerializeField]
    public List<GameObject> TreeList = new List<GameObject>();

    public int minTreeSize;
    public int maxTreeSize;

    [SerializeField]
    public List<GameObject> RockList = new List<GameObject>();

    public int minRockSize;
    public int maxRockSize;

    public List<GameObject> CactusList = new List<GameObject>();

    public int minCactusSize;
    public int maxCactusSize;

    public int GetTreeListSize()
    {
        return TreeList.Count;
    }

    public GameObject GetTree(int index)
    {
        return TreeList[index];
    }

    public int GetRockListSize()
    {
        return RockList.Count;
    }

    public GameObject GetRock(int index)
    {
        return RockList[index];
    }



}
