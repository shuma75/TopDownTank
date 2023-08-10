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
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            gameObject.tag = "Player";

            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();

            rb.velocity = transform.rotation * Vector2.up * Speed;
            delay = DOVirtual.DelayedCall(LifeTime, () =>
            {
                animator.SetBool("Exp", true);
                rb.velocity = Vector2.zero;
            });
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(photonView.IsMine)
        {
            bool a = collision.CompareTag("Bullet") && isBreak;
            if (collision.CompareTag("Object") || collision.CompareTag("Enemy") || a)
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
