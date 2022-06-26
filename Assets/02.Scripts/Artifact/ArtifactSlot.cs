using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactSlot : MonoBehaviour
{
    public Vector3 meshPositionOffset;
    public Vector3 meshRotationOffset;
    public Vector3 meshScaleOffset;
    [Space]
    public Artifact thisArtifact;
    [Space]
    public ArtifactCollection artifactCollection;
    [Space]
    public Vector3 descriptionMeshScaleOffset;
    

    private void Start()
    {
        artifactCollection = GetComponentInParent<ArtifactCollection>();
    }

    public void AddArtifactThisSlot(Artifact artifact)
    {
        thisArtifact = artifact;
        var artifactChild = Instantiate(artifact.ArtifactObject, transform);
        artifactChild.transform.localPosition = meshPositionOffset;
        artifactChild.transform.localRotation = Quaternion.Euler(meshRotationOffset.x, meshRotationOffset.y, meshRotationOffset.z);
        artifactChild.transform.localScale = meshScaleOffset;
        if(artifactChild.GetComponentInChildren<MeshRenderer>())
            artifactChild.GetComponentInChildren<MeshRenderer>().materials = PrefabCollect.instance.DefaultVertexMatWithStencil;
        else if(artifactChild.GetComponentInChildren<MeshRenderer>())
            artifactChild.GetComponentInChildren<SkinnedMeshRenderer>().materials = PrefabCollect.instance.DefaultVertexMatWithStencil;
        artifactChild.layer = LayerMask.NameToLayer("UI");
    }

    public void SelectThisArtifact()
    {
        if (artifactCollection.descriptionMesh)
        {
            Destroy(artifactCollection.descriptionMesh);
            artifactCollection.descriptionMesh = null;
        }

        var mesh = Instantiate(thisArtifact.ArtifactObject, artifactCollection.DescriptionMesh);
        mesh.transform.localPosition = new Vector3(0,0,0);
        mesh.transform.rotation = Quaternion.Euler(meshRotationOffset.x, meshRotationOffset.y, meshRotationOffset.z);
        mesh.transform.localScale = descriptionMeshScaleOffset;

        artifactCollection.ArtifactNameLocalization.StringReference.SetReference("Artifact", thisArtifact.nameLocalizationKey);
        artifactCollection.ArtifactDescriptionLocalization.StringReference.SetReference("Artifact", thisArtifact.descriptionLocalizationKey);

        switch (thisArtifact.artifactTier)
        {
            case ArtifactTier.Common:
                artifactCollection.ArtifactName.color = PrefabCollect.instance.commonNameColor;
                break;

            case ArtifactTier.Rare:
                artifactCollection.ArtifactName.color = PrefabCollect.instance.rareNameColor;
                break;

            case ArtifactTier.Unique:
                artifactCollection.ArtifactName.color = PrefabCollect.instance.uniqueNameColor;
                break;

            case ArtifactTier.Epic:
                artifactCollection.ArtifactName.color = PrefabCollect.instance.epicNameColor;
                break;
        }

        string artifactAbilityKey = thisArtifact.nameLocalizationKey.Replace("Name-", "");
        artifactCollection.ArtifactAbilityLocalization.StringReference.SetReference("Artifact", "Ability-" + artifactAbilityKey);

        artifactCollection.descriptionMesh = mesh;

    }
}
