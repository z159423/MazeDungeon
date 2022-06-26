using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class SoundManager : MonoBehaviour {

    public AudioMixer audioMixer;
    public AudioSource audioSourceFrefab;
    [Space]

    public AudioClip SwordSwingSound;
    public AudioClip SwordSmiteSound;
    public AudioClip BowLaunchSound;
    public AudioClip HitedSound;
    public AudioClip ExplosionSound;
    public AudioClip ShieldBash;
    public AudioClip WoodBrokenSound;
    public AudioClip DoorBrokenSound;
    public AudioClip PunchSound;
    public AudioClip EatChipsSound;
    public AudioClip CoinDrop;
    public AudioClip CoinCollect;
    public AudioClip WeaponBroken;

    public AudioClip[] footSteps;

    AudioSource myAudio;

    public static SoundManager instance;

    private void Awake()
    {
        if (SoundManager.instance == null)
            SoundManager.instance = this;
    }

    // Use this for initialization
    void Start () {
        myAudio = GetComponent<AudioSource>();
	}

    public void PlaySound_SwingSword(AudioSource audioSource)
    {
        audioSource.PlayOneShot(SwordSwingSound);
    }

    public void PlaySound_SwordSmite(AudioSource audioSource)
    {
        audioSource.PlayOneShot(SwordSmiteSound);
    }

    public void PlaySound_BowLaunch(AudioSource audioSource)
    {
        audioSource.PlayOneShot(BowLaunchSound);
    }

    public void PlaySound_Hit_Sword(AudioSource audioSource)
    {
        audioSource.PlayOneShot(HitedSound);
    }

    public void PlayShieldBashSound(AudioSource audioSource)
    {
        audioSource.PlayOneShot(ShieldBash);
    }

    public AudioClip GetExplosionSound()
    {
        return ExplosionSound;
    }

    public AudioClip ReturnSoundClip_Hit_Sound()
    {
        return HitedSound;
    }

    public AudioClip getShieldBashSound()
    {
        return ShieldBash;
    }

    public AudioClip getWoodBrokenSound()
    {
        return WoodBrokenSound;
    }

    public AudioClip getDoorBrokenSound()
    {
        return DoorBrokenSound;
    }

    public AudioClip getPunchSound()
    {
        return PunchSound;
    }
    public AudioClip getEatChipsSound()
    {
        return EatChipsSound;
    }

    public AudioClip getRandomFootStepSound()
    {
        return footSteps[Random.Range(0, footSteps.Length)];
    }

    // Update is called once per frame
    void Update () {
		
	}
}
