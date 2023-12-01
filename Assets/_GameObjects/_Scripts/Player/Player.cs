using System;
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
    public PlayerHitBox playerHitBox { get; private set; }
    public PlayerCollisionDetection collisionDetection { get; private set; }
    public PlayerAnimator playerAnimator { get; private set; }
    public WeaponHandler weaponHandler { get; private set; }
    public ShootingManager shootingManager { get; private set; }
    public TimeController timeController { get; private set; }

    public UserInput userInput { get; private set; }

    public ObjectPooler objectPooler { get; private set; }

    public bool IsPlayerActive { get { return isPlayerActive; } }

    public static Action SetUpPlayer;
    public static Action PlayerDied;

    private void OnEnable()
    {
        SetUpPlayer += SetUp;
        PlayerDied += OnPlayerDied;
        GameManager.GameSetCompleted += SetPlayerActive;
    }

    private void OnDisable()
    {
        SetUpPlayer -= SetUp;
        PlayerDied -= OnPlayerDied;
        GameManager.GameSetCompleted -= SetPlayerActive;
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
        playerHp = GetComponent<PlyaerHp>();
        playerHitBox = GetComponentInChildren<PlayerHitBox>();
        collisionDetection = GetComponent<PlayerCollisionDetection>();
        playerAnimator = GetComponent<PlayerAnimator>();
        weaponHandler = GetComponent<WeaponHandler>();
        shootingManager = GetComponent<ShootingManager>();
        timeController = GetComponent<TimeController>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetUp()
    {
        userInput = UserInput.Instance;
        objectPooler = ObjectPooler.Instance;

        isPlayerActive = false;

        userInput.SetUserInputActive(isPlayerActive);

        playerMovement.SetUp(characterController, userInput, collisionDetection, playerAnimator);
        playerHp.SetUp();
        playerHitBox.SetUp(this);
        playerAnimator.SetUp();
        weaponHandler.SetUp(this);
        timeController.SetUp(this);

        playerMovement.SetPlayerActive(isPlayerActive);
    }

    public void SetPlayerActive()
    {
        isPlayerActive = true;

        userInput.SetUserInputActive(isPlayerActive);

        playerMovement.SetPlayerActive(isPlayerActive);
    }

    private void OnPlayerDied()
    {
        isPlayerActive = false;

        userInput.SetUserInputActive(isPlayerActive);

        playerMovement.SetPlayerActive(isPlayerActive);

        UiManager.Instance.ShowGameOverScreen();

        SoundManager.StopAudio("bg");
    }

    public void AmmoPickedUp(int amt)
    {
        weaponHandler.AddAmmo(amt);
    }

    public void GrenadePickedUp(int amt)
    {
        weaponHandler.AddGrenade(amt);
    }

    public void HpPickedUp(int amt)
    {
        playerHp.UpdateHp(amt);
    }
}
