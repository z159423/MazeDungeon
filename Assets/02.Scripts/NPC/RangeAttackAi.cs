using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttackAi : MonoBehaviour
{
    public float minRangeAttack = 2.5f;
    public float maxRangeAttack = 15;
    public GameObject projectile;
    public Transform rangeAttackFirePos;
    public int projectileFireAngle;
    public int projectileAmount = 1;

    private NPC_AI Ai;

    private void Start()
    {
        Ai = GetComponent<NPC_AI>();
    }

    public void FireRangeAttack()
    {
        if (GetComponent<NPC_AI>().Target == null)
            return;

        int Angel = 0;
        if (projectileAmount > 1)
            Angel = projectileFireAngle / (projectileAmount - 1);

        for (int i = 0; i < projectileAmount; i++)
        {
            var arrow = Instantiate(projectile, rangeAttackFirePos.position, Quaternion.identity).GetComponent<ProjectileLogic>();
            arrow.Setting(gameObject, Mathf.RoundToInt(GetComponent<NPCStats>().minDamage.GetFinalStatValue()), Mathf.RoundToInt(GetComponent<NPCStats>().maxDamage.GetFinalStatValue()));
            arrow.Target = GetComponent<NPC_AI>().Target;

            arrow.Fire(GetComponent<NPC_AI>().Target.bounds.center, rangeAttackFirePos.position, gameObject, (Angel * (i)) - (projectileFireAngle / 2));
        }
    }
}
