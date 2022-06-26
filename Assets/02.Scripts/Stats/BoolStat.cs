using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoolStat
{
    private List<bool> BoolModifiers = new List<bool>();

    public void AddBoolModifier()
    {
        BoolModifiers.Add(true);
    }

    public void RemoveBoolModifier()
    {
        BoolModifiers.Remove(true);
    }

    public void ClearBoolStat()
    {
        BoolModifiers.Clear();
    }

    public bool GetBoolValue()
    {
        foreach(bool boolInList in BoolModifiers)
        {
            return true;
        }
        return false;
    }
}
