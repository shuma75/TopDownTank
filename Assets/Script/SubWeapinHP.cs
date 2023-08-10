using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

[RequireComponent(typeof(Animator),typeof(PhotonView),typeof(PhotonTransformView))]
[RequireComponent(typeof(PhotonAnimatorView))]
public class SubWeapinHP : MonoBehaviourPunCallbacks
{
    [SerializeField] private int HP;
    [SerializeField] private GameObject chi;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if(photonView.IsMine)
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
        }
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
