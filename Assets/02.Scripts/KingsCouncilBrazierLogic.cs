using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingsCouncilBrazierLogic : MonoBehaviour
{
    public Animator []lights;
    public ParticleSystem[] particleSystems;
    public bool playerPassing = false;

    // Start is called before the first frame update
    void Start()
    {
        lights = GetComponentsInChildren<Animator>();
        particleSystems = GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem particle in particleSystems)
        {
            particle.gameObject.active = false;
        }
    }

    private void Update()
    {
        if(playerPassing)
        {
            foreach (Animator light in lights)
            {
                light.SetTrigger("FireStart");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(lights.Length != 0 && particleSystems.Length != 0)
            {
                playerPassing = true;
                
                foreach(ParticleSystem particle in particleSystems)
                {
                    particle.gameObject.active = true;
                }
            }
        }
    }
}
