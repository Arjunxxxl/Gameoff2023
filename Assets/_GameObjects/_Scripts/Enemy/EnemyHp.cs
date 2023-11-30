using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHp : MonoBehaviour
{
    [Header("HP Data")]
    [SerializeField] private int maxHp;
    [SerializeField] private int currentHp;

    [Header("Hit Effect")]
    [SerializeField] private ParticleSystem hitEfect;

    private Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp(Enemy enemy)
    {
        this.enemy = enemy;

        currentHp = maxHp;
    }

    public void Damage(int val)
    {
        currentHp -= val;

        if(currentHp <= 0)
        {
            currentHp = 0;

            ObjectPooler.Instance.SpawnFormPool("Enemy Explosion", transform.position + enemy.enemyMovement.EnemyCenterOffset, Quaternion.identity).GetComponent<ParticleSystem>().Play();

            EnemySpawner.RemoveEnemy?.Invoke(enemy);

            gameObject.SetActive(false);
        }
        else
        {
            hitEfect.Play();
        }
    }
}
