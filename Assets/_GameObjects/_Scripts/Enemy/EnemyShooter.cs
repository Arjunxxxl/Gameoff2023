using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Shooting Data")]
    [SerializeField] private bool canShoot;
    [SerializeField] private bool isDirectLOSExist;
    [SerializeField] private float shootingRange;
    [SerializeField] private LayerMask playerLayer;

    [Header("Firing Data")]
    [SerializeField] private float firingDelay;
    [SerializeField] private float currentFiringTimeElapsed;

    private Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfCanShoot();
        CheckIfDirectLODExist();
        Shoot();
    }

    public void SetUp(Enemy enemy)
    {
        this.enemy = enemy;

        canShoot = false;
        isDirectLOSExist = false;

        currentFiringTimeElapsed = 0;
    }

    #region Shooting Activation
    private void CheckIfCanShoot()
    {
        canShoot = enemy.enemyMovement.IsAgentActive && enemy.enemyMovement.RemainingDistance < shootingRange;
    }

    private void CheckIfDirectLODExist()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, (enemy.Player.position - transform.position).normalized, out hit, 1000, playerLayer);

        isDirectLOSExist = hit.collider != null;
    }
    #endregion

    #region Shooting
    private void Shoot()
    {
        if(canShoot && isDirectLOSExist)
        {
            currentFiringTimeElapsed += Time.deltaTime;

            if(currentFiringTimeElapsed >= firingDelay)
            {
                //Fire
                currentFiringTimeElapsed = 0;
            }
        }
        else
        {
            currentFiringTimeElapsed = 0;
        }

        enemy.enemyAnimator.SetFire(canShoot && isDirectLOSExist);
    }
    #endregion
}
