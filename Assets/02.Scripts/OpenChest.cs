using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenChest : MonoBehaviour
{
    public bool isOpened = false;

    public bool needKey = true;
    public GameObject Lock;

    public Transform itemPopupPosition;
    public Transform artifactDropPotition;

    public List<Item> iteminChest = new List<Item>();
    public int mingoldInChest;
    public int maxgoldInChest;

    public int currentGold;
    public Vector3 coinPopupDir;

    [Range(0,5)]
    public int itemAmount;
    [Range(0, 100)]
    public int itemPercent = 100;

    [Range(0, 5)]
    public int artifactAmount = 0;
    [Range(0, 100)]
    public int artifactPercent = 100;

    [Space]

    public bool nothing = false;
    [Range(0,100)]
    public int nothingPercent;
    public float bombPopPower = 1000;

    private void Start()
    {
        currentGold = Random.Range(mingoldInChest, maxgoldInChest);
        //CreateIteminChest();

        if (DungeonGenerator.instance.CurrentStageNumber == 0)
            needKey = false;

        if (needKey && Lock != null)
            Lock.SetActive(true);
    }

    public bool ChestOpen(GameObject player = null)
    {
        if(Inventory.instance.keyAmount <= 0 && needKey)
        {
            InfoMessageManager.instance.PopupInfoMessage("NotEnoughKey");
            AudioManager.instance.GenerateAudioAndPlaySFX("doorLocked", GetComponent<Collider>().bounds.center);
            return false;
        }

        if(player.GetComponentInChildren<PlayerArtifact>())
        {
           if(player.GetComponentInChildren<PlayerArtifact>().ArtifactContain(ArtifactType.SkeletonKey) && Random.Range(0,10) < 5)
            {
                //스켈레톤키 능력 성공
            }
            else if(needKey)
            {
                Inventory.instance.UseKey(1);

                if (Lock != null)
                {
                    Lock.transform.SetParent(null);
                    Lock.GetComponent<Rigidbody>().isKinematic = false;
                }
                    
            }
        }

        GetComponent<Animator>().SetTrigger("Chest Open");
        isOpened = true;
        GetComponent<Collider>().enabled = false;

        AudioManager.instance.GenerateAudioAndPlaySFX("doorOpen", GetComponent<Collider>().bounds.center);

        int nothingValue = Random.Range(0, 100);

        if (nothing && nothingValue < nothingPercent)
        {
            //폭탄 스폰
            Invoke("NothingBomb", .5f); 
            //NothingBomb();
        }
        else
        {
            StartCoroutine(popupItem());
            popupGold();
            for (int i = 0; i < artifactAmount; i++)
            {
                int value = Random.Range(0, 100);

                if(value < artifactPercent)
                {
                    var artifact = ArtifactDropTable.instance.GenerateArtifact(artifactDropPotition.position);

                    artifact.GetComponentInChildren<ArtifactPickUp>().isOnSale = false;
                }
            }
        }

        return true;
    }

    private void CreateIteminChest()
    {
        for(int i = 0; i < itemAmount; i++)
        {
            var itemInstance = DropItem.CreateItemInstance(DataMannager.instance.itemDataBase, null);
            iteminChest.Add(itemInstance);
        }
    }

    IEnumerator popupItem()
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < itemAmount; i++)
        {
            /*GameObject popUpItem = Instantiate(DataMannager.instance.itemDataBase.itemPickupPrefab, itemPopupPosition.position, Quaternion.identity);
            ItemPickup itemPickUp = popUpItem.GetComponent<ItemPickup>();

            itemPickUp.item = iteminChest[i];*/

            int value = Random.Range(0, 100);

            if(value < itemPercent)
            {
                GameObject popUpItem = DropItem.instance.CreateEquipmentDropItem(itemPopupPosition, GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerStats>().playerClass, 1);

                var rb = popUpItem.GetComponent<Rigidbody>();
                rb.AddForce((Vector3.up * 12) + new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3)), ForceMode.Impulse);
                rb.AddTorque(new Vector3(50, 0, 0), ForceMode.Impulse);
            }

            yield return new WaitForSeconds(0.15f);
        }
    }

    void popupGold()
    {
        DropItem.instance.DropCoin(currentGold, itemPopupPosition.position, coinPopupDir, 0.1f);
    }

    public void NothingBomb()
    {
        var bomb = Instantiate(PrefabCollect.instance.nothingBomb, itemPopupPosition.position, Quaternion.identity);
        bomb.GetComponentInChildren<Rigidbody>().velocity = (new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f)) * bombPopPower);
    }

    private void OnDestroy()
    {
        DungeonGenerator.instance.SpawndTresureChest.Remove(gameObject);
        Destroy(Lock);
    }
}
