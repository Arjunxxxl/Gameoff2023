using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    private Enemy enemy;

    public void SetUp(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void DealDamage(int val)
    {
        enemy.enemyHp.Damage(val);
    }
}
