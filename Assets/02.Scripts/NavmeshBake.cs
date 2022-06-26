using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEditor;

public class NavmeshBake : MonoBehaviour
{
    public static NavmeshBake instance;

    public NavMeshSurface surface;
    public static Dictionary<Vector3, GameObject> NavMeshList = new Dictionary<Vector3, GameObject>();
    public List<NavMeshSurface> NavMeshSurfaces = new List<NavMeshSurface>();

    public List<NavMeshBuildSource> navMeshBuildSources = new List<NavMeshBuildSource>();

    public MapCameraPosition mapCameraPosition;

    //public NavMeshSurface MeshSurface;

    NavMeshBuildSettings navMeshBuildSettings;

    void Start()
    {
        instance = this;
        surface = transform.gameObject.GetComponent<NavMeshSurface>();

        //Invoke("BuildNav",3);
    }

    public void BuildNav()
    {
        //MeshSurface.BuildNavMesh();

        //StartCoroutine(BuildNavmeshAsync());

        /*var NavMeshDataSync = NavMeshBuilder.UpdateNavMeshDataAsync(surface.navMeshData, navMeshBuildSettings, navMeshBuildSources, new Bounds());

        if(NavMeshDataSync.isDone)
        {
            print("navmeshbuild Done");
        }*/

        //Task.Run(() => BuildNavmeshTask());
        surface.BuildNavMesh();

        mapCameraPosition.ChangeMapCameraPosition();

        /*for (int i = 0; i < NavMeshSurfaces.Count; i++)
        {
            NavMeshSurfaces[i].BuildNavMesh();
        }*/
    }

    public void DeleteNavData()
    {
        surface.RemoveData();
    }

    async Task BuildNavmeshTask()
    {
        await Task.Run(() =>
        {
            print("TaskAsync시작");
            for (int i = 0; i < NavMeshSurfaces.Count; i++)
            {
                print(i + " 번째 네브메쉬 빌드중");
                NavMeshSurfaces[i].BuildNavMesh();
            }
        });


    }

    IEnumerator BuildNavmeshAsync()
    {
        for (int i = 0; i < NavMeshSurfaces.Count; i++)
        {
            NavMeshSurfaces[i].BuildNavMesh();
        }

        yield return 0;
    }

    public void AddNavMeshSurface(GameObject gameObject)
    {
        NavMeshSurface surface = gameObject.AddComponent<NavMeshSurface>();

        surface.collectObjects = CollectObjects.Children;
        surface.overrideTileSize = true;
        surface.overrideVoxelSize = true;
        surface.voxelSize = 0.5f;
        surface.tileSize = 100;

        NavMeshList.Add(gameObject.transform.position, gameObject);
        NavMeshSurfaces.Add(surface);
        surface.BuildNavMesh();

    }

}
