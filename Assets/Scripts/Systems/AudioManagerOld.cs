using System;
using UnityEngine;

public class AudioManagerOld : MonoBehaviour
{
    public SoundOld[] sounds;
    public static AudioManagerOld Instance;
    void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;

        foreach (SoundOld s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void PlaySound(string name)
    {
        SoundOld s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    //put the command beneath where you want to play a sound and put the name of your sound in the ""
    //FindObjectOfType<AudioManager>().PlaySound("");
}
