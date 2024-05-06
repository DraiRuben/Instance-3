using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicUiControler : MonoBehaviour
{
    public Slider _GlobalVolumeSlider, _MusicSlider, _SfxSlider;
    [SerializeField] private AudioMixer _Mixer;
    private static float _GlobalSliderValue = 1;
    private static float _MusicSliderValue = 0.5f;
    private static float _SfxSliderValue = 1;
    private void Start()
    {
        if (PlayerPrefs.HasKey(nameof(_GlobalSliderValue))) _GlobalSliderValue = PlayerPrefs.GetFloat(nameof(_GlobalSliderValue));
        if (PlayerPrefs.HasKey(nameof(_MusicSliderValue))) _MusicSliderValue = PlayerPrefs.GetFloat(nameof(_MusicSliderValue));
        if (PlayerPrefs.HasKey(nameof(_SfxSliderValue))) _SfxSliderValue = PlayerPrefs.GetFloat(nameof(_SfxSliderValue));

        _GlobalVolumeSlider.value = _GlobalSliderValue;
        _MusicSlider.value = _MusicSliderValue;
        _SfxSlider.value = _SfxSliderValue;

        _GlobalVolumeSlider.onValueChanged.AddListener(x => GlobalVolume());
        _MusicSlider.onValueChanged.AddListener(x => MusicVolume());
        _SfxSlider.onValueChanged.AddListener(x => SfxVolume());
        GlobalVolume();
        MusicVolume();
        SfxVolume();
    }
    private void OnDisable()
    {
        PlayerPrefs.SetFloat(nameof(_GlobalSliderValue), _GlobalSliderValue);
        PlayerPrefs.SetFloat(nameof(_MusicSliderValue), _MusicSliderValue);
        PlayerPrefs.SetFloat(nameof(_SfxSliderValue), _SfxSliderValue);
    }
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
        _GlobalSliderValue = _GlobalVolumeSlider.value;
        _Mixer.SetFloat("MasterVolume", Mathf.Log10(_GlobalVolumeSlider.value) * 20);
    }
    public void MusicVolume()
    {
        _MusicSliderValue = _MusicSlider.value;
        _Mixer.SetFloat("MusicVolume", Mathf.Log10(_MusicSlider.value) * 20);
    }

    public void SfxVolume()
    {
        _SfxSliderValue = _SfxSlider.value;
        _Mixer.SetFloat("SFXVolume", Mathf.Log10(_SfxSlider.value) * 20);
    }
}
