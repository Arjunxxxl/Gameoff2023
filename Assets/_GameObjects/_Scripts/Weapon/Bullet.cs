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
    [SerializeField] private GameObject bulletHitObj;

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
    public void ActivateBullet(bool isMoveToPoint, Vector3 destinationPos, GameObject hitObj)
    {
        this.isMoveToPoint = isMoveToPoint;
        bulletHitObj = hitObj;

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

    private void DeactivateBullet(bool isHit)
    {
        bulletActiveTimeElapsed = 0;
        isBulletActive = false;

        if (bulletHitObj != null && bulletHitObj.GetComponent<EnemyHitBox>())
        {
            bulletHitObj.GetComponent<EnemyHitBox>().DealDamage(1);
        }
        else
        {
            ObjectPooler.Instance.SpawnFormPool("BulletImpactEffect", transform.position,
                                            Quaternion.Euler(transform.rotation.eulerAngles.x,
                                                            transform.rotation.eulerAngles.y * (isHit ? -1 : 1),
                                                            transform.rotation.eulerAngles.z)).GetComponent<ParticleSystem>().Play();
        }

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
            transform.position = Vector3.MoveTowards(transform.position, destinationPos, moveSpeed * Time.unscaledDeltaTime);

            if(Vector3.Distance(transform.position, destinationPos) < 0.05f)
            {
                DeactivateBullet(true);
            }
        }
        else
        {
            transform.Translate(moveDir * moveSpeed * Time.unscaledDeltaTime, Space.World);
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

        bulletActiveTimeElapsed += Time.unscaledDeltaTime;

        if(bulletActiveTimeElapsed >= bulletMaxActiveDuration)
        {
            DeactivateBullet(false);
        }
    }
    #endregion
}
