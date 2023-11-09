using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private bool isPlayerActive;

    [Header("Refs")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private UserInput userInput;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerCollisionDetection collisionDetection;
    [SerializeField] private PlayerAnimator playerAnimator;
    [SerializeField] private WeaponHandler weaponHandler;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
        collisionDetection = GetComponent<PlayerCollisionDetection>();
        playerAnimator = GetComponent<PlayerAnimator>();
        weaponHandler = GetComponent<WeaponHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        userInput = UserInput.Instance;

        SetUp();
        SetPlayerActive();
    }

    public void SetUp()
    {
        Cursor.lockState = CursorLockMode.Locked;

        isPlayerActive = false;

        userInput.SetUserInputActive(isPlayerActive);

        playerMovement.SetUp(characterController, userInput, collisionDetection, playerAnimator);
        playerMovement.SetPlayerActive(isPlayerActive);
        weaponHandler.SetUp(this, userInput, collisionDetection);
    }

    public void SetPlayerActive()
    {
        isPlayerActive = true;

        userInput.SetUserInputActive(isPlayerActive);

        playerMovement.SetPlayerActive(isPlayerActive);
    }
}
