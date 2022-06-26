using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitIronbar : MonoBehaviour
{
    public void OpenExit()
    {
        GetComponent<Animator>().SetTrigger("Open");
    }
}
