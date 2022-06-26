using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChamberManager : MonoBehaviour
{
    public static ChamberManager Instantce;

    public ChamberGenerateSetting[] ChamberSettings;
    public MiniBossCount[] miniBossCount;

    private void Awake()
    {
        Instantce = this;
    }

    [System.Serializable]
    public class ChamberGenerateSetting
    {
        public int StageLevel;
        public ChamberCount MaxChamberCount;
    }

    [System.Serializable]
    public class ChamberCount
    {
        public int SmallChamberCount = 5;
        public int MediumChamberCount = 2;
        public int LargeChamberCount = 1;
        public int BossChamberCount = 1;
        public int TreasureChamberCount = 1;
        public int ShopChambercount = 1;
        public int SpecialChambercount = 1;
        public int TrapChamberCount = 1;
    }

    [System.Serializable]
    public class MiniBossCount
    {
        public int miniBossCount = 0;
    }
}
