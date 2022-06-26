using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerIn : MonoBehaviour
{
    public GameObject mapIcon;

    public List<Collider> alreayCheck = new List<Collider>();

    private void Start()
    {
        if(mapIcon == null)
        {
            mapIcon = PrefabCollect.instance.Room_Minimap;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PlayerCheck") && !alreayCheck.Contains(other))
        {
            if (mapIcon == null)
            {
                mapIcon = PrefabCollect.instance.Room_Minimap;
            }
            Instantiate(mapIcon, other.transform.position + mapIcon.transform.position, mapIcon.transform.rotation, other.transform);
            alreayCheck.Add(other);
            //other.enabled = false;
        }
    }
}
