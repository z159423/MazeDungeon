using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wraith : MonoBehaviour
{
    public float appearStartDist = 10f;

    public SkinnedMeshRenderer[] meshed;

    private NPC_AI ai;

    // Start is called before the first frame update
    void Start()
    {
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in meshed)
        {
            skinnedMeshRenderer.material = Instantiate(skinnedMeshRenderer.material);
        }

        foreach (SkinnedMeshRenderer skinnedMeshRenderer in meshed)
        {
            for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
            {
                skinnedMeshRenderer.materials[i].SetFloat("_Opacity", 0.1f);
            }
        }

        ai = GetComponent<NPC_AI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(ai.Target != null && ai.monsterState != MonsterState.wander)
        {
            float transparency = (100f + (ai.TargetDist * -5f)) / 100f;

            transparency = Mathf.Clamp(transparency, 0.1f, 1);

            foreach (SkinnedMeshRenderer skinnedMeshRenderer in meshed)
            {
                skinnedMeshRenderer.material.SetFloat("_Opacity", transparency);
            }
        }
    }
}
