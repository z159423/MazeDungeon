using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueKnight : MonoBehaviour
{
    public bool statueForm = true;

    public void FormChange()
    {
        statueForm = !statueForm;
    }
}
