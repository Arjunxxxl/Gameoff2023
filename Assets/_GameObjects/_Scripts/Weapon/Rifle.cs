using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon
{
    [Header("Bullet Spawn Data")]
    [SerializeField] private Transform bulletSpawnT;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    #region Attack
    public void SpawnBullet()
    {
        GameObject obj = objectPooler.SpawnFormPool("rifle bullet", bulletSpawnT.position, Quaternion.LookRotation(bulletSpawnT.forward, Vector3.up));
        obj.GetComponent<Bullet>().ActivateBullet();
    }
    #endregion
}