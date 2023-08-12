using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ItemBox : MonoBehaviourPunCallbacks
{
    [SerializeField] string[] items;

    GameObject a;
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            StartCoroutine(Set());
        }
    }

    IEnumerator Set()
    {
        while (true)
        {
            yield return a == null;
            yield return new WaitForSeconds(10);

            if(a == null)
            {
                a = PhotonNetwork.Instantiate(items[Random.Range(0, items.Length)], transform.position, Quaternion.identity);
            }
        }
    }
}
