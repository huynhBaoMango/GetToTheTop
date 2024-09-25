using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class PlayerController : NetworkBehaviour
{
    [Header("Base Setup")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0f;

    [HideInInspector]
    public bool canMove = true;

    [SerializeField]
    private float cameraYOffset = 0.4f;
    private Camera playerCamera;

    [Header("Animator Setup")]
    public Animation anim;
    [SerializeField]
    private int PlayerSelfLayer = 7; // L?p c?a ch�nh player
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private List<GameObject> playerModels = new List<GameObject>();

    public override void OnStartClient()
    {
        base.OnStartClient();

        // Ki?m tra n?u ?�y l� player ch? s? h?u
        if (base.IsOwner)
        {
            playerCamera = Camera.main;
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
            playerCamera.transform.SetParent(transform);

            // Ki?m tra v� kh?i t?o v? kh� c?a player
            if (TryGetComponent(out PlayerWeapone playerWeapon))
                playerWeapon.InitializeWeapons(playerCamera.transform);

            // ??t l?p cho player ch? s? h?u
            gameObject.layer = PlayerSelfLayer;
            foreach (var obj in playerModels)
            {
                obj.layer = PlayerSelfLayer;
            }
        }
        else
        {
            gameObject.GetComponent<PlayerController>().enabled = false; // T?t script PlayerController cho c�c player kh�ng ph?i ch? s? h?u
        }
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Kh�a con tr? chu?t
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // ?n con tr? chu?t
    }

    void Update()
    {
        if (base.IsOwner)  // Ki?m tra n?u ?�y l� player ch? s? h?u
        {
            HandleMovement(); // X? l� di chuy?n

            // G?i v? tr� c?a player l�n server
            UpdateServerPosition(transform.position);
        }
    }

    private void HandleMovement()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift); // Ki?m tra n?u ?ang ch?y
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;

        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // X? l� nh?y
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // �p d?ng l?c tr?ng l?c
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Di chuy?n player
        characterController.Move(moveDirection * Time.deltaTime);

        // ?i?u khi?n xoay camera v� nh�n v?t
        if (canMove && playerCamera != null)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        // C?p nh?t animator (n?u c�)
        Vector3 localVelocity = transform.InverseTransformDirection(characterController.velocity);
        animator.SetFloat("VelocityX", localVelocity.x);
        animator.SetFloat("VelocityZ", localVelocity.z);
    }

    // G?i v? tr� c?a client l�n server
    [ServerRpc]
    private void UpdateServerPosition(Vector3 newPosition)
    {
        // C?p nh?t v? tr� tr�n server
        transform.position = newPosition;

        // Ph�t l?i v? tr� cho t?t c? c�c client
        UpdateClientPosition(newPosition);
    }

    // Server ph�t l?i v? tr� c?a player cho t?t c? client
    [ObserversRpc]
    private void UpdateClientPosition(Vector3 newPosition)
    {
        // C?p nh?t v? tr� cho c�c client kh�c (kh�ng c?p nh?t cho player ch? s? h?u)
        if (!base.IsOwner)
        {
            transform.position = newPosition;
        }
    }
}
