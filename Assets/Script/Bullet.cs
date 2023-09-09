using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;

public class Bullet : MonoBehaviourPunCallbacks
{
    Rigidbody2D rb;
    Animator animator;
    [SerializeField] float Speed;
    [SerializeField] float LifeTime;
    [SerializeField] bool isBreak;

    Tween delay;
    bool ex;
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            if(CompareTag("Bullet"))ex = true;
            else ex = false;
            gameObject.tag = "Player";

            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();

            rb.velocity = transform.rotation * Vector2.up * Speed;
            delay = DOVirtual.DelayedCall(LifeTime, () =>
            {
                animator.SetBool("Exp", true);
                if (ex) photonView.RPC(nameof(PlayeSE), RpcTarget.All);
                rb.velocity = Vector2.zero;
            });
        }
    }

    [PunRPC]
    private void PlayeSE()
    {
        AudioManager.Instance.PlaySE(1);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(photonView.IsMine)
        {
            bool a = collision.CompareTag("Bullet");
            bool b = collision.CompareTag("Bullet1") && isBreak;
            bool c = collision.CompareTag("Bullet2") && isBreak;
            if (collision.CompareTag("Object") || collision.CompareTag("Enemy") || a || b || c)
            {
                delay.Complete(true);
            }
        }
    }

    public void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
