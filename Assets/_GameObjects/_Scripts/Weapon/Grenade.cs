using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private bool isGrenadeActive;

    [Header("Grenade Movement Data")]
    [SerializeField] private bool isMoveToPoint;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveSpeedDefault;
    [SerializeField] private Vector3 moveDir;
    [SerializeField] private Vector3 destinationPos;

    [Header("Rotation")]
    [SerializeField] private Vector3 roationDirection;
    [SerializeField] private float roationSpeed;

    [Header("Grenade Active Duration")]
    [SerializeField] private float grenadeMaxActiveDuration;
    [SerializeField] private float grenadeActiveTimeElapsed;

    [Header("Explosion Data")]
    [SerializeField] private float explosionRange;
    [SerializeField] private LayerMask explosionLayer;
    [SerializeField] private List<GameObject> objectsInEplosionRange;

    [Header("Damage")]
    [SerializeField] private int damageAmt;

    Rigidbody rb;

    // Update is called once per frame
    void Update()
    {
        //MoveGrenade();
        //Rotate();

        UpdateGrenadeActiveDuration();
    } 

    #region SetUp
    public void ActivateGrenade(bool isMoveToPoint, Vector3 forwardDir, Vector3 destinationPos, float playerSpeed)
    {
        if(!rb)
        {
            rb = GetComponent<Rigidbody>();
        }

        this.isMoveToPoint = isMoveToPoint;

        moveSpeed = moveSpeedDefault + playerSpeed;

        isGrenadeActive = true;
        grenadeActiveTimeElapsed = 0;

        roationDirection = Vector3.one * Random.Range(- 1f, 1f);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (isMoveToPoint)
        {
            this.destinationPos = destinationPos;
            moveDir = (destinationPos - transform.position).normalized + (Vector3.up * Vector3.Magnitude(destinationPos - transform.position) / 1000f);
            rb.AddForce(moveDir * moveSpeed, ForceMode.VelocityChange);
        }
        else
        {
            moveDir = forwardDir + Vector3.up * 0.25f;
            rb.AddForce(moveDir * moveSpeed, ForceMode.VelocityChange);
        }

        rb.angularVelocity = roationDirection * moveSpeed;
    }

    private void DeactivateGrenade()
    {
        grenadeActiveTimeElapsed = 0;
        isGrenadeActive = false;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        TryDoingDamage();

        CameraShake.ShakeCamera?.Invoke(10, 2);

        ObjectPooler.Instance.SpawnFormPool("GrenadeExplosionEffect", transform.position, Quaternion.identity).GetComponent<ParticleSystem>().Play();

        gameObject.SetActive(false);
    }
    #endregion

    #region Movement
    private void MoveGrenade()
    {
        if (!isGrenadeActive)
        {
            return;
        }

        if (isMoveToPoint)
        {
            transform.position = Vector3.MoveTowards(transform.position, destinationPos, moveSpeed * Time.unscaledDeltaTime);

            if (Vector3.Distance(transform.position, destinationPos) < 0.05f)
            {
                DeactivateGrenade();
            }
        }
        else
        {
            transform.Translate(moveDir * moveSpeed * Time.unscaledDeltaTime, Space.World);
        }
    }

    private void Rotate()
    {
        if(!isGrenadeActive)
        {
            return;
        }

        transform.Rotate(Time.unscaledDeltaTime * roationSpeed * roationDirection, Space.World);
    }
    #endregion

    #region Time Active Duration
    private void UpdateGrenadeActiveDuration()
    {
        if (!isGrenadeActive)
        {
            return;
        }

        grenadeActiveTimeElapsed += Time.deltaTime;

        if (grenadeActiveTimeElapsed >= grenadeMaxActiveDuration)
        {
            DeactivateGrenade();
        }
    }
    #endregion

    #region Damage
    private void TryDoingDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRange, explosionLayer);

        objectsInEplosionRange.Clear();

        foreach (var hitCollider in hitColliders)
        {
            objectsInEplosionRange.Add(hitCollider.gameObject);

            if(hitCollider.GetComponent<EnemyHitBox>())
            {
                hitCollider.GetComponent<EnemyHitBox>().DealDamage(damageAmt);
            }
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.25f);

        Gizmos.DrawSphere(transform.position, explosionRange);
    }
}
