using System;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] _MusicSounds, _SfxSounds;
    public AudioSource _MusicSource, _SfxSource;
    public static AudioManager _Instance;
    private void Awake()
    {
        if (_Instance) Destroy(gameObject);
        else _Instance = this;
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(_MusicSounds, x => x._Name == name);
        if (s == null){
            print("music not found");
        }
        else{
            _MusicSource.clip = s._AudioClip;
            _MusicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(_SfxSounds, x => x._Name == name);
        if (s == null){
            print("SFX not found");
        }
        else{
            _SfxSource.clip = s._AudioClip;
            _SfxSource.Play();
        }
    }

    public void ToggleMusic()
    {
        _MusicSource.mute = ! _MusicSource.mute;
    }

    public void ToggleSFX()
    {
        _SfxSource.mute = ! _SfxSource.mute;
    }

    public void MusicVolume(float volume)
    {
        _MusicSource.volume = volume;
    }

    public void SfxVolume(float volume)
    {
        _SfxSource.volume = volume;
    }
}