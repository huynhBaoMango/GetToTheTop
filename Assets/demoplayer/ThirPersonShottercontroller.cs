using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirPersonShottercontroller : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualcamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform pfBullletProjectite;
    [SerializeField] private Transform SpawnBullletPosition;
    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private Animator animator;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {

        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 ScreenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);


        Ray ray = Camera.main.ScreenPointToRay(ScreenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }


        if (starterAssetsInputs.aim)
        {
            aimVirtualcamera.gameObject.SetActive(true);
            thirdPersonController.SetSentivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;


            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));


            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
        else
        {

            aimVirtualcamera.gameObject.SetActive(false);
            thirdPersonController.SetSentivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);


            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
        }


        if (starterAssetsInputs.shoot)
        {
            Shoot(mouseWorldPosition);
            starterAssetsInputs.shoot = false;
        }
    }


    private void Shoot(Vector3 targetPosition)
    {
        Vector3 aimDir = (targetPosition - SpawnBullletPosition.position).normalized;


        Instantiate(pfBullletProjectite, SpawnBullletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
    }
}