using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private BulletType bulletType;
    [SerializeField] private bool isBulletActive;

    [Header("Bullet Movement Data")]
    [SerializeField] private bool isMoveToPoint;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector3 moveDir;
    [SerializeField] private Vector3 destinationPos;

    [Header("Bullet Active Duration")]
    [SerializeField] private float bulletMaxActiveDuration;
    [SerializeField] private float bulletActiveTimeElapsed;

    // Update is called once per frame
    void Update()
    {
        MoveBullet();
        UpdateBulletActiveDuration();
    }

    #region SetUp
    public void ActivateBullet(bool isMoveToPoint, Vector3 destinationPos)
    {
        this.isMoveToPoint = isMoveToPoint;

        if (isMoveToPoint)
        {
            this.destinationPos = destinationPos;
        }
        else
        {
            moveDir = transform.forward;
        }

        isBulletActive = true;
        bulletActiveTimeElapsed = 0;
    }

    private void DeactivateBullet()
    {
        bulletActiveTimeElapsed = 0;
        isBulletActive = false;

        gameObject.SetActive(false);
    }
    #endregion

    #region Movement
    private void MoveBullet()
    {
        if(!isBulletActive)
        {
            return;
        }

        if (isMoveToPoint)
        {
            transform.position = Vector3.MoveTowards(transform.position, destinationPos, moveSpeed * Time.deltaTime);

            if(Vector3.Distance(transform.position, destinationPos) < 0.05f)
            {
                DeactivateBullet();
            }
        }
        else
        {
            transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
        }
    }
    #endregion

    #region Time Active Duration
    private void UpdateBulletActiveDuration()
    { 
        if(!isBulletActive) 
        {
            return; 
        }

        bulletActiveTimeElapsed += Time.deltaTime;

        if(bulletActiveTimeElapsed >= bulletMaxActiveDuration)
        {
            DeactivateBullet();
        }
    }
    #endregion
}
