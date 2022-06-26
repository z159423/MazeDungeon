using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIconGenerator : MonoBehaviour
{

    public GameObject mapIcon;
    public GameObject doorIcon;
    public List<GameObject> Doors;

    public bool PlayerPassed = false;
    
    void Start()
    {
        if (mapIcon == null)
        {
            mapIcon = PrefabCollect.instance.Room_Minimap;
        }

        DoorBeacon[] Doorstemp;
        Doorstemp = transform.root.GetComponentsInChildren<DoorBeacon>();
        foreach(DoorBeacon beacon in Doorstemp)
        {
            Doors.Add(beacon.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        for(int i = 0; i < Doors.Count; i++)
        {
            if(Doors[i] == null)
            {
                Doors.RemoveAt(i);
            }
        }

        if (other.CompareTag("Player") && PlayerPassed == false)
        {
            if (mapIcon == null)
            {
                mapIcon = PrefabCollect.instance.Room_Minimap;
            }
            Instantiate(mapIcon, transform.position + mapIcon.transform.position, mapIcon.transform.rotation, transform);
            /*Collider collider = GetComponent<Collider>();
            collider.enabled = false;*/

            PlayerPassed = true;

            foreach(GameObject door in Doors)
            {
                Instantiate(doorIcon,door.GetComponent<Collider>().bounds.center + doorIcon.transform.position, doorIcon.transform.rotation, transform);
            }
            //other.enabled = false;
        }
    }
}
