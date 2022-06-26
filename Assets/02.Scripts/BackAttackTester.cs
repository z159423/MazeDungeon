using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackAttackTester : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform ObjectA;
    public Transform ObjectB;

    public Text objectAangle;
    public Text objectBangle;
    public Text dif;
    public Text resiltDif;
    public Text result;

    // Update is called once per frame
    void Update()
    {
        objectAangle.text = "오브젝트 A : " + ObjectA.rotation.eulerAngles.y.ToString();
        objectBangle.text = "오브젝트 B : " + ObjectB.rotation.eulerAngles.y.ToString();

        dif.text = "각도차 : " + (ObjectA.rotation.eulerAngles.y - ObjectB.rotation.eulerAngles.y).ToString();

        float diff = ObjectA.rotation.eulerAngles.y - ObjectB.rotation.eulerAngles.y;

        if(Mathf.Abs(diff) > 180)
        {
            diff = Mathf.Abs(diff) - 360;
        }

        resiltDif.text = "정제값 : " +  Mathf.Abs(diff).ToString();

        if (Mathf.Abs(diff) < 60)
        {
            result.text = "백어택";
        }
        else
        {
            result.text = "";
        }
    }
}
