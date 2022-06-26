using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceShield : MonoBehaviour
{
    public float maxScale = 2;
    public float scaleTime = 2;

    public CharacterStats ownerStat;

    private float currentScale = 0;
    // Update is called once per frame
    void Update()
    {
        if (maxScale > currentScale)
        {
            currentScale += Time.deltaTime * scaleTime;
        }

        transform.localScale = new Vector3(currentScale, currentScale, currentScale);

        if(ownerStat.shieldStat.GetTotalShieldValue() <= 0)
        {
            Destroy(gameObject);
        }
        
    }
}
