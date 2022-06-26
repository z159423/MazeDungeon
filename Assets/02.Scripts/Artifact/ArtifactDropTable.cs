using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactDropTable : MonoBehaviour
{
    public List<GameObject> AllArtifact = new List<GameObject>();

    [Space]

    public List<GameObject> CommonArtifact = new List<GameObject>();
    public List<GameObject> RareArtifact = new List<GameObject>();
    public List<GameObject> UniqueArtifact = new List<GameObject>();
    public List<GameObject> EpicArtifact = new List<GameObject>();

    public static ArtifactDropTable instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        AllArtifact.AddRange(CommonArtifact);
        AllArtifact.AddRange(RareArtifact);
        AllArtifact.AddRange(UniqueArtifact);
        AllArtifact.AddRange(EpicArtifact);
    }


    public GameObject GenerateArtifact(Vector3 position)
    {
        while (true)
        {
            int number = Random.Range(0, 100);
            int artifactCount = 0;
            Color pickUpParticleColor = new Color(255,255,255,255);

            GameObject artifact = null;

            if(CommonArtifact.Count == 0 && RareArtifact.Count == 0 && UniqueArtifact.Count == 0 && EpicArtifact.Count == 0)
            {
                Debug.LogError("더이상 스폰할 아티펙트가 없습니다.");

                break;
            }

            if (0 <= number && 50 > number)
            {
                if (CommonArtifact.Count == 0)
                    continue;

                artifactCount = Random.Range(0, CommonArtifact.Count);

                artifact = CommonArtifact[artifactCount];
                CommonArtifact.RemoveAt(artifactCount);
            }
            else if (50 <= number && 80 > number)
            {
                if (RareArtifact.Count == 0)
                    continue;

                artifactCount = Random.Range(0, RareArtifact.Count);

                artifact = RareArtifact[artifactCount];
                RareArtifact.RemoveAt(artifactCount);
            }
            else if (80 <= number && 98 > number)
            {
                if (UniqueArtifact.Count == 0)
                    continue;

                artifactCount = Random.Range(0, UniqueArtifact.Count);

                artifact = UniqueArtifact[artifactCount];
                UniqueArtifact.RemoveAt(artifactCount);
            }
            else if (98 <= number && 100 > number)
            {
                if (EpicArtifact.Count == 0)
                    continue;

                artifactCount = Random.Range(0, EpicArtifact.Count);

                artifact = EpicArtifact[artifactCount];
                EpicArtifact.RemoveAt(artifactCount);
            }

            GameObject artifactObject = Instantiate(artifact, position, Quaternion.identity);

            return artifactObject;

        }

        return null;
    }

}
