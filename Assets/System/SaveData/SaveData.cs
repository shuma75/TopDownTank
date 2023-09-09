using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    /// <summary>
    /// 実際にセーブするクラスの例
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        /// <summary>
        /// オーディオ系の設定
        /// </summary>
        public AudioSetting AudioSetting { get; set; }
    }

public class AudioSetting
{
    public float MasterVolume { get; set; }
    public float BGMVolume { get; set; }
    public float SEVolume { get; set; }
    public float BGSVolume { get; set; }
    public float VoiceVolume { get; set; }
}
