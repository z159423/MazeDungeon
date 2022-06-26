using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#pragma warning disable 0414

public class DragItem : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerDownHandler ,IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    public Item thisItem;
    public GameObject pickupprefab;
    public GameObject Equipmentparent;
    public GameObject viewDetail;
    public Image image;

    private GameObject UI;
    private GameObject mesh;
    private UIManager allUI;
    private Vector2 pointerOffset;
    private Vector2 toolTipPotition;
    private RectTransform canvasRectTransform;
    private RectTransform rectTransform;
    private RectTransform viewDetailRect;
    private Transform draggedItemBox;
    private GameObject oldSlot;
    private Item oldItem;
    private Inventory inventory;
    private Button button;
    private CanvasGroup canvasGroup;
    private Iteminfo iteminfo;
    private Camera ui_Camera;

    private bool PointerEnter = false;
    Quaternion newrotation = Quaternion.Euler(0, 10000000000, 0);

    public string itemLocationName;

    // Use this for initialization
    void Start()
    {
        ui_Camera = GameObject.FindWithTag("UI_Camera").GetComponent<Camera>();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        draggedItemBox = GameObject.FindGameObjectWithTag("DraggingBox").transform;
        inventory = Inventory.instance;
        if(transform.parent.GetComponent<InventorySlot>() != null)
            thisItem = transform.parent.GetComponent<InventorySlot>().item;
        //pickupprefab = thisItem.pickupprefab;
        image = transform.GetChild(0).GetComponent<Image>();
        button = transform.GetComponent<Button>();
        UI = GameObject.FindGameObjectWithTag("UI");
        iteminfo = transform.GetComponent<Iteminfo>();
        mesh = GetComponentInChildren<MeshFilter>().gameObject;
        viewDetail = UI.transform.Find("ToolTip").gameObject;
        allUI = UIManager.instance;

        Canvas canvas = GetComponentInParent<Canvas>();
        viewDetailRect = viewDetail.GetComponent<RectTransform>();
        //If the canvas is active we instantiate the variables
        if (canvas != null)
        {
            canvasRectTransform = canvas.transform as RectTransform;          //instantiated
        }

        mesh.transform.localPosition += new Vector3(0, 0, -10);

        if (thisItem.itemtype == ItemType.consumable)
            mesh.transform.localScale = mesh.transform.localScale * 1.5f;
    }

    void Update()
    {
        itemLocationName = transform.parent.name;

        if (PointerEnter == true)
        {
            //mesh.transform.Rotate(Vector3.down * 2);      //아이템위에 마우스커서를 올려두면 천천히 회전
        }
    }

    private void OnDisable()
    {
        if(ToolTip.instance.gameObject.activeSelf)
        {
            ToolTip.instance.HideToolTip();
        }
        
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData data)
    {
        oldSlot = transform.parent.gameObject;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        PointerEnter = true;
        ItemPickUpToolTip.instance.isCheckingInventoryItem = true;
        ItemPickUpToolTip.instance.GetItemInfo(thisItem, true);
        ItemPickUpToolTip.instance.ShowToolTip();
        
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData data)
    {
        PointerEnter = false;
        ItemPickUpToolTip.instance.isCheckingInventoryItem = false;
        GameObject mesh = GetComponentInChildren<MeshFilter>().gameObject;
        mesh.transform.rotation = new Quaternion(0, 0, 0,0);
        ItemPickUpToolTip.instance.HideToolTip();
        
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {

    }

    void IPointerUpHandler.OnPointerUp(PointerEventData data)
    {
        if (data.button == PointerEventData.InputButton.Right)
        {
            GameObject mesh = GetComponentInChildren<MeshFilter>().gameObject;
            mesh.transform.rotation = new Quaternion(0, 0, 0, 0);
            ItemPickUpToolTip.instance.HideToolTip();

            if (thisItem.itemtype == ItemType.consumable) //아이템이 만약 소모성 아이템이면
            {
                Debug.Log(iteminfo.itemName + "소모품사용");

                //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>().Usingpotion(thisItem.effectiveDose_Hp, thisItem.effectiveDose_Mp);
                bool succese = thisItem.Use();
                if (thisItem.consumableType == ConsumableType.SkillBook)
                {
                    if (succese == true)
                    {
                        ToolTip.instance.HideToolTip();
                        Destroy(transform.gameObject);
                        Inventory.instance.Remove(thisItem);
                    }
                    else if (succese == false)
                    {
                        Debug.Log("이미 배운 스킬입니다.");
                    }
                }
                else if (thisItem.consumableType == ConsumableType.Potion)
                {
                    if (succese)
                    {
                        ToolTip.instance.HideToolTip();
                        Destroy(transform.gameObject);
                        Inventory.instance.Remove(thisItem);
                    }
                    else
                    {
                        print("Full HP라 소모품을 사용하지 못했습니다.");
                    }

                }
            }
            else if (transform.parent.GetComponent<EquipmentSlots>() != null)
            {
                //EquipmentManager.instance.Unequip2((int)transform.GetComponent<Iteminfo>().itemtype);
                var success = Inventory.instance.Add(thisItem);

                if (success)
                {
                    EquipmentManager.instance.Unequip((int)transform.parent.GetComponent<EquipmentSlots>().equipSlot);

                    ToolTip.instance.HideToolTip();
                }


                /*foreach (InventorySlot slot in Inventory.instance.slots)
                {
                    if (!slot.GetComponentInChildren<DragItem>())
                    {
                        Debug.Log("장착해제");
                        //transform.SetParent(inventory.itemsParent.transform.GetChild(i).transform);
                        transform.position = transform.parent.position;
                        slot.CopyValuesFromItem(thisItem);
                        Destroy(transform.gameObject);
                        
                        break;
                    }
                }*/

            }
            else if (EquipmentManager.instance.currentEquipment[(int)thisItem.itemtype] != null)        //이미 작용중인 아이템이 있는경우
            {
                /*if (thisItem.weaponType == WeaponType.Dagger && EquipmentManager.instance.currentEquipment[(int)thisItem.itemtype].weaponType == WeaponType.Dagger)
                {
                    if (EquipmentManager.instance.currentEquipment[(int)ItemType.SecondaryWeapon] != null) // 만약 장비창 방패칸에 이미 장착중인 아이템이 있는경우
                    {
                        Inventory.instance.Add(EquipmentManager.instance.currentEquipment[(int)ItemType.SecondaryWeapon]);
                        EquipmentManager.instance.Unequip((int)ItemType.SecondaryWeapon);
                        Destroy(EquipmentManager.instance.Equipment.transform.GetChild((int)ItemType.SecondaryWeapon).GetComponentInChildren<DragItem>().gameObject);
                    }
                    ToolTip.instance.HideToolTip();
                    Inventory.instance.Remove(thisItem);
                    EquipmentManager.instance.LeftDaggerEquip(thisItem);
                    transform.SetParent(EquipmentManager.instance.Equipment.transform.GetChild((int)ItemType.SecondaryWeapon).transform);
                    transform.position = EquipmentManager.instance.Equipment.transform.GetChild((int)ItemType.SecondaryWeapon).transform.position;
                    //Destroy(gameObject);
                }
                else if (thisItem.weaponType != WeaponType.Dagger && (EquipmentManager.instance.currentEquipment[(int)thisItem.itemtype] != null
                    || EquipmentManager.instance.currentEquipment[(int)ItemType.SecondaryWeapon] != null) && thisItem.itemtype == ItemType.Weapon)
                {
                    ToolTip.instance.HideToolTip();
                    Inventory.instance.Remove(thisItem);

                    if (EquipmentManager.instance.currentEquipment[(int)ItemType.SecondaryWeapon] != null && thisItem.weaponType != WeaponType.Sword)
                    {
                        Inventory.instance.Add(EquipmentManager.instance.currentEquipment[(int)ItemType.SecondaryWeapon]);
                        EquipmentManager.instance.Unequip((int)ItemType.SecondaryWeapon);
                        Destroy(EquipmentManager.instance.Equipment.transform.GetChild((int)ItemType.SecondaryWeapon).GetComponentInChildren<DragItem>().gameObject);

                    }

                    if (EquipmentManager.instance.currentEquipment[(int)ItemType.Weapon] != null)
                    {
                        Inventory.instance.Add(EquipmentManager.instance.currentEquipment[(int)ItemType.Weapon]);
                        Destroy(EquipmentManager.instance.Equipment.transform.GetChild((int)ItemType.Weapon).GetComponentInChildren<DragItem>().gameObject);
                        EquipmentManager.instance.Unequip((int)ItemType.Weapon);
                    }

                    EquipmentManager.instance.Equip(thisItem);
                    Destroy(gameObject);
                    //transform.SetParent(EquipmentManager.instance.Equipment.transform.GetChild((int)thisItem.itemtype));
                    //transform.position = EquipmentManager.instance.Equipment.transform.GetChild((int)thisItem.itemtype).transform.position;
                }
                else if (thisItem.weaponType == WeaponType.Dagger && EquipmentManager.instance.currentEquipment[(int)ItemType.Weapon] != null)
                {
                    if(EquipmentManager.instance.currentEquipment[(int)ItemType.SecondaryWeapon] != null)
                    {
                        //Inventory.instance.Add(EquipmentManager.instance.currentEquipment[(int)ItemType.SecondaryWeapon]);
                        //EquipmentManager.instance.Unequip((int)ItemType.SecondaryWeapon);
                        //Destroy(EquipmentManager.instance.Equipment.transform.GetChild((int)ItemType.SecondaryWeapon).GetComponentInChildren<DragItem>().gameObject);
                    }

                    Transform oldSlot = transform.parent.transform;
                    Debug.Log("장착하고 있던" + EquipmentManager.instance.currentEquipment[(int)thisItem.itemtype].Name + "장착해제");

                    GameObject currentEquipment = EquipmentManager.instance.Equipment.transform.GetChild((int)thisItem.itemtype).GetComponentInChildren<DragItem>().gameObject;

                    //transform.SetParent(draggedItemBox);

                    Inventory.instance.Add(EquipmentManager.instance.currentEquipment[(int)thisItem.itemtype], currentEquipment);
                    Inventory.instance.Remove(thisItem);

                    //transform.SetParent(EquipmentManager.instance.Equipment.transform.GetChild((int)thisItem.itemtype).transform);
                    //transform.position = EquipmentManager.instance.Equipment.transform.GetChild((int)thisItem.itemtype).transform.position;

                    EquipmentManager.instance.Unequip((int)EquipmentManager.instance.currentEquipment[(int)thisItem.itemtype].itemtype);
                    EquipmentManager.instance.Equip(thisItem);

                    Destroy(gameObject);
                }*/
                //else
                //{
                ToolTip.instance.HideToolTip();
                Transform oldSlot = transform.parent.transform;
                Debug.Log("장착하고 있던" + EquipmentManager.instance.currentEquipment[(int)thisItem.itemtype].Name + "장착해제");

                GameObject currentEquipment = EquipmentManager.instance.Equipment.transform.GetChild((int)thisItem.itemtype).GetComponentInChildren<DragItem>().gameObject;

                transform.SetParent(draggedItemBox);

                Inventory.instance.Remove(thisItem);
                Inventory.instance.Add(EquipmentManager.instance.currentEquipment[(int)thisItem.itemtype], currentEquipment);

                //transform.SetParent(EquipmentManager.instance.Equipment.transform.GetChild((int)thisItem.itemtype).transform);
                //transform.position = EquipmentManager.instance.Equipment.transform.GetChild((int)thisItem.itemtype).transform.position;

                EquipmentManager.instance.Unequip((int)EquipmentManager.instance.currentEquipment[(int)thisItem.itemtype].itemtype);
                EquipmentManager.instance.Equip(thisItem);

                for (int i = 0; i < Inventory.instance.space; i++)
                {
                    if (Inventory.instance.itemsParent.transform.GetChild(i).childCount == 0)
                    {
                        break;
                    }
                }

                Destroy(gameObject);
                //}
            }
            else if (thisItem.weaponType == WeaponType.Bow && EquipmentManager.instance.currentEquipment[(int)ItemType.SecondaryWeapon] != null) //활 오른쪽 클릭으로 장착 관련
            {
                print("방패창에 장비가 있는 상태에서 활을 오른쪽 클릭으로 장착함");
                Inventory.instance.Add(EquipmentManager.instance.currentEquipment[(int)ItemType.SecondaryWeapon]);
                EquipmentManager.instance.Unequip((int)ItemType.SecondaryWeapon);
                Destroy(EquipmentManager.instance.Equipment.transform.GetChild((int)ItemType.SecondaryWeapon).GetComponentInChildren<DragItem>().gameObject);

                EquipmentManager.instance.Equip(thisItem);
                //transform.SetParent(EquipmentManager.instance.Equipment.transform.GetChild((int)thisItem.itemtype));
                //transform.position = EquipmentManager.instance.Equipment.transform.GetChild((int)thisItem.itemtype).transform.position;
            }
            else if (thisItem.weaponType == WeaponType.Dagger)   //담검을 오른쪽으로 장착하고 쉴드칸에 단검이 아닌 장비가 장착중일때
            {
                EquipmentManager.instance.Equip(thisItem);
                Inventory.instance.Remove(thisItem);
                //transform.SetParent(EquipmentManager.instance.Equipment.transform.GetChild((int)thisItem.itemtype));
                //transform.position = EquipmentManager.instance.Equipment.transform.GetChild((int)thisItem.itemtype).transform.position;

                if (EquipmentManager.instance.currentEquipment[(int)ItemType.SecondaryWeapon] != null)
                {
                    if (EquipmentManager.instance.currentEquipment[(int)ItemType.SecondaryWeapon].secondaryWeaponType != SecondaryWeaponType.HandCrossBow)
                    {
                        print("담검을 오른쪽으로 장착하고 쉴드칸에 단검이 아닌 장비가 장착중일때");
                        Inventory.instance.Add(EquipmentManager.instance.currentEquipment[(int)ItemType.SecondaryWeapon]);
                        EquipmentManager.instance.Unequip((int)ItemType.SecondaryWeapon);
                        Destroy(EquipmentManager.instance.Equipment.transform.GetChild((int)ItemType.SecondaryWeapon).GetComponentInChildren<DragItem>().gameObject);
                    }
                }

                Destroy(gameObject);
            }
            else if (thisItem.itemtype == ItemType.SecondaryWeapon)
            {
                if (EquipmentManager.instance.currentEquipment[(int)ItemType.Weapon] != null)
                {
                    if (thisItem.secondaryWeaponType == SecondaryWeaponType.HandCrossBow)
                    {

                    }
                    else if (EquipmentManager.instance.currentEquipment[(int)ItemType.Weapon].weaponType != WeaponType.Sword &&
                       EquipmentManager.instance.currentEquipment[(int)ItemType.Weapon].weaponType != WeaponType.Wand)
                    {
                        print(EquipmentManager.instance.currentEquipment[(int)ItemType.Weapon].Name + " 장책해제");
                        Inventory.instance.Add(EquipmentManager.instance.currentEquipment[(int)ItemType.Weapon]);
                        EquipmentManager.instance.Unequip((int)ItemType.Weapon);
                        Destroy(EquipmentManager.instance.Equipment.transform.GetChild((int)ItemType.Weapon).GetComponentInChildren<DragItem>().gameObject);
                    }
                }
                print("쉴드 장착");
                ToolTip.instance.HideToolTip();
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out pointerOffset);
                oldSlot = transform.parent.gameObject;
                //transform.SetParent(allUI.equipment.transform.GetChild(0).GetChild((int)thisItem.itemtype));
                EquipmentManager.instance.Equip(thisItem);

                transform.position = transform.parent.position;
                inventory.Remove(thisItem);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log(thisItem + " 사용함");
                resetRotation();
                ToolTip.instance.HideToolTip();
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out pointerOffset);
                oldSlot = transform.parent.gameObject;
                //transform.SetParent(allUI.equipment.transform.GetChild(0).GetChild((int)thisItem.itemtype));
                EquipmentManager.instance.Equip(thisItem);

                transform.position = transform.parent.position;
                inventory.Remove(thisItem);
                CleaningInventorySlot(thisItem);

                Destroy(gameObject);
            }
        }
    }

    public void OnDrag(PointerEventData data)
    {
        //Debug.Log(data.pointerEnter); 
        Transform newSlot = null;
        if (rectTransform == null)
            newSlot = data.pointerEnter.transform;

        if (data.button == PointerEventData.InputButton.Left)
        {

            if (transform.GetComponentInParent<EquipmentSlots>() != null)
            {
                EquipmentManager.instance.UnequipWithNotDestroyItemObject((int)transform.parent.GetComponent<EquipmentSlots>().equipSlot);
            }

            transform.SetParent(draggedItemBox);

            rectTransform.SetAsLastSibling();
            //if (transform.parent.GetComponent<InventorySlot>() != null)
            //transform.parent.GetComponent<InventorySlot>().ClearSlot();
            
            //inventory.Remove(thisItem);                         //인벤토리에서 아이템 드래그하면 아이템 사라지게 하는 함수
            canvasGroup.blocksRaycasts = false;

            image.enabled = true;
            image.sprite = thisItem.icon;
            //draggedItemBox.GetChild(0).position = Input.mousePosition;

            Vector2 localPointerPosition;

            draggedItemBox.GetChild(0).localPosition = new Vector3(0,0,0);

            GetComponentInChildren<MeshRenderer>().materials = PrefabCollect.instance.DefaultVertexMat;

            //draggedItemBox.GetChild(0).position = ui_Camera.ScreenToViewportPoint();
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, PadCursor.instance.GetCurrentCursorPosition(), data.pressEventCamera, out localPointerPosition))
            {
                draggedItemBox.localPosition = localPointerPosition - pointerOffset;        //아이템 드래그시 아이템이 마우스 위치로 이동하게 함
                //Debug.Log(canvasRectTransform + " " + Input.mousePosition + " " + data.pressEventCamera + " " + localPointerPosition);
            }

            EquipmentManager.instance.onEquipmentChange();
        }
    }

    public void OnEndDrag(PointerEventData data)
    {
        if (data.button == PointerEventData.InputButton.Left)
        {
            if (data.pointerEnter == null)
            {
                Debug.Log(thisItem + " 를 밖으로 버림");

                GameObject dropitem = Instantiate(pickupprefab, GameObject.FindGameObjectWithTag("Player").transform.position + (GameObject.FindGameObjectWithTag("Player").transform.forward * 2) + new Vector3(0, 2, 0), Quaternion.identity);
                dropitem.gameObject.name = thisItem.Name;
                dropitem.GetComponent<ItemPickup>().item = thisItem;
                dropitem.GetComponent<ItemPickup>().name = thisItem.name;

                if (thisItem.skinedMesh == null)
                {
                    dropitem.GetComponentInChildren<MeshFilter>().mesh = thisItem.mesh2;
                    dropitem.GetComponentInChildren<MeshCollider>().sharedMesh = thisItem.mesh2;

                }
                else
                {
                    dropitem.GetComponentInChildren<MeshFilter>().mesh = thisItem.skinedMesh.sharedMesh;
                    dropitem.GetComponentInChildren<MeshCollider>().sharedMesh = thisItem.skinedMesh.sharedMesh;
                    dropitem.GetComponentInChildren<MeshRenderer>().materials = thisItem.skinedMesh.sharedMaterials;
                }

                Inventory.instance.Remove(thisItem);
                Destroy(this.gameObject);

                if (Inventory.instance.onInventoryChangedCallback != null)
                {
                    Inventory.instance.onInventoryChangedCallback.Invoke();
                }

            }
            else if (data.button == PointerEventData.InputButton.Left)
            {
                //image.enabled = false;
                //image.sprite = null;
                canvasGroup.blocksRaycasts = true;

                Transform newSlot = null;

                if (data.pointerEnter != null)
                    newSlot = data.pointerEnter.transform;

                Debug.Log(newSlot + "위에 내려놓음");

                if (newSlot != null && data.pointerEnter.tag == "Slot")
                {

                    Debug.Log("인벤토리 아이템 위치를 변경함");

                    //transform.SetParent(oldSlot.transform);
                    //inventory.DragItem(thisItem);
                    transform.SetParent(newSlot.transform);
                    //transform.parent.GetComponent<InventorySlot>().Getcomponent();
                    //transform.parent.GetComponent<InventorySlot>().AddItem(thisItem);

                    if (oldSlot.GetComponent<EquipmentSlots>() != null)
                    {
                        if (thisItem.weaponType == WeaponType.Dagger && oldSlot.GetComponent<EquipmentSlots>().equipSlot == ItemType.SecondaryWeapon)
                        {
                            EquipmentManager.instance.Unequip((int)ItemType.SecondaryWeapon);
                        }
                        else
                        {
                            EquipmentManager.instance.Unequip((int)thisItem.itemtype);
                        }
                    }

                    Transform iconrect = transform.GetChild(0).transform;
                    iconrect.SetAsLastSibling();

                    Vector3 pos = transform.parent.position;
                    transform.position = pos;
                    if (oldSlot.tag == "EquipmentSlot")
                    {
                        inventory.AddOnlyList(thisItem);
                    }

                    if(oldSlot.GetComponent<InventorySlot>())
                    {
                        InventorySlot tempSlot = new InventorySlot();

                        tempSlot.CopyValuesFromOtherSlot(oldSlot.GetComponent<InventorySlot>());
                        oldSlot.GetComponent<InventorySlot>().ClearSlot(false);
                        newSlot.GetComponent<InventorySlot>().CopyValuesFromOtherSlot(tempSlot);
                    }else if(oldSlot.GetComponent<EquipmentSlots>())
                    {
                        newSlot.GetComponent<InventorySlot>().CopyValuesFromItem(thisItem);
                    }

                    GetComponentInChildren<MeshRenderer>().materials = PrefabCollect.instance.DefaultVertexMatWithStencil;
                    GetComponentInChildren<Image>().enabled = true;
                    //button.enabled = true;
                }
                else if (data.pointerEnter.tag == "Icon" && oldSlot.tag == "Slot")//아이템이 있으면 위치를 변경
                {

                    if (data.pointerEnter.transform.parent.parent.tag == "EquipmentSlot" && thisItem.itemtype == data.pointerEnter.transform.parent.parent.GetComponent<EquipmentSlots>().equipSlot)
                    {
                        GameObject 장착하고있던아이템 = data.pointerEnter.transform.parent.transform.gameObject;

                        transform.SetParent(data.pointerEnter.transform.parent.parent.transform);
                        Vector3 pos = transform.parent.position;
                        장착하고있던아이템.transform.position = oldSlot.transform.position;

                        //EquipmentManager.instance.Unequip((int)장착하고있던아이템.transform.GetComponent<Iteminfo>().itemtype);


                        장착하고있던아이템.transform.SetParent(oldSlot.transform);
                        transform.position = pos;
                        EquipmentManager.instance.Equip(thisItem);

                        Debug.Log("장착하고있던 아이템 " + 장착하고있던아이템 + "장착해제, 아이템 " + thisItem + "장착");
                    }
                    else if (oldSlot.tag == data.pointerEnter.transform.parent.parent.tag)
                    {
                        GameObject standitem = data.pointerEnter.transform.parent.transform.gameObject;

                        transform.SetParent(data.pointerEnter.transform.parent.parent.transform);
                        Vector3 pos = transform.parent.position;
                        standitem.transform.position = oldSlot.transform.position;

                        standitem.transform.SetParent(oldSlot.transform);
                        transform.position = pos;

                        //inventory.items.Add(thisItem);
                        transform.parent.GetComponent<InventorySlot>().Getcomponent();
                        transform.parent.GetComponent<InventorySlot>().AddItem(thisItem);
                        standitem.transform.parent.GetComponent<InventorySlot>().Getcomponent();
                        standitem.transform.parent.GetComponent<InventorySlot>().item = standitem.GetComponent<DragItem>().thisItem;

                        Debug.Log(thisItem + "와" + standitem.GetComponent<DragItem>().thisItem + "의 자리를 변경");
                    }
                    else if (data.pointerEnter.transform.parent.parent.tag == "HotbarSlot")
                    {
                        GameObject 장착하고있던아이템 = data.pointerEnter.transform.parent.transform.gameObject;

                        transform.SetParent(data.pointerEnter.transform.parent.parent.transform);
                        Vector3 pos = transform.parent.position;

                        장착하고있던아이템.transform.SetParent(oldSlot.transform);
                        transform.position = pos;
                        장착하고있던아이템.transform.position = 장착하고있던아이템.transform.parent.position;
                    }
                    else
                    {
                        transform.SetParent(oldSlot.transform);
                        transform.parent.GetComponent<InventorySlot>().AddItem(thisItem);
                        transform.position = transform.parent.transform.position;

                        Debug.Log("종류가 일치하지 않음");
                    }

                    GetComponentInChildren<MeshRenderer>().materials = PrefabCollect.instance.DefaultVertexMatWithStencil;
                }
                else if (data.pointerEnter.tag == "HotbarSlot") // 단축키 창에 드래그 했을시
                {
                    transform.SetParent(newSlot.parent.transform);
                    transform.parent.GetComponent<InventorySlot>().Getcomponent_Hotkey();
                    transform.parent.GetComponent<InventorySlot>().AddItem(thisItem);

                    Vector3 pos = transform.parent.position;
                    transform.position = pos;

                    Debug.Log((thisItem + " 를 단축창에 올려놓음"));
                }
                else if (data.pointerEnter.tag == "EquipmentSlot") //장비창 슬롯에 드래그 했을시
                {
                    if(oldSlot.GetComponent<EquipmentSlots>() != null)  //장비창에서 장비창으로 드래그 했을때
                    {
                        print(oldSlot + "  " + newSlot.GetComponentInParent<EquipmentSlots>().gameObject);

                        if(oldSlot == newSlot.GetComponentInParent<EquipmentSlots>().gameObject)
                        {
                            EquipmentManager.instance.Equip(thisItem);
                            //transform.SetParent(oldSlot.transform);
                            //transform.position = transform.parent.transform.position;

                            Debug.Log(("같은 장비창에 드래그 하였습니다."));
                            Destroy(gameObject);
                        }
                        else if(thisItem.itemtype == data.pointerEnter.transform.parent.GetComponent<EquipmentSlots>().equipSlot)
                        {
                            EquipmentManager.instance.Equip(thisItem);
                            transform.SetParent(data.pointerEnter.transform.parent.transform);
                            Vector3 pos = transform.parent.position;
                            transform.position = pos;

                            if(oldSlot.GetComponent<InventorySlot>())
                                oldSlot.GetComponent<InventorySlot>().ClearSlot(false);
                        }
                        /*
                        else if(thisItem.weaponType == data.pointerEnter.transform.parent.GetComponent<EquipmentSlots>().weaponType && thisItem.itemtype == ItemType.Weapon)
                        {
                            EquipmentManager.instance.LeftDaggerEquip(thisItem);
                            transform.SetParent(data.pointerEnter.transform.parent.transform);
                            Vector3 pos = transform.parent.position;
                            transform.position = pos;
                            if (oldSlot.GetComponent<InventorySlot>())
                                oldSlot.GetComponent<InventorySlot>().ClearSlot(false);
                        }*/
                        else
                        {
                            //transform.parent.GetComponent<EquipmentSlots>().AddItem(thisItem);
                            //transform.SetParent(oldSlot.transform);
                            //transform.position = transform.parent.transform.position;

                            /*if (oldSlot.GetComponent<EquipmentSlots>().equipSlot == ItemType.SecondaryWeapon && thisItem.weaponType == WeaponType.Dagger)
                            {
                                EquipmentManager.instance.LeftDaggerEquip(thisItem);
                            }
                            else
                            {
                                
                            }*/

                            EquipmentManager.instance.Equip(thisItem);

                            Destroy(gameObject);

                            Debug.Log("종류가 일치하지 않음");
                        }
                    }
                    else if (thisItem.itemtype == data.pointerEnter.transform.parent.GetComponent<EquipmentSlots>().equipSlot)
                    {
                        //thisItem.Use();
                        EquipmentManager.instance.Equip(thisItem);
                        //transform.SetParent(data.pointerEnter.transform.parent.transform);
                        Vector3 pos = transform.parent.position;
                        transform.position = pos;
                        inventory.items.Remove(thisItem);
                        Destroy(this.gameObject);

                        if (oldSlot.GetComponent<InventorySlot>())
                            oldSlot.GetComponent<InventorySlot>().ClearSlot(false);
                    }
                    /*else if(thisItem.itemtype == ItemType.Weapon &&
                        thisItem.weaponType == data.pointerEnter.transform.parent.GetComponent<EquipmentSlots>().weaponType)
                    {       //단검을 쉴드슬롯에 드래그 했을때
                        EquipmentManager.instance.LeftDaggerEquip(thisItem);
                        transform.SetParent(data.pointerEnter.transform.parent.transform);
                        Vector3 pos = transform.parent.position;
                        transform.position = pos;
                        inventory.items.Remove(thisItem);

                        if (oldSlot.GetComponent<InventorySlot>())
                            oldSlot.GetComponent<InventorySlot>().ClearSlot(false);
                    }*/
                    else
                    {
                        transform.SetParent(oldSlot.transform);
                        transform.parent.GetComponent<InventorySlot>().AddItem(thisItem);
                        transform.position = transform.parent.transform.position;

                        Debug.Log("종류가 일치하지 않음");
                    }

                    EquipmentManager.instance.onEquipmentChange();

                    Debug.Log("장비창에 드래그됨");
                }
                else if(data.pointerEnter.tag == "Icon" && oldSlot.tag == "EquipmentSlot")  //단검끼리 자리를 바꾸기
                {
                    /*if(thisItem.weaponType == WeaponType.Dagger && data.pointerEnter.GetComponentInParent<DragItem>().thisItem.weaponType == WeaponType.Dagger)
                    {
                        GameObject 반대쪽단검 = data.pointerEnter.transform.parent.transform.gameObject;

                        EquipmentManager.instance.Unequip((int)반대쪽단검.transform.parent.GetComponent<EquipmentSlots>().equipSlot);
                        EquipmentManager.instance.Unequip((int)oldSlot.transform.GetComponent<EquipmentSlots>().equipSlot);

                        transform.SetParent(data.pointerEnter.transform.parent.transform.parent.transform);
                        반대쪽단검.transform.SetParent(oldSlot.transform);

                        transform.position = transform.parent.transform.position;
                        반대쪽단검.transform.position = oldSlot.transform.position;

                        if(transform.GetComponentInParent<EquipmentSlots>().equipSlot == ItemType.Weapon)
                        {
                            EquipmentManager.instance.Equip(thisItem);
                        } else if(transform.GetComponentInParent<EquipmentSlots>().equipSlot == ItemType.SecondaryWeapon)
                        {
                            EquipmentManager.instance.LeftDaggerEquip(thisItem);
                        }

                        if(반대쪽단검.GetComponentInParent<EquipmentSlots>().equipSlot == ItemType.Weapon)
                        {
                            EquipmentManager.instance.Equip(반대쪽단검.GetComponent<DragItem>().thisItem);
                        }
                        else if(반대쪽단검.GetComponentInParent<EquipmentSlots>().equipSlot == ItemType.SecondaryWeapon)
                        {
                            EquipmentManager.instance.LeftDaggerEquip(반대쪽단검.GetComponent<DragItem>().thisItem);
                        }

                        Debug.Log(transform.name + " 의 자리와 " + 반대쪽단검.name + " 자리를 변경");

                    }
                    else */if(data.pointerEnter.GetComponentInParent<EquipmentSlots>())
                    {
                        Debug.Log("잘못된 장비창 슬롯으로 드래그 하여 다시 원래 장비창 슬롯으로 돌아감");

                        EquipmentManager.instance.Equip(thisItem);

                        //transform.SetParent(oldSlot.transform);
                        //transform.position = transform.parent.position;
                        GetComponentInChildren<MeshRenderer>().materials = PrefabCollect.instance.DefaultVertexMat;
                        Destroy(gameObject);
                    }
                    else
                    {
                        Debug.Log("일치하는 조건이 없어 인벤토리로 이동");

                        bool Ininventory = Inventory.instance.Add(transform.GetComponent<DragItem>().thisItem);

                        if(Ininventory == false)
                        {
                            GameObject dropitem = Instantiate(pickupprefab, GameObject.FindGameObjectWithTag("Player").transform.position + (GameObject.FindGameObjectWithTag("Player").transform.forward * 2) + new Vector3(0, 2, 0), Quaternion.identity);
                            dropitem.gameObject.name = thisItem.Name;
                            dropitem.GetComponent<ItemPickup>().item = thisItem;
                        }
                        Destroy(gameObject);
                    }
                }
                else
                {
                    //inventory.Add(thisItem);
                    Debug.Log("일치하는 조건이 없어서 이전 슬롯으로 다시 돌아감");

                    transform.SetParent(oldSlot.transform);
                    if (transform.parent.GetComponent<InventorySlot>() != null)
                        transform.parent.GetComponent<InventorySlot>().AddItem(thisItem);
                    transform.position = transform.parent.position;
                }

            }
            else
            {
                //Debug.Log("삭제");
                //Destroy(this.gameObject);
            }

            draggedItemBox.localPosition = new Vector3(0, 0, -10);
            //GetComponentInChildren<MeshRenderer>().materials = PrefabCollect.instance.DefaultVertexMatWithStencil;

            if (Inventory.instance.onInventoryChangedCallback != null)
            {
                Inventory.instance.onInventoryChangedCallback.Invoke();
            }
        }
    }

    public void UseItem()
    {
        if (thisItem != null)
        {
            if (transform.childCount != 0)
            {
                Destroy(transform.gameObject);
                thisItem.Use();
            }

        }


    }

    public void Setparent(Transform trans)
    {
        transform.SetParent(trans);
    }

    public void resetRotation()
    {
        PointerEnter = false;
        GameObject mesh = GetComponentInChildren<MeshFilter>().gameObject;
        mesh.transform.rotation = new Quaternion(0, 0, 0, 0);
    }


    public void Get_Componenet()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        draggedItemBox = GameObject.FindGameObjectWithTag("DraggingBox").transform;
        inventory = Inventory.instance;
        if (transform.parent.GetComponent<InventorySlot>() != null)
            thisItem = transform.parent.GetComponent<InventorySlot>().item;
        //pickupprefab = thisItem.pickupprefab;
        image = transform.GetChild(0).GetComponent<Image>();
        button = transform.GetComponent<Button>();
        UI = GameObject.FindGameObjectWithTag("UI");
        iteminfo = transform.GetComponent<Iteminfo>();
    }

    void CleaningInventorySlot(Item item)
    {
        foreach (InventorySlot slot in Inventory.instance.slots)
        {
            if (slot.item)
            {
                if (!slot.ItemObject)
                {
                    slot.item = null;

                }
            }
        }
    }

}
