using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundSys {


    public static void PlaySound(AudioClip sound)
    {
        float lowVol = 0.25f;
        float highVol = 1.25f;


        Debug.Log("Called the PlaySound in SoundSys");

        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();

        audioSource.PlayOneShot(sound);


    }

  
}
