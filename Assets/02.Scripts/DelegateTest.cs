using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateTest : MonoBehaviour {

    #region Singleton

    public static DelegateTest instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found!");
            return;
        }
        instance = this;
    }

    #endregion

    public delegate void somethinghappen();
    public somethinghappen happencallback;

    void happen01()
    {
        Debug.Log("happen01");
        happencallback.Invoke();
    }
}
