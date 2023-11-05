using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private bool isPlayerActive;
    
    [Header("Move Actions")]
    [SerializeField] private bool isWalking;
    [SerializeField] private bool isRunning;
    [SerializeField] private bool isCrouching;

    [Header("Movement Data")]
    [SerializeField] private Vector3 inputDir;
    [SerializeField] private Vector3 moveDir;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveSpeedY;
    [SerializeField] private float moveAirDampingMul;

    [Header("Speed Data")]
    [SerializeField] private float moveSpeedFinal;
    [SerializeField] private float moveSpeedLerp;
    [SerializeField] private float moveSpeedWalk;
    [SerializeField] private float moveSpeedRun;
    [SerializeField] private float moveSpeedCrouch;
    [SerializeField] private float moveSpeedDash;

    [Header("Dash")]
    [SerializeField] private bool isDashActive;
    [SerializeField] private float dashActiveDuration;
    [SerializeField] private float dashActiveTimeElapsed;

    [Header("Jump")]
    [SerializeField] private bool jump;
    [SerializeField] private bool jumpActive;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isForwardJump;
    [SerializeField] private bool isBackwardJump;
    [SerializeField] private float minJumpActiveDuration;
    [SerializeField] private float jumpActiveTimeElepsed;
    [SerializeField] private float jumpForceApplied;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpForceMin;
    [SerializeField] private float jumpForceMax;
    [SerializeField] private float jumpForceDash;

    [Header("Grounded")]
    [SerializeField] private bool isGrounded;

    [Header("Gravity")]
    [SerializeField] private Vector3 gravity;
    [SerializeField] private Vector3 gravityMinimun;
    [SerializeField] private Vector3 gravityNormal;
    [SerializeField] private Vector3 gravityDash;
    [SerializeField] private float gravityLerpSpeed;

    [Header("Rotation")]
    [SerializeField] private float mouseX;
    [SerializeField] private Vector3 finalRotationAngle;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rotationSpeedGround;
    [SerializeField] private float rotationSpeedAir;
    [SerializeField] private float rotationSpeedChangeLerpSpeed;

    [Header("Crouching Data")]
    [SerializeField] private float characterControllerHeight;
    [SerializeField] private Vector3 characterControllerCenter;
    [Space]
    [SerializeField] private float characterControllerHeight_Crouch;
    [SerializeField] private float characterControllerHeight_Standing;
    [Space]
    [SerializeField] private Vector3 characterControllerCenter_Crouch;
    [SerializeField] private Vector3 characterControllerCenter_Stanting;

    private CharacterController characterController;
    private UserInput userInput;
    private PlayerCollisionDetection collisionDetection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetMoveInput();

        CheckGround();
        ApplyGravity();

        if (isPlayerActive)
        {
            SetCharacterControllerHeightAndCenter();

            Dash();

            SetMoveSpeed();

            SetJumpActive();
            Jump();

            Move();
            Rotate();
        }
    }

    #region SetUp
    public void SetUp(CharacterController characterController, UserInput userInput, PlayerCollisionDetection collisionDetection)
    {
        this.characterController = characterController;
        this.userInput = userInput;
        this.collisionDetection = collisionDetection;

        isGrounded = false;

        gravity = gravityMinimun;
    }

    public void SetPlayerActive(bool isPlayerActive)
    {
        this.isPlayerActive = isPlayerActive;

        moveSpeed = moveSpeedWalk;
    }
    #endregion

    #region GetInput
    private void GetMoveInput()
    {
        inputDir = userInput.UserInputDir;

        if (!isGrounded)
        {
            inputDir.x *= moveAirDampingMul;
        }

        jump = userInput.UserInputDir.y != 0;

        isWalking = !userInput.IsSprinting;
        isRunning = userInput.IsSprinting;
        isCrouching = userInput.IsCrouching;

        mouseX = userInput.MouseInputDir.x;

        inputDir.Normalize();
    }
    #endregion

    #region Move 
    private void SetMoveSpeed()
    {
        if (isDashActive)
        {
            moveSpeedFinal = moveSpeedDash;
        }
        else if (isCrouching && !isJumping)
        {
            moveSpeedFinal = moveSpeedCrouch;
        }
        else if (isWalking && !isJumping)
        {
            moveSpeedFinal = moveSpeedWalk;
        }
        else if (isRunning && !isJumping)
        {
            moveSpeedFinal = inputDir.z > 0 ? moveSpeedRun : moveSpeedWalk;
        }
        
        moveSpeed = Mathf.Lerp(moveSpeed, moveSpeedFinal, Time.deltaTime * moveSpeedLerp);
    }

    private void Move()
    {
        moveDir = inputDir.x * transform.right + inputDir.y * transform.up + inputDir.z * transform.forward;
        moveDir.y = moveSpeedY;

        characterController.Move(moveDir * moveSpeed * Time.deltaTime);
    }
    #endregion

    #region Jump
    private void SetJumpActive()
    {
        if (jump && !isJumping)
        {
            if (!jumpActive)
            {
                jumpActiveTimeElepsed = 0;
            }

            if(Vector3.Dot(moveDir, transform.forward) > 0)
            {
                isForwardJump = true;
                isBackwardJump = false;
            }
            else if (Vector3.Dot(moveDir, transform.forward) < 0)
            {
                isForwardJump = false;
                isBackwardJump = true;
            }
            else
            {
                isForwardJump = false;
                isBackwardJump = false;
            }

            jumpActive = true;
            isJumping = true;
        }

        if(jumpActive)
        {
            jumpActiveTimeElepsed += Time.deltaTime;

            if(jumpActiveTimeElepsed > minJumpActiveDuration && !jump)
            {
                jumpActiveTimeElepsed = 0;
                jumpActive = false;
            }
        }
    }

    private void Jump()
    {
        if(jumpActive && isGrounded)
        {
            if (moveDir.x != 0 || moveDir.z != 0)
            {
                if (isDashActive)
                {
                    jumpForceApplied = jumpForceDash;
                }
                else
                {
                    jumpForceApplied = Mathf.Clamp(jumpForce * moveSpeed / moveSpeedWalk, jumpForceMin, jumpForceMax);
                }
            }
            else
            {
                jumpForceApplied = jumpForce;
            }

            moveSpeedY = jumpForceApplied;
        }
    }
    #endregion

    #region Grounded
    private void CheckGround()
    {
        isGrounded = collisionDetection.IsGrounded;
    }
    #endregion

    #region Gravity
    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            if (isDashActive)
            {
                gravity = Vector3.Lerp(gravity, gravityDash, Time.deltaTime * gravityLerpSpeed);
                moveSpeedY += gravityDash.y * Time.deltaTime;
            }
            else
            {
                gravity = Vector3.Lerp(gravity, gravityNormal, Time.deltaTime * gravityLerpSpeed);
                moveSpeedY += gravity.y * Time.deltaTime;
            }
        }
        else
        {
            gravity = gravityMinimun;

            moveSpeedY = 0;

            if(isJumping && isCrouching && inputDir.z > 0)
            {
                SetDashActive();
            }

            isJumping = false;
            isForwardJump = false;
            isBackwardJump = false;
        }
    }
    #endregion

    #region Rotation
    private void Rotate()
    {
        rotationSpeed = Mathf.Lerp(rotationSpeed, isGrounded ? rotationSpeedGround : rotationSpeedAir, Time.deltaTime * rotationSpeedChangeLerpSpeed);

        finalRotationAngle = transform.rotation.eulerAngles;
        finalRotationAngle.y += mouseX;

        finalRotationAngle.y = Mathf.Repeat(finalRotationAngle.y, 360);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(finalRotationAngle), Time.deltaTime * rotationSpeed);
    }
    #endregion

    #region Dash
    private void SetDashActive()
    {
        isDashActive = true;
    }

    private void Dash()
    {
        if(isDashActive)
        {
            dashActiveTimeElapsed += Time.deltaTime;

            if(dashActiveTimeElapsed > dashActiveDuration)
            {
                dashActiveTimeElapsed = 0;
                isDashActive = false;
            }
        }
    }
    #endregion

    #region Character Controller Heigh / Center
    private void SetCharacterControllerHeightAndCenter()
    {
        characterController.height = isCrouching ? characterControllerHeight_Crouch : characterControllerHeight_Standing;
        characterController.center = isCrouching ? characterControllerCenter_Crouch : characterControllerCenter_Stanting;
    }
    #endregion
}
