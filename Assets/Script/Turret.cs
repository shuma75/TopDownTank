using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

public class Turret : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform Barrel,po;
    [SerializeField] string BulletName;
    [SerializeField] Animator animator;
    [SerializeField] List<Transform> target;
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            StartCoroutine(Attack());
        }
    }

    private void Update()
    {
        if(photonView.IsMine && target.Count > 0)
        {
            Vector2 lookDir = target[0].position - transform.position;

            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg + 90f;

            Barrel.transform.rotation = Quaternion.RotateTowards(Barrel.transform.rotation, Quaternion.Euler(0, 0, angle), 30 * Time.deltaTime);
        }
    }

    IEnumerator Attack()
    {
        while (true)
        {
            if(!GameManager.instance.inGame)break;
            yield return new WaitForSeconds(2);

            if(target.Count > 0 )
            {
                Vector2 lookDir = target[0].position - transform.position;

                float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg + 90f;
                PhotonNetwork.Instantiate(BulletName, po.position, Quaternion.Euler(0, 0, angle - 180f));
                animator.SetBool("Shot", true);
                DOVirtual.DelayedCall(1, () => animator.SetBool("Shot", false));
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (photonView.IsMine)
        {
            if (collision.CompareTag("Enemy"))
            {
                target.Add(collision.transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (photonView.IsMine)
        {
            target.Remove(collision.transform);
        }
    }
}
