using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    [Header("Enemy Target")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform enemyTarget;
    [SerializeField] private float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveEnemyTargetToPlayer();
    }

    public void SetUp(Transform player)
    {
        this.player = player;

        enemyTarget.SetParent(null);
    }

    private void MoveEnemyTargetToPlayer()
    {
        enemyTarget.position = Vector3.Lerp(enemyTarget.position, player.position, 1 - Mathf.Pow(0.5f, Time.deltaTime * moveSpeed));
    }
}
