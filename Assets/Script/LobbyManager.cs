using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("カメラ")]
    [SerializeField] CinemachineVirtualCamera Title;
    [SerializeField] CinemachineVirtualCamera LobbyList, Lobby, Option, Credit;
    [Header("タイトル")]
    [SerializeField] InputField NameInput;
    [SerializeField] Button StartButton;
    [SerializeField] Button EndButton;
    [SerializeField] Button OptionButton;
    [Header("ロビーリスト")]
    [SerializeField] InputField RoomNameInput;
    [SerializeField] Button CreateRoomButton;
    [SerializeField] Toggle IsVisible;
    [SerializeField] Dropdown MenberCount;
    [SerializeField] Button LiOptionButton;
    [SerializeField] Button LiBackButton;
    [Header("ロビー")]
    [SerializeField] Button GameStartButton;
    [SerializeField] Button LoQuitButton;
    [SerializeField] Button LoOptionButton;
    [SerializeField] Text[] PlayerName;
    [Header("オプション")]
    [SerializeField] Button CreditButton;
    [Header("クレジット")]
    [SerializeField] Button BackButton;

    [SerializeField] Button GameEndButton;

    // Start is called before the first frame update
    void Start()
    {
        StartButton.onClick.AddListener(() => GameStart());
        CreateRoomButton.onClick.AddListener(() => CreateRoom());
        LiBackButton.onClick.AddListener(() =>
        { 
            LobbyList.Priority = 0;
            if (PhotonNetwork.IsConnected)
                PhotonNetwork.Disconnect();
        });
        GameStartButton.onClick.AddListener(()=>StartGame());
        LoQuitButton.onClick.AddListener(()=>PhotonNetwork.LeaveRoom());
        GameEndButton.onClick.AddListener(()=>EndGame());
    }

    private void GameStart()
    {
        if(NameInput.text.Length > 0)
        {
            PhotonNetwork.NickName = NameInput.text;

            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        LobbyList.Priority = 10;
    }

    private void CreateRoom()
    {
        if(RoomNameInput.text.Length > 0)
        {
            RoomOptions options = new RoomOptions();
            options.IsVisible = IsVisible.isOn;
            switch (MenberCount.value)
            {
                case 0:
                    options.MaxPlayers = 2;
                    break;
                case 1:
                    options.MaxPlayers = 3;
                    break;
                case 2:
                    options.MaxPlayers = 4;
                    break;
                case 3:
                    options.MaxPlayers = 5;
                    Debug.Log("5");
                    break;
                default:
                    options.MaxPlayers = 2;
                    break;
            }

            PhotonNetwork.JoinOrCreateRoom(RoomNameInput.text, options, TypedLobby.Default);
        }
        else
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("失敗");
    }

    public override void OnJoinedRoom()
    {
        Lobby.Priority = 15;

        SetMenberList();
    }

    public override void OnLeftRoom()
    {
        Lobby.Priority = 0;
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        SetMenberList();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        SetMenberList();
    }

    private void SetMenberList()
    {
        foreach(Text text in PlayerName)
        {
            text.transform.parent.gameObject.SetActive(false);
        }

        var players = PhotonNetwork.PlayerList;

        for (int i = 0;i < players.Length;i++)
        {
            PlayerName[i].transform.parent.gameObject.SetActive(true);
            PlayerName[i].text = players[i].NickName;
        }

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            GameStartButton.interactable = true;
        }
        else
        {
            GameStartButton.interactable = false;
        }
    }

    private void StartGame()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient/* && PhotonNetwork.PlayerList.Length >= 2*/)
        {
            photonView.RPC(nameof(MoveGameScene), RpcTarget.All);
        }
    }

    [PunRPC]
    private void MoveGameScene()
    {
        PhotonNetwork.IsMessageQueueRunning = false;

        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
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
        StartCoroutine(EndGameIE());
    }

    private IEnumerator EndGameIE()
    {
        PhotonNetwork.IsMessageQueueRunning = false;

        yield return SceneManager.UnloadSceneAsync("Game", UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

        PhotonNetwork.IsMessageQueueRunning = true;
    }
}
