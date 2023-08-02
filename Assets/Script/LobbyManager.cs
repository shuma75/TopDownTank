using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

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
    [Header("オプション")]
    [SerializeField] Button CreditButton;
    [Header("クレジット")]
    [SerializeField] Button BackButton;
    
    // Start is called before the first frame update
    void Start()
    {
        StartButton.onClick.AddListener(()=>GameStart());
        CreateRoomButton.onClick.AddListener(()=>CreateRoom());
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
    }
}
