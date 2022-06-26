using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIcon : MonoBehaviour
{
    public GameObject playerIcon;

    private GameObject GeneratedIcon;

    // Start is called before the first frame update
    void Start()
    {
        GeneratedIcon = Instantiate(playerIcon);
    }

    // Update is called once per frame
    void Update()
    {
        GeneratedIcon.transform.position = transform.position + new Vector3(0, 200, 0);
        GeneratedIcon.transform.rotation = transform.rotation;
    }
}
