using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionDetection : MonoBehaviour
{
    [Header("Ground Detection")]
    [SerializeField] private float groundSphereCastDistance;
    [SerializeField] private float groundSphereCastRadius;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool isGrounded_Phantom;
    [SerializeField] private bool isGrounded;

    [Header("Ground Phantom Time")]
    [SerializeField] private float phantomTime;
    [SerializeField] private float currentPhantomTimeElapsed;

    public bool IsGrounded { get { return isGrounded; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DetectGround();
        GroundDelectionPhantom();
    }

    #region Ground Detection
    private void DetectGround()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, groundSphereCastRadius, Vector3.down, groundSphereCastDistance, groundMask);

        isGrounded_Phantom = hits.Length > 0;

        if(isGrounded_Phantom)
        {
            isGrounded = true;
        }
    }

    private void GroundDelectionPhantom()
    {
        if(!isGrounded_Phantom && isGrounded)
        {
            currentPhantomTimeElapsed += Time.deltaTime;
            if(currentPhantomTimeElapsed >= phantomTime)
            {
                currentPhantomTimeElapsed = 0;
                isGrounded = false;
            }
        }
    }
    #endregion

    #region Trigger Collision
    private void OnTriggerEnter(Collider other)
    {
        switch(other.tag)
        {
            case "Weapon":
                WeaponHandler.EquipWeapon?.Invoke(other.GetComponent<Weapon>());
                break;
        }
    }
    #endregion

    #region Debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawSphere(transform.position + Vector3.down * groundSphereCastDistance, groundSphereCastRadius);
    }
    #endregion
}
