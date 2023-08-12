using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Linq;

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
    [SerializeField] private Button EndButton;
    [SerializeField] private Text Log;
    [SerializeField] private Transform LogParent;
    [SerializeField] private RankElement[] rankElements;

    [SerializeField] private Transform RankParent;
    [SerializeField] private RankElement[] rankElement;

    [SerializeField] private Transform[] Res;

    public bool inGame;

    public Player local;
    // Start is called before the first frame update
    void Start()
    {
        playerScore = new PlayerScore[PhotonNetwork.PlayerList.Length];
        EndButton.onClick.AddListener(()=>EndGame());
        for(int i = 0;i < playerScore.Length; i++)
        {
            playerScore[i].id = i;
        }
        inGame = false;

        List<string> nameList = new List<string>();
        List<int> killList = new List<int>();

        var sort = playerScore.OrderByDescending(x => x.kill);

        foreach (PlayerScore data in sort)
        {
            nameList.Add(PhotonNetwork.PlayerList[data.id].NickName);
            killList.Add(data.kill);
        }

        photonView.RPC(nameof(ReloadRanking), RpcTarget.All, nameList.ToArray(), killList.ToArray());

        int time;
        switch (LobbyManager.Instance.GameTime.value)
        {
            case 0:
                time = 60;
                break;
            case 1:
                time = 120;
                break;
            case 2:
                time = 180;
                break;
            case 3:
                time = 240;
                break;
            case 4:
                time = 300;
                break;
            case 5:
                time = 600;
                break;
            default:
                time = 30;
                break;

        }

        timer.Subscribe(x =>
        {
            TimerText.text = x.ToString();
        }).AddTo(this);

        if (photonView.IsMine)
        {
            photonView.RPC(nameof(StartTimer), RpcTarget.AllViaServer, time);
        }
    }

    [PunRPC]
    private void StartTimer(int time)
    {
        StartCoroutine(Timer(time));
    }

    public void SetHitCount(int player)
    {
        playerScore[player - 1].hit++;
    }

    public void SetKillCount(int player)
    {
        playerScore[player - 1].kill++;

        List<string> nameList = new List<string>();
        List<int> killList = new List<int>();

        var sort = playerScore.OrderByDescending(x => x.kill);

        foreach (PlayerScore data in sort)
        {
            nameList.Add(PhotonNetwork.PlayerList[data.id].NickName);
            killList.Add(data.kill);
        }

        ReloadRanking(nameList.ToArray(), killList.ToArray());

        if(PhotonNetwork.LocalPlayer.ActorNumber == player)
        {
            local.SetTrophy();
        }
    }

    public void SetDeathCount(int player)
    {
        playerScore[player - 1].death++;
    }

    IEnumerator Timer(int time)
    {
        timer.Value = time;
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


        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            List<string> nameList = new List<string>();
            List<int> killList = new List<int>();

            var sort = playerScore.OrderByDescending(x => x.kill);

            foreach (PlayerScore data in sort)
            {
                nameList.Add(PhotonNetwork.PlayerList[data.id].NickName);
                killList.Add(data.kill);
            }

            photonView.RPC(nameof(ShowResult), RpcTarget.All, nameList.ToArray(), killList.ToArray());
        }
    }

    [PunRPC]
    private void ShowResult(string[] nameList, int[] killList)
    {
        RankParent.parent.gameObject.SetActive(true);
        int x = -1, index= -1;
        for(int i = 0;i < nameList.Length; i++)
        {
            if (x != killList[i]) index++;
            var a = Instantiate(rankElement[index], RankParent);
            a.SetData(nameList[i], killList[i]);
            x = killList[i];
        }
    }

    [PunRPC]
    private void ReloadRanking(string[] nameList, int[] killList)
    {
        foreach (RankElement x in rankElements)
        {
            x.gameObject.SetActive(false);
        }

        for (int i = 0; i < nameList.Length; i++)
        {
            rankElements[i].gameObject.SetActive(true);
            rankElements[i].SetData(nameList[i], killList[i]);
        }
    }
    
    private void EndGame()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            photonView.RPC(nameof(EndGameRPC), RpcTarget.All);
        }
    }

    [PunRPC]
    private void EndGameRPC()
    {
        //StartCoroutine(LobbyManager.Instance.EndGameIE());
        SceneManager.UnloadSceneAsync("Game", UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
    }

    public void SetLog(string kill, string death)
    {
        photonView.RPC(nameof(SetLogRPC), RpcTarget.All, kill, death);
    }

    [PunRPC]
    private void SetLogRPC(string kill, string death)
    {
        Text a = Instantiate(Log, LogParent);
        a.transform.SetAsFirstSibling();
        a.text = kill + "‚ª" + death + "‚ð“|‚µ‚½";
    }

    public Transform ResPosi(int a)
    {
        if(a < 0)
        {
            return Res[Random.Range(0, Res.Length)];
        }
        else
        {
            return Res[a];
        }
    }
}
