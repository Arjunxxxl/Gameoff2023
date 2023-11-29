using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform player;

    [Header("Enemy Type")]
    [SerializeField] private List<GameObject> enemyTypes;

    public EnemyMovement enemyMovement {  get; private set; }
    public EnemyShooter enemyShooter {  get; private set; }
    public EnemyAnimator enemyAnimator {  get; private set; }
    public EnemyHp enemyHp { get; private set; }
    public EnemyHitBox enemyHitBox { get; private set; }

    public Transform Player {  get { return player; } }

    // Start is called before the first frame update
    void Start()
    {
        SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        enemyShooter = GetComponent<EnemyShooter>();
        enemyAnimator = GetComponent<EnemyAnimator>();
        enemyHp = GetComponent<EnemyHp>();
        enemyHitBox = GetComponentInChildren<EnemyHitBox>();

        SetRandonEnemyType();

        enemyMovement.SetUp(this, player);
        enemyShooter.SetUp(this);
        enemyHp.SetUp(this);
        enemyHitBox.SetUp(this);
    }

    private void SetRandonEnemyType()
    {
        for (int i = 0; i < enemyTypes.Count; i++)
        {
            enemyTypes[i].SetActive(false);
        }

        enemyTypes[Random.Range(0, enemyTypes.Count)].SetActive(true);
    }
}
