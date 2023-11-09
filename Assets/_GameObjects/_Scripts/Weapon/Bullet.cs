using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private BulletType bulletType;
    [SerializeField] private bool isBulletActive;

    [Header("Bullet Movement Data")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector3 moveDir;

    [Header("Bullet Active Duration")]
    [SerializeField] private float bulletMaxActiveDuration;
    [SerializeField] private float bulletActiveTimeElapsed;

    // Update is called once per frame
    void Update()
    {
        MoveBullet();
        UpdateBulletActiveDuration();
    }

    public void ActivateBullet()
    {
        isBulletActive = true;
        bulletActiveTimeElapsed = 0;

        moveDir = transform.forward;
    }

    private void MoveBullet()
    {
        if(!isBulletActive)
        {
            return;
        }

        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
    }

    private void UpdateBulletActiveDuration()
    { 
        if(!isBulletActive) 
        {
            return; 
        }

        bulletActiveTimeElapsed += Time.deltaTime;

        if(bulletActiveTimeElapsed >= bulletMaxActiveDuration)
        {
            bulletActiveTimeElapsed = 0;
            isBulletActive = false;

            gameObject.SetActive(false);
        }
    }
}
