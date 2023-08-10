using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

public class Turret : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform Barrel,po;

    [SerializeField] List<Transform> target;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Attack());
    }

    private void Update()
    {
        if(target.Count > 0)
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
            yield return new WaitForSeconds(1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (photonView.IsMine)
        {
            if (collision.CompareTag("Player"))
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
