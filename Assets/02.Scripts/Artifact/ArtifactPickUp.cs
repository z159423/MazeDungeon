using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactPickUp : MonoBehaviour
{
    public Artifact artifact;

    public bool isOnSale = false;
    public int PurchaseValue = 0;

    public bool PickUp(GameObject player)
    {
        if(isOnSale)
        {
            if (Inventory.instance.coinAmount < PurchaseValue)
            {
                InfoMessageManager.instance.PopupInfoMessage("NotEnoughMoney");
                Debug.Log("코인이 부족하여 구매할 수 없습니다 코인갯수 :  " + Inventory.instance.coinAmount);
                return false;
            }
            else
            {
                Inventory.instance.LoseCoin(PurchaseValue);
                player.GetComponentInChildren<PlayerArtifact>().OnGetArtifact(artifact, player);
                AudioManager.instance.GenerateAudioAndPlaySFX("getArtifact", GetComponentInChildren<Collider>().bounds.center);
                Destroy(gameObject);
                return true;
            }
        }
        else
        {
            player.GetComponentInChildren<PlayerArtifact>().OnGetArtifact(artifact, player);
            AudioManager.instance.GenerateAudioAndPlaySFX("getArtifact", GetComponentInChildren<Collider>().bounds.center);
            Destroy(gameObject);
            return true;
        }
        
    }

    public void ChangePurchaseValue()
    {
        switch(artifact.artifactTier)
        {
            case ArtifactTier.Common:
                PurchaseValue = 100;
                break;

            case ArtifactTier.Rare:
                PurchaseValue = 200;
                break;

            case ArtifactTier.Unique:
                PurchaseValue = 350;
                break;

            case ArtifactTier.Epic:
                PurchaseValue = 500;
                break;
        }
    }

    private void OnDisable()
    {
        PlayerArtifact playerArtifact = null;

        if (GameObject.FindGameObjectWithTag("Player") == null)
            return;

        if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerArtifact>() != null)
            playerArtifact = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerArtifact>();

        if (playerArtifact == null)
            return;

        switch (artifact.artifactTier)
        {
            case ArtifactTier.Common:

                foreach(GameObject artifact in ArtifactDropTable.instance.CommonArtifact)
                {
                    if (artifact.GetComponent<ArtifactPickUp>().artifact.artifactType == this.artifact.artifactType)
                        return;

                    if (playerArtifact.ArtifactContain(this.artifact.artifactType))
                        return;
                }

                foreach(GameObject artifact in ArtifactDropTable.instance.AllArtifact)
                {
                    if(artifact.GetComponent<ArtifactPickUp>().artifact.artifactType == this.artifact.artifactType)
                        ArtifactDropTable.instance.CommonArtifact.Add(artifact);
                }

                break;

            case ArtifactTier.Rare:

                foreach (GameObject artifact in ArtifactDropTable.instance.RareArtifact)
                {
                    if (artifact.GetComponent<ArtifactPickUp>().artifact.artifactType == this.artifact.artifactType)
                        return;

                    if (playerArtifact.ArtifactContain(this.artifact.artifactType))
                        return;

                }

                foreach (GameObject artifact in ArtifactDropTable.instance.AllArtifact)
                {
                    if (artifact.GetComponent<ArtifactPickUp>().artifact.artifactType == this.artifact.artifactType)
                        ArtifactDropTable.instance.RareArtifact.Add(artifact);
                }

                break;

            case ArtifactTier.Unique:

                foreach (GameObject artifact in ArtifactDropTable.instance.UniqueArtifact)
                {
                    if (artifact.GetComponent<ArtifactPickUp>().artifact.artifactType == this.artifact.artifactType)
                        return;

                    if (playerArtifact.ArtifactContain(this.artifact.artifactType))
                        return;

                    
                }

                foreach (GameObject artifact in ArtifactDropTable.instance.AllArtifact)
                {
                    if (artifact.GetComponent<ArtifactPickUp>().artifact.artifactType == this.artifact.artifactType)
                        ArtifactDropTable.instance.UniqueArtifact.Add(artifact);
                }

                break;

            case ArtifactTier.Epic:

                foreach (GameObject artifact in ArtifactDropTable.instance.EpicArtifact)
                {
                    if (artifact.GetComponent<ArtifactPickUp>().artifact.artifactType == this.artifact.artifactType)
                        return;

                    if (playerArtifact.ArtifactContain(this.artifact.artifactType))
                        return;
                }

                foreach (GameObject artifact in ArtifactDropTable.instance.AllArtifact)
                {
                    if (artifact.GetComponent<ArtifactPickUp>().artifact.artifactType == this.artifact.artifactType)
                        ArtifactDropTable.instance.EpicArtifact.Add(artifact);
                }

                break;
        }
    }
}
