#region

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

#endregion

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicVolumeSlider;

    private void OnEnable()
    {
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
    }

    private void OnDisable()
    {
        musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
    }

    void OnMusicVolumeChanged(float value)
    {
        audioMixer.SetFloat("MusicVolume", Linear2DB(value));
    }

    float Linear2DB(float linear)
    {
        return Mathf.Log10(Mathf.Clamp(linear, 0.00001f, 1f)) * 20f;
    }
}