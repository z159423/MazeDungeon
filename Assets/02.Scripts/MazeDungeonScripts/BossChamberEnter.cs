using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossChamberEnter : MonoBehaviour
{
    public Door door;
    public bool closed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && closed == false)
        {
            closed = true;
            door.CloseDoor();

            foreach (NPCStats nPCStats in MazeDungeonNpcSpawner.instance.spawnedBoss)
            {
                
                //nPCStats.GetComponent<Animator>().ResetTrigger("")

                AudioManager.instance.PlayRandomBossFightBGM();

                nPCStats.transform.position = MazeDungeonNpcSpawner.instance.bossBeacons[0].transform.position;

                if (nPCStats.GetComponent<NPC_AI>().bossType == BossType.GiantSkeleton 
                    || nPCStats.GetComponent<NPC_AI>().bossType == BossType.Golem
                    || nPCStats.GetComponent<NPC_AI>().bossType == BossType.EnergyGolem
                    || nPCStats.GetComponent<NPC_AI>().bossType == BossType.Guros
                    || nPCStats.GetComponent<NPC_AI>().bossType == BossType.SkeletonGuardian)
                {
                    nPCStats.GetComponent<NPC_AI>().DiggingDustParticlePlay();
                    EZCameraShake.CameraShaker.Instance.ShakeOnce(4, 4, 1, 1);
                }

                if(nPCStats.GetComponent<NPC_AI>().bossType == BossType.Lich)
                {
                    ParticleGenerator.instance.GenerateGroundParticle(nPCStats.transform.position, "SummonCircle", 10f);
                    EZCameraShake.CameraShaker.Instance.ShakeOnce(4, 4, 1, 1);
                }

                if (nPCStats.GetComponent<NPC_AI>().bossType == BossType.Skulder)
                {
                    Destroy(Instantiate(PrefabCollect.instance.FireFlameSummon, nPCStats.transform.position,Quaternion.identity),8f);
                    EZCameraShake.CameraShaker.Instance.ShakeOnce(4, 4, 1, 1);
                }

                if (nPCStats.GetComponent<NPC_AI>().bossType == BossType.WraithKing)
                {
                    Destroy(Instantiate(PrefabCollect.instance.PurpleFlameSummon, nPCStats.transform.position, Quaternion.identity), 8f);
                    EZCameraShake.CameraShaker.Instance.ShakeOnce(4, 4, 1, 1);
                }

                nPCStats.GetComponent<Animator>().SetTrigger("Summon");
            }
        }
    }
}
