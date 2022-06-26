using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mimic : MonoBehaviour
{
    public Collider targetItem;
    private Collider sallowingItem;
    [Space]

    public float searchingDist;
    public float swallowDist;
    public float searchDelay;
    public float spitOutPower;
    public LayerMask ItemPickUpMask;
    public LayerMask TargetMask;

    [Space]
    public Transform TongueParent;
    public Transform SpitOutPosition;
    public Transform SpitOutDirection;
    public bool searchingItem = true;
    public bool TraceItem = false;
    public bool isSwallowing = false;
    public bool isSpiting = false;

    [Space]
    public int itemRecombinationLimit = 3;
    public int currentItemRecombinationCount = 0;

    private Animator animator;
    private NPC_AI ai;
    private NavMeshAgent agent;
    private List<Item> swallowedItem = new List<Item>();
    private List<ItemPickup> SpitedOutItem = new List<ItemPickup>();

    [SerializeField]
    private float distToItem;

    [Space]
    [SerializeField] private TextBubble textBubble;
    private bool hungryTextGenerated = false;

    // Start is called before the first frame update
    void Start()
    {
        ai = GetComponent<NPC_AI>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        StartCoroutine(SearchDropItem());
    }

    private void Update()
    {
        if(sallowingItem != null)
        {
            sallowingItem.transform.position = TongueParent.position;
        }
    }

    private void ItemSwallow()
    {
        isSwallowing = true;
        //agent.Stop();
        ai.monsterState = MonsterState.idle;

        animator.SetTrigger("Swallow");
    }

    public void SpitOutItem()
    {
        GameObject itemPickUp = DropItem.instance.CreateEquipmentDropItem(SpitOutPosition, GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerStats>().playerClass, 1);

        Vector3 direction = SpitOutDirection.position - transform.position;

        itemPickUp.GetComponent<Rigidbody>().AddForce(direction.normalized * spitOutPower, ForceMode.Force);

        SpitedOutItem.Add(itemPickUp.GetComponent<ItemPickup>());
        isSpiting = false;

        currentItemRecombinationCount++;

        if (itemRecombinationLimit == currentItemRecombinationCount)
            StartCoroutine(DieFewSecondsLater());
    }

    public void TranslateToTongue()
    {
        if (targetItem == null)
            return;

        targetItem.transform.SetParent(TongueParent);
        //targetItem.transform.position = TongueParent.transform.position;
        targetItem.transform.localPosition = Vector3.zero;
        targetItem.GetComponent<Rigidbody>().isKinematic = true;
        targetItem.GetComponent<Rigidbody>().useGravity = false;

        foreach (Collider collider in targetItem.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }

        sallowingItem = targetItem;
    }

    public void SwallowedItem()
    {
        if (targetItem == null)
            return;

        swallowedItem.Add(targetItem.GetComponent<ItemPickup>().item);

        Destroy(targetItem.gameObject);
        targetItem = null;

        searchingItem = true;
        TraceItem = false;

        sallowingItem = null;
    }

    IEnumerator SearchDropItem()
    {
        WaitForSeconds delay = new WaitForSeconds(searchDelay);

        while (gameObject.activeSelf)
        {
            yield return delay;
            if(swallowedItem.Count >= 2)
            {
                isSpiting = true;
                ai.monsterState = MonsterState.idle;
                swallowedItem.Clear();
                animator.SetTrigger("SpitOut");

            }
            if (targetItem == null && !isSpiting)
            {
                Collider[] colls = Physics.OverlapSphere(this.transform.position, searchingDist, ItemPickUpMask);

                ai.canWander = true;

                foreach (Collider collider in colls)
                {
                    if (collider.GetComponent<ItemPickup>())
                    {
                        if(collider.GetComponent<ItemPickup>().item && !collider.GetComponent<ItemPickup>().itemOnSale 
                            && !SpitedOutItem.Contains(collider.GetComponent<ItemPickup>()) && collider.GetComponent<ItemPickup>().item.itemtype != ItemType.consumable)
                        {
                            targetItem = collider;

                            ai.canWander = false;
                            agent.SetDestination(collider.transform.position);
                            TraceItem = true;
                            searchingItem = false;
                            break;
                        }
                    }
                }
            }
            else if (targetItem != null && !isSwallowing)
            {
                ai.monsterState = MonsterState.trace;

                var dist = Vector3.Distance(transform.position, targetItem.transform.position);
                distToItem = dist;

                agent.SetDestination(targetItem.transform.position);

                if (dist < swallowDist)
                {
                    ItemSwallow();
                }
            }
            /*else
            {
                ai.canWander = true;
            }*/

            if(!hungryTextGenerated)
            {
                Collider[] colls = Physics.OverlapSphere(this.transform.position, searchingDist, TargetMask);

                foreach (Collider collider in colls)
                {
                    if (collider.GetComponent<PlayerStats>())
                    {
                        GenerateHungryTextBubble();
                    }
                }
            }

        }
    }

    IEnumerator DieFewSecondsLater()
    {
        yield return new WaitForSeconds(3f);

        GetComponent<NPCStats>().KillThisNPC();
    }

    public void GenerateHungryTextBubble()
    {
        if (hungryTextGenerated)
            return;

        hungryTextGenerated = true;
        StopCoroutine(textBubble.SpawnTextBubble());
        StartCoroutine(textBubble.SpawnTextBubble());
        textBubble.ChangeText("mimic-hungry1");
    }
}
