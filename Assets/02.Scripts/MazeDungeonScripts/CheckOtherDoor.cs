using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckOtherDoor : MonoBehaviour
{
    public bool Spawned = false;

    public bool NoExistFrontRoom = true;
    public GameObject Wall;
    public GameObject Doorway;

    void Start()
    {
        Invoke("CheckSpawn", 0.1f);
        Invoke("DestroyDoor", 4f);
        //Destroy(gameObject, 4f);
    }

    public void CheckSpawn()
    {
        Spawned = true;
    }

    private void DestroyDoor()
    {
        if(NoExistFrontRoom == true && Wall != null)
        {
            Debug.Log("문 정면에 방이 없어서 문을 삭제함 " + transform.position);
            Transform wallposition = Wall.transform;

            wallposition.localPosition = new Vector3(wallposition.localPosition.x, 0, wallposition.localPosition.z);
            wallposition.localScale = new Vector3(wallposition.localScale.x, 16.52f, wallposition.localScale.z);

            Destroy(Doorway);
            Destroy(gameObject.transform.parent.gameObject);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        //print(other);
        if (gameObject.tag == "BreakableDoor" && other.CompareTag("BreakableDoor") && other.GetComponent<CheckOtherDoor>() != null)
        {
            if (other.GetComponent<CheckOtherDoor>().Spawned == false)
            {
                print("겹치는 문 하나 삭제함");
                NoExistFrontRoom = false;
                Destroy(other.GetComponentInParent<DestroyableObject>().gameObject);
            }
        }
    }
}
