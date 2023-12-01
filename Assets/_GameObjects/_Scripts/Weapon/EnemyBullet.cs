using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private bool isBulletActive;

    [Header("Bullet Movement Data")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector3 moveDir;

    [Header("Bullet Active Duration")]
    [SerializeField] private float bulletMaxActiveDuration;
    [SerializeField] private float bulletActiveTimeElapsed;

    [Header("Explosion Data")]
    [SerializeField] private float damageRange;
    [SerializeField] private LayerMask damageLayer;

    [Header("Bullet Particle")]
    [SerializeField] private ParticleSystem bulleftPArticleEffect;

    // Update is called once per frame
    void Update()
    {
        MoveBullet();
        TryDoingDamage();
        UpdateBulletActiveDuration();
    }

    #region SetUp
    public void ActivateBullet(Vector3 _moveDir)
    {
        moveDir = _moveDir;

        isBulletActive = true;
        bulletActiveTimeElapsed = 0;

        if (bulleftPArticleEffect != null)
        {
            bulleftPArticleEffect.Play();
        }
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
        if (!isBulletActive)
        {
            return;
        }

        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
    }
    #endregion

    #region Time Active Duration
    private void UpdateBulletActiveDuration()
    {
        if (!isBulletActive)
        {
            return;
        }

        bulletActiveTimeElapsed += Time.unscaledDeltaTime;

        if (bulletActiveTimeElapsed >= bulletMaxActiveDuration)
        {
            DeactivateBullet();
        }
    }
    #endregion

    #region Damage
    private void TryDoingDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRange, damageLayer);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.GetComponent<PlayerHitBox>())
            {
                Debug.Log("Hit");
                hitCollider.GetComponent<PlayerHitBox>().DealDamage(1);
            }

            DeactivateBullet();
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.25f);

        Gizmos.DrawSphere(transform.position, damageRange);
    }
}
