using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Components;

public class ArtifactCollection : MonoBehaviour
{
    public GameObject artifactSlot;
    [Space]
    public GameObject descriptionMesh;
    [Space]

    public Transform GridParent;
    public Transform DescriptionMesh;
    public TextMeshProUGUI ArtifactName;
    public TextMeshProUGUI ArtifactDescription;
    public LocalizeStringEvent ArtifactNameLocalization;
    public LocalizeStringEvent ArtifactDescriptionLocalization;
    public LocalizeStringEvent ArtifactAbilityLocalization;


    public void GetNewArtifact(Artifact artifact)
    {
        var slot = Instantiate(artifactSlot, GridParent);
        slot.GetComponentInChildren<ArtifactSlot>().AddArtifactThisSlot(artifact);
    }

    public void TurnArtifactTap()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        UIManager.instance.IsAnyUiOn();
    }
}
