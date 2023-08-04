using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankElement : MonoBehaviour
{
    [SerializeField] Text PlayerName;
    [SerializeField] Text KillCount;
    
    public void SetData(string playerName, int count)
    {
        PlayerName.text = playerName;
        KillCount.text = count + "KILL";
    }
}
