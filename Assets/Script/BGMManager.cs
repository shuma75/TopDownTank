using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : SimgletonMonoBehaviour<BGMManager>
{
    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayBGM();
    }

    public void PlayBGM()
    {
        AudioManager.Instance.PlayBGM(0);
    }
}
