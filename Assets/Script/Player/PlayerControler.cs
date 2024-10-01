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
    [SerializeField]
    private GameObject headObject;
    [HideInInspector]
    public bool canMove = true;

    [SerializeField]
    private float cameraYOffset = 0.4f;
    private Camera playerCamera;

    [Header("Animator Setup")]
    public Animation anim;
    [SerializeField]
    private int PlayerSelfLayer = 7; // L?p c?a chính player
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private List<GameObject> playerModels = new List<GameObject>();
    //[SerializeField] private Transform rightHandTransform;
    public override void OnStartClient()
    {
        base.OnStartClient();

        // Ki?m tra n?u ?ây là player ch? s? h?u
        if (base.IsOwner)
        {
            playerCamera = Camera.main;
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
            playerCamera.transform.SetParent(transform);
            playerCamera.transform.position = transform.position + new Vector3(0.067f, 1.6f, 0.40f);
            headObject.SetActive(false);


            gameObject.layer = PlayerSelfLayer;
            foreach (var obj in playerModels)
            {
                obj.layer = PlayerSelfLayer;
            }
        }
        else
        {
            headObject.SetActive(true);
            gameObject.GetComponent<PlayerController>().enabled = false; // T?t script PlayerController cho các player không ph?i ch? s? h?u
        }
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Khóa con tr? chu?t
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // ?n con tr? chu?t
    }

    void Update()
    {
        if (base.IsOwner)  // Ki?m tra n?u ?ây là player ch? s? h?u
        {
            HandleMovement(); // X? lý di chuy?n

            // G?i v? trí c?a player lên server
            UpdateServerPosition(transform.position);
        }
    }

    private void HandleMovement()
    {
        // Kiểm tra xem có đang chạy hay không
        bool isRunning = Input.GetKey(KeyCode.LeftShift); // Kiểm tra nếu đang chạy
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Tính tốc độ dựa trên trạng thái chạy
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;

        // Tính toán hướng di chuyển
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // Xử lý trọng lực
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Xử lý reload
        if (Input.GetKeyDown("r"))
        {
            ReloadAnimationServerRpc();
        }

        // Cập nhật di chuyển của nhân vật
        characterController.Move(moveDirection * Time.deltaTime);

        // Cập nhật góc nhìn của camera
        if (canMove && playerCamera != null)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        // Cập nhật animator
        Vector3 localVelocity = transform.InverseTransformDirection(characterController.velocity);
        animator.SetFloat("VelocityX", localVelocity.x);
        animator.SetFloat("VelocityZ", localVelocity.z);
        animator.SetBool("IsRunning", isRunning); // Thêm để kiểm tra trạng thái chạy
    }


    // G?i v? trí c?a client lên server
    [ServerRpc]
    private void UpdateServerPosition(Vector3 newPosition)
    {
        // C?p nh?t v? trí trên server
        transform.position = newPosition;

        // Phát l?i v? trí cho t?t c? các client
        UpdateClientPosition(newPosition);
    }
    [ServerRpc]
    public void ReloadAnimationServerRpc()
    {
        // Phát lệnh cho các client thực hiện animation Reload
        ReloadAnimationClientRpc();
    }

    [ObserversRpc]
    private void ReloadAnimationClientRpc()
    {
        animator.SetTrigger("Reload");
    }

    // Server phát l?i v? trí c?a player cho t?t c? client
    [ObserversRpc]
    private void UpdateClientPosition(Vector3 newPosition)
    {
        // C?p nh?t v? trí cho các client khác (không c?p nh?t cho player ch? s? h?u)
        if (!base.IsOwner)
        {
            transform.position = newPosition;
        }
    }

}
