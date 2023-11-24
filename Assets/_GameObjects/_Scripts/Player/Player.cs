using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private bool isPlayerActive;

    public CharacterController characterController { get; private set; }
    public PlayerMovement playerMovement { get; private set; }
    public PlyaerHp playerHp { get; private set; }
    public PlayerCollisionDetection collisionDetection { get; private set; }
    public PlayerAnimator playerAnimator { get; private set; }
    public WeaponHandler weaponHandler { get; private set; }
    public ShootingManager shootingManager { get; private set; }
    public TimeController timeController { get; private set; }

    public UserInput userInput { get; private set; }

    public ObjectPooler objectPooler { get; private set; }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
        playerHp = GetComponent<PlyaerHp>();
        collisionDetection = GetComponent<PlayerCollisionDetection>();
        playerAnimator = GetComponent<PlayerAnimator>();
        weaponHandler = GetComponent<WeaponHandler>();
        shootingManager = GetComponent<ShootingManager>();
        timeController = GetComponent<TimeController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        userInput = UserInput.Instance;
        objectPooler = ObjectPooler.Instance;

        SetUp();
        SetPlayerActive();
    }

    public void SetUp()
    {
        //Cursor.lockState = CursorLockMode.Locked;

        isPlayerActive = false;

        userInput.SetUserInputActive(isPlayerActive);

        playerMovement.SetUp(characterController, userInput, collisionDetection, playerAnimator);
        playerHp.SetUp();
        playerAnimator.SetUp();
        playerMovement.SetPlayerActive(isPlayerActive);
        weaponHandler.SetUp(this);
        timeController.SetUp(this);
    }

    public void SetPlayerActive()
    {
        isPlayerActive = true;

        userInput.SetUserInputActive(isPlayerActive);

        playerMovement.SetPlayerActive(isPlayerActive);
    }
}
