using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : SimgletonMonoBehaviour<AudioManager>
{
    private List<AudioSource> _seAudioSource = new List<AudioSource>();
    private List<AudioSource> _bgmAudioSource = new List<AudioSource>();
    private List<AudioSource> _bgsAudioSource = new List<AudioSource>();

    private List<AudioClip> SEs = new List<AudioClip>();
    private List<AudioClip> BGMs = new List<AudioClip>();
    private List<AudioClip> BGSs = new List<AudioClip>();

    private int _index = 0;

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        new GameObject("AudioManager", typeof(AudioManager));
    }

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);

        AudioMixer mixer = Resources.Load("Mixer/AudioMixer") as AudioMixer;
        for (int i = 0; i < 5; i++)
        {
            _seAudioSource.Add(gameObject.AddComponent<AudioSource>());
            _seAudioSource[i].outputAudioMixerGroup = mixer.FindMatchingGroups("SE")[0];
            _seAudioSource[i].playOnAwake = false;
            _bgmAudioSource.Add(gameObject.AddComponent<AudioSource>());
            _bgmAudioSource[i].outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
            _bgmAudioSource[i].playOnAwake = false;
            _bgsAudioSource.Add(gameObject.AddComponent<AudioSource>());
            _bgsAudioSource[i].outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
            _bgsAudioSource[i].playOnAwake = false;
        }

        SEs.Add(Resources.Load("��C1") as AudioClip);//0
        SEs.Add(Resources.Load("����2") as AudioClip);//1
        SEs.Add(Resources.Load("���P�b�g�����`���[") as AudioClip);//2
        SEs.Add(Resources.Load("��^�_���e�𔭎�") as AudioClip);//3
        SEs.Add(Resources.Load("�Ԃ̃A�C�h�����O") as AudioClip);//4
        SEs.Add(Resources.Load("�Ԃ̃u���[�L") as AudioClip);//5
        SEs.Add(Resources.Load("���{�b�g������2") as AudioClip);//6
        SEs.Add(Resources.Load("����{�^��������18") as AudioClip);//7
        SEs.Add(Resources.Load("����{�^��������14") as AudioClip);//8

        BGMs.Add(Resources.Load("252_long_BPM120") as AudioClip);
        BGMs.Add(Resources.Load("269_long_BPM168") as AudioClip);

    }

    public void PlaySE(int index)
    {
        _seAudioSource[0].PlayOneShot(SEs[index]);
    }

    public void PlayBGM(int index)
    {
        _bgmAudioSource[0].clip = BGMs[index];
        _bgmAudioSource[0].Play();
    }

    public void StopBGM()
    {
        _bgmAudioSource[0].Stop();
    }
}
