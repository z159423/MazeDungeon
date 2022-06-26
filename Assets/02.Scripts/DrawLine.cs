using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{

    public int width = 100;
    public int height = 100;

    // Start is called before the first frame update
    void Start()
    {
        float Y = 0;

        float sampleX = 0, sampleY = 0;

        /*
        for(int i = 0; i < 100; i++)
        {
            Debug.DrawLine(new Vector3(0, 0, i), new Vector3(100, 0, i), Color.black, 10000);
        }*/

        for(int x = 0; x < 100; x++)
        {
            sampleX  = 0f;
            sampleY  = 0f;

            for (int z = 0; z < 100; z++)
            {
                Y = Mathf.PerlinNoise(sampleX, sampleY);

                Debug.DrawLine(new Vector3(x, Y*10, z), new Vector3(x, Y*10, z+1), Color.black, 10000);
                Debug.DrawLine(new Vector3(x, Y*10, z+1), new Vector3(x+1, Y*10, z), Color.black, 10000);
                Debug.DrawLine(new Vector3(x+1, Y * 10, z + 1), new Vector3(x, Y * 10, z+1), Color.black, 10000);

                Debug.Log(Y);

                sampleX += 0.1f;
                sampleY += 0.1f;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
