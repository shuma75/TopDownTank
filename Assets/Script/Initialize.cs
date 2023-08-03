using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Initialize : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject GameManage;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;

        // ランダムな座標に自身のアバター（ネットワークオブジェクト）を生成する
        var position = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
        var localPlayer = PhotonNetwork.LocalPlayer;
        PhotonNetwork.Instantiate("Player" + localPlayer.ActorNumber, position, Quaternion.identity);
    }
}
