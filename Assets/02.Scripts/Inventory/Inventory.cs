using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    #region Singleton

    public static Inventory instance;

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
    public OnItemChanged onInventoryChangedCallback;

    public delegate void OnCoinChanged();
    public OnCoinChanged OnCoinChangedCallBack;

    public delegate void OnKeyChanged();
    public OnKeyChanged OnKeyChangedCallBack;

    public Transform itemsParent;

    public int space = 20;
    public int coinAmount;
    public int keyAmount;
    public string[] inventoryitemName;

    [SerializeField]
    public List<Item> items = new List<Item>();
    public List<Iteminfo> iteminfos = new List<Iteminfo>();
    [SerializeField]
    public GameObject prefabItem;
    [SerializeField]
    private GameObject prefabPickup;

    public GameObject mesh;

    public Vector3 MeshSize;

    public InventorySlot[] slots;

    public Material[] defaultVertexMat;

    public GameObject Test;
    private void Start()
    {
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
        inventoryitemName = new string[itemsParent.childCount];
    }

    public bool Add(Item item)
    {
        if (items.Count >= space)
        {
            Debug.Log("인벤토리에 자리가 없습니다.");
            InfoMessageManager.instance.PopupInfoMessage("NotEnoughInvenTorySpace");
            return false;
        }
            foreach (InventorySlot slot in slots)
            {
                if (!slot.GetComponentInChildren<DragItem>())
                {
                    //items.Add(item);
                GameObject itemObject = Instantiate(prefabItem, slot.transform);

                //var itemInstance = Item.CreateInstance<Item>();
                //itemInstance = item.getCopy();

                var itemInstance = Instantiate(item) as Item;

                slot.Getcomponent();
                slot.AddItem(itemInstance);
                //itemObject.name = itemInstance.Name;
                itemInstance.onSaleItem = false;

                items.Add(itemInstance);

                DragItem dragitem = itemObject.GetComponent<DragItem>();

                dragitem.thisItem = itemInstance;

                GameObject Mesh = Instantiate(mesh);

                Debug.Log(itemInstance);

                Mesh.GetComponent<MeshFilter>().mesh = itemInstance.skinedMesh.sharedMesh;
                //Mesh.GetComponentInChildren<MeshCollider>().sharedMesh = dragitem.thisItem.mesh.sharedMesh;
                //Mesh.GetComponent<MeshRenderer>().materials = itemInstance.skinedMesh.sharedMaterials;
                Mesh.GetComponent<MeshRenderer>().materials = PrefabCollect.instance.DefaultVertexMatWithStencil;
                Mesh.transform.SetParent(slot.transform.GetChild(0).transform);
                Mesh.transform.localPosition = new Vector3(0, 0, 0);
                Mesh.transform.rotation = new Quaternion(0, 0, 0,0);
                Mesh.transform.localScale = MeshSize;

                itemObject.transform.localScale = new Vector3(1.5f,1.5f,1.5f);

                //item.IndexItemInList = i;

                if (onInventoryChangedCallback != null)
                {
                    onInventoryChangedCallback.Invoke();
                }

                break;
                }
            }            
           
        return true;
    }

    public bool AddOnlyList(Item item)
    {
        if (items.Count >= space)
        {
            Debug.Log("Not enough room.");
            return false;
        }
        for (int i = 0; i < space; i++)
        {
            if (itemsParent.transform.GetChild(i).childCount == 0)
            {
                items.Add(item);

                //itemsParent.transform.GetChild(i).GetComponent<InventorySlot>().AddItem(item);
                break;
            }
        }

        return true;
    }


    public bool Add(Item item, GameObject 아이템오브젝트)
    {
        if (!item.isDefaultItem)
        {
            if (items.Count >= space)
            {
                Debug.Log("Not enough room.");
                return false;
            }
            for (int i = 0; i < space; i++)
            {
                if (itemsParent.transform.GetChild(i).childCount == 0)
                {

                    //items.Add(item);
                    //Instantiate(prefabItem, itemsParent.GetChild(i).transform);
                    아이템오브젝트.transform.SetParent(itemsParent.GetChild(i).transform);
                    아이템오브젝트.transform.position = itemsParent.GetChild(i).transform.position;
                    slots[i].Getcomponent();
                    items.Add(item);
                    slots[i].AddItem(item);

                    아이템오브젝트.GetComponentInChildren<MeshRenderer>().materials = PrefabCollect.instance.DefaultVertexMatWithStencil;
                    //item.IndexItemInList = i;
                    break;
                }
            }

        }
        return true;
    }

    public bool Add(Item item, Iteminfo iteminfo)
    {
        if (!item.isDefaultItem)
        {
            if (items.Count >= space)
            {
                Debug.Log("Not enough room.");
                return false;
            }
            for (int i = 0; i < space; i++)
            {
                if (itemsParent.transform.GetChild(i).childCount == 0)
                {
                    GameObject gotitem = Instantiate(prefabItem, itemsParent.GetChild(i).transform);

                    gotitem.GetComponent<Iteminfo>().name = iteminfo.name;
                    gotitem.GetComponent<Iteminfo>().itemName = iteminfo.name;
                    gotitem.GetComponent<Iteminfo>().itemID = iteminfo.itemID;
                    gotitem.GetComponent<Iteminfo>().itemtype = iteminfo.itemtype;
                    gotitem.GetComponent<Iteminfo>().MaxStack = iteminfo.MaxStack;
                    gotitem.GetComponent<Iteminfo>().mesh = iteminfo.mesh;
                    gotitem.GetComponent<Iteminfo>().icon = iteminfo.icon;
                    gotitem.GetComponent<Iteminfo>().item = iteminfo.item;
                    gotitem.GetComponent<Iteminfo>().itemName = iteminfo.name;
                    gotitem.GetComponent<Iteminfo>().effectiveDose_Hp = iteminfo.effectiveDose_Hp;
                    gotitem.GetComponent<Iteminfo>().effectiveDose_Mp = iteminfo.effectiveDose_Mp;
                    gotitem.GetComponent<Iteminfo>().armorModifier = iteminfo.armorModifier;
                    gotitem.GetComponent<Iteminfo>().damageModifier = iteminfo.damageModifier;
                    gotitem.GetComponent<Iteminfo>().pickupprefab = iteminfo.pickupprefab;


                    //items.Add(item);
                    iteminfos.Add(gotitem.GetComponent<Iteminfo>());
                    slots[i].Getcomponent();
                    items.Add(item);
                    slots[i].AddItem(item);
                    //item.IndexItemInList = i;
                    break;
                }
            }

        }
        return true;
    }

    public bool Add(Iteminfo iteminfo)
    {
        if (!iteminfo.isDefaultItem)
        {
            if (iteminfos.Count >= space)
            {
                Debug.Log("Not enough room.");
                return false;
            }
            for (int i = 0; i < space; i++)
            {
                if (itemsParent.transform.GetChild(i).childCount == 0)
                {

                    iteminfos.Add(iteminfo);
                    break;
                }
            }

        }
        return true;
    }

    public bool Add(Item item, int slotnum)
    {
        if (!item.isDefaultItem)
        {
            if (items.Count >= space)
            {
                Debug.Log("Not enough room.");
                return false;
            }

            if (itemsParent.transform.GetChild(slotnum).childCount == 0)
            {
                Instantiate(prefabItem, itemsParent.GetChild(slotnum).transform);
                //items.Add(item);
                slots[slotnum].Getcomponent();
                items.Add(item);
                slots[slotnum].AddItem(item);
            }
        }
        return true;
    }

    public bool DragItem(Item item)
    {
        if (!item.isDefaultItem)
        {
            if (items.Count >= space)
            {
                Debug.Log("Not enough room.");
                return false;
            }
            for (int i = 0; i < space; i++)
            {
                if (itemsParent.transform.GetChild(i).childCount == 0)
                {
                    //Instantiate(prefabItem, itemsParent.GetChild(i).transform);
                    //items.Add(item);
                    
                    items.Insert(i, item);
                    
                    break;
                }
            }

        }
        return true;
    }

    public void Remove(Item item)
    {
        items.Remove(item);

        print(item);

        foreach(InventorySlot slot in slots)
        {
            if(slot.item == item)
            {
                slot.ClearSlot(true);
                //slot.item = null;
                //Destroy(slot.ItemObject);
                if (onInventoryChangedCallback != null)
                {
                    onInventoryChangedCallback.Invoke();
                }
                return;
            }
        }
    }

    public void RemoveItemObject(Item item)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.GetComponentInChildren<DragItem>())
            {
                if(slot.GetComponentInChildren<DragItem>().thisItem == item)
                {
                    Destroy(slot.GetComponentInChildren<DragItem>().gameObject);
                    print(item);

                    Test = slot.GetComponentInChildren<DragItem>().gameObject;
                    return;
                }
            }
        }

    }
    public void Remove(Item item, Iteminfo iteminfo)
    {
        items.Remove(item);
        iteminfos.Remove(iteminfo);

        if (Inventory.instance.onInventoryChangedCallback != null)
        {
            Inventory.instance.onInventoryChangedCallback.Invoke();
        }
    }

    public void Remove(Iteminfo iteminfo)
    {
        iteminfos.Remove(iteminfo);
    }
    public void Remove(int num)
    {
        items.RemoveAt(num);
    }

    public void InventoryItemNameReset()
    {
        for(int i = 0; i < itemsParent.childCount; i++)
        if(itemsParent.GetChild(i).childCount == 1)
            inventoryitemName[i] =  itemsParent.GetChild(i).GetChild(0).name;
    }

    public void GetCoin(int value)
    {
        coinAmount += value;
        UIManager.instance.CoinAnimator.SetTrigger("GetCoin");
        OnCoinChangedCallBack();
    }

    public void LoseCoin(int value)
    {
        coinAmount -= value;
        OnCoinChangedCallBack();
    }

    public void GetKey(int value)
    {
        keyAmount += value;
        UIManager.instance.KeyAnimator.SetTrigger("GetKey");
        OnKeyChangedCallBack();
    }

    public void UseKey(int value)
    {
        keyAmount -= value;

        keyAmount = Mathf.Clamp(keyAmount, 0, 10000);
        OnKeyChangedCallBack();
    }
}
