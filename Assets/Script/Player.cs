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
    [SerializeField] string[] SubName;

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
    [SerializeField] GameObject[] Subs;
    [SerializeField] Text Count;
    [SerializeField] Text SubCount;
    [SerializeField] Text[] Discription;
    [SerializeField] DiscriptionData data;
    [SerializeField] Transform Trophy;
    [SerializeField] GameObject trophy;

    int HP;
    int bullet, subcount;
    bool shotable, subable, muteki;
    // Start is called before the first frame update
    void Start()
    {
        if(photonView.IsMine)
        {
            rb = GetComponent<Rigidbody2D>();
            player = GetComponent<Animator>();
            Transform a = GameManager.instance.ResPosi(photonView.Owner.ActorNumber - 1);
            transform.position = a.position;
            transform.rotation = a.rotation;

            GameManager.instance.local = this;
            
            shotable = true;
            subable = true;
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
            subcount = 2;
            Count.text = "Å~Åá";
            SubCount.text = "Å~" + subcount;

            Discription[0].text = data.MainName[0];
            Discription[1].text = data.SubName[0];
            Discription[2].text = data.MainDiscription[0];
            Discription[3].text = data.SubDiscription[0];
        }
        transform.parent = GameManager.instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine && HP > 0 && GameManager.instance.inGame)
        {
            if(muteki)DOVirtual.DelayedCall(0.4f,()=>muteki = false);
            var Vertical = Input.GetAxis("Vertical");
            var Horizontal = Input.GetAxis("Horizontal");

            if(Mathf.Abs(Vertical) > 0)
            {
                tiya.Play();
            }
            else
            {
                tiya.Stop();
            }

            rb.velocity = transform.rotation * new Vector2(0, -Vertical * Speed);
            rb.angularVelocity = -Horizontal * AngleSpeed;

            var MousePosi = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 lookDir = MousePosi - transform.position;

            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg + 90f;

            Barrel.transform.DORotate(new Vector3(0, 0, angle), 0.1f);

            if (Input.GetMouseButtonDown(0) && shotable && bullet > 0)
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
                        bullet--;
                        if(bullet <= 0)
                        {
                            for (int i = 0; i < Bullets.Length; i++)
                            {
                                Bullets[i].SetActive(false);
                                Subs[i].SetActive(false);
                            }
                            Bullets[0].SetActive(true);
                            Subs[0].SetActive(true);

                            kind = BulletKind.Normal;
                            DOTween.To(() => PlayerCamera.m_Lens.OrthographicSize, (value) => PlayerCamera.m_Lens.OrthographicSize = value, 5, 1).SetEase(Ease.InOutExpo);
                            Barrel.sprite = Barrels[0];
                            bullet = int.MaxValue;
                            subcount = 2;
                            Count.text = "Å~Åá";
                            SubCount.text = "Å~" + subcount;

                            DOVirtual.DelayedCall(0.5f, () =>
                            {
                                shotable = true;
                            });
                            break;
                        }
                        Count.text = "Å~" + bullet.ToString();

                        DOVirtual.DelayedCall(1, () =>
                        {
                            shotable = true;
                        });
                        break;
                    case BulletKind.Long:
                        PhotonNetwork.Instantiate(BulletName[2], po.position, Quaternion.Euler(0, 0, angle - 180f));
                        bullet--;
                        if (bullet <= 0)
                        {
                            for (int i = 0; i < Bullets.Length; i++)
                            {
                                Bullets[i].SetActive(false);
                                Subs[i].SetActive(false);
                            }
                            Bullets[0].SetActive(true);
                            Subs[0].SetActive(true);

                            kind = BulletKind.Normal;
                            DOTween.To(() => PlayerCamera.m_Lens.OrthographicSize, (value) => PlayerCamera.m_Lens.OrthographicSize = value, 5, 1).SetEase(Ease.InOutExpo);
                            Barrel.sprite = Barrels[0];
                            bullet = int.MaxValue;
                            subcount = 2;
                            Count.text = "Å~Åá";
                            SubCount.text = "Å~" + subcount;

                            DOVirtual.DelayedCall(0.5f, () =>
                            {
                                shotable = true;
                            });
                            break;
                        }
                        Count.text = "Å~" + bullet.ToString();

                        DOVirtual.DelayedCall(2f, () =>
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

            if (Input.GetMouseButtonDown(1) && subable && subcount > 0)
            {
                subable = false;
                switch (kind)
                {
                    case BulletKind.Normal:
                        PhotonNetwork.Instantiate(SubName[0], transform.position, Quaternion.Euler(0, 0, angle - 180f));
                        subcount--;
                        SubCount.text = "Å~" + subcount;
                        break;
                    case BulletKind.Explosion:
                        PhotonNetwork.Instantiate(SubName[1], transform.position, Quaternion.Euler(0, 0, angle - 180f));
                        subcount--;
                        SubCount.text = "Å~" + subcount;
                        break;
                    case BulletKind.Long:
                        PhotonNetwork.Instantiate(SubName[2], transform.position, Quaternion.Euler(0, 0, angle - 180f));
                        subcount--;
                        SubCount.text = "Å~" + subcount;
                        break;
                }
                DOVirtual.DelayedCall(1,()=> subable = true);
            }


        }
        
        if(photonView.IsMine &&!GameManager.instance.inGame)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            muteki = true;
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
                photonView.RPC(nameof(Hit), RpcTarget.All, owner.Owner.ActorNumber);
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

                    photonView.RPC(nameof(Kill), RpcTarget.All, owner.Owner.ActorNumber);
                    photonView.RPC(nameof(Death), RpcTarget.All, photonView.Owner.ActorNumber);

                    GameManager.instance.SetLog(owner.Owner.NickName, photonView.Owner.NickName);
                }
                else
                {
                    muteki = true;
                    DOVirtual.DelayedCall(0.4f, ()=>muteki = false);
                }
            }
        }

        if (collision.CompareTag("Nor"))
        {
            for (int i = 0; i < Bullets.Length; i++)
            {
                Bullets[i].SetActive(false);
                Subs[i].SetActive(false);
            }
            Bullets[0].SetActive(true);
            Subs[0].SetActive(true);

            kind = BulletKind.Normal;
            DOTween.To(() => PlayerCamera.m_Lens.OrthographicSize, (value) => PlayerCamera.m_Lens.OrthographicSize = value, 5, 1).SetEase(Ease.InOutExpo);
            Barrel.sprite = Barrels[0];
            bullet = int.MaxValue;
            subcount = 2;
            Count.text = "Å~Åá";
            SubCount.text = "Å~" + subcount;
            Discription[0].text = data.MainName[0];
            Discription[1].text = data.SubName[0];
            Discription[2].text = data.MainDiscription[0];
            Discription[3].text = data.SubDiscription[0];
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Exp"))
        {
            for(int i = 0; i < Bullets.Length; i++)
            {
                Bullets[i].SetActive(false);
                Subs[i].SetActive(false);
            }
            Bullets[1].SetActive(true);
            Subs[1].SetActive(true);

            kind = BulletKind.Explosion;
            DOTween.To(() => PlayerCamera.m_Lens.OrthographicSize, (value) => PlayerCamera.m_Lens.OrthographicSize = value, 5, 1).SetEase(Ease.InOutExpo);
            Barrel.sprite = Barrels[1];
            bullet = 15;
            subcount = 3;
            Count.text = "Å~" + bullet.ToString();
            SubCount.text = "Å~" + subcount;
            Discription[0].text = data.MainName[1];
            Discription[1].text = data.SubName[1];
            Discription[2].text = data.MainDiscription[1];
            Discription[3].text = data.SubDiscription[1];

            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Lon"))
        {
            for (int i = 0; i < Bullets.Length; i++)
            {
                Bullets[i].SetActive(false);
                Subs[i].SetActive(false);
            }
            Bullets[2].SetActive(true);
            Subs[2].SetActive(true);

            kind = BulletKind.Long;
            DOTween.To(() => PlayerCamera.m_Lens.OrthographicSize, (value) => PlayerCamera.m_Lens.OrthographicSize = value, 10, 1).SetEase(Ease.InOutExpo);
            Barrel.sprite = Barrels[2];
            bullet = 5;
            subcount = 3;
            Count.text = "Å~" + bullet.ToString();
            SubCount.text = "Å~" + subcount;
            Discription[0].text = data.MainName[2];
            Discription[1].text = data.SubName[2];
            Discription[2].text = data.MainDiscription[2];
            Discription[3].text = data.SubDiscription[2];

            Destroy(collision.gameObject);
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

            Transform a = GameManager.instance.ResPosi(-1);
            transform.position = a.position;
            transform.rotation = a.rotation;
            player.SetBool("Death", false);
            muteki = true;
            DOVirtual.DelayedCall(1, ()=>muteki = false);
        }
    }

    [PunRPC]
    private void Hit(int player)
    {
        GameManager.instance.SetHitCount(player);
    }

    [PunRPC]
    private void Kill(int player)
    {
        GameManager.instance.SetKillCount(player);
    }

    [PunRPC]
    private void Death(int player)
    {
        GameManager.instance.SetDeathCount(player);
    }

    public void SetTrophy()
    {
        Instantiate(trophy, Trophy);
    }
}
