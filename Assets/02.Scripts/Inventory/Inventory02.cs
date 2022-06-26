using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory02 : MonoBehaviour {

    #region Singleton

    public static Inventory02 instance;

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

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public static int space = 20;

    [SerializeField]
    public Item[] items = new Item[space];

    public bool Add(Item newitem)
    {
        for (int i = 0; i < space; i++)
        {
            if (items[i] != null)
            {
                items[i] = newitem;
                return false;
            }
        }
        return true;
    }

    public void Remove(Item item)
    {
        items[0] = null;

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }
}
