using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider Masterslider;
    [SerializeField] Slider BGMslider;
    [SerializeField] Slider SEslider;
    [SerializeField] Slider BGSslider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVolume()
    {
        audioMixer.SetFloat("Master", ConvertVolume2dB(Masterslider.value));
        audioMixer.SetFloat("BGM", ConvertVolume2dB(BGMslider.value));
        audioMixer.SetFloat("SE", ConvertVolume2dB(SEslider.value));
        audioMixer.SetFloat("BGS", ConvertVolume2dB(BGSslider.value));
    }

    // 0 ~ 1の値をdB( デシベル )に変換.
    float ConvertVolume2dB(float volume) => Mathf.Clamp(20f * Mathf.Log10(Mathf.Clamp(volume, 0f, 1f)), -80f, 0f);

}
