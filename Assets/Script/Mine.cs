using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Mine : MonoBehaviourPunCallbacks
{
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        transform.parent = GameManager.instance.transform;
        if (photonView.IsMine)
        {
            transform.gameObject.layer = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (photonView.IsMine)
        {
            if (collision.CompareTag("Enemy"))
            {
                animator.SetBool("Destroy", true);
                photonView.RPC(nameof(PlayExSE), RpcTarget.All);  
            }
        }
    }

    [PunRPC]
    private void PlayExSE()
    {
        AudioManager.Instance.PlaySE(1);
    }
}
