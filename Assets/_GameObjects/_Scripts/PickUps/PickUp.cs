using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] private PickUpType pickUpType;
    [SerializeField] private int pickUpAmout;
    [SerializeField] private float pickUpRange;
    [SerializeField] private LayerMask playerLayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckForPlayerInRange();
    }

    private void CheckForPlayerInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickUpRange, playerLayer);

        if(hitColliders.Length > 0)
        {
            switch(pickUpType)
            {
                case PickUpType.ammo_pickup:
                    hitColliders[0].GetComponent<Player>().AmmoPickedUp(pickUpAmout);
                    break;

                case PickUpType.grenade_pickup:
                    hitColliders[0].GetComponent<Player>().GrenadePickedUp(pickUpAmout);
                    break;

                case PickUpType.hp_pickup:
                    hitColliders[0].GetComponent<Player>().HpPickedUp(pickUpAmout);
                    break;
            }

            SoundManager.PlayAudio?.Invoke("pick up", true, true);

            gameObject.SetActive(false);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.25f);

        Gizmos.DrawSphere(transform.position, pickUpRange);
    }
}
