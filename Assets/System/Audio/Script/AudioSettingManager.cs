using System.Collections;
using System.Collections.Generic;
using Taki.TakiAESJsonSave;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;

    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _seSlider;

    private void Start()
    {
        _masterSlider.onValueChanged.AddListener((x)=>SaveData("Master", x));
        _bgmSlider.onValueChanged.AddListener((x)=>SaveData("BGM", x));
        _seSlider.onValueChanged.AddListener((x)=>SaveData("SE", x));

        SetAudioSetting();
    }

    private void SaveData(string nam, float volume)
    {
        PlayerPrefs.SetFloat(nam, volume);
        SetVolume();
    }

    public void SetAudioSetting()
    {
        _masterSlider.value = PlayerPrefs.GetFloat("Master");
        _bgmSlider.value = PlayerPrefs.GetFloat("BGM");
        _seSlider.value = PlayerPrefs.GetFloat("SE");

        SetVolume();
    }

    public void SetVolume()
    {
        _audioMixer.SetFloat("Master", ConvertVolume2dB(_masterSlider.value));
        _audioMixer.SetFloat("BGM", ConvertVolume2dB(_bgmSlider.value));
        _audioMixer.SetFloat("SE", ConvertVolume2dB(_seSlider.value));
    }

    // 0 ~ 1の値をdB( デシベル )に変換.
    float ConvertVolume2dB(float volume) => Mathf.Clamp(20f * Mathf.Log10(Mathf.Clamp(volume, 0f, 1f)), -80f, 0f);
}
