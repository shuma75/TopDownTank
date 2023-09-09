using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

[RequireComponent(typeof(Animator), typeof(PhotonView), typeof(PhotonTransformView))]
[RequireComponent(typeof(PhotonAnimatorView))]
public class SubWeapinHP : MonoBehaviourPunCallbacks
{
    [SerializeField] private int HP;
    [SerializeField] private GameObject chi;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D _rb;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();

        _rb.velocity =  transform.rotation * Vector2.up * 2;
        DOVirtual.DelayedCall(0.7f, () =>
        {
            _rb.velocity = Vector2.zero;
        });
        transform.parent = GameManager.instance.transform;

        if (photonView.IsMine)
        {
            if (chi != null) chi.tag = "Player";
            gameObject.tag = "Player";
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (photonView.IsMine)
        {
            if (collision.CompareTag("Bullet") || collision.CompareTag("Bullet1") || collision.CompareTag("Bullet2"))
            {
                photonView.RPC(nameof(PlayExSE), RpcTarget.All);
                var owner = collision.GetComponent<PhotonView>();
                switch (collision.tag)
                {
                    case "Bullet":
                        HP -= 2;
                        break;
                    case "Bullet1":
                        HP--;
                        break;
                    case "Bullet2":
                        HP -= 3;
                        break;
                }

                if (HP <= 0)
                {
                    animator.SetBool("Destroy", true);
                }
            }

            if(!collision.CompareTag("Player")) _rb.velocity = Vector2.zero;
        }
    }

    [PunRPC]
    private void PlayExSE()
    {
        AudioManager.Instance.PlaySE(6);
    }

    public void DestroySub()
    {
        Destroy(gameObject);
    }

    public void FalseSub()
    {
        spriteRenderer.enabled = false;
    }
}
