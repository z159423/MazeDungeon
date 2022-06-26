using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconFlasing : MonoBehaviour
{
    public Animator animator;
    public Slider slider;

    // Update is called once per frame
    void Update()
    {
        if(slider.value > slider.maxValue * .35f)
        {
            animator.SetBool("Flasing", true);
        }
        else
        {
            animator.SetBool("Flasing", false);
        }
    }
}
