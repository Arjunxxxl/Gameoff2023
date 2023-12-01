using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Navigation Data")]
    [SerializeField] private bool isDestinationReached;
    [SerializeField] private float moveSpeedMin;
    [SerializeField] private float moveSpeedMax;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float stoppingDistance;
    [SerializeField] private float remainingDistance;
    [SerializeField] private Vector3 enemyCenterOffset;

    [Header("Active Data")]
    [SerializeField] private bool isAgentActive;
    [SerializeField] private bool chasePlayer;
    [SerializeField] private float activationRange;
    [SerializeField] private LayerMask playerLayer;

    private Enemy enemy;
    private NavMeshAgent agent;

    public bool IsAgentActive { get { return isAgentActive; } }
    public float RemainingDistance { get { return remainingDistance; } }
    public Vector3 EnemyCenterOffset { get { return enemyCenterOffset; } }
     
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CheckForPlayerInRange();
        MoveAgent();
        Rotate();
    }

    public void SetUp(Enemy enemy, Transform target)
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        this.enemy = enemy;

        agent.speed = Random.Range(moveSpeedMin, moveSpeedMax);
        agent.angularSpeed = turnSpeed;
        agent.stoppingDistance = stoppingDistance;

        this.target = target;

        isAgentActive = true;
        chasePlayer = false;
    }

    public void SetChasePlayer(bool chasePlayer)
    {
        this.chasePlayer = chasePlayer;
    }

    #region Activation/Deactivation
    private void CheckForPlayerInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, activationRange, playerLayer);

        isAgentActive = hitColliders.Length > 0;
    }
    #endregion

    #region Move
    private void MoveAgent()
    {
        if (isAgentActive || chasePlayer)
        {
            agent.SetDestination(target.position - enemyCenterOffset);

            remainingDistance = agent.remainingDistance;
            isDestinationReached = remainingDistance - stoppingDistance <= 0;
        }
        else
        {
            remainingDistance = -1;
            isDestinationReached = false;
        }

        enemy.enemyAnimator.SetRunning(((isAgentActive || chasePlayer) && !isDestinationReached) ? 1 : 0);
    }

    private void Rotate()
    {
        if(isDestinationReached)
        {
            Vector3 rot = Vector3.RotateTowards(transform.forward, target.transform.position - transform.position - enemyCenterOffset, Time.deltaTime * 10, 0f);
            transform.rotation = Quaternion.LookRotation(rot);
        }
    }
    #endregion


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.25f);

        Gizmos.DrawSphere(transform.position, activationRange);
    }
}
