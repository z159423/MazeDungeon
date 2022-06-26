using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MapCameraPosition : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;

    public void ChangeMapCameraPosition()
    {
        transform.position = new Vector3(navMeshSurface.navMeshData.sourceBounds.center.x, transform.position.y, navMeshSurface.navMeshData.sourceBounds.center.z);

        GetComponent<Camera>().orthographicSize = (navMeshSurface.navMeshData.sourceBounds.size.x + navMeshSurface.navMeshData.sourceBounds.size.z) / 2.5f;

    }
}
