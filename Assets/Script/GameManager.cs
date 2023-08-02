using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public struct PlayerScore
    {
        public int kill;
        public int death;
        public int hit;
    }

    public PlayerScore[] playerScore;

    public ReactiveProperty<int> timer;
    [SerializeField] private Text TimerText;
    public bool inGame;
    // Start is called before the first frame update
    void Start()
    {
        playerScore = new PlayerScore[5];
        inGame = false;
        timer.Subscribe(x =>
        {
            TimerText.text = x.ToString();
        }).AddTo(this);

        StartCoroutine(Timer());
    }

    public void SetHitCount(Photon.Realtime.Player player)
    {
        playerScore[player.ActorNumber - 1].hit++;
        Debug.LogError(playerScore[player.ActorNumber - 1].hit);
    }

    public void SetKillCount(Photon.Realtime.Player player)
    {
        playerScore[player.ActorNumber-1].kill++;
    }

    public void SetDeathCount(Photon.Realtime.Player player)
    {
        playerScore[player.ActorNumber - 1].death++;
    }

    IEnumerator Timer()
    {
        for(int i = 5;i > 0; i--)
        {
            yield return new WaitForSeconds(1);

            Debug.Log(i);
        }
        yield return new WaitForSeconds(1);

        Debug.Log("Start!!");

        timer.Value = 10;
        inGame = true;

        while (timer.Value > 0)
        {
            yield return new WaitForSeconds(1);
            timer.Value--;
        }

        inGame = false;
        Debug.Log("End");
    }
}
