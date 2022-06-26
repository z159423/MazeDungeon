using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat {

    [SerializeField]
    private float baseValue = 0;

    [SerializeField]
    private bool minValue = false;
    [SerializeField]
    private float MinValue = 0;

    [SerializeField]
    private bool maxValue = false;
    [SerializeField]
    private float MaxValue = 1000;

    private List<int> IntModifiers = new List<int>();
    private List<float> PercentModifiers = new List<float>();


    public float GetFinalStatValue ()
    {
        float finalValue = baseValue;
        IntModifiers.ForEach(x => finalValue += x);

        float totalPercent = 1;

        PercentModifiers.ForEach(x => totalPercent += x);

        finalValue = finalValue * totalPercent;

        if(maxValue)
            finalValue = Mathf.Clamp(finalValue, 0, MaxValue);

        if (minValue)
            finalValue = Mathf.Clamp(finalValue, MinValue, finalValue);

        return finalValue;
    }

    public float GetFinalStatValueAsMultiflyFloat()
    {
        float finalValue = baseValue;
        IntModifiers.ForEach(x => finalValue += x);

        float totalPercent = 1;

        PercentModifiers.ForEach(x => totalPercent += x);

        totalPercent = Mathf.Clamp(totalPercent, 0, 2);

        finalValue = finalValue * totalPercent;

        if (maxValue)
            finalValue = Mathf.Clamp(finalValue, 0, MaxValue);

        if (minValue)
            finalValue = Mathf.Clamp(finalValue, MinValue, finalValue);

        return finalValue;
    }

    public float GetFinalStatValueAsAddFloat()
    {
        float finalValue = baseValue;
        IntModifiers.ForEach(x => finalValue += x);

        float totalPercent = 0;

        PercentModifiers.ForEach(x => totalPercent += x);

        finalValue += totalPercent;

        if (maxValue)
            finalValue = Mathf.Clamp(finalValue, 0, MaxValue);

        if (minValue)
            finalValue = Mathf.Clamp(finalValue, MinValue, finalValue);

        return finalValue;
    }

    public void AddIntModifier (int modifier)
    {
        if (modifier != 0)
            IntModifiers.Add(modifier);
    }

    public void RemoveIntModifier (int modifier)
    {
        if (modifier != 0)
            IntModifiers.Remove(modifier);
    }

    public void AddPercentModifier (float modifier)
    {
        if (modifier != 0)
            PercentModifiers.Add(modifier);
    }

    public void RemovePercentModifier (float modifier)
    {
        if (modifier != 0)
            PercentModifiers.Remove(modifier);
    }

    public void PlusValueAtFirstValue(float value)
    {
        float result = GetFinalStatValue() + value;
        ClearIntModifier();

        AddIntModifier(Mathf.RoundToInt(result));
    }

    public void ClearIntModifier()
    {
        IntModifiers.Clear();
    }

    public void ResetModifier()
    {
        IntModifiers.Clear();
        PercentModifiers.Clear();
    }

    public void SetBaseValue(float value)
    {
        baseValue = value;
    }

    public void SetMaxValue(float value)
    {
        MaxValue = value;
    }
}
