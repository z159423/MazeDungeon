using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour {

    #region Singleton

    public static EquipmentManager instance;        //변수 선언부

    private void Awake()        //변수 초기화부
    {
        instance = this;

        int numSlots = System.Enum.GetNames(typeof(ItemType)).Length;
        currentEquipment = new Item[numSlots];
        currentSkinnedMeshes = new SkinnedMeshRenderer[numSlots];
    }

    #endregion

    public Item[] defaultItems;
    public SkinnedMeshRenderer targetMesh;
    public Item[] currentEquipment;
    //public Iteminfo[] itemInfo;
    //public Iteminfo[] currentEquipment2;
    public SkinnedMeshRenderer[] currentSkinnedMeshes;
    public GameObject Equipment;

    public PlayerController01 playerController;

    public List<GameObject> equipmentSlotList = new List<GameObject>();

    public delegate void OnEquipmentChanged(Item newItem, Item oldItem);
    public OnEquipmentChanged onEquipmentChanged;

    public delegate void OnEquipmentChanged2(EquipmentManager manager);
    public OnEquipmentChanged2 onEquipmentChanged2;

    Inventory inventory;

    EquipmentUI EUI;

    private void Start()
    {
        EUI = EquipmentUI.instance;
        inventory = Inventory.instance;

        EquipDefaultItems();
        targetMesh = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<SkinnedMeshRenderer>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerController01>();

        onEquipmentChanged += OnEquipmentChange;
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.U))
            UnequipAll();*/

        if (targetMesh == null)
        {
            targetMesh = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<SkinnedMeshRenderer>();
        }
    }

    public void Equip (Item newItem)
    {
        int slotIndex = (int)newItem.itemtype;
        print(slotIndex);
        Unequip(slotIndex);
        Item oldItem = null;

        print(newItem.Name + " 장착함");

        if(currentEquipment[slotIndex] != null)
        {
            oldItem = currentEquipment[slotIndex];
            inventory.Add(oldItem);
        }

        EUI.EquipItem(newItem);

        currentEquipment[slotIndex] = newItem;
        if (newItem.itemtype != ItemType.Accessory)
        {
            SkinnedMeshRenderer newMesh = Instantiate<SkinnedMeshRenderer>(newItem.skinedMesh);
            newMesh.transform.parent = targetMesh.transform;
            newMesh.gameObject.layer = 2;

            for (int i = 0; i < newMesh.materials.Length; i++)
            {
                newMesh.materials[i] = Instantiate<Material>(newMesh.materials[i]);     //매테리얼 인스턴트화
            }

            newMesh.bones = targetMesh.bones;
            newMesh.rootBone = targetMesh.rootBone;
            currentSkinnedMeshes[slotIndex] = newMesh;
            newMesh.updateWhenOffscreen = true;
        }

        GameObject itemObject = Instantiate(Inventory.instance.prefabItem, Equipment.transform.GetChild((int)(newItem.itemtype)).transform);

        var itemInstance = Instantiate(newItem) as Item;
        itemObject.name = itemInstance.Name;
        DragItem dragitem = itemObject.GetComponent<DragItem>();
        dragitem.thisItem = newItem;
        GameObject Mesh = Instantiate(Inventory.instance.mesh);
        Debug.Log(itemInstance);
        Mesh.GetComponent<MeshFilter>().mesh = itemInstance.skinedMesh.sharedMesh;
        Mesh.GetComponent<MeshRenderer>().materials = itemInstance.skinedMesh.sharedMaterials;
        Mesh.transform.SetParent(itemObject.transform);
        Mesh.transform.localPosition = new Vector3(0, 0, 0);
        Mesh.transform.rotation = new Quaternion(0, 0, 0, 0);
        Mesh.transform.localScale = Inventory.instance.MeshSize;

        print(Mesh.transform.parent);

        itemObject.transform.localScale = new Vector3(1, 1, 1);

        itemObject.GetComponent<DragItem>().thisItem.name = itemInstance.Name;

        if (onEquipmentChanged != null)
        {
            onEquipmentChanged.Invoke(newItem, oldItem);
        }

        if (onEquipmentChanged != null)
        {
            onEquipmentChanged2.Invoke(this);
        }
    }

    public void LeftDaggerEquip(Item newItem)
    {
        int slotIndex = (int)ItemType.SecondaryWeapon;
        Unequip(slotIndex);
        Item oldItem = null;

        if (currentEquipment[slotIndex] != null)
        {
            oldItem = currentEquipment[slotIndex];
            inventory.Add(oldItem);
        }

        if (onEquipmentChanged != null)
        {
            onEquipmentChanged.Invoke(newItem, oldItem);
        }

        EUI.EquipItem(newItem);

        currentEquipment[slotIndex] = newItem;


        SkinnedMeshRenderer newMesh = Instantiate<SkinnedMeshRenderer>(newItem.daggerLeftMesh);
        newMesh.transform.parent = targetMesh.transform;

        newMesh.bones = targetMesh.bones;
        newMesh.rootBone = targetMesh.rootBone;
        currentSkinnedMeshes[slotIndex] = newMesh;


        if (onEquipmentChanged != null)
        {
            onEquipmentChanged2.Invoke(this);
        }
    }
  
    public Item Unequip (int slotIndex)
    {
        Debug.Log("아이템 장착해제");
        if(currentEquipment[slotIndex] != null)
        {
            Debug.Log(currentEquipment[slotIndex]);

            if (currentSkinnedMeshes[slotIndex] != null)
            {
                Destroy(currentSkinnedMeshes[slotIndex].gameObject);
            }

            //Iteminfo oldItem = currentEquipment2[slotIndex];
            Item olditem2 = currentEquipment[slotIndex];

            //inventory.Add(olditem2);

            currentEquipment[slotIndex] = null;

            //EUI.UnEquipItem(slotIndex);

            if (onEquipmentChanged != null)
            {
                onEquipmentChanged.Invoke(null, olditem2);
            }

            if (onEquipmentChanged != null)
            {
                onEquipmentChanged2.Invoke(this);
            }

            if (equipmentSlotList[slotIndex].GetComponentInChildren<DragItem>() != null)
            {
                Destroy(equipmentSlotList[slotIndex].GetComponentInChildren<DragItem>().gameObject);
            }

            //Destroy(Equipment.transform.GetChild(slotIndex).GetComponentInChildren<DragItem>().transform.gameObject);
            return olditem2;
        }
        return null;
        
    }

    public Item UnequipWithNotDestroyItemObject(int slotIndex)
    {
        Debug.Log("아이템 장착해제");
        if (currentEquipment[slotIndex] != null)
        {
            if (currentSkinnedMeshes[slotIndex] != null)
            {
                Destroy(currentSkinnedMeshes[slotIndex].gameObject);
            }

            Item olditem2 = currentEquipment[slotIndex];

            currentEquipment[slotIndex] = null;

            if (onEquipmentChanged != null)
            {
                onEquipmentChanged.Invoke(null, olditem2);
            }

            if (onEquipmentChanged != null)
            {
                onEquipmentChanged2.Invoke(this);
            }
            return olditem2;
        }
        return null;

    }

    public void DestroyEquipment(GameObject player)
    {
        for(int i = 0; i < currentEquipment.Length; i++)
        {
            if (currentEquipment[i] != null)
            {
                if (currentEquipment[i].currentLimit <= 0 && currentEquipment[i].canBreakable == true)
                {
                    Unequip(i);
                    player.GetComponentInParent<AudioSource>().PlayOneShot(SoundManager.instance.WeaponBroken);
                    print("무기가 부셔졌습니다. " + i);
                }
            }
        }
    }
    #region NotUse
    /*
    public Iteminfo Unequip2(int slotIndex)
    {

        if (itemInfo[slotIndex] != null)
        {
            if (currentMeshes[slotIndex] != null)
            {
                Destroy(currentMeshes[slotIndex].gameObject);
            }

            Iteminfo oldItem = itemInfo[slotIndex];

            currentEquipment[slotIndex] = null;

            itemInfo[slotIndex] = null;

            EUI.UnEquipItem(slotIndex);
            Debug.Log("Unequip2 작동");
            if (onEquipmentChanged != null)
            {
                //onEquipmentChanged.Invoke(null, oldItem);
            }
            return oldItem;
        }
        return null;

    }
    */
    /*
  public void Equip2(Iteminfo newItem)
  {
      int slotIndex = (int)newItem.itemtype;
      //Unequip(slotIndex);
      Iteminfo oldItem = null;

      if (itemInfo[slotIndex] != null)
      {
          oldItem = currentEquipment2[slotIndex];
          inventory.Add(oldItem);
      }

      if (onEquipmentChanged != null)
      {
          //onEquipmentChanged.Invoke(newItem, oldItem);
      }

      itemInfo[slotIndex] = newItem;

      newItem.gameObject.transform.SetParent(Equipment.transform.GetChild(slotIndex).transform);

      //EUI.EquipItem(newItem);
      //itemInfo[slotIndex] = newItem;
      SkinnedMeshRenderer newMesh = Instantiate<SkinnedMeshRenderer>(newItem.mesh);
      newMesh.transform.parent = targetMesh.transform;

      newMesh.bones = targetMesh.bones;
      newMesh.rootBone = targetMesh.rootBone;
      currentMeshes[slotIndex] = newMesh;

      Debug.Log(newItem.itemName + "장착");
  }

  public void Equip3(Item newItem)
  {
      int slotIndex = (int)newItem.itemtype;
      Unequip(slotIndex);
      Item oldItem = null;

      if (currentEquipment[slotIndex] != null)
      {
          oldItem = currentEquipment[slotIndex];
          inventory.Add(oldItem);
      }

      if (onEquipmentChanged != null)
      {
          onEquipmentChanged.Invoke(newItem, oldItem);
      }

      //EUI.EquipItem(newItem);

      currentEquipment[slotIndex] = newItem;
      SkinnedMeshRenderer newMesh = Instantiate<SkinnedMeshRenderer>(newItem.skinedMesh);
      newMesh.transform.parent = targetMesh.transform;

      newMesh.bones = targetMesh.bones;
      newMesh.rootBone = targetMesh.rootBone;
      currentMeshes[slotIndex] = newMesh;
  }
  */
    #endregion
    public void UnequipAll()
    {
        for(int i = 0; i < currentEquipment.Length; i++)
        {
            Unequip(i);
            EUI.UnEquipItem(i);
        }
        EquipDefaultItems();
    }

    void EquipDefaultItems()
    {
        foreach(Item item in defaultItems)
        {
            Equip(item);
        }
    }

    void OnEquipmentChange(Item newItem, Item oldItem)
    {
        if(currentEquipment[1])
        {
            playerController.DefaultCloat.GetComponent<SkinnedMeshRenderer>().enabled = false;
            playerController.Body.active = true;

        }
        else
        {
            playerController.DefaultCloat.GetComponent<SkinnedMeshRenderer>().enabled = true;
            playerController.Body.active = false;
        }

        if (currentEquipment[2])
        {
            playerController.Foot1.GetComponent<SkinnedMeshRenderer>().enabled = false;
            playerController.Foot2.GetComponent<SkinnedMeshRenderer>().enabled = false;
        }
        else
        {
            playerController.Foot1.GetComponent<SkinnedMeshRenderer>().enabled = true;
            playerController.Foot2.GetComponent<SkinnedMeshRenderer>().enabled = true;
        }
    }

    public void onEquipmentChange()
    {
        if (onEquipmentChanged != null)
        {
            onEquipmentChanged2.Invoke(this);
        }
    }
}
