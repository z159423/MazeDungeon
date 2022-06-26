using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoguePassiveIcon : MonoBehaviour
{
    public Animator animator;
    public Slider slider;

    void Update()
    {
        if (slider.value > slider.maxValue * .7f)
        {
            animator.SetBool("Hide", true);
        }
        else
        {
            animator.SetBool("Hide", false);
        }
    }
}
