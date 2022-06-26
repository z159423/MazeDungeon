using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BowChargingIcon : MonoBehaviour
{
    public Animator animator;
    public Slider slider;

    void Update()
    {
        if (slider.value > slider.maxValue * .4f)
        {
            animator.SetBool("Flasing", true); 
        }
        else
        {
            animator.SetBool("Flasing", false);
        }
    }
}
