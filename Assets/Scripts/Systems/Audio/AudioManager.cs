using System;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Sound[] _MusicSounds, _SfxSounds;
    public AudioSource _MusicSource, _SfxSource;
    public static AudioManager _Instance;
    UnityEngine.SceneManagement.Scene _Scene;
    private void Awake()
    {
        if (_Instance) Destroy(gameObject);
        else _Instance = this;
        _Scene = SceneManager.GetActiveScene();
    }

    private void Update()
    {
        if (_Scene.buildIndex == 0 && !_MusicSource.isPlaying)
        {
            //main screen audio
            PlayMusic("mainScreenLoop");
        }
        else if (_Scene.buildIndex == 1 && !_MusicSource.isPlaying)
        {
            //funfair audio
            PlayMusic("funfairLoop");
        }
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

    public void PlaySFX(string name, bool overlapSounds = false)
    {
        Sound s = Array.Find(_SfxSounds, x => x._Name == name);
        if (s == null){
            print("SFX not found");
        }
        else{
            if (overlapSounds)
            {
                _SfxSource.PlayOneShot(s._AudioClip);
            }
            else
            {
                _SfxSource.clip = s._AudioClip;

                _SfxSource.Play();
            }   
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


}

/* Type "AudioManager._Instance.X( );" 
     * place it where you want to play/modify a sound

     * if you want to play a sound replace X by either PlayMusic or PlaySFX
     * and put the name of the sound you want to play in the (" ")

     * if you want to mute all the sounds in a category replace X by either ToggleMusic or ToggleSFX

     * if you want to modify the volume in a category replace X by either MusicVolume or SfxVolume
     * and put the value you want the sound to be in the ( )*/