using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    private bool isInputsActive;

    [Header("Inputs")]
    [SerializeField] private Vector3 input;
    [SerializeField] private Vector2 mouseInput;
    [SerializeField] private bool isSprinting;
    [SerializeField] private bool isCrouching;

    [Header("Customization")]
    [SerializeField] private float mouseSensivityX;
    [SerializeField] private float mouseSensivityY;

    public Vector3 UserInputDir { get { return input; } }
    public Vector3 MouseInputDir { get { return mouseInput; } }
    public bool IsSprinting { get { return isSprinting; } }
    public bool IsCrouching { get { return isCrouching; } }

    #region SingleTon
    public static UserInput Instance;
    private void Awake()
    {
        if(!Instance)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isInputsActive)
        {
            return;
        }

        GetMoveInput();
        GetJumpInput();
        GetSprintInput();
        GetCrouchInput();

        MouseInput();
    }

    public void SetUserInputActive(bool isActive)
    {
        this.isInputsActive = isActive;
    }

    private void GetMoveInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.z = Input.GetAxisRaw("Vertical");
    }

    private void GetJumpInput()
    {
        input.y = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    private void GetSprintInput()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private void GetCrouchInput()
    {
        isCrouching = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        if (input.y != 0)
        {
            isCrouching = false;
        }
    }

    private void MouseInput()
    {
        mouseInput.x = Input.GetAxis("Mouse X");
        mouseInput.y = Input.GetAxis("Mouse Y");

        mouseInput.x *= mouseSensivityX;
        mouseInput.y *= mouseSensivityY;
    }
}
