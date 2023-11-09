using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected WeaponType weaponType;

    [Header("Weapon Parent")]
    [SerializeField] protected Transform weaponParentT;
    [SerializeField] protected Player player;

    [Header("Attack Time Data")]
    [SerializeField] protected bool userAttackInput;
    [SerializeField] protected bool isAttacking;
    [SerializeField] protected bool attacked;
    [SerializeField] protected float attackDelay;
    [SerializeField] protected float nextAttackDelay;
    [SerializeField] protected float attackTimeElapsed;

    [Header("Attack Particle")]
    [SerializeField] private ParticleSystem attackParticleSystem;

    [Header("Enemies")]

    protected ObjectPooler objectPooler;
    [SerializeField] protected TweenUtil tweenUtil;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        objectPooler = ObjectPooler.Instance;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateAtackingTime();
    }

    public void SetUp(Player player)
    {
        this.player = player;

        attackTimeElapsed = 0;
        attacked = false;
    }

    #region Weapon Type / Transfrom
    public WeaponType GetWeaponType()
    {
        return weaponType;
    }

    public void SetNewParent(Transform parent)
    {
        weaponParentT = parent;
        transform.SetParent(parent);
    }
    #endregion

    #region Attack
    public virtual void ActivateWeapon(bool userAttackInput)
    {
        this.userAttackInput = userAttackInput;

        if(this.userAttackInput)
        {
            isAttacking = true;
        }
    }

    protected virtual void UpdateAtackingTime()
    {
        if (!isAttacking)
        {
            return;
        }

        tweenUtil.PlayTween("Recoil Move Pullback");

        attackTimeElapsed += Time.deltaTime;

        if(attackTimeElapsed > attackDelay)
        {
            if (!attacked)
            {
                attacked = true;

                if (attackParticleSystem != null)
                {
                    attackParticleSystem.Play();
                }

                InvokeAttackPerWeapon();
            }
        }

        if (attackTimeElapsed > nextAttackDelay)
        {
            attackTimeElapsed = 0;

            attacked = false;
            isAttacking = false;
        }
    }

    private void InvokeAttackPerWeapon()
    {
        switch(weaponType)
        {
            case WeaponType.Rifle:
                GetComponent<Rifle>().SpawnBullet();
                break;
        }
    }
    #endregion
}
