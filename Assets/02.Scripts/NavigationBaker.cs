using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationBaker : MonoBehaviour
{

    public NavMeshSurface[] surfaces;
    public List<NavMeshSurface> surfaces1 = new List<NavMeshSurface>();

    void Start()
    {
        Invoke("GetNavMesh", 1);
    }

    void Update()
    {

    }

    void GetNavMesh()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.GetComponent<NavMeshSurface>() == null)
                transform.GetChild(i).gameObject.AddComponent<NavMeshSurface>();
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            surfaces1.Add(transform.GetChild(i).gameObject.GetComponent<NavMeshSurface>());
        }

        for (int i = 0; i < surfaces.Length; i++)
        {
            //surfaces[i].BuildNavMesh();
            
        }
        //surfaces1[0].gameObject.GetComponent<NavMeshSurface>().BuildNavMesh();
    }

}
