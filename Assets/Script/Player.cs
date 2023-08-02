using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviourPunCallbacks
{
    Rigidbody2D rb;

    [SerializeField] float Speed;
    [SerializeField] float AngleSpeed;
    [SerializeField] string BulletName;

    [SerializeField] Transform Barrel;
    [SerializeField] Transform po;
    [SerializeField] Animator animator, player;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject forward;
    [SerializeField] ParticleSystem tiya;

    [SerializeField] Sprite full, empty;
    [SerializeField] Image[] heart;

    int HP;
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
            HP = 3;
            canvas.SetActive(true);
            forward.SetActive(true);
            for (int i = 0; i < 3; i++)
            {
                if (i < HP)
                {
                    heart[i].sprite = full;
                }
                else
                {
                    heart[i].sprite = empty;
                }
            }
        }

        tiya = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine && HP > 0)
        {
            var Vertical = Input.GetAxis("Vertical");
            var Horizontal = Input.GetAxis("Horizontal");

            rb.velocity = transform.rotation * new Vector2(0, -Vertical * Speed);
            rb.angularVelocity = -Horizontal * AngleSpeed;

            var MousePosi = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 lookDir = MousePosi - transform.position;

            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg + 90f;

            Barrel.DORotate(new Vector3(0, 0, angle), 0.1f);

            if (Input.GetMouseButtonDown(0) && shotable)
            {
                PhotonNetwork.Instantiate(BulletName, po.position, Quaternion.Euler(0, 0, angle - 180f));
                animator.SetBool("Shot", true);
                shotable = false;
                DOVirtual.DelayedCall(1, () =>
                {
                    shotable = true;
                    animator.SetBool("Shot", false);
                });
            }

            
        }
        tiya.startRotation = -transform.eulerAngles.z * Mathf.Deg2Rad;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (photonView.IsMine)
        {
            if (collision.CompareTag("Bullet") && !muteki)
            {
                
                Debug.LogWarning("hit!!");
                HP--;

                for(int i = 0;i < 3;i++)
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

                if (HP == 0)
                {
                    player.SetBool("Death", true);
                    DOVirtual.DelayedCall(3, () =>ReSpawn());
                }
                else
                {
                    muteki = true;
                    DOVirtual.DelayedCall(0.5f, ()=>muteki = false);
                }
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
            HP = 3;
            for (int i = 0; i < 3; i++)
            {
                heart[i].sprite = full;
            }

            player.SetBool("Death", false);
        }
    }
}
