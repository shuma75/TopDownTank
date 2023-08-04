using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Initialize : MonoBehaviourPunCallbacks
{
    [SerializeField] GameManager GameManage;
    private bool[] ChPl;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;

        // ランダムな座標に自身のアバター（ネットワークオブジェクト）を生成する
        var position = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
        var localPlayer = PhotonNetwork.LocalPlayer;
        PhotonNetwork.Instantiate("Player" + localPlayer.ActorNumber, position, Quaternion.identity);

        if(PhotonNetwork.LocalPlayer.IsMasterClient) ChPl = new bool[PhotonNetwork.PlayerList.Length];

        photonView.RPC(nameof(CheckPlayer), RpcTarget.MasterClient, localPlayer.ActorNumber);
    }

    [PunRPC]
    private void CheckPlayer(int id)
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            ChPl[id] = true;

            for(int i = 0; i < ChPl.Length; i++)
            {
                if (!ChPl[i]) return;
            }

            photonView.RPC(nameof(StartGame), RpcTarget.All);
        }
    }

    [PunRPC]
    private void StartGame()
    {
        GameManage.enabled = true;
    }
}
