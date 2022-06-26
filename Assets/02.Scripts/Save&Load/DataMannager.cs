using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using LitJson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class DataMannager : MonoBehaviour
{
    [SerializeField]
    public class Iteminfo
    {
        public string ItemName;
        public int itemID;
        public int itemPosition;

        public Iteminfo(string itemname,int itemid, int itemposition)
        {
            this.ItemName = itemname;
            this.itemID = itemid;
            this.itemPosition = itemposition;
        }
    }

    public class Data
    {
        public string playerName;

        public double playerPositionX;
        public double playerPositionY;
        public double playerPositionZ;

        public double playerRotationX;
        public double playerRotationY;
        public double playerRotationZ;

        public int playerLvl;
        public int PlayerMaxExp;
        public double playerMaxHp;
        public double playerMaxMp;
        public double playerMaxStamina;

        public int playerCurrentExp;
        public double playerCurrentHp;
        public double playerCurrentMp;
        public double playerCurrentStamina;

        public List<Iteminfo> inventoryIteminfo = new List<Iteminfo>();
        public List<Iteminfo> equipmentIteminfo = new List<Iteminfo>();

        //public List<Item> itemInInventoryList = new List<Item>();
        //public Item[] itemEquipmentInfo = new Item[System.Enum.GetNames(typeof(ItemType)).Length];
    }

    Data DataFile = new Data();

    PlayerStats playerstats;

    EquipmentManager equipmentManager = EquipmentManager.instance;

    Inventory inventory = Inventory.instance;

    private GameObject UI;
    private EquipmentUI equipmentUI;

    public ItemDataBase itemDataBase;

    public static DataMannager instance;

    private void Start()
    {
        playerstats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        equipmentManager = EquipmentManager.instance.GetComponent<EquipmentManager>();
        inventory = Inventory.instance.GetComponent<Inventory>();
        UI = GameObject.FindGameObjectWithTag("UI");
        equipmentUI = EquipmentUI.instance;
        instance = this;
    }



    public void DataValue()
    {
        DataFile.playerName = playerstats.transform.name;
        
        DataFile.playerPositionX = playerstats.transform.position.x;
        DataFile.playerPositionY = playerstats.transform.position.y;
        DataFile.playerPositionZ = playerstats.transform.position.z;

        DataFile.playerRotationX = playerstats.transform.rotation.x;
        DataFile.playerRotationY = playerstats.transform.rotation.y;
        DataFile.playerRotationZ = playerstats.transform.rotation.z;

        DataFile.playerLvl = playerstats.PlayerLvl;
        DataFile.PlayerMaxExp = (int)playerstats.getExpSlider().maxValue;
        DataFile.playerMaxHp = playerstats.getHpSlider().maxValue;
        DataFile.playerMaxMp = playerstats.getMpSlider().maxValue;
        DataFile.playerCurrentExp = (int)playerstats.playerCurrentExp;
        DataFile.playerCurrentHp = playerstats.currentHealth;
        DataFile.playerCurrentMp = playerstats.playerCurrentMp;
        DataFile.playerCurrentStamina = playerstats.playerCurrentStamina;

        inventory.InventoryItemNameReset();
        for (int i = 0; i < inventory.itemsParent.childCount; i++)
        {
            int itempositionnum = 0;

                if (inventory.itemsParent.GetChild(i).childCount == 1)  //인벤토리 itemsParent.GetChild(j)의 자식수가 1개일때 실행
                {
                    Debug.Log(inventory.itemsParent.GetChild(i).GetChild(0).name + " =? " + inventory.inventoryitemName[i]);

                    if (inventory.itemsParent.GetChild(i).GetChild(0).name == inventory.inventoryitemName[i])
                        itempositionnum = i;

                    DataFile.inventoryIteminfo.Add(new Iteminfo(inventory.inventoryitemName[i], inventory.itemsParent.GetChild(i).GetChild(0).GetComponent<DragItem>().thisItem.itemID, itempositionnum));
                }   


            
        }

        for (int i = 0; i < equipmentManager.currentEquipment.Length; i++)
        {
            if(equipmentManager.currentEquipment[i] != null)
            {
                DataFile.equipmentIteminfo.Add(new Iteminfo(equipmentManager.currentEquipment[i].Name,equipmentManager.currentEquipment[i].itemID, 0));
            }
        }

       
    }

    public void Save()
    {
        Debug.Log("저장하기");

        DataValue();

        //string save = JsonUtility.ToJson(DataFile);

        //Debug.Log(save);

        string SaveData = ObjectToJson(DataFile);

        File.WriteAllText(Application.dataPath + "/Resources/ItemData.json", SaveData);

    }

    public void Load()
    {
        Debug.Log("불러오기");

        string JsonString = File.ReadAllText(Application.dataPath + "/Resources/ItemData.json");

        Data loadData = JsonToOject<Data>(JsonString);

        playerstats.transform.position = new Vector3((float)loadData.playerPositionX, (float)loadData.playerPositionY, (float)loadData.playerPositionZ);
        playerstats.transform.rotation = new Quaternion((float)loadData.playerRotationX, (float)loadData.playerRotationY, (float)loadData.playerRotationZ, 0);

        playerstats.PlayerLvl = loadData.playerLvl;
        playerstats.getExpSlider().maxValue = loadData.PlayerMaxExp;
        playerstats.playerCurrentExp = loadData.playerCurrentExp;
        playerstats.currentHealth = (int)loadData.playerCurrentHp;
        playerstats.playerCurrentMp = (float)loadData.playerCurrentMp;
        playerstats.playerCurrentStamina = (float)loadData.playerCurrentStamina;

        for (int i = 0; i < loadData.inventoryIteminfo.Count; i++)
        {
            inventory.Add(itemDataBase.GetItemByID(loadData.inventoryIteminfo[i].itemID), loadData.inventoryIteminfo[i].itemPosition);
        }

        for(int i = 0; i < loadData.equipmentIteminfo.Count; i++)
        {
            equipmentManager.Equip(itemDataBase.GetItemByID(loadData.equipmentIteminfo[i].itemID));

            Item item = itemDataBase.GetItemByID((int)loadData.equipmentIteminfo[i].itemID);
            GameObject Item = Instantiate(inventory.prefabItem, UI.transform.GetChild(1).GetChild(0).transform.GetChild((int)item.itemtype));
            Item.name = item.Name;
            Item.GetComponent<DragItem>().Get_Componenet();
            Item.GetComponent<DragItem>().thisItem = item;
            Item.GetComponent<DragItem>().image.sprite = item.icon;
        }
        

        Debug.Log(loadData);

        /*
        for (int i = 0; i < SaveData.Count; i++)
        {
            Debug.Log(SaveData[i].ToString());
        }
        */
        //playerstats.PlayerLvl = SaveData.
    }

    string ObjectToJson(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    T JsonToOject<T>(string jsonData)
    {
        return JsonConvert.DeserializeObject<T>(jsonData);
    }

    /*
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.items;
        }
​
    public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.items = array;
            return JsonUtility.ToJson(wrapper);
        }
​
    public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }
​
        [SerializeField]
        private class Wrapper<T>
        {
            public T[] items;
        }
    }
    */
}