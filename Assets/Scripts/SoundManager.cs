using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public AudioClip openSound;
    public AudioClip explosionSound;
    public AudioClip flagRemoveSound;
    public AudioClip flagSound;

    public static SoundManager Instance
    {
        get
        {
            return FindObjectOfType<SoundManager>();
        }
    }

    AudioSource AudioSource_
    {
        get
        {
            return GetComponent<AudioSource>();
        }
    }

    public void PlayOpenSound()
    {
        AudioSource_.PlayOneShot(openSound);
    }

    public void PlayExplosionSound()
    {
        AudioSource_.Stop();
        AudioSource_.PlayOneShot(explosionSound);
    }

    public void PlayFlagSound()
    {
        AudioSource_.PlayOneShot(flagSound);
    }

    public void PlayFlagRemoveSound()
    {

        AudioSource_.PlayOneShot(flagRemoveSound);
    }
}
