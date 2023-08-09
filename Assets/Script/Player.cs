using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviourPunCallbacks
{
    enum BulletKind
    {
        Normal,
        Explosion,
        Long
    }

    Rigidbody2D rb;

    [SerializeField] float Speed;
    [SerializeField] float AngleSpeed;
    [SerializeField] string[] BulletName;

    [SerializeField] SpriteRenderer Barrel;
    [SerializeField] Sprite[] Barrels;
    [SerializeField] Transform po;
    [SerializeField] Animator animator, player;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject forward;
    [SerializeField] ParticleSystem tiya;
    [SerializeField] CinemachineVirtualCamera PlayerCamera;

    [SerializeField] BulletKind kind;

    [SerializeField] Sprite full, empty;
    [SerializeField] Image[] heart;
    [SerializeField] GameObject[] Bullets;
    [SerializeField] Text Count;

    int HP;
    int bullet;
    bool shotable, muteki;
    // Start is called before the first frame update
    void Start()
    {
        if(photonView.IsMine)
        {
            rb = GetComponent<Rigidbody2D>();
            player = GetComponent<Animator>();
            
            shotable = true;
            muteki = false;
            transform.tag = "Player";
            HP = 5;
            canvas.SetActive(true);
            //forward.SetActive(true);
            for (int i = 0; i < 5; i++)
            {
                heart[i].sprite = full;
            }
            PlayerCamera.Priority = 100;
            bullet = int.MaxValue;
        }
        transform.parent = GameManager.instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine && HP > 0 && GameManager.instance.inGame)
        {
            var Vertical = Input.GetAxis("Vertical");
            var Horizontal = Input.GetAxis("Horizontal");

            if(Mathf.Abs(Vertical) > 0)
            {
                tiya.Play();
            }
            else
            {
                tiya.Pause();
            }

            rb.velocity = transform.rotation * new Vector2(0, -Vertical * Speed);
            rb.angularVelocity = -Horizontal * AngleSpeed;

            var MousePosi = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 lookDir = MousePosi - transform.position;

            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg + 90f;

            Barrel.transform.DORotate(new Vector3(0, 0, angle), 0.1f);

            if (Input.GetMouseButtonDown(0) && shotable)
            {
                animator.SetBool("Shot", true);
                shotable = false;

                switch (kind)
                {
                    case BulletKind.Normal:
                        PhotonNetwork.Instantiate(BulletName[0], po.position, Quaternion.Euler(0, 0, angle - 180f));

                        DOVirtual.DelayedCall(0.5f, () =>
                        {
                            shotable = true;
                        });
                        break;
                    case BulletKind.Explosion:
                        PhotonNetwork.Instantiate(BulletName[1], po.position, Quaternion.Euler(0, 0, angle - 180f));

                        DOVirtual.DelayedCall(1, () =>
                        {
                            shotable = true;
                        });
                        break;
                    case BulletKind.Long:
                        PhotonNetwork.Instantiate(BulletName[2], po.position, Quaternion.Euler(0, 0, angle - 180f));

                        DOVirtual.DelayedCall(1.5f, () =>
                        {
                            shotable = true;
                        });
                        break;
                }
            }
            else
            {
                animator.SetBool("Shot", false);
            }


        }
        
        if(photonView.IsMine &&!GameManager.instance.inGame)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
        }

        tiya.startRotation = -transform.eulerAngles.z * Mathf.Deg2Rad;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (photonView.IsMine)
        {
            if (collision.CompareTag("Bullet") || collision.CompareTag("Bullet1") || collision.CompareTag("Bullet2"))
            {
                if (muteki) return;
                var owner = collision.GetComponent<PhotonView>();
                photonView.RPC(nameof(Hit), RpcTarget.All, owner.Owner);
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

                for(int i = 0;i < 5;i++)
                {
                    if(i < HP)
                    {
                        heart[i].sprite = full;
                    }
                    else
                    {
                        heart[i].sprite = empty;
                    }
                }

                if (HP <= 0)
                {
                    player.SetBool("Death", true);
                    DOVirtual.DelayedCall(3, () =>ReSpawn());

                    photonView.RPC(nameof(Kill), RpcTarget.All, owner.Owner);
                    photonView.RPC(nameof(Death), RpcTarget.All, photonView.Owner);

                    GameManager.instance.SetLog(owner.Owner.NickName, photonView.Owner.NickName);
                }
                else
                {
                    muteki = true;
                    DOVirtual.DelayedCall(0.4f, ()=>muteki = false);
                }
            }
            else if (collision.CompareTag("Nor"))
            {
                kind = BulletKind.Normal;
                DOTween.To(() => PlayerCamera.m_Lens.OrthographicSize, (value) => PlayerCamera.m_Lens.OrthographicSize = value, 5, 1).SetEase(Ease.InOutExpo);
                Barrel.sprite = Barrels[0];
                bullet = int.MaxValue;
                PhotonNetwork.Destroy(collision.gameObject);
            }
            else if (collision.CompareTag("Exp"))
            {
                kind = BulletKind.Explosion;
                DOTween.To(() => PlayerCamera.m_Lens.OrthographicSize, (value) => PlayerCamera.m_Lens.OrthographicSize = value, 5, 1).SetEase(Ease.InOutExpo);
                Barrel.sprite = Barrels[1];
                bullet = 15;
                PhotonNetwork.Destroy(collision.gameObject);
            }
            else if (collision.CompareTag("Lon"))
            {
                kind = BulletKind.Long;
                DOTween.To(() => PlayerCamera.m_Lens.OrthographicSize, (value) => PlayerCamera.m_Lens.OrthographicSize = value, 10, 1).SetEase(Ease.InOutExpo);
                Barrel.sprite = Barrels[2];
                bullet = 10;
                PhotonNetwork.Destroy(collision.gameObject);
            }
        }
    }

    public void DestroyObject()
    {
        if (photonView.IsMine)
        {
            Destroy(gameObject);
        }
    }

    public void ReSpawn()
    {
        if(photonView.IsMine)
        {
            HP = 5;
            for (int i = 0; i < 5; i++)
            {
                heart[i].sprite = full;
            }

            transform.position = new Vector3(Random.Range(-8, 8), Random.Range(-8, 8));
            player.SetBool("Death", false);
            muteki = true;
            DOVirtual.DelayedCall(1, ()=>muteki = false);
        }
    }

    [PunRPC]
    private void Hit(Photon.Realtime.Player player)
    {
        GameManager.instance.SetHitCount(player);
    }

    [PunRPC]
    private void Kill(Photon.Realtime.Player player)
    {
        GameManager.instance.SetKillCount(player);
    }

    [PunRPC]
    private void Death(Photon.Realtime.Player player)
    {
        GameManager.instance.SetDeathCount(player);
    }
}
