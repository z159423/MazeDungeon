using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Door : MonoBehaviour
{

    public bool bossRoomDoor = false;
    [Space]

    public bool needKey = false;
    public bool firstStageNotNeedKey = false;
    public GameObject Lock;

    private void Start()
    {
        if (DungeonGenerator.instance.CurrentStageNumber == 0)
            needKey = false;

        if (needKey && Lock != null)
            Lock.SetActive(true);
    }

    public bool OpenDoor(GameObject player = null)
    {
        if (Inventory.instance.keyAmount <= 0 && needKey)
        {
            InfoMessageManager.instance.PopupInfoMessage("NotEnoughKey");
            AudioManager.instance.GenerateAudioAndPlaySFX("doorLocked", GetComponent<Collider>().bounds.center);
            return false;
        }

        if(player != null)
        {
            if (player.GetComponentInChildren<PlayerArtifact>())
            {
                if (player.GetComponentInChildren<PlayerArtifact>().ArtifactContain(ArtifactType.SkeletonKey) && Random.Range(0, 10) < 5)
                {
                    //스켈레톤키 능력 성공
                }
                else if (needKey)
                {
                    Inventory.instance.UseKey(1);

                    if (Lock != null)
                    {
                        Lock.transform.SetParent(null);
                        Lock.GetComponent<Rigidbody>().isKinematic = false;
                    }
                        
                }
            }
        }
        

        GetComponentInChildren<Animator>().SetTrigger("Open");
        GetComponent<Collider>().enabled = false;

        if(bossRoomDoor)
        {
            EZCameraShake.CameraShaker.Instance.ShakeOnce(4,4,1,1);
            AudioManager.instance.GenerateAudioAndPlaySFX("bossDoorOpen1", GetComponent<Collider>().bounds.center);
        }
        else
        {
            AudioManager.instance.GenerateAudioAndPlaySFX("doorOpen", GetComponent<Collider>().bounds.center);
        }

        return true;
    }

    public void CloseDoor()
    {
        GetComponentInChildren<Animator>().SetTrigger("Close");

        if (bossRoomDoor)
        {
            EZCameraShake.CameraShaker.Instance.ShakeOnce(4, 4, 1, 1);
        }
    }

    private void OnDestroy()
    {
        Destroy(Lock);
    }
}


