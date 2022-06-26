using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LootEffectController : MonoBehaviour
{
    public Transform TargetObject;
    public Vector3 Offset;
    VisualEffect lootEffect;

    // Start is called before the first frame update
    void Start()
    {
        lootEffect = GetComponent<VisualEffect>();
        lootEffect.SetVector3("Offset", Offset);
    }

    // Update is called once per frame
    void Update()
    {
        lootEffect.SetVector3("Position", TargetObject.position);
    }
}
