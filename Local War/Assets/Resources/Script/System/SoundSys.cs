using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundSys {

    private static GameObject oneSoundGameObject;
    private static AudioSource oneAudioSource;


    public static void PlaySound(AudioClip sound)
    {
        //Debug.Log("Called the PlaySound in SoundSys");

        float lowVol = 0.25f;
        float highVol = 1.0f;
        float vol = Random.Range(lowVol, highVol);

        if (oneSoundGameObject == null)
        {
            oneSoundGameObject = new GameObject("Single Sound");
            oneAudioSource = oneSoundGameObject.AddComponent<AudioSource>();
        }

        oneAudioSource.PlayOneShot(sound, vol);

    }


  
}
