using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcVisibleChecking : MonoBehaviour
{
    public bool isVisible = false;
    private void OnBecameVisible()
    {
        isVisible = true;
    }

    private void OnBecameInvisible()
    {
        isVisible = false;
    }

    public bool visibleTest()
    {
        return VisibleTester.instance.VisibleTest(this.transform.position);
    }
}
