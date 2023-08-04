using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using DG.Tweening;

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
        public int id;
        public int kill;
        public int death;
        public int hit;
    }

    public PlayerScore[] playerScore;

    public ReactiveProperty<int> timer;
    [SerializeField] private Text TimerText;
    [SerializeField] private Text CountDownText;

    [SerializeField] private Transform RankParent;
    [SerializeField] private RankElement[] rankElement;

    public bool inGame;
    // Start is called before the first frame update
    void Start()
    {
        playerScore = new PlayerScore[PhotonNetwork.PlayerList.Length];
        for(int i = 0;i < playerScore.Length; i++)
        {
            playerScore[i].id = i;
        }
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
            CountDownText.transform.DOScale(0,0);
            Debug.Log(i);
            CountDownText.text = i.ToString();
            CountDownText.transform.DOScale(1, 1);

        }
        yield return new WaitForSeconds(1);
        CountDownText.transform.DOScale(0, 0);
        CountDownText.transform.DOScale(1.3f, 1).OnComplete(()=>CountDownText.gameObject.SetActive(false));
        Debug.Log("Start!!");
        CountDownText.text = "STRAT";


        timer.Value = 30;
        inGame = true;

        while (timer.Value > 0)
        {
            yield return new WaitForSeconds(1);
            timer.Value--;
            if(timer.Value < 10)
            {
                CountDownText.gameObject.SetActive(true);
                CountDownText.transform.DOScale(0, 0);
                CountDownText.text = timer.Value.ToString();
                CountDownText.color = Color.red;
                CountDownText.transform.DOScale(1, 1);
            }
        }

        yield return new WaitForSeconds(1);
        CountDownText.transform.DOScale(0, 0);
        CountDownText.text = "FINNISH";
        
        CountDownText.transform.DOScale(1, 1).OnComplete(() => CountDownText.gameObject.SetActive(false));

        inGame = false;
        Debug.Log("End");

        yield return new WaitForSeconds(1);

        string[] nameList = new string[PhotonNetwork.PlayerList.Length];
        int[] killList = new int[PhotonNetwork.PlayerList.Length];


    }

    private void ShowResult(string[] nameList, int[] killList)
    {
        for(int i = 0;i < nameList.Length; i++)
        {

        }
    }
}
