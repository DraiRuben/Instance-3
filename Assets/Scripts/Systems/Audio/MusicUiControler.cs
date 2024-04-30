using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicUiControler : MonoBehaviour
{
    public Slider _GlobalVolumeSlider, _MusicSlider, _SfxSlider;

    public void ToggleMusic()
    {
        AudioManager._Instance.ToggleMusic();
    }

    public void ToggleSFX()
    {
        AudioManager._Instance.ToggleSFX();
    }

    public void GlobalVolume()
    {
        AudioManager._Instance.GlobalVolume(_GlobalVolumeSlider.value);
    }

    public void MusicVolume()
    {
        AudioManager._Instance.MusicVolume(_MusicSlider.value);
    }

    public void SfxVolume()
    {
        AudioManager._Instance.SfxVolume(_SfxSlider.value);
    }
}
