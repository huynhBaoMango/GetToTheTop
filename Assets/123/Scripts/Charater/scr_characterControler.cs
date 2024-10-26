using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Scr_Models;

public class scr_characterControler : MonoBehaviour
{
    private CharacterController characterController;
    private Defaultinput defaultInput;
    [HideInInspector]
    public Vector2 input_Movement;
    [HideInInspector]
    public Vector2 input_View;

    private Vector3 newCameraRotation;
    private Vector3 newCharacterRotation;

    [Header("References")]
    public Transform cameraHolder;
    public Transform camera;
    public Transform feetTranform;

    [Header("Settings")]
    public PlayerSettingsModel playerSettings;
    public float viewClampYMin = -70;
    public float viewClampYMax = 80;
    public LayerMask playerMask;
    public LayerMask groundMask;

    [Header("Gravity")]
    public float gravityAmout;
    public float gravityMin;
    private float playerGravity;


    public Vector3 jumpingForce;
    private Vector3 jumpingForceVelocity;

    [Header("Stance")]
    public PlayerStance playerStance;
    public float playerStanceSmoothing;
    public CharacterStance playerStandStance;
    public CharacterStance playerCrouchStance;
    public CharacterStance playerProneStance;
    private float stanceCheckErrorMargin = 0.05f;
    private float cameraHeight;
    private float cameraHeightVelocity;

    private Vector3 stanceCapsuleCenterVelocity;
    private float stanceCapsuleHeightVelocity;

    [HideInInspector]
    public bool isSprinting;

    private Vector3 newMovementSpeed;
    private Vector3 newMovementSpeedVelocity;


    [Header("Weapon")]
    public scr_WeaponComtroller currentWeapon;
    public float weaponAnimationSpeed;

    [HideInInspector]
    public bool isGrounded;
    [HideInInspector]
    public bool isFalling;

    [Header("Leaning")]
    public Transform LeanPivot;
    private float currentLean;
    private float targetLean;
    public float leanAngle;
    public float leanSmoothing;
    private float leanVelocity;

    private bool isLeanLeft;
    private bool isLeanRight;

    [Header("Aiming In")]
    public bool isAimingIn;

    #region - Awake -


    private void Awake()
    {
        defaultInput = new Defaultinput();

        defaultInput.Character.Movement.performed += e => input_Movement = e.ReadValue<Vector2>();
        defaultInput.Character.View.performed += e => input_View = e.ReadValue<Vector2>();
        defaultInput.Character.Jump.performed += e => Jump();
        defaultInput.Character.Crouch.performed += e => Crouch();
        defaultInput.Character.Prone.performed += e => Prone();
        defaultInput.Character.Sprint.performed += e => ToggleSprint();
        defaultInput.Character.SprintReleased.performed += e => StopSprint();
        
        defaultInput.Character.LeanLeft.performed += e => isLeanLeft = true;
        defaultInput.Character.LeanLeftReleased.performed += e => isLeanLeft = false;
        defaultInput.Character.LeanRight.performed += e => isLeanRight = true;
        defaultInput.Character.LeanRightReleased.performed += e => isLeanRight = false;

        defaultInput.Weapon.FirePressed.performed += e => AimingInPressed();
        defaultInput.Weapon.Fire2Released.performed += e => AimingInReleased();

        defaultInput.Weapon.Fire1Pressed.performed += e => ShootingPressed();
        defaultInput.Weapon.Fire1Released.performed += e => ShootingReleased();


        defaultInput.Enable();


        newCameraRotation = cameraHolder.localRotation.eulerAngles;
        newCharacterRotation = transform.localRotation.eulerAngles;

        characterController = GetComponent<CharacterController>();

        cameraHeight = cameraHolder.localPosition.y;

        if (currentWeapon)
        {
            currentWeapon.Initialise(this);
        }

    }

    #endregion

    #region - Update -

    private void Update()
    {
        CalculateView();
        CalculateMovement();
        CalculateJump();
        CulculateStance();
        SetIsGrounded();
        SetIsFalling();
        CalulateAinmingIn();
        CaculateLeaning();
    }

    #endregion

    #region - Shooting -

    private void ShootingPressed()
    {
        if (currentWeapon)
        {
            currentWeapon.isShooting = true;
        }
    }

    private void ShootingReleased()
    {
        if (currentWeapon)
        {
            currentWeapon.isShooting = false;
        }
    }

    #endregion

    #region - Aiming In -

    private void AimingInPressed()
    {
        isAimingIn = true;
    }

    private void AimingInReleased()
    {
        isAimingIn = false;
    }

    private void CalulateAinmingIn()
    {
        if (!currentWeapon)
        {
            return;
        }

        currentWeapon.isAnimingIn = isAimingIn;


    }

    #endregion

    #region - IsFalling / isGrounded - 

    private void SetIsGrounded()
    {
        isGrounded = Physics.CheckSphere(feetTranform.position, playerSettings.isGroundedRadius, groundMask);
    }
    private void SetIsFalling()
    {
        isFalling = (!isGrounded && characterController.velocity.magnitude > playerSettings.isFallingSpeed);
    }

    #endregion

    #region - View / Movement

    private void CalculateView()
    {
        newCharacterRotation.y += (isAimingIn ? playerSettings.ViewXSensitivity * playerSettings.AnimingSensitivityEffector : playerSettings.ViewXSensitivity) * (playerSettings.ViewXInverted ? input_View.x : -input_View.x) * Time.deltaTime;
        transform.rotation = Quaternion.Euler(newCharacterRotation);

        newCameraRotation.x += (isAimingIn ? playerSettings.ViewYSensitivity * playerSettings.AnimingSensitivityEffector : playerSettings.ViewYSensitivity) * (playerSettings.ViewYInverted ? input_View.y : -input_View.y) * Time.deltaTime;
        newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, viewClampYMin, viewClampYMax); 

        cameraHolder.localRotation = Quaternion.Euler(newCameraRotation);
    }

    private void CalculateMovement()
    {
        if (input_Movement.y <= 0.2f)
        {
            isSprinting = false;
        }

        var verticalSpeed = playerSettings.WalkingForwardSpeed;
        var horizontalSpeed = playerSettings.WalkingStrafeSpeed;

        if (isSprinting)
        {
            verticalSpeed = playerSettings.RunningForwardSpeed;
            horizontalSpeed = playerSettings.RunningStrafeSpeed;
        }

        if (!isGrounded)
        {
            playerSettings.SpeedEffector = playerSettings.FallingSpeedEffector;
        }
        else if (playerStance == PlayerStance.Crouch)
        {
            playerSettings.SpeedEffector = playerSettings.CrouchSpeedEffector;
        }
        else if(playerStance == PlayerStance.Prone)
        {
            playerSettings.SpeedEffector = playerSettings.ProneSpeedEffector;
        }
        else if (isAimingIn)
        {
            playerSettings.SpeedEffector = playerSettings.AinmingSpeedEffector;
        }
        else
        {
            playerSettings.SpeedEffector = 1;
        }

        weaponAnimationSpeed = characterController.velocity.magnitude / playerSettings.WalkingForwardSpeed * playerSettings.SpeedEffector;

        if (weaponAnimationSpeed > 1)
        {
            weaponAnimationSpeed = 1;
        }

        verticalSpeed *= playerSettings.SpeedEffector;
        horizontalSpeed *= playerSettings.SpeedEffector;



        newMovementSpeed = Vector3.SmoothDamp(newMovementSpeed, new Vector3(horizontalSpeed * input_Movement.x * Time.deltaTime, 0, verticalSpeed * input_Movement.y * Time.deltaTime), ref newMovementSpeedVelocity, isGrounded ? playerSettings.MovementSmoothing : playerSettings.FallingSmoothing);
        var movementSpeed = transform.TransformDirection(newMovementSpeed);

        if (playerGravity > gravityMin)
        {
            playerGravity -= gravityAmout * Time.deltaTime;
        }
        

        if (playerGravity < -0.1f && isGrounded)
        {
            playerGravity = -0.1f; 
        }

        movementSpeed.y += playerGravity;
        movementSpeed += jumpingForce * Time.deltaTime;
        
        characterController.Move(movementSpeed);
    }

    #endregion

    #region - Leaning -

    private void CaculateLeaning()
    {
        if (isLeanLeft)
        {
            targetLean = leanAngle;
        }
        else if (isLeanRight)
        {
            targetLean = -leanAngle;
        }
        else
        {
            targetLean = 0;
        }


        currentLean = Mathf.SmoothDamp(currentLean, targetLean, ref leanVelocity, leanSmoothing);
        LeanPivot.localRotation = Quaternion.Euler(new Vector3(0, 0, currentLean));
    }

    #endregion

    #region - Jumping -

    private void CalculateJump()
    {
        jumpingForce = Vector3.SmoothDamp(jumpingForce, Vector3.zero, ref jumpingForceVelocity, playerSettings.JumpingFalloff);
    }

    private void Jump()
    {
        if (!isGrounded || playerStance == PlayerStance.Prone)
        {
            return;
        }
        if (playerStance == PlayerStance.Crouch)
        {
            if (StanceCheck(playerStandStance.StanceCollider.height))
            {
                return;
            }

            playerStance = PlayerStance.Stand;
            return;
        }

        //Jump
        jumpingForce = Vector3.up * playerSettings.JumpingHeight;
        playerGravity = 0;

        currentWeapon.TriggerJump();

    }

    #endregion

    #region - Stance - 

    private void CulculateStance()
    {
        var currentStance = playerStandStance;

        if (playerStance == PlayerStance.Crouch)
        {
            currentStance = playerCrouchStance;
        }
        else if (playerStance == PlayerStance.Prone)
        {
            currentStance = playerProneStance;
        }



        cameraHeight = Mathf.SmoothDamp(cameraHolder.localPosition.y, currentStance.CameraHeight, ref cameraHeightVelocity, playerStanceSmoothing);
        cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x, cameraHeight, cameraHolder.localPosition.y);

        characterController.height = Mathf.SmoothDamp(characterController.height, currentStance.StanceCollider.height, ref stanceCapsuleHeightVelocity, playerStanceSmoothing);
        characterController.center = Vector3.SmoothDamp(characterController.center, currentStance.StanceCollider.center, ref stanceCapsuleCenterVelocity, playerStanceSmoothing);

    }

    private void Crouch()
    {
        if(playerStance == PlayerStance.Crouch)
        {
            if(StanceCheck(playerStandStance.StanceCollider.height))
            {
                return;
            }

            playerStance = PlayerStance.Stand;
            return;
        }
        if (StanceCheck(playerCrouchStance.StanceCollider.height))
        {
            return;
        }

        playerStance = PlayerStance.Crouch;
    }

    private void Prone()
    {
        playerStance = PlayerStance.Prone;
    }

    private bool StanceCheck(float stanceCheckheight)
    {
        var start = new Vector3(feetTranform.position.x, feetTranform.position.y + characterController.radius + stanceCheckErrorMargin, feetTranform.position.z);
        var end = new Vector3(feetTranform.position.x, feetTranform.position.y - characterController.radius - stanceCheckErrorMargin + stanceCheckheight, feetTranform.position.z);




        return Physics.CheckCapsule(start, end, characterController.radius, playerMask);
    }

    #endregion

    #region - Sprinting -

    private void ToggleSprint()
    {
        if (input_Movement.y <= 0.2f)
        {
            isSprinting = false;
            return;
        }

        isSprinting = !isSprinting;
    }

    private void StopSprint()
    {
        if (playerSettings.sprintingHold)
        {
            isSprinting = false;
        }

            
    }

    #endregion

    #region - Gizoms

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(feetTranform.position, playerSettings.isGroundedRadius);
    }


    #endregion




}
